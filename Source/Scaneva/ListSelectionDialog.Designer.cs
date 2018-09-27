namespace Scaneva
{
    partial class ListSelectionDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelListBox = new System.Windows.Forms.Label();
            this.labelTextEntry = new System.Windows.Forms.Label();
            this.textBoxEntry = new System.Windows.Forms.TextBox();
            this.richTextBoxHelp = new System.Windows.Forms.RichTextBox();
            this.treeViewSelection = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOk.Location = new System.Drawing.Point(12, 294);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(582, 294);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelListBox
            // 
            this.labelListBox.AutoSize = true;
            this.labelListBox.Location = new System.Drawing.Point(13, 14);
            this.labelListBox.Name = "labelListBox";
            this.labelListBox.Size = new System.Drawing.Size(64, 13);
            this.labelListBox.TabIndex = 3;
            this.labelListBox.Text = "Select Type";
            // 
            // labelTextEntry
            // 
            this.labelTextEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTextEntry.AutoSize = true;
            this.labelTextEntry.Location = new System.Drawing.Point(13, 246);
            this.labelTextEntry.Name = "labelTextEntry";
            this.labelTextEntry.Size = new System.Drawing.Size(35, 13);
            this.labelTextEntry.TabIndex = 4;
            this.labelTextEntry.Text = "Name";
            // 
            // textBoxEntry
            // 
            this.textBoxEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxEntry.Location = new System.Drawing.Point(12, 262);
            this.textBoxEntry.Name = "textBoxEntry";
            this.textBoxEntry.Size = new System.Drawing.Size(217, 20);
            this.textBoxEntry.TabIndex = 5;
            // 
            // richTextBoxHelp
            // 
            this.richTextBoxHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxHelp.Location = new System.Drawing.Point(245, 30);
            this.richTextBoxHelp.Name = "richTextBoxHelp";
            this.richTextBoxHelp.ReadOnly = true;
            this.richTextBoxHelp.Size = new System.Drawing.Size(412, 252);
            this.richTextBoxHelp.TabIndex = 6;
            this.richTextBoxHelp.Text = "";
            // 
            // treeViewSelection
            // 
            this.treeViewSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeViewSelection.Location = new System.Drawing.Point(12, 30);
            this.treeViewSelection.Name = "treeViewSelection";
            this.treeViewSelection.Size = new System.Drawing.Size(216, 186);
            this.treeViewSelection.TabIndex = 7;
            this.treeViewSelection.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSelection_AfterSelect);
            // 
            // ListSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 329);
            this.Controls.Add(this.treeViewSelection);
            this.Controls.Add(this.richTextBoxHelp);
            this.Controls.Add(this.textBoxEntry);
            this.Controls.Add(this.labelTextEntry);
            this.Controls.Add(this.labelListBox);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Name = "ListSelectionDialog";
            this.Text = "Add HW";
            this.Load += new System.EventHandler(this.ListSelectionDialog_Load);
            this.Shown += new System.EventHandler(this.ListSelectionDialog_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelListBox;
        private System.Windows.Forms.Label labelTextEntry;
        private System.Windows.Forms.TextBox textBoxEntry;
        private System.Windows.Forms.RichTextBox richTextBoxHelp;
        private System.Windows.Forms.TreeView treeViewSelection;
    }
}