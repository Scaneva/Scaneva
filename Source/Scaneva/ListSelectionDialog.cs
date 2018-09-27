#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="ListSelectionDialog.cs" company="Scaneva">
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Scaneva.Tools;

namespace Scaneva
{
    public partial class ListSelectionDialog : Form
    {
        public ListSelectionDialog()
        {
            InitializeComponent();
            SelectedValue = null;
        }

        private int desiredStartLocationX = -1;
        private int desiredStartLocationY = -1;

        public ListSelectionDialog(int x, int y)
               : this()
        {
            // here store the value for x & y into instance variables
            this.desiredStartLocationX = x;
            this.desiredStartLocationY = y;
        }

        private Dictionary<string, Type> selectionValues;
        private Dictionary<string, TreeNode> groups = new Dictionary<string, TreeNode>();

        public Dictionary<string, Type> SelectionValues
        {
            get
            {
                return selectionValues;
            }
            set
            {
                selectionValues = value;

                treeViewSelection.Nodes.Clear();
                groups.Clear();

                foreach (KeyValuePair<string, Type> kvp in value)
                {
                    // TODO add Groups
                    string grpStr = "Other";
                    var att = kvp.Value.GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault() as CategoryAttribute;
                    if (att != null)
                    {
                        grpStr = att.Category;
                    }

                    TreeNode tn = new TreeNode(kvp.Key);
                    tn.Tag = kvp.Value;

                    if (!groups.ContainsKey(grpStr))
                    {
                        TreeNode groupNode = new TreeNode(grpStr);
                        treeViewSelection.Nodes.Add(groupNode);
                        groups.Add(grpStr, groupNode);
                    }

                    groups[grpStr].Nodes.Add(tn);
                }

                treeViewSelection.Sort();
            }
        }

        public string SelectedValue { get; set; }
        public string TextEntry
        {
            get
            {
                return textBoxEntry.Text;
            }
            set
            {
                textBoxEntry.Text = value;
            }
        }

        public string TextEntryLabel
        {
            set
            {
                labelTextEntry.Text = value;
            }
        }

        public string ListBoxLabel
        {
            set
            {
                labelListBox.Text = value;
            }
        }

        private void ListSelectionDialog_Shown(object sender, EventArgs e)
        {
            SelectedValue = null;
            textBoxEntry.Text = null;
            treeViewSelection.SelectedNode = null;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if ((treeViewSelection.SelectedNode != null) && (treeViewSelection.SelectedNode.Tag != null))
            {
                this.SelectedValue = treeViewSelection.SelectedNode.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.SelectedValue = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ListSelectionDialog_Load(object sender, EventArgs e)
        {
            if ((desiredStartLocationX != -1) && (desiredStartLocationY != -1))
            {
                this.SetDesktopLocation(desiredStartLocationX, desiredStartLocationY);
            }
        }

        private void treeViewSelection_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((treeViewSelection.SelectedNode != null) && (treeViewSelection.SelectedNode.Tag != null))
            {
                string val = treeViewSelection.SelectedNode.Text;
                string helptext = selectionValues[val].getHelpText();
                if (helptext != null)
                {
                    richTextBoxHelp.Rtf = helptext;
                }
                else
                {
                    helptext = selectionValues[val].getDescription();
                    richTextBoxHelp.Rtf = "";
                    richTextBoxHelp.Text = helptext;
                }
            }
            else
            {
                richTextBoxHelp.Rtf = "";
            }
        }
    }
}
