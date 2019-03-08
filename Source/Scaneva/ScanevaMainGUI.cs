#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ScanevaMainGUI.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Christoph Pieper)
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, see http://www.gnu.org/licenses/.
//  </copyright>
//  <summary>
//  Url: https://github.com/Scaneva
//  </summary>
// --------------------------------------------------------------------------------------------------------------------
#endregion

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Scaneva.Core;
using Scaneva.Core.ExperimentData;
using Scaneva.Core.Experiments;
using Scaneva.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.PropertyGridInternal;

namespace Scaneva
{
    public partial class ScanevaMainGUI : Form
    {
        private ScanevaCore core = null;

        // rememebr changes
        private ParametrizableObject editedObject = null;
        private bool scanMethodHasUnsavedChanges = false;
        private bool hwStoreHasUnsaveChanges = false;
        private Position currentPos = new Position();

        private string methodFile = "New Method.smf";

        public ScanevaMainGUI()
        {
            InitializeComponent();
            updateTitleBar();

            // Change culture to en-US for UI
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            //
            btnMoveUp.Click += new System.EventHandler(MovePositioner);
            btnMoveDown.Click += new System.EventHandler(MovePositioner);
            btnMoveLeft.Click += new System.EventHandler(MovePositioner);
            btnMoveRight.Click += new System.EventHandler(MovePositioner);
            btnMoveBackward.Click += new System.EventHandler(MovePositioner);
            btnMoveForward.Click += new System.EventHandler(MovePositioner);
            btnMoveRelative.Click += new System.EventHandler(MovePositioner);
            btnMoveAbsolute.Click += new System.EventHandler(MovePositioner);
            btnStopMovement.Click += new System.EventHandler(MovePositioner);
            //

            // set sorting
            propertyGrid1.PropertySort = PropertySort.Categorized;

            ScanevaCoreSettings settings = BuildCoreSettings();
            core = new ScanevaCore(settings);

            // Load HW Settings           
            core.LoadHardwareSettings();
            refreshHWList();

            hwStoreHasUnsaveChanges = false;
            scanMethodHasUnsavedChanges = false;

            core.log.LogEntryAdded += Log_LogEntryAdded;
            core.log.StatusUpdated += Log_StatusUpdated;
            core.NotifyScanDataUpdated += Core_NotifyScanDataUpdated;
            core.NotifyScanEnded += Core_NotifyScanEnded;

            lockSettings(enViewState.hwConfigView);
        }

        private void updateTitleBar()
        {
            this.Text = "Scaneva - " + methodFile + (scanMethodHasUnsavedChanges ? "*" : "");
        }

        private ScanevaCoreSettings BuildCoreSettings()
        {
            ScanevaCoreSettings settings = new ScanevaCoreSettings();
            settings.DefaultScanMethodDirectory = Properties.Settings.Default.DefaultScanMethodDirectory;
            settings.HWSettingsFilePath = Properties.Settings.Default.HWSettingsFilePath;
            settings.LogDirectory = Properties.Settings.Default.LogDirectory;
            settings.PositionStoreFilePath = Properties.Settings.Default.PositionStoreFilePath;
            settings.ScanResultDirectory = Properties.Settings.Default.ScanResultDirectory;

            return settings;
        }

        private void Log_StatusUpdated(object sender, StatusUpdatedEventArgs e)
        {
            switch (e.StatusId)
            {
                case 0:
                    Position newPos = e.StatusMessage as Position;
                    if (newPos != null)
                    {
                        UpdatePosDisplay(newPos);
                    }

                    break;

                default:
                    break;
            }
        }

        private void refreshHWList()
        {
            checkedListBoxHardware.Items.Clear();
            foreach (KeyValuePair<string, IHWManager> entry in core.hwStore)
            {
                checkedListBoxHardware.Items.Add(entry.Key, entry.Value.IsEnabled);
            }
        }

        private void refreshMethod(bool reload = false)
        {
            if (reload)
            {
                core.BuildMethodTree(treeViewScanMethod.Nodes);
            }
            treeViewScanMethod.Refresh();
        }

        private void Log_LogEntryAdded(object sender, LogEntryAddedEventArgs e)
        {
            setToolStripText(e.LogEntry);
        }

        delegate void setToolStripTextDelegate(string text);

        private void setToolStripText(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new setToolStripTextDelegate(setToolStripText), new object[] { text });
            }
            else
            {
                // Check if Error
                if ((text.Length > 19) && (text.Substring(14, 5).ToUpper() == "ERROR"))
                {
                    // Error Message
                    toolStripStatusError.Text = text;
                }
                else
                {
                    // Do things
                    toolStripStatusLabel1.Text = text;
                }
            }
        }

        private void checkedListBoxHardware_SelectedValueChanged(object sender, EventArgs e)
        {
            string key = (string)checkedListBoxHardware.SelectedItem;
            if (!String.IsNullOrEmpty(key))
            {
                ParametrizableObject element = (ParametrizableObject)core.hwStore[key];
                setEditObject(element);
            }
        }

        private void ScanevaMainGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop Scan if running
            core.CancelScanMethod();

            stopLiveManualInput = true;
            Thread.Sleep(100);

            if (hwStoreHasUnsaveChanges)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to save pending changes to HW settings before exit?", "Save Changes?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    core.SaveHardwareSettings();
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do nothing
                }
            }

            if (scanMethodHasUnsavedChanges)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to save changes to Scan Method before exit?", "Save Changes to method?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    toolStripButtonSaveMethod_Click(null, null);
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do nothing
                }
            }
            

            // Release all Hardware
            try
            {
                core.ReleaseAllHardware();
            }
            catch (Exception ex)
            {
                core.log.Add(ex.ToString());
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            var sender = s as PropertyGrid;
            //string propertyName = e.ChangedItem.Parent
            var obj = sender.SelectedObject;

            var parent = e.ChangedItem;
            string pName = parent.Label;
            while ((parent.Parent != null) && (!(typeof(IRootGridEntry).IsAssignableFrom(parent.Parent.GetType()))))
            {
                parent = parent.Parent;
                pName = parent.Label + "." + pName;
            }

            if (editedObject != null)
            {
                if ((typeof(IHWManager)).IsAssignableFrom(editedObject.GetType()))
                {
                    (editedObject as ParametrizableObject).ParameterChanged(pName);
                    hwStoreHasUnsaveChanges = true;
                }
                else if ((typeof(IExperiment)).IsAssignableFrom(editedObject.GetType()))
                {
                    (editedObject as ParametrizableObject).ParameterChanged(pName);
                    scanMethodHasUnsavedChanges = true;
                    updateTitleBar();
                }
            }
            else if (propertyGrid1.SelectedObject.GetType().Equals(typeof(DynamicSettingsClass)))
            {
                // Manual Transducer Channel Setting
                DynamicSettingsClass cc = propertyGrid1.SelectedObject as DynamicSettingsClass;
                ITransducer hw = cc?.RefObject as ITransducer;
                if (hw != null)
                {
                    if (e.ChangedItem.Value.GetType().Equals(typeof(double)))
                    {
                        hw.Channels.Find(x => (x.Name == e.ChangedItem.Label)).SetValue((double)e.ChangedItem.Value);
                    }
                }
            }

            // refresh para box
            sender.Refresh();
        }

        private void buttonRemoveHW_Click(object sender, EventArgs e)
        {
            string key = (string)checkedListBoxHardware.SelectedItem;
            if (!String.IsNullOrEmpty(key))
            {
                // Release HW Component before Deleting
                core.hwStore[key].Release();
                core.RemoveHardware(key);
                checkedListBoxHardware.Items.Remove(key);
                hwStoreHasUnsaveChanges = true;
                setEditObject(null);
            }
        }

        private void buttonAddHW_Click(object sender, EventArgs e)
        {
            ListSelectionDialog dialog = new ListSelectionDialog(Cursor.Position.X, Cursor.Position.Y);
            dialog.Text = "Add HW";
            dialog.ListBoxLabel = "Select HW Type:";
            dialog.TextEntryLabel = "Enter New HW Name:";

            // Get all HW Types
            dialog.SelectionValues = core.availableHWTypes;

            DialogResult dResult = dialog.ShowDialog();
            string hwName = dialog.TextEntry;
            string hwTypeName = dialog.SelectedValue;

            if (dResult == DialogResult.OK)
            {
                if ((String.IsNullOrEmpty(hwName)) || core.hwStore.ContainsKey(hwName))
                {
                    string message = "Please enter a valid unique name for the new HW.";
                    string caption = "Invalid Name";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;

                    // Displays the MessageBox.
                    MessageBox.Show(message, caption, buttons);
                }
                else
                {
                    // Create new HW and refresh listBox
                    core.AddHardware(hwName, hwTypeName);
                    hwStoreHasUnsaveChanges = true;

                    refreshHWList();
                }
            }
        }

        private void buttonDeleteExp_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewScanMethod.SelectedNode;
            if (node != null)
            {
                // Create new Experiment and refresh listBox
                core.RemoveExperimentFromScanMethod(treeViewScanMethod.Nodes, node);
                scanMethodHasUnsavedChanges = true;

                treeViewScanMethod.SelectedNode = null;
                setEditObject(null);
                refreshMethod();
            }
        }

        private void buttonAddExp_Click(object sender, EventArgs e)
        {
            ListSelectionDialog dialog = new ListSelectionDialog(Cursor.Position.X, Cursor.Position.Y);
            dialog.Text = "Add Experiment";
            dialog.ListBoxLabel = "Select Experiment Type:";
            dialog.TextEntryLabel = "Enter New Experiment Name:";

            // Get all Experiment Types
            dialog.SelectionValues = core.availableExperiments;

            DialogResult dResult = dialog.ShowDialog();
            string expName = dialog.TextEntry;
            string expTypeName = dialog.SelectedValue;

            if (dResult == DialogResult.OK)
            {
                if (String.IsNullOrEmpty(expName))
                {
                    string message = "Please enter a valid unique name for the new Experiment.";
                    string caption = "Invalid Name";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;

                    // Displays the MessageBox.
                    MessageBox.Show(message, caption, buttons);
                }
                else
                {
                    TreeNode insertAfter = treeViewScanMethod.SelectedNode;

                    // Create new Experiment and update treeView
                    TreeNode newExp = core.AddExperimentToScanMethod(treeViewScanMethod.Nodes, insertAfter, expName, expTypeName);
                    treeViewScanMethod.SelectedNode = newExp;

                    scanMethodHasUnsavedChanges = true;
                    updateTitleBar();
                }
            }
        }

        private void treeViewScanMethod_AfterSelect(object sender, TreeViewEventArgs e)
        {
            IExperiment element = treeViewScanMethod.SelectedNode.Tag as IExperiment;
            if (element != null)
            {
                ParametrizableObject obj = element as ParametrizableObject;
                setEditObject(obj);
            }
        }

        private void toolStripButtonSaveSettings_Click(object sender, EventArgs e)
        {
            core.SaveHardwareSettings();
            hwStoreHasUnsaveChanges = false;
        }

        private void setEditObject(ParametrizableObject editedObject)
        {
            //editedObject.GetType().GetProperties();

            this.editedObject = editedObject;
            if (editedObject == null)
            {
                propertyGrid1.SelectedObject = null;
            }
            else
            {
                propertyGrid1.SelectedObject = editedObject.GetSettings();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            setEditObject(null);
            treeViewScanMethod.SelectedNode = null;
            checkedListBoxHardware.SelectedIndex = -1;

            // Update Manual Control ComboBoxes
            if (tabControl1.SelectedTab.Name == "tabPageManualControl")
            {
                updateManualControlComboBoxes();
            }
            else
            {
                // stop Live input if still running
                StartLiveInput(false);
            }
        }

        private void StartLiveInput(bool bStart)
        {
            if (bStart)
            {
                // start update Tast
                if ((liveManualInputTask == null) || (liveManualInputTask.IsCanceled) || (liveManualInputTask.IsCompleted))
                {
                    stopLiveManualInput = false;

                    liveManualInputTask = Task.Factory.StartNew(new Action(liveManualInput), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                    // change display text
                    button_StartLive.Text = "Stop Input";
                }
            }
            else
            {
                // Stop
                stopLiveManualInput = true;
                button_StartLive.Text = "Start Input";
            }
        }

        private bool stopLiveManualInput = false;
        private Task liveManualInputTask;
        private string manualInputChan;

        private void liveManualInput()
        {
            string oldChan = "";
            TransducerChannel tchan = null;
            LineSeries series = null;
            double time = 0.0;
            int delayMS = 100;
            PlotModel plotModelManual = null;

            DateTime startDate = DateTime.Now;

            while (!stopLiveManualInput)
            {
                System.Threading.Thread.Sleep(delayMS);

                refreshTransducerValues();

                string chan = manualInputChan;

                TimeSpan timeDifference = DateTime.Now.Subtract(startDate);
                time = timeDifference.TotalMilliseconds * 0.001;

                if ((chan != null) && (chan != oldChan))
                {
                    oldChan = chan;
                    tchan = transducerChannels[chan];
                    startDate = DateTime.Now;
                    time = 0.0;

                    // set up graph
                    plotModelManual = new PlotModel() { Title = chan };
                    plotView_ManualTab.Model = plotModelManual;

                    plotModelManual.LegendPosition = LegendPosition.BottomCenter;
                    plotModelManual.LegendOrientation = LegendOrientation.Horizontal;
                    plotModelManual.LegendPlacement = LegendPlacement.Outside;

                    plotModelManual.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Time (s)" });
                    plotModelManual.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Value (" + ((tchan.Prefix == enuPrefix.none) ? "" : tchan.Prefix.ToString()) + tchan.Unit + ")" });

                    series = new LineSeries();
                    plotModelManual.Series.Add(series);
                    //series.Title = tchan.Name;
                }

                if (tchan != null)
                {
                    // Get value
                    double value = tchan.GetValue();

                    series.Points.Add(new DataPoint(time, value));

                    // Limit to 100 points
                    if (series.Points.Count > 100)
                    {
                        series.Points.RemoveAt(0);
                    }
                    plotModelManual.InvalidatePlot(true);
                }

                // Also update pos value
                //UpdatePosDisplay(core.positionStore.CurrentAbsolutePosition());
            }
        }

        private Dictionary<string, IHWManager> positioners;
        private Dictionary<string, TransducerChannel> transducerChannels;


        private void updateManualControlComboBoxes()
        {
            var value = core.hwStore;
            positioners = value.Where(x => typeof(IPositioner).IsAssignableFrom(x.Value.GetType())).Select(x => x).ToDictionary(x => x.Key, x => x.Value);
            comboBox_ManualPositioner.Items.Clear();
            foreach (KeyValuePair<string, IHWManager> compo in positioners)
            {
                // only add enabled Positioners
                if (compo.Value.IsEnabled)
                {
                    comboBox_ManualPositioner.Items.Add(compo.Key);
                }
                if (comboBox_ManualPositioner.Items.Count > 0)
                {
                    comboBox_ManualPositioner.SelectedIndex = 0;
                }
            }

            transducerChannels = new Dictionary<string, TransducerChannel>();
            var transducers = value.Where(x => typeof(ITransducer).IsAssignableFrom(x.Value.GetType())).Select(x => x).ToArray();

            comboBox_ManualInput.Items.Clear();

            // Iterate Transducers
            foreach (KeyValuePair<string, IHWManager> ele in transducers)
            {
                // only add enabled Positioners
                if (ele.Value.IsEnabled)
                {
                    ITransducer ducer = (ITransducer)ele.Value;
                    // Iterate Channels
                    foreach (TransducerChannel chan in ducer.Channels)
                    {
                        if ((chan.ChannelType == enuChannelType.passive) || (chan.ChannelType == enuChannelType.mixed))
                        {
                            string name = ele.Key + "." + chan.Name;
                            transducerChannels.Add(name, chan);
                            comboBox_ManualInput.Items.Add(name);
                        }
                    }
                }
            }
        }

        private void toolStripLoadMethod_Click(object sender, EventArgs e)
        {
            string path = string.Empty;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Scaneva Method Files (*.smf)|*.smf";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.InitialDirectory = LogHelper.getMainDirectory();

            if (File.GetAttributes(Properties.Settings.Default.DefaultScanMethodDirectory).HasFlag(FileAttributes.Directory))
            {
                openFileDialog1.InitialDirectory = Properties.Settings.Default.DefaultScanMethodDirectory;
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName;

                core.LoadScanMethod(path);
                treeViewScanMethod.SelectedNode = null;
                scanMethodHasUnsavedChanges = false;

                refreshMethod(true);
                setEditObject(null);

                scanMethodHasUnsavedChanges = false;
                methodFile = Path.GetFileName(path);
                updateTitleBar();
            }
        }

        private void toolStripButtonSaveMethod_Click(object sender, EventArgs e)
        {
            string path = string.Empty;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Scaneva Method Files (*.smf)|*.smf";
            saveFileDialog1.InitialDirectory = LogHelper.getMainDirectory();

            if (File.GetAttributes(Properties.Settings.Default.DefaultScanMethodDirectory).HasFlag(FileAttributes.Directory))
            {
                saveFileDialog1.InitialDirectory = Properties.Settings.Default.DefaultScanMethodDirectory;
            }

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = saveFileDialog1.FileName;

                core.SaveScanMethod(path);
                scanMethodHasUnsavedChanges = false;
                methodFile = Path.GetFileName(path);
                updateTitleBar();
            }
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewScanMethod.SelectedNode;
            if (node != null)
            {
                // Move element Up and refresh listBox
                core.MoveExperimentInScanMethodUp(treeViewScanMethod.Nodes, node);
                treeViewScanMethod.SelectedNode = node;

                scanMethodHasUnsavedChanges = true;
                updateTitleBar();
                refreshMethod();
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewScanMethod.SelectedNode;
            if (node != null)
            {
                // Move element Up and refresh listBox
                core.MoveExperimentInScanMethodDown(treeViewScanMethod.Nodes, node);
                treeViewScanMethod.SelectedNode = node;

                scanMethodHasUnsavedChanges = true;
                updateTitleBar();

                refreshMethod();
            }
        }

        private List<TabPage> hiddenPages = new List<TabPage>();

        private enum enViewState
        {
            hwConfigView = 0,
            hwInitializedView = 1,
            runView = 2
        }

        private delegate void lockSettingsDelegate(enViewState viewState);

        private void lockSettings(enViewState viewState = enViewState.runView)
        {
            if (tabControl1.InvokeRequired)
            {
                tabControl1.Invoke(
                    new lockSettingsDelegate(lockSettings),
                    new object[] { viewState }
                    );
            }
            else
            {
                int i = 0;
                foreach (TabPage page in hiddenPages)
                {
                    tabControl1.TabPages.Insert(i++, page);
                }
                hiddenPages.Clear();

                // reset all Tabs
                switch (viewState)
                {
                    case enViewState.hwConfigView:
                        //hideTabPage("tabPageHW");
                        hideTabPage("tabPageManualControl");
                        hideTabPage("tabPageScan");
                        hideTabPage("tabPagePlotView");

                        toolStripButtonStop.Enabled = false;
                        toolStripButtonRun.Enabled = false;
                        toolStripButtonConnect.Enabled = true;
                        break;

                    case enViewState.hwInitializedView:
                        hideTabPage("tabPageHW");
                        //hideTabPage("tabPageManualControl");
                        //hideTabPage("tabPageScan");
                        //hideTabPage("tabPagePlotView");

                        toolStripButtonStop.Enabled = false;
                        toolStripButtonRun.Enabled = true;
                        toolStripButtonStop.Image = Properties.Resources.Stop_Disabled;
                        toolStripButtonConnect.Enabled = false;
                        break;

                    case enViewState.runView:
                        hideTabPage("tabPageHW");
                        hideTabPage("tabPageManualControl");
                        hideTabPage("tabPageScan");
                        //hideTabPage("tabPagePlotView");

                        toolStripButtonStop.Enabled = true;
                        toolStripButtonRun.Enabled = false;
                        toolStripButtonStop.Image = Properties.Resources.Stop;
                        toolStripButtonConnect.Enabled = false;
                        break;

                    default:
                        break;
                }
            }
        }

        private void hideTabPage(string name)
        {
            TabPage page = tabControl1.TabPages[name];
            if (page != null)
            {
                hiddenPages.Add(page);
                tabControl1.TabPages.Remove(page);
            }
        }

        private void toolStripButtonRun_Click(object sender, EventArgs e)
        {
            if (core.scanMethod.Count > 0)
            {
                // Validate Experiment parameters
                bool valOk = true;
                foreach (IExperiment exp in core.scanMethod)
                {
                    valOk = ValidateMethodParameters(exp) && valOk;
                }

                if (valOk)
                {
                    // Ask for Result Directory Name
                    InputDialog dialog = new InputDialog(Cursor.Position.X, Cursor.Position.Y);
                    dialog.Text = "Run Result Path";
                    dialog.TextEntryLabel = "Enter Run Name:";
                    dialog.TextEntry = methodFile.Split(new string[] { ".smf" }, StringSplitOptions.RemoveEmptyEntries).First() + " " + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss");

                    DialogResult dResult = dialog.ShowDialog();
                    string runName = dialog.TextEntry;

                    if (dResult == DialogResult.OK)
                    {
                        toolStripStatusError.Text = "";

                        // Stop Live Input if running
                        StartLiveInput(false);
                        Thread.Sleep(10);

                        lockSettings();

                        setEditObject(null);
                        treeViewScanMethod.SelectedNode = null;
                        checkedListBoxHardware.SelectedIndex = -1;

                        // Initialized by Button
                        //core.InitializeAllHardware();

                        recentScanDataFree = null;
                        recentScanData = null;

                        ResetPlotView("Unnamed");
                        core.RunScanMethod(runName);
                    }
                }
            }
            else
            {
                // Test Test Test
                core.positionStore.SetPosition("Pos1", new Position(1, 1, 1));
                core.positionStore.SetPosition("Pos2", new Position(1, 2, 3));
                core.positionStore.SetPosition("Top Left", new Position(-20.5, 146.672, 1e5));

                core.positionStore.SaveAll();

                core.positionStore.DeletePosition("Pos1");
                core.positionStore.DeletePosition("Pos2");

                core.positionStore.LoadAll();

                Position cls1 = core.positionStore.GetAbsolutePosition("Top Left");
                Position cls2 = core.positionStore.GetAbsolutePosition("Pos2");

                //core.InitializeAllHardware();
                //var hw = (PS_PalmSens)core.hwStore["PalmSens4"];

                //List<TransducerChannel> chans = hw.Channels;

                //TransducerChannel chan = chans.First(x => x.Name == "Potential");
                //chan.SetValue(0.3f);

                //TransducerChannel chan2 = chans.First(x => x.Name == "Current");

                //for (int i = 0; i < 20; i++)
                //{
                //    float value = chan2.GetValue();
                //    core.log.Add("Current Value: " + value);
                //    System.Threading.Thread.Sleep(10);
                //}
            }

        }

        private static bool ValidateMethodParameters(IExperiment exp)
        {
            string errorText = String.Empty;
            bool valOk = exp.CheckParametersOk(out errorText);

            if (errorText != String.Empty)
            {
                MessageBox.Show(errorText, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // is the exp a container?
            if (typeof(ExperimentContainer).IsAssignableFrom(exp.GetType()))
            {
                ExperimentContainer cont = (ExperimentContainer)exp;

                foreach (IExperiment childExp in cont)
                {
                    valOk = ValidateMethodParameters(childExp) && valOk;
                }
            }

            return valOk;
        }

        private void Core_NotifyScanEnded(object sender, ExperimentEndedEventArgs e)
        {
            lockSettings(enViewState.hwInitializedView);
        }

        private PlotModel plotModel = null;
        private PlotModel scanModel = null;
        private HeatMapSeries heatMapSeries = null;
        private ScatterSeries scatter = null;
        private ScanData recentScanData = null;
        private ScanDataFreeform recentScanDataFree = null;
        private List<LineSeries> currentSeries = null;

        private void ResetPlotView(string title)
        {
            plotModel = new PlotModel { Title = title };
            plotView1.Model = plotModel;

            plotModel.LegendPosition = LegendPosition.BottomCenter;
            plotModel.LegendOrientation = LegendOrientation.Horizontal;
            plotModel.LegendPlacement = LegendPlacement.Outside;

            currentSeries = new List<LineSeries>();
        }

        private void UpdateScanPlot(ScanData data, string selectedSeries)
        {
            if (heatMapSeries != null)
            {
                heatMapSeries.Data = data.GetScanData(selectedSeries);
                heatMapSeries.Invalidate();

                // Refresh
                //scanModel.InvalidatePlot(true);
                plotViewScan.InvalidatePlot(true);
            }
        }

        private void UpdateScanPlotFree(ScanDataFreeform data, string selectedSeries, bool redraw = false)
        {
            if (scatter != null)
            {
                List<double[]> scanData = data.GetScanData(selectedSeries);
                if(redraw)
                {
                    scatter.Points.Clear();
                }

                //plotViewScan.Model.Axes.Where(x => x.Position == AxisPosition.Top).First().Title = data.GetValueAxisTitel(selectedSeries);

                int pointsAdded = scatter.Points.Count;
                int totalPoints = scanData.Count;

                for (int i = pointsAdded; i < totalPoints; i++)
                {
                    double[] pointData = scanData[i];
                    scatter.Points.Add(new ScatterPoint(pointData[0], pointData[1], 5, pointData[2])); // 3rd arg => size,  4th arg => value
                }

                // Refresh
                //scanModel.InvalidatePlot(true);
                plotViewScan.InvalidatePlot(true);
            }
        }

        private delegate void Core_NotifyScanDataUpdatedDelegate(object sender, ScanDataEventArgs e);

        private void Core_NotifyScanDataUpdated(object sender, ScanDataEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Core_NotifyScanDataUpdatedDelegate(Core_NotifyScanDataUpdated), new object[] { sender, e });
            }
            else
            {
                // Do things
                // is it Scan Data?
                if (e.Data.GetType().Equals(typeof(ScanData)))
                {
                    recentScanData = e.Data as ScanData;
                    recentScanDataFree = null;

                    // Is new experiment?
                    if (e.IsUpdatedData == false)
                    {
                        scanModel = new PlotModel();
                        comboBoxScanDisplayChannel.Items.Clear();
                        comboBoxScanDisplayChannel.Items.AddRange(recentScanData.GetDatasetNames().ToArray());
                        if (comboBoxScanDisplayChannel.Items.Count > 0)
                        {
                            comboBoxScanDisplayChannel.SelectedIndex = 0;
                        }

                        // Color axis (the X and Y axes are generated automatically)
                        scanModel.Axes.Add(new LinearColorAxis
                        {
                            Key = "ColorAxis",
                            //Palette = OxyPalettes.Rainbow(100)
                            Palette = OxyPalettes.Jet(500),
                            HighColor = OxyColors.Gray,
                            LowColor = OxyColors.Black,
                            Position = AxisPosition.Top
                        });

                        scanModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
                        scanModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, StartPosition = 1, EndPosition = 0, Title = "Y" });

                        scatter = null;
                        heatMapSeries = new HeatMapSeries
                        {
                            X0 = recentScanData.X0,
                            X1 = recentScanData.X1,
                            Y0 = recentScanData.Y0,
                            Y1 = recentScanData.Y1,
                            Interpolate = false,
                            //Data = recentScanData.GetScanData(0),
                            RenderMethod = HeatMapRenderMethod.Bitmap,
                        };

                        scanModel.Series.Add(heatMapSeries);
                        plotViewScan.Model = scanModel;
                    }

                    if (comboBoxScanDisplayChannel.SelectedItem != null)
                    {
                        UpdateScanPlot(recentScanData, comboBoxScanDisplayChannel.SelectedItem.ToString());
                    }
                }
                else if (e.Data.GetType().Equals(typeof(ScanDataFreeform)))
                {
                    recentScanDataFree = e.Data as ScanDataFreeform;
                    recentScanData = null;

                    // Is new experiment?
                    if (e.IsUpdatedData == false)
                    {
                        scanModel = new PlotModel();
                        comboBoxScanDisplayChannel.Items.Clear();
                        comboBoxScanDisplayChannel.Items.AddRange(recentScanDataFree.GetDatasetNames().ToArray());
                        if (comboBoxScanDisplayChannel.Items.Count > 0)
                        {
                            comboBoxScanDisplayChannel.SelectedIndex = 0;
                        }

                        // Color axis (the X and Y axes are generated automatically)
                        scanModel.Axes.Add(new LinearColorAxis
                        {
                            Key = "ColorAxis",
                            //Palette = OxyPalettes.Rainbow(100)
                            Palette = OxyPalettes.Jet(500),
                            HighColor = OxyColors.Gray,
                            LowColor = OxyColors.Black,
                            Position = AxisPosition.Top
                        });

                        scanModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
                        scanModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, StartPosition = 1, EndPosition = 0, Title = "Y" });

                        heatMapSeries = null;
                        scatter = new ScatterSeries { ColorAxisKey = "ColorAxis" };

                        scanModel.Series.Add(scatter);
                        plotViewScan.Model = scanModel;
                    }

                    if (comboBoxScanDisplayChannel.SelectedItem != null)
                    {
                        UpdateScanPlotFree(recentScanDataFree, comboBoxScanDisplayChannel.SelectedItem.ToString());
                    }
                }
                else
                {

                    IExperimentData data = e.Data;

                    // Is new experiment?
                    if (e.IsUpdatedData == false)
                    {
                        ResetPlotView(e.ExperimentName);
                    }

                    // Add missing series
                    for (int i = currentSeries.Count; i < e.Data.GetDatasets(); i++)
                    {
                        if (i == 0)
                        {
                            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = e.Data.GetAxisName(i, 0) + " (" + e.Data.GetAxisUnits(i, 0) + ")" });
                            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Key = "Primary", Title = e.Data.GetAxisName(i, 1) + " (" + e.Data.GetAxisUnits(i, 1) + ")" });
                        }
                        else if ((i == 1) && ((e.Data.GetAxisName(0, 1) != e.Data.GetAxisName(1, 1)) || (e.Data.GetAxisUnits(0, 1) != e.Data.GetAxisUnits(1, 1))))
                        { 
                            // Add seconf y-Axis
                            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Right, Key = "Secondary", Title = e.Data.GetAxisName(i, 1) + " (" + e.Data.GetAxisUnits(i, 1) + ")" });
                        }

                        currentSeries.Add(new LineSeries());

                        // Assign correct Y-Axis Key
                        currentSeries[i].YAxisKey = ((i == 1) && (plotModel.Axes.Count > 2)) ? "Secondary" : "Primary";

                        currentSeries[i].Title = e.Data.GetExperimentName() + " (" + e.Data.GetDatasetName(i) + ")";
                        //currentSeries[i].XAxis.Title = e.Data.GetAxisName(i, 0) + " (" + e.Data.GetAxisUnits(i, 0) + ")";
                        //currentSeries[i].YAxis.Title = e.Data.GetAxisName(i, 1) + " (" + e.Data.GetAxisUnits(i, 1) + ")";
                        plotModel.Series.Add(currentSeries[i]);
                    }

                    // Add new Datapoints
                    for (int i = 0; i < e.Data.GetDatasets(); i++)
                    {
                        double[][] data2D = data.Get2DData(i);

                        // HACK Warning: if the second Value is NaN we had an overflow in time trace buffer and we clear the series
                        if ((data2D.Length > 1) && (data2D[1].Length > 1) && (double.IsNaN(data2D[1][1])))
                        {
                            currentSeries[i].Points.Clear();
                        }

                        for (int j = currentSeries[i].Points.Count; ((j < data2D[0].Length) && (!double.IsNaN(data2D[1][j]))); j++)
                        {
                            currentSeries[i].Points.Add(new DataPoint(data2D[0][j], data2D[1][j]));
                        }
                    }

                    // Refresh
                    plotModel.InvalidatePlot(true);
                }
            }
        }

        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            core.CancelScanMethod();
        }

        private void buttonIncLevel_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewScanMethod.SelectedNode;
            if (node != null)
            {
                // Move element Up and refresh listBox
                core.MoveExperimentInScanMethodIncLevel(treeViewScanMethod.Nodes, node);
                treeViewScanMethod.SelectedNode = node;

                scanMethodHasUnsavedChanges = true;

                refreshMethod();
            }
        }

        private void buttonDecLevel_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewScanMethod.SelectedNode;
            if (node != null)
            {
                // Move element Up and refresh listBox
                core.MoveExperimentInScanMethodDecLevel(treeViewScanMethod.Nodes, node);
                treeViewScanMethod.SelectedNode = node;

                scanMethodHasUnsavedChanges = true;

                refreshMethod();
            }
        }

        delegate void UpdatePosDisplayDelegate(Position newPos);

        private void UpdatePosDisplay(Position newPos)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new UpdatePosDisplayDelegate(UpdatePosDisplay), new object[] { newPos });
            }
            else
            {
                // Do things
                toolStripStatusCurrentPosition.Text = newPos.HumanReadable();
            }
        }

        private void MovePositioner(object sender, EventArgs e)
        {
            enuPositionerStatus pstat = enuPositionerStatus.Error;
            if ((comboBox_ManualPositioner.SelectedItem != null) && (positioners.ContainsKey(comboBox_ManualPositioner.SelectedItem.ToString())))
            {
                IPositioner positioner = positioners[comboBox_ManualPositioner.SelectedItem.ToString()] as IPositioner;
                if (positioner != null)
                {
                    switch ((sender as Button).Name)
                    {
                        case "btnMoveLeft":
                            pstat = positioner.SetAxisSpeed(enuAxes.XAxis, Convert.ToDouble(txtXSpeed.Text));
                            pstat = positioner.SetAxisRelativePosition(enuAxes.XAxis, -Math.Abs(Convert.ToDouble(txtXIncrement.Text)));
                            break;
                        case "btnMoveRight":
                            pstat = positioner.SetAxisSpeed(enuAxes.XAxis, Convert.ToDouble(txtXSpeed.Text));
                            pstat = positioner.SetAxisRelativePosition(enuAxes.XAxis, Math.Abs(Convert.ToDouble(txtXIncrement.Text)));
                            break;
                        case "btnMoveBackward":
                            pstat = positioner.SetAxisSpeed(enuAxes.YAxis, Convert.ToDouble(txtYSpeed.Text));
                            pstat = positioner.SetAxisRelativePosition(enuAxes.YAxis, -Math.Abs(Convert.ToDouble(txtYIncrement.Text)));
                            break;
                        case "btnMoveForward":
                            pstat = positioner.SetAxisSpeed(enuAxes.YAxis, Convert.ToDouble(txtYSpeed.Text));
                            pstat = positioner.SetAxisRelativePosition(enuAxes.YAxis, Math.Abs(Convert.ToDouble(txtYIncrement.Text)));
                            break;
                        case "btnMoveUp":
                            pstat = positioner.SetAxisSpeed(enuAxes.ZAxis, Convert.ToDouble(txtZSpeed.Text));
                            pstat = positioner.SetAxisRelativePosition(enuAxes.ZAxis, Math.Abs(Convert.ToDouble(txtZIncrement.Text)));
                            break;
                        case "btnMoveDown":
                            pstat = positioner.SetAxisSpeed(enuAxes.ZAxis, Convert.ToDouble(txtZSpeed.Text));
                            pstat = positioner.SetAxisRelativePosition(enuAxes.ZAxis, -Math.Abs(Convert.ToDouble(txtZIncrement.Text)));
                            break;
                        case "btnMoveRelative":
                            Position increments = new Position(Convert.ToDouble(txtXIncrement.Text), Convert.ToDouble(txtYIncrement.Text), Convert.ToDouble(txtZIncrement.Text));
                            Position rspeeds = new Position(Convert.ToDouble(txtXSpeed.Text), Convert.ToDouble(txtYSpeed.Text), Convert.ToDouble(txtZSpeed.Text));
                            pstat = positioner.SetSpeeds(rspeeds);
                            if (pstat == enuPositionerStatus.Ready)
                            {
                                pstat = positioner.SetRelativePosition(increments);
                            }
                            break;
                        case "btnMoveAbsolute":
                            Position pos = new Position(Convert.ToDouble(txtXIncrement.Text), Convert.ToDouble(txtYIncrement.Text), Convert.ToDouble(txtZIncrement.Text));
                            Position aspeeds = new Position(Convert.ToDouble(txtXSpeed.Text), Convert.ToDouble(txtYSpeed.Text), Convert.ToDouble(txtZSpeed.Text));
                            pstat = positioner.SetSpeeds(aspeeds);
                            if (pstat == enuPositionerStatus.Ready)
                            {
                                pstat = positioner.SetAbsolutePosition(pos);
                            }
                            break;
                        case "btnStopMovement":
                            pstat = positioner.StopMovement();//todo: stop during the movement - extra thread?
                            break;
                        default:
                            break;
                    }
                    UpdatePosDisplay(core.positionStore.CurrentAbsolutePosition());
                }
                //todo: if (pstat != enuPositionerStatus.Ready) 
                
            }
        }

        private void buttonStorePosition_Click(object sender, EventArgs e)
        {
            InputDialog dialog = new InputDialog(Cursor.Position.X, Cursor.Position.Y);
            dialog.Text = "Position Name";
            dialog.TextEntryLabel = "Enter New Position Name:";

            DialogResult dResult = dialog.ShowDialog();
            string newName = dialog.TextEntry;

            if (dResult == DialogResult.OK)
            {
                core.positionStore.SetPosition(newName, currentPos.Copy);
                if (!listBoxStoredPosition.Items.Contains(newName))
                {
                    listBoxStoredPosition.Items.Add(newName);
                }
            }
        }

        private void buttonSetHome_Click(object sender, EventArgs e)
        {
            core.positionStore.SetPosition("Home", currentPos.Copy);
        }

        private void buttonMoveToPos_Click(object sender, EventArgs e)
        {
            if (listBoxStoredPosition.SelectedItem != null)
            {
                //currentPos = core.positionStore.GetAbsolutePosition(listBoxStoredPosition.SelectedItem as string).Copy;
                Position newPos = core.positionStore.GetAbsolutePosition(listBoxStoredPosition.SelectedItem as string).Copy;

                if ((comboBox_ManualPositioner.SelectedItem != null) && (positioners.ContainsKey(comboBox_ManualPositioner.SelectedItem.ToString())))
                {
                    IPositioner positioner = positioners[comboBox_ManualPositioner.SelectedItem.ToString()] as IPositioner;
                    if (positioner != null)
                    {
                        Position aspeeds = new Position(Convert.ToDouble(txtXSpeed.Text), Convert.ToDouble(txtYSpeed.Text), Convert.ToDouble(txtZSpeed.Text));

                        enuPositionerStatus pstat = positioner.SetSpeeds(aspeeds);
                        if (pstat == enuPositionerStatus.Ready)
                        {
                            pstat = positioner.SetAbsolutePosition(newPos);
                        }
                    }
                }
                        
                UpdatePosDisplay(core.positionStore.CurrentAbsolutePosition());
            }
        }

        private void buttonDelPosition_Click(object sender, EventArgs e)
        {
            if (listBoxStoredPosition.SelectedItem != null)
            {
                core.positionStore.DeletePosition(listBoxStoredPosition.SelectedItem as string);
                listBoxStoredPosition.Items.Remove(listBoxStoredPosition.SelectedItem);
            }
        }

        private void comboBox_ManualInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            manualInputChan = comboBox_ManualInput.Text;
        }

        private void toolStripButtonConnect_Click(object sender, EventArgs e)
        {
            core.InitializeAllHardware();

            // Add initialized HW to list:
            listBoxManualHwSelect.Items.Clear();
            listBoxManualHwSelect.Items.AddRange(core.hwStore.Where(x => (x.Value.IsEnabled && (typeof(ITransducer).IsAssignableFrom(x.Value.GetType())))).Select(x => x.Key).ToArray());

            // load Position Store
            foreach (var item in core.positionStore.PositionsList())
            {
                if (item != "Home")
                {
                    listBoxStoredPosition.Items.Add(item);
                }
            }
            UpdatePosDisplay(core.positionStore.CurrentAbsolutePosition());

            // Enable other panes
            lockSettings(enViewState.hwInitializedView);
        }

        private void checkedListBoxHardware_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Update Enabled Value of Checked HW
            string key = (string)checkedListBoxHardware.Items[e.Index];
            if (!String.IsNullOrEmpty(key))
            {
                core.hwStore[key].IsEnabled = (e.NewValue == CheckState.Checked);
                hwStoreHasUnsaveChanges = true;
            }
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {

        }

        private void textBoxPosX_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnMoveRelative_Click(object sender, EventArgs e)
        {

        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {

        }

        private void tabPagePlotView_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxScanDisplayChannel_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((recentScanData != null) && (comboBoxScanDisplayChannel.SelectedItem != null))
            {
                UpdateScanPlot(recentScanData, comboBoxScanDisplayChannel.SelectedItem.ToString());
            }
            else if ((recentScanDataFree != null) && (comboBoxScanDisplayChannel.SelectedItem != null))
            {
                UpdateScanPlotFree(recentScanDataFree, comboBoxScanDisplayChannel.SelectedItem.ToString(), true);
            }
        }

        private void button_StartLive_Click(object sender, EventArgs e)
        {
            // start update Tast
            if ((liveManualInputTask == null) || (liveManualInputTask.IsCanceled) || (liveManualInputTask.IsCompleted))
            {
                StartLiveInput(true);
            }
            else
            {
                StartLiveInput(false);
            }
        }

        private void editSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsDialog dialog = new SettingsDialog(Properties.Settings.Default);

            DialogResult dResult = dialog.ShowDialog();

            if (dResult == DialogResult.OK)
            {
                Properties.Settings.Default.Save();
                core.Settings = BuildCoreSettings();
            }
            else
            {
                Properties.Settings.Default.Reload();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        private void listBoxManualHwSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            string hwKey = listBoxManualHwSelect.SelectedItem as string;
            if (hwKey != null)
            {
                ITransducer hw = core.hwStore[hwKey] as ITransducer;

                DynamicSettingsClass expando = new DynamicSettingsClass();
                expando.Name = hwKey;
                expando.RefObject = hw;

                foreach (var chan in hw.Channels)
                {
                    string unit = (chan.Prefix == enuPrefix.none) ? chan.Unit : chan.Prefix + chan.Unit;
                    expando.Add(new CustomProperty(chan.Name, chan.Name + " (" + unit + ")", hwKey, double.NaN, typeof(double), chan.ChannelType == enuChannelType.passive, true));
                }

                propertyGrid1.SelectedObject = expando;
                refreshTransducerValues(false);
            }            
        }

        private void refreshTransducerValues(bool passiveChansOnly = true)
        {
            DynamicSettingsClass cc = propertyGrid1.SelectedObject as DynamicSettingsClass;
            ITransducer hw = cc?.RefObject as ITransducer;
            if (hw != null)
            {
                foreach (var tchan in hw.Channels)
                {
                    double val = tchan.GetValue();
                    CustomProperty cProp = cc.Find(x => (x.Name == tchan.Name));
                    if ((cProp != null) && ((!passiveChansOnly) || (tchan.ChannelType != enuChannelType.active)))
                    {
                        cProp.Value = val;
                    }
                }
            }
            propertyGrid1.Invoke(new MethodInvoker(delegate { propertyGrid1.Refresh(); }));
        }


    }
}
