namespace Scaneva
{
    partial class InputDialog
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
            this.textBoxEntry = new System.Windows.Forms.TextBox();
            this.labelTextEntry = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxEntry
            // 
            this.textBoxEntry.Location = new System.Drawing.Point(12, 31);
            this.textBoxEntry.Name = "textBoxEntry";
            this.textBoxEntry.Size = new System.Drawing.Size(215, 20);
            this.textBoxEntry.TabIndex = 9;
            // 
            // labelTextEntry
            // 
            this.labelTextEntry.AutoSize = true;
            this.labelTextEntry.Location = new System.Drawing.Point(13, 15);
            this.labelTextEntry.Name = "labelTextEntry";
            this.labelTextEntry.Size = new System.Drawing.Size(35, 13);
            this.labelTextEntry.TabIndex = 8;
            this.labelTextEntry.Text = "Name";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(153, 63);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(12, 63);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // InputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 99);
            this.Controls.Add(this.textBoxEntry);
            this.Controls.Add(this.labelTextEntry);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Name = "InputDialog";
            this.Text = "InputDialog";
            this.Load += new System.EventHandler(this.InputDialog_Load);
            this.Shown += new System.EventHandler(this.InputDialog_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxEntry;
        private System.Windows.Forms.Label labelTextEntry;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
    }
}