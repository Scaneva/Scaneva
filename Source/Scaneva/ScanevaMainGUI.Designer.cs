namespace Scaneva
{
    partial class ScanevaMainGUI
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanevaMainGUI));
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusCurrentPosition = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusError = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.settingsComboBox = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageHW = new System.Windows.Forms.TabPage();
            this.checkedListBoxHardware = new System.Windows.Forms.CheckedListBox();
            this.buttonRemoveHW = new System.Windows.Forms.Button();
            this.buttonAddHW = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPageManualControl = new System.Windows.Forms.TabPage();
            this.btnStopMovement = new System.Windows.Forms.Button();
            this.txtZSpeed = new System.Windows.Forms.TextBox();
            this.txtYSpeed = new System.Windows.Forms.TextBox();
            this.btnMoveAbsolute = new System.Windows.Forms.Button();
            this.btnMoveRelative = new System.Windows.Forms.Button();
            this.txtXSpeed = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBox_ManualPositioner = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtZIncrement = new System.Windows.Forms.TextBox();
            this.txtYIncrement = new System.Windows.Forms.TextBox();
            this.txtXIncrement = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_StartLive = new System.Windows.Forms.Button();
            this.plotView_ManualTab = new OxyPlot.WindowsForms.PlotView();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBox_ManualInput = new System.Windows.Forms.ComboBox();
            this.buttonSetHome = new System.Windows.Forms.Button();
            this.buttonDelPosition = new System.Windows.Forms.Button();
            this.buttonMoveToPos = new System.Windows.Forms.Button();
            this.btnSavePosition = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.listBoxStoredPosition = new System.Windows.Forms.ListBox();
            this.tabPageScan = new System.Windows.Forms.TabPage();
            this.buttonDeleteExp = new System.Windows.Forms.Button();
            this.buttonAddExp = new System.Windows.Forms.Button();
            this.treeViewScanMethod = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPagePlotView = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label13 = new System.Windows.Forms.Label();
            this.comboBoxScanDisplayChannel = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.plotViewScan = new OxyPlot.WindowsForms.PlotView();
            this.label12 = new System.Windows.Forms.Label();
            this.plotView1 = new OxyPlot.WindowsForms.PlotView();
            this.informationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMoveRight = new System.Windows.Forms.Button();
            this.btnMoveLeft = new System.Windows.Forms.Button();
            this.btnMoveForward = new System.Windows.Forms.Button();
            this.btnMoveBackward = new System.Windows.Forms.Button();
            this.buttonIncLevel = new System.Windows.Forms.Button();
            this.buttonDecLevel = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.toolStripButtonSaveSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonConnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripLoadMethod = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveMethod = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRun = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
            this.mainStatusStrip.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.mainToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageHW.SuspendLayout();
            this.tabPageManualControl.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPageScan.SuspendLayout();
            this.tabPagePlotView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainStatusStrip
            // 
            this.mainStatusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusCurrentPosition,
            this.toolStripStatusError});
            this.mainStatusStrip.Location = new System.Drawing.Point(0, 472);
            this.mainStatusStrip.Name = "mainStatusStrip";
            this.mainStatusStrip.Size = new System.Drawing.Size(1178, 22);
            this.mainStatusStrip.TabIndex = 0;
            this.mainStatusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Status";
            // 
            // toolStripStatusCurrentPosition
            // 
            this.toolStripStatusCurrentPosition.BackColor = System.Drawing.SystemColors.Info;
            this.toolStripStatusCurrentPosition.Name = "toolStripStatusCurrentPosition";
            this.toolStripStatusCurrentPosition.Size = new System.Drawing.Size(111, 17);
            this.toolStripStatusCurrentPosition.Text = "Actual position: n/a";
            this.toolStripStatusCurrentPosition.Click += new System.EventHandler(this.toolStripStatusLabel2_Click);
            // 
            // toolStripStatusError
            // 
            this.toolStripStatusError.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripStatusError.ForeColor = System.Drawing.Color.Red;
            this.toolStripStatusError.Name = "toolStripStatusError";
            this.toolStripStatusError.Size = new System.Drawing.Size(1013, 17);
            this.toolStripStatusError.Spring = true;
            this.toolStripStatusError.Text = "Last error: ";
            this.toolStripStatusError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.informationToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(1178, 24);
            this.mainMenuStrip.TabIndex = 1;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editSettingsToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // editSettingsToolStripMenuItem
            // 
            this.editSettingsToolStripMenuItem.Name = "editSettingsToolStripMenuItem";
            this.editSettingsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.editSettingsToolStripMenuItem.Text = "Edit Scaneva Settings";
            this.editSettingsToolStripMenuItem.Click += new System.EventHandler(this.editSettingsToolStripMenuItem_Click);
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSaveSettings,
            this.toolStripSeparator1,
            this.toolStripButtonConnect,
            this.toolStripSeparator2,
            this.toolStripLoadMethod,
            this.toolStripButtonSaveMethod,
            this.toolStripSeparator3,
            this.toolStripButtonRun,
            this.toolStripButtonStop});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 24);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(1178, 39);
            this.mainToolStrip.TabIndex = 2;
            this.mainToolStrip.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 39);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 63);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.settingsComboBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1178, 409);
            this.splitContainer1.SplitterDistance = 377;
            this.splitContainer1.TabIndex = 3;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid1.Location = new System.Drawing.Point(4, 51);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(360, 362);
            this.propertyGrid1.TabIndex = 2;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5);
            this.label1.Size = new System.Drawing.Size(360, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Settings";
            // 
            // settingsComboBox
            // 
            this.settingsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsComboBox.FormattingEnabled = true;
            this.settingsComboBox.ItemHeight = 16;
            this.settingsComboBox.Location = new System.Drawing.Point(4, 27);
            this.settingsComboBox.Name = "settingsComboBox";
            this.settingsComboBox.Size = new System.Drawing.Size(360, 24);
            this.settingsComboBox.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageHW);
            this.tabControl1.Controls.Add(this.tabPageManualControl);
            this.tabControl1.Controls.Add(this.tabPageScan);
            this.tabControl1.Controls.Add(this.tabPagePlotView);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(791, 417);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageHW
            // 
            this.tabPageHW.Controls.Add(this.checkedListBoxHardware);
            this.tabPageHW.Controls.Add(this.buttonRemoveHW);
            this.tabPageHW.Controls.Add(this.buttonAddHW);
            this.tabPageHW.Controls.Add(this.label4);
            this.tabPageHW.Location = new System.Drawing.Point(4, 22);
            this.tabPageHW.Name = "tabPageHW";
            this.tabPageHW.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHW.Size = new System.Drawing.Size(783, 391);
            this.tabPageHW.TabIndex = 0;
            this.tabPageHW.Text = "Hardware Settings";
            this.tabPageHW.UseVisualStyleBackColor = true;
            // 
            // checkedListBoxHardware
            // 
            this.checkedListBoxHardware.FormattingEnabled = true;
            this.checkedListBoxHardware.Location = new System.Drawing.Point(14, 29);
            this.checkedListBoxHardware.Name = "checkedListBoxHardware";
            this.checkedListBoxHardware.Size = new System.Drawing.Size(185, 169);
            this.checkedListBoxHardware.TabIndex = 2;
            this.checkedListBoxHardware.ThreeDCheckBoxes = true;
            this.checkedListBoxHardware.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxHardware_ItemCheck);
            this.checkedListBoxHardware.SelectedValueChanged += new System.EventHandler(this.checkedListBoxHardware_SelectedValueChanged);
            // 
            // buttonRemoveHW
            // 
            this.buttonRemoveHW.Location = new System.Drawing.Point(205, 58);
            this.buttonRemoveHW.Name = "buttonRemoveHW";
            this.buttonRemoveHW.Size = new System.Drawing.Size(109, 23);
            this.buttonRemoveHW.TabIndex = 5;
            this.buttonRemoveHW.Text = "Remove HW";
            this.buttonRemoveHW.UseVisualStyleBackColor = true;
            this.buttonRemoveHW.Click += new System.EventHandler(this.buttonRemoveHW_Click);
            // 
            // buttonAddHW
            // 
            this.buttonAddHW.Location = new System.Drawing.Point(205, 29);
            this.buttonAddHW.Name = "buttonAddHW";
            this.buttonAddHW.Size = new System.Drawing.Size(109, 23);
            this.buttonAddHW.TabIndex = 4;
            this.buttonAddHW.Text = "Add HW";
            this.buttonAddHW.UseVisualStyleBackColor = true;
            this.buttonAddHW.Click += new System.EventHandler(this.buttonAddHW_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Available Hardware";
            // 
            // tabPageManualControl
            // 
            this.tabPageManualControl.Controls.Add(this.btnStopMovement);
            this.tabPageManualControl.Controls.Add(this.txtZSpeed);
            this.tabPageManualControl.Controls.Add(this.txtYSpeed);
            this.tabPageManualControl.Controls.Add(this.btnMoveAbsolute);
            this.tabPageManualControl.Controls.Add(this.btnMoveRelative);
            this.tabPageManualControl.Controls.Add(this.txtXSpeed);
            this.tabPageManualControl.Controls.Add(this.label10);
            this.tabPageManualControl.Controls.Add(this.comboBox_ManualPositioner);
            this.tabPageManualControl.Controls.Add(this.label9);
            this.tabPageManualControl.Controls.Add(this.txtZIncrement);
            this.tabPageManualControl.Controls.Add(this.txtYIncrement);
            this.tabPageManualControl.Controls.Add(this.txtXIncrement);
            this.tabPageManualControl.Controls.Add(this.label8);
            this.tabPageManualControl.Controls.Add(this.label7);
            this.tabPageManualControl.Controls.Add(this.btnMoveDown);
            this.tabPageManualControl.Controls.Add(this.btnMoveUp);
            this.tabPageManualControl.Controls.Add(this.groupBox2);
            this.tabPageManualControl.Controls.Add(this.buttonSetHome);
            this.tabPageManualControl.Controls.Add(this.buttonDelPosition);
            this.tabPageManualControl.Controls.Add(this.buttonMoveToPos);
            this.tabPageManualControl.Controls.Add(this.btnSavePosition);
            this.tabPageManualControl.Controls.Add(this.label6);
            this.tabPageManualControl.Controls.Add(this.btnMoveRight);
            this.tabPageManualControl.Controls.Add(this.btnMoveLeft);
            this.tabPageManualControl.Controls.Add(this.btnMoveForward);
            this.tabPageManualControl.Controls.Add(this.btnMoveBackward);
            this.tabPageManualControl.Controls.Add(this.listBoxStoredPosition);
            this.tabPageManualControl.Location = new System.Drawing.Point(4, 22);
            this.tabPageManualControl.Name = "tabPageManualControl";
            this.tabPageManualControl.Size = new System.Drawing.Size(783, 391);
            this.tabPageManualControl.TabIndex = 4;
            this.tabPageManualControl.Text = "Manual Control";
            this.tabPageManualControl.UseVisualStyleBackColor = true;
            // 
            // btnStopMovement
            // 
            this.btnStopMovement.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStopMovement.Location = new System.Drawing.Point(395, 186);
            this.btnStopMovement.Name = "btnStopMovement";
            this.btnStopMovement.Size = new System.Drawing.Size(34, 34);
            this.btnStopMovement.TabIndex = 52;
            this.btnStopMovement.Text = "S";
            this.btnStopMovement.UseVisualStyleBackColor = true;
            // 
            // txtZSpeed
            // 
            this.txtZSpeed.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.txtZSpeed.Location = new System.Drawing.Point(488, 109);
            this.txtZSpeed.Name = "txtZSpeed";
            this.txtZSpeed.Size = new System.Drawing.Size(100, 20);
            this.txtZSpeed.TabIndex = 51;
            this.txtZSpeed.Text = "1000";
            // 
            // txtYSpeed
            // 
            this.txtYSpeed.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.txtYSpeed.Location = new System.Drawing.Point(488, 83);
            this.txtYSpeed.Name = "txtYSpeed";
            this.txtYSpeed.Size = new System.Drawing.Size(100, 20);
            this.txtYSpeed.TabIndex = 50;
            this.txtYSpeed.Text = "1000";
            // 
            // btnMoveAbsolute
            // 
            this.btnMoveAbsolute.Location = new System.Drawing.Point(488, 168);
            this.btnMoveAbsolute.Name = "btnMoveAbsolute";
            this.btnMoveAbsolute.Size = new System.Drawing.Size(100, 24);
            this.btnMoveAbsolute.TabIndex = 49;
            this.btnMoveAbsolute.Text = "Move absolute";
            this.btnMoveAbsolute.UseVisualStyleBackColor = true;
            // 
            // btnMoveRelative
            // 
            this.btnMoveRelative.Location = new System.Drawing.Point(488, 141);
            this.btnMoveRelative.Name = "btnMoveRelative";
            this.btnMoveRelative.Size = new System.Drawing.Size(100, 21);
            this.btnMoveRelative.TabIndex = 48;
            this.btnMoveRelative.Text = "Move relative";
            this.btnMoveRelative.UseVisualStyleBackColor = true;
            this.btnMoveRelative.Click += new System.EventHandler(this.btnMoveRelative_Click);
            // 
            // txtXSpeed
            // 
            this.txtXSpeed.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.txtXSpeed.Location = new System.Drawing.Point(488, 57);
            this.txtXSpeed.Name = "txtXSpeed";
            this.txtXSpeed.Size = new System.Drawing.Size(100, 20);
            this.txtXSpeed.TabIndex = 47;
            this.txtXSpeed.Text = "1000";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(342, 4);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(78, 16);
            this.label10.TabIndex = 46;
            this.label10.Text = "Positioner";
            // 
            // comboBox_ManualPositioner
            // 
            this.comboBox_ManualPositioner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ManualPositioner.FormattingEnabled = true;
            this.comboBox_ManualPositioner.Location = new System.Drawing.Point(345, 29);
            this.comboBox_ManualPositioner.Name = "comboBox_ManualPositioner";
            this.comboBox_ManualPositioner.Size = new System.Drawing.Size(125, 21);
            this.comboBox_ManualPositioner.TabIndex = 45;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(347, 109);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 16);
            this.label9.TabIndex = 44;
            this.label9.Text = "Z";
            // 
            // txtZIncrement
            // 
            this.txtZIncrement.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.txtZIncrement.Location = new System.Drawing.Point(370, 108);
            this.txtZIncrement.Name = "txtZIncrement";
            this.txtZIncrement.Size = new System.Drawing.Size(100, 20);
            this.txtZIncrement.TabIndex = 43;
            this.txtZIncrement.Text = "1000";
            // 
            // txtYIncrement
            // 
            this.txtYIncrement.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.txtYIncrement.Location = new System.Drawing.Point(370, 82);
            this.txtYIncrement.Name = "txtYIncrement";
            this.txtYIncrement.Size = new System.Drawing.Size(100, 20);
            this.txtYIncrement.TabIndex = 42;
            this.txtYIncrement.Text = "1000";
            // 
            // txtXIncrement
            // 
            this.txtXIncrement.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.txtXIncrement.Location = new System.Drawing.Point(370, 56);
            this.txtXIncrement.Name = "txtXIncrement";
            this.txtXIncrement.Size = new System.Drawing.Size(100, 20);
            this.txtXIncrement.TabIndex = 41;
            this.txtXIncrement.Text = "1000";
            this.txtXIncrement.TextChanged += new System.EventHandler(this.textBoxPosX_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(347, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(18, 16);
            this.label8.TabIndex = 40;
            this.label8.Text = "Y";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(347, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 16);
            this.label7.TabIndex = 39;
            this.label7.Text = "X";
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveDown.Location = new System.Drawing.Point(395, 224);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(34, 34);
            this.btnMoveDown.TabIndex = 38;
            this.btnMoveDown.Text = "Z-";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveUp.Location = new System.Drawing.Point(395, 147);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(34, 34);
            this.btnMoveUp.TabIndex = 37;
            this.btnMoveUp.Text = "Z+";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_StartLive);
            this.groupBox2.Controls.Add(this.plotView_ManualTab);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.comboBox_ManualInput);
            this.groupBox2.Location = new System.Drawing.Point(14, 299);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(426, 269);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Live Input";
            // 
            // button_StartLive
            // 
            this.button_StartLive.Location = new System.Drawing.Point(229, 28);
            this.button_StartLive.Name = "button_StartLive";
            this.button_StartLive.Size = new System.Drawing.Size(109, 23);
            this.button_StartLive.TabIndex = 35;
            this.button_StartLive.Text = "Start Input";
            this.button_StartLive.UseVisualStyleBackColor = true;
            this.button_StartLive.Click += new System.EventHandler(this.button_StartLive_Click);
            // 
            // plotView_ManualTab
            // 
            this.plotView_ManualTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plotView_ManualTab.Location = new System.Drawing.Point(25, 60);
            this.plotView_ManualTab.Name = "plotView_ManualTab";
            this.plotView_ManualTab.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plotView_ManualTab.Size = new System.Drawing.Size(378, 188);
            this.plotView_ManualTab.TabIndex = 28;
            this.plotView_ManualTab.Text = "plotView2";
            this.plotView_ManualTab.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plotView_ManualTab.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plotView_ManualTab.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(22, 31);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 16);
            this.label11.TabIndex = 34;
            this.label11.Text = "Channel";
            // 
            // comboBox_ManualInput
            // 
            this.comboBox_ManualInput.FormattingEnabled = true;
            this.comboBox_ManualInput.Location = new System.Drawing.Point(92, 29);
            this.comboBox_ManualInput.Name = "comboBox_ManualInput";
            this.comboBox_ManualInput.Size = new System.Drawing.Size(120, 21);
            this.comboBox_ManualInput.TabIndex = 33;
            this.comboBox_ManualInput.SelectedIndexChanged += new System.EventHandler(this.comboBox_ManualInput_SelectedIndexChanged);
            // 
            // buttonSetHome
            // 
            this.buttonSetHome.Location = new System.Drawing.Point(205, 88);
            this.buttonSetHome.Name = "buttonSetHome";
            this.buttonSetHome.Size = new System.Drawing.Size(109, 23);
            this.buttonSetHome.TabIndex = 26;
            this.buttonSetHome.Text = "Set Home Position";
            this.buttonSetHome.UseVisualStyleBackColor = true;
            this.buttonSetHome.Click += new System.EventHandler(this.buttonSetHome_Click);
            // 
            // buttonDelPosition
            // 
            this.buttonDelPosition.Location = new System.Drawing.Point(205, 159);
            this.buttonDelPosition.Name = "buttonDelPosition";
            this.buttonDelPosition.Size = new System.Drawing.Size(109, 23);
            this.buttonDelPosition.TabIndex = 8;
            this.buttonDelPosition.Text = "Delete Position";
            this.buttonDelPosition.UseVisualStyleBackColor = true;
            this.buttonDelPosition.Click += new System.EventHandler(this.buttonDelPosition_Click);
            // 
            // buttonMoveToPos
            // 
            this.buttonMoveToPos.Location = new System.Drawing.Point(205, 59);
            this.buttonMoveToPos.Name = "buttonMoveToPos";
            this.buttonMoveToPos.Size = new System.Drawing.Size(109, 23);
            this.buttonMoveToPos.TabIndex = 7;
            this.buttonMoveToPos.Text = "Move to Position";
            this.buttonMoveToPos.UseVisualStyleBackColor = true;
            this.buttonMoveToPos.Click += new System.EventHandler(this.buttonMoveToPos_Click);
            // 
            // btnSavePosition
            // 
            this.btnSavePosition.Location = new System.Drawing.Point(205, 30);
            this.btnSavePosition.Name = "btnSavePosition";
            this.btnSavePosition.Size = new System.Drawing.Size(109, 23);
            this.btnSavePosition.TabIndex = 6;
            this.btnSavePosition.Text = "Store Position";
            this.btnSavePosition.UseVisualStyleBackColor = true;
            this.btnSavePosition.Click += new System.EventHandler(this.buttonStorePosition_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Stored Positions";
            // 
            // listBoxStoredPosition
            // 
            this.listBoxStoredPosition.FormattingEnabled = true;
            this.listBoxStoredPosition.Location = new System.Drawing.Point(14, 30);
            this.listBoxStoredPosition.Name = "listBoxStoredPosition";
            this.listBoxStoredPosition.Size = new System.Drawing.Size(185, 225);
            this.listBoxStoredPosition.Sorted = true;
            this.listBoxStoredPosition.TabIndex = 4;
            // 
            // tabPageScan
            // 
            this.tabPageScan.Controls.Add(this.buttonDeleteExp);
            this.tabPageScan.Controls.Add(this.buttonAddExp);
            this.tabPageScan.Controls.Add(this.treeViewScanMethod);
            this.tabPageScan.Controls.Add(this.label3);
            this.tabPageScan.Controls.Add(this.buttonIncLevel);
            this.tabPageScan.Controls.Add(this.buttonDecLevel);
            this.tabPageScan.Controls.Add(this.buttonDown);
            this.tabPageScan.Controls.Add(this.buttonUp);
            this.tabPageScan.Location = new System.Drawing.Point(4, 22);
            this.tabPageScan.Name = "tabPageScan";
            this.tabPageScan.Size = new System.Drawing.Size(783, 391);
            this.tabPageScan.TabIndex = 2;
            this.tabPageScan.Text = "Scan Method";
            this.tabPageScan.UseVisualStyleBackColor = true;
            // 
            // buttonDeleteExp
            // 
            this.buttonDeleteExp.Location = new System.Drawing.Point(115, 248);
            this.buttonDeleteExp.Name = "buttonDeleteExp";
            this.buttonDeleteExp.Size = new System.Drawing.Size(94, 23);
            this.buttonDeleteExp.TabIndex = 20;
            this.buttonDeleteExp.Text = "Remove";
            this.buttonDeleteExp.UseVisualStyleBackColor = true;
            this.buttonDeleteExp.Click += new System.EventHandler(this.buttonDeleteExp_Click);
            // 
            // buttonAddExp
            // 
            this.buttonAddExp.Location = new System.Drawing.Point(14, 248);
            this.buttonAddExp.Name = "buttonAddExp";
            this.buttonAddExp.Size = new System.Drawing.Size(95, 23);
            this.buttonAddExp.TabIndex = 19;
            this.buttonAddExp.Text = "Add Experiment";
            this.buttonAddExp.UseVisualStyleBackColor = true;
            this.buttonAddExp.Click += new System.EventHandler(this.buttonAddExp_Click);
            // 
            // treeViewScanMethod
            // 
            this.treeViewScanMethod.FullRowSelect = true;
            this.treeViewScanMethod.HideSelection = false;
            this.treeViewScanMethod.Location = new System.Drawing.Point(14, 29);
            this.treeViewScanMethod.Name = "treeViewScanMethod";
            this.treeViewScanMethod.Size = new System.Drawing.Size(195, 212);
            this.treeViewScanMethod.TabIndex = 16;
            this.treeViewScanMethod.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewScanMethod_AfterSelect);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Experiments in method";
            // 
            // tabPagePlotView
            // 
            this.tabPagePlotView.Controls.Add(this.splitContainer2);
            this.tabPagePlotView.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlotView.Name = "tabPagePlotView";
            this.tabPagePlotView.Size = new System.Drawing.Size(783, 391);
            this.tabPagePlotView.TabIndex = 3;
            this.tabPagePlotView.Text = "Data View";
            this.tabPagePlotView.UseVisualStyleBackColor = true;
            this.tabPagePlotView.Click += new System.EventHandler(this.tabPagePlotView_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.label13);
            this.splitContainer2.Panel1.Controls.Add(this.comboBoxScanDisplayChannel);
            this.splitContainer2.Panel1.Controls.Add(this.label5);
            this.splitContainer2.Panel1.Controls.Add(this.plotViewScan);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.label12);
            this.splitContainer2.Panel2.Controls.Add(this.plotView1);
            this.splitContainer2.Size = new System.Drawing.Size(777, 385);
            this.splitContainer2.SplitterDistance = 346;
            this.splitContainer2.TabIndex = 50;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(14, 51);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(49, 13);
            this.label13.TabIndex = 51;
            this.label13.Text = "Channel:";
            // 
            // comboBoxScanDisplayChannel
            // 
            this.comboBoxScanDisplayChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxScanDisplayChannel.FormattingEnabled = true;
            this.comboBoxScanDisplayChannel.Location = new System.Drawing.Point(69, 48);
            this.comboBoxScanDisplayChannel.Name = "comboBoxScanDisplayChannel";
            this.comboBoxScanDisplayChannel.Size = new System.Drawing.Size(130, 21);
            this.comboBoxScanDisplayChannel.TabIndex = 50;
            this.comboBoxScanDisplayChannel.SelectedValueChanged += new System.EventHandler(this.comboBoxScanDisplayChannel_SelectedValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(14, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 16);
            this.label5.TabIndex = 47;
            this.label5.Text = "Scan";
            // 
            // plotViewScan
            // 
            this.plotViewScan.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plotViewScan.Location = new System.Drawing.Point(17, 75);
            this.plotViewScan.Name = "plotViewScan";
            this.plotViewScan.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plotViewScan.Size = new System.Drawing.Size(314, 285);
            this.plotViewScan.TabIndex = 49;
            this.plotViewScan.Text = "plotView2";
            this.plotViewScan.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plotViewScan.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plotViewScan.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(14, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(37, 16);
            this.label12.TabIndex = 48;
            this.label12.Text = "Live";
            // 
            // plotView1
            // 
            this.plotView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plotView1.Location = new System.Drawing.Point(17, 39);
            this.plotView1.Name = "plotView1";
            this.plotView1.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plotView1.Size = new System.Drawing.Size(391, 321);
            this.plotView1.TabIndex = 0;
            this.plotView1.Text = "plotView1";
            this.plotView1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plotView1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plotView1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // informationToolStripMenuItem
            // 
            this.informationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.informationToolStripMenuItem.Name = "informationToolStripMenuItem";
            this.informationToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.informationToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // btnMoveRight
            // 
            this.btnMoveRight.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveRight.Image")));
            this.btnMoveRight.Location = new System.Drawing.Point(435, 186);
            this.btnMoveRight.Name = "btnMoveRight";
            this.btnMoveRight.Size = new System.Drawing.Size(34, 34);
            this.btnMoveRight.TabIndex = 36;
            this.btnMoveRight.UseVisualStyleBackColor = true;
            // 
            // btnMoveLeft
            // 
            this.btnMoveLeft.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveLeft.Image")));
            this.btnMoveLeft.Location = new System.Drawing.Point(355, 184);
            this.btnMoveLeft.Name = "btnMoveLeft";
            this.btnMoveLeft.Size = new System.Drawing.Size(34, 34);
            this.btnMoveLeft.TabIndex = 35;
            this.btnMoveLeft.UseVisualStyleBackColor = true;
            // 
            // btnMoveForward
            // 
            this.btnMoveForward.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveForward.Image")));
            this.btnMoveForward.Location = new System.Drawing.Point(355, 224);
            this.btnMoveForward.Name = "btnMoveForward";
            this.btnMoveForward.Size = new System.Drawing.Size(34, 34);
            this.btnMoveForward.TabIndex = 34;
            this.btnMoveForward.UseVisualStyleBackColor = true;
            // 
            // btnMoveBackward
            // 
            this.btnMoveBackward.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveBackward.Image")));
            this.btnMoveBackward.Location = new System.Drawing.Point(435, 147);
            this.btnMoveBackward.Name = "btnMoveBackward";
            this.btnMoveBackward.Size = new System.Drawing.Size(34, 34);
            this.btnMoveBackward.TabIndex = 33;
            this.btnMoveBackward.UseVisualStyleBackColor = true;
            // 
            // buttonIncLevel
            // 
            this.buttonIncLevel.Image = ((System.Drawing.Image)(resources.GetObject("buttonIncLevel.Image")));
            this.buttonIncLevel.Location = new System.Drawing.Point(215, 136);
            this.buttonIncLevel.Name = "buttonIncLevel";
            this.buttonIncLevel.Size = new System.Drawing.Size(34, 34);
            this.buttonIncLevel.TabIndex = 18;
            this.buttonIncLevel.UseVisualStyleBackColor = true;
            this.buttonIncLevel.Click += new System.EventHandler(this.buttonIncLevel_Click);
            // 
            // buttonDecLevel
            // 
            this.buttonDecLevel.Image = ((System.Drawing.Image)(resources.GetObject("buttonDecLevel.Image")));
            this.buttonDecLevel.Location = new System.Drawing.Point(215, 176);
            this.buttonDecLevel.Name = "buttonDecLevel";
            this.buttonDecLevel.Size = new System.Drawing.Size(34, 34);
            this.buttonDecLevel.TabIndex = 17;
            this.buttonDecLevel.UseVisualStyleBackColor = true;
            this.buttonDecLevel.Click += new System.EventHandler(this.buttonDecLevel_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Image = ((System.Drawing.Image)(resources.GetObject("buttonDown.Image")));
            this.buttonDown.Location = new System.Drawing.Point(215, 96);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(34, 34);
            this.buttonDown.TabIndex = 13;
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.Image = ((System.Drawing.Image)(resources.GetObject("buttonUp.Image")));
            this.buttonUp.Location = new System.Drawing.Point(215, 56);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(34, 34);
            this.buttonUp.TabIndex = 12;
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // toolStripButtonSaveSettings
            // 
            this.toolStripButtonSaveSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveSettings.Image = global::Scaneva.Properties.Resources.Save_HW_green_red;
            this.toolStripButtonSaveSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveSettings.Name = "toolStripButtonSaveSettings";
            this.toolStripButtonSaveSettings.Size = new System.Drawing.Size(36, 36);
            this.toolStripButtonSaveSettings.Text = "toolStripButton1";
            this.toolStripButtonSaveSettings.ToolTipText = "Save Settings";
            this.toolStripButtonSaveSettings.Click += new System.EventHandler(this.toolStripButtonSaveSettings_Click);
            // 
            // toolStripButtonConnect
            // 
            this.toolStripButtonConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonConnect.Image = global::Scaneva.Properties.Resources.plug_green;
            this.toolStripButtonConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConnect.Name = "toolStripButtonConnect";
            this.toolStripButtonConnect.Size = new System.Drawing.Size(36, 36);
            this.toolStripButtonConnect.Text = "Initialize HW";
            this.toolStripButtonConnect.Click += new System.EventHandler(this.toolStripButtonConnect_Click);
            // 
            // toolStripLoadMethod
            // 
            this.toolStripLoadMethod.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLoadMethod.Image = global::Scaneva.Properties.Resources.Open_folder_green;
            this.toolStripLoadMethod.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripLoadMethod.Name = "toolStripLoadMethod";
            this.toolStripLoadMethod.Size = new System.Drawing.Size(36, 36);
            this.toolStripLoadMethod.Text = "toolStripButton2";
            this.toolStripLoadMethod.ToolTipText = "Load Method";
            this.toolStripLoadMethod.Click += new System.EventHandler(this.toolStripLoadMethod_Click);
            // 
            // toolStripButtonSaveMethod
            // 
            this.toolStripButtonSaveMethod.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveMethod.Image = global::Scaneva.Properties.Resources.Save_method_green_red;
            this.toolStripButtonSaveMethod.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolStripButtonSaveMethod.Name = "toolStripButtonSaveMethod";
            this.toolStripButtonSaveMethod.Size = new System.Drawing.Size(36, 36);
            this.toolStripButtonSaveMethod.Text = "Save Method";
            this.toolStripButtonSaveMethod.Click += new System.EventHandler(this.toolStripButtonSaveMethod_Click);
            // 
            // toolStripButtonRun
            // 
            this.toolStripButtonRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRun.Image = global::Scaneva.Properties.Resources.Run_grey;
            this.toolStripButtonRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRun.Name = "toolStripButtonRun";
            this.toolStripButtonRun.Size = new System.Drawing.Size(36, 36);
            this.toolStripButtonRun.Text = "Run";
            this.toolStripButtonRun.Click += new System.EventHandler(this.toolStripButtonRun_Click);
            // 
            // toolStripButtonStop
            // 
            this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStop.Image = global::Scaneva.Properties.Resources.Stop_grey;
            this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStop.Name = "toolStripButtonStop";
            this.toolStripButtonStop.Size = new System.Drawing.Size(36, 36);
            this.toolStripButtonStop.Text = "Stop";
            this.toolStripButtonStop.ToolTipText = "Stop";
            this.toolStripButtonStop.Click += new System.EventHandler(this.toolStripButtonStop_Click);
            // 
            // ScanevaMainGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1178, 494);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mainToolStrip);
            this.Controls.Add(this.mainStatusStrip);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "ScanevaMainGUI";
            this.Text = "Scaneva";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScanevaMainGUI_FormClosing);
            this.mainStatusStrip.ResumeLayout(false);
            this.mainStatusStrip.PerformLayout();
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageHW.ResumeLayout(false);
            this.tabPageHW.PerformLayout();
            this.tabPageManualControl.ResumeLayout(false);
            this.tabPageManualControl.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPageScan.ResumeLayout(false);
            this.tabPageScan.PerformLayout();
            this.tabPagePlotView.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveSettings;
        private System.Windows.Forms.ToolStripButton toolStripLoadMethod;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox settingsComboBox;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveMethod;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonStop;
        private System.Windows.Forms.ToolStripButton toolStripButtonRun;
        private System.Windows.Forms.ToolStripButton toolStripButtonConnect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageHW;
        private System.Windows.Forms.CheckedListBox checkedListBoxHardware;
        private System.Windows.Forms.Button buttonRemoveHW;
        private System.Windows.Forms.Button buttonAddHW;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPageManualControl;
        private System.Windows.Forms.GroupBox groupBox2;
        private OxyPlot.WindowsForms.PlotView plotView_ManualTab;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBox_ManualInput;
        private System.Windows.Forms.Button buttonSetHome;
        private System.Windows.Forms.Button buttonDelPosition;
        private System.Windows.Forms.Button buttonMoveToPos;
        private System.Windows.Forms.Button btnSavePosition;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox listBoxStoredPosition;
        private System.Windows.Forms.TabPage tabPageScan;
        private System.Windows.Forms.Button buttonIncLevel;
        private System.Windows.Forms.Button buttonDecLevel;
        private System.Windows.Forms.TreeView treeViewScanMethod;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusCurrentPosition;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboBox_ManualPositioner;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtZIncrement;
        private System.Windows.Forms.TextBox txtYIncrement;
        private System.Windows.Forms.TextBox txtXIncrement;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveRight;
        private System.Windows.Forms.Button btnMoveLeft;
        private System.Windows.Forms.Button btnMoveForward;
        private System.Windows.Forms.Button btnMoveBackward;
        private System.Windows.Forms.TextBox txtXSpeed;
        private System.Windows.Forms.Button btnMoveAbsolute;
        private System.Windows.Forms.Button btnMoveRelative;
        private System.Windows.Forms.TextBox txtZSpeed;
        private System.Windows.Forms.TextBox txtYSpeed;
        private System.Windows.Forms.Button btnStopMovement;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusError;
        private System.Windows.Forms.TabPage tabPagePlotView;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private OxyPlot.WindowsForms.PlotView plotViewScan;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label5;
        private OxyPlot.WindowsForms.PlotView plotView1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox comboBoxScanDisplayChannel;
        private System.Windows.Forms.Button button_StartLive;
        private System.Windows.Forms.ToolStripMenuItem editSettingsToolStripMenuItem;
        private System.Windows.Forms.Button buttonDeleteExp;
        private System.Windows.Forms.Button buttonAddExp;
        private System.Windows.Forms.ToolStripMenuItem informationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

