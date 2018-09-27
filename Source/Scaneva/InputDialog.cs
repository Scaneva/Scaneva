#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="InputDialog.cs" company="Scaneva">
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

namespace Scaneva
{
    public partial class InputDialog : Form
    {
        public InputDialog()
        {
            InitializeComponent();

            // Disable resize
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private int desiredStartLocationX = -1;
        private int desiredStartLocationY = -1;

        public InputDialog(int x, int y)
               : this()
        {
            // here store the value for x & y into instance variables
            this.desiredStartLocationX = x;
            this.desiredStartLocationY = y;
        }

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

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InputDialog_Shown(object sender, EventArgs e)
        {
            textBoxEntry.Text = null;
        }

        private void InputDialog_Load(object sender, EventArgs e)
        {
            if ((desiredStartLocationX != -1) && (desiredStartLocationY != -1))
            {
                this.SetDesktopLocation(desiredStartLocationX, desiredStartLocationY);
            }
        }
    }
}
