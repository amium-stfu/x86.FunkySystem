
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FunkySystem.UI
{
    partial class FormKeyboard
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
            panelQuertz = new FlowLayoutPanel();
            tbResult = new TextBox();
            tableKeypad = new TableLayoutPanel();
            panelNumBlock = new FlowLayoutPanel();
            btnClear = new Button();
            btnRemove = new Button();
            panel1 = new Panel();
            btnPaste = new Button();
            btnCopy = new Button();
            btnAbort = new Button();
            btnCheck = new Button();
            tableKeypad.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panelQuertz
            // 
            panelQuertz.Dock = DockStyle.Fill;
            panelQuertz.Location = new Point(6, 151);
            panelQuertz.Margin = new Padding(0);
            panelQuertz.Name = "panelQuertz";
            panelQuertz.Size = new Size(1363, 412);
            panelQuertz.TabIndex = 0;
            // 
            // tbResult
            // 
            tbResult.BackColor = Color.LightGray;
            tbResult.BorderStyle = BorderStyle.None;
            tableKeypad.SetColumnSpan(tbResult, 2);
            tbResult.Dock = DockStyle.Fill;
            tbResult.Font = new Font("Calibri", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tbResult.Location = new Point(10, 15);
            tbResult.Margin = new Padding(4, 3, 4, 3);
            tbResult.Multiline = true;
            tbResult.Name = "tbResult";
            tbResult.Size = new Size(1619, 133);
            tbResult.TabIndex = 16;
            tbResult.KeyDown += tbResult_KeyDown;
            tbResult.KeyPress += tbResult_KeyPress;
            // 
            // tableKeypad
            // 
            tableKeypad.BackColor = SystemColors.ActiveCaptionText;
            tableKeypad.ColumnCount = 6;
            tableKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 6F));
            tableKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75.38746F));
            tableKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.60552F));
            tableKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5.003508F));
            tableKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5.003508F));
            tableKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16F));
            tableKeypad.Controls.Add(tbResult, 1, 1);
            tableKeypad.Controls.Add(panelQuertz, 1, 2);
            tableKeypad.Controls.Add(panelNumBlock, 2, 2);
            tableKeypad.Controls.Add(btnClear, 4, 1);
            tableKeypad.Controls.Add(btnRemove, 3, 1);
            tableKeypad.Controls.Add(panel1, 4, 2);
            tableKeypad.Dock = DockStyle.Fill;
            tableKeypad.Location = new Point(0, 0);
            tableKeypad.Margin = new Padding(4, 3, 4, 3);
            tableKeypad.Name = "tableKeypad";
            tableKeypad.RowCount = 4;
            tableKeypad.RowStyles.Add(new RowStyle(SizeType.Absolute, 12F));
            tableKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 74F));
            tableKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 1F));
            tableKeypad.Size = new Size(1830, 570);
            tableKeypad.TabIndex = 1;
            // 
            // panelNumBlock
            // 
            tableKeypad.SetColumnSpan(panelNumBlock, 2);
            panelNumBlock.Dock = DockStyle.Fill;
            panelNumBlock.Location = new Point(1373, 154);
            panelNumBlock.Margin = new Padding(4, 3, 4, 3);
            panelNumBlock.Name = "panelNumBlock";
            panelNumBlock.Size = new Size(346, 406);
            panelNumBlock.TabIndex = 18;
            // 
            // btnClear
            // 
            btnClear.BackColor = SystemColors.ActiveBorder;
            btnClear.Dock = DockStyle.Fill;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClear.Location = new Point(1727, 15);
            btnClear.Margin = new Padding(4, 3, 4, 3);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(82, 133);
            btnClear.TabIndex = 19;
            btnClear.Text = "DEL";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += btnClear_Click;
            // 
            // btnRemove
            // 
            btnRemove.BackColor = SystemColors.ActiveBorder;
            btnRemove.Dock = DockStyle.Fill;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRemove.Location = new Point(1637, 15);
            btnRemove.Margin = new Padding(4, 3, 4, 3);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(82, 133);
            btnRemove.TabIndex = 19;
            btnRemove.Text = "<";
            btnRemove.UseVisualStyleBackColor = false;
            btnRemove.Click += btnRemove_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnPaste);
            panel1.Controls.Add(btnCopy);
            panel1.Controls.Add(btnAbort);
            panel1.Controls.Add(btnCheck);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(1727, 154);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(82, 406);
            panel1.TabIndex = 20;
            // 
            // btnPaste
            // 
            btnPaste.BackColor = SystemColors.ActiveBorder;
            btnPaste.Dock = DockStyle.Top;
            btnPaste.FlatStyle = FlatStyle.Flat;
            btnPaste.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPaste.Location = new Point(0, 39);
            btnPaste.Margin = new Padding(4, 3, 4, 3);
            btnPaste.Name = "btnPaste";
            btnPaste.Size = new Size(82, 39);
            btnPaste.TabIndex = 19;
            btnPaste.Text = "Paste";
            btnPaste.UseVisualStyleBackColor = false;
            btnPaste.Click += btnPaste_Click;
            // 
            // btnCopy
            // 
            btnCopy.BackColor = SystemColors.ActiveBorder;
            btnCopy.Dock = DockStyle.Top;
            btnCopy.FlatStyle = FlatStyle.Flat;
            btnCopy.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCopy.Location = new Point(0, 0);
            btnCopy.Margin = new Padding(4, 3, 4, 3);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(82, 39);
            btnCopy.TabIndex = 19;
            btnCopy.Text = "Copy";
            btnCopy.UseVisualStyleBackColor = false;
            btnCopy.Click += btnCopy_Click;
            // 
            // btnAbort
            // 
            btnAbort.BackColor = SystemColors.ActiveBorder;
            btnAbort.Dock = DockStyle.Bottom;
            btnAbort.FlatStyle = FlatStyle.Flat;
            btnAbort.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnAbort.Location = new Point(0, 277);
            btnAbort.Margin = new Padding(4, 3, 4, 3);
            btnAbort.Name = "btnAbort";
            btnAbort.Size = new Size(82, 39);
            btnAbort.TabIndex = 19;
            btnAbort.UseVisualStyleBackColor = false;
            btnAbort.Click += btnAbort_Click;
            // 
            // btnCheck
            // 
            btnCheck.BackColor = SystemColors.ActiveBorder;
            btnCheck.Dock = DockStyle.Bottom;
            btnCheck.FlatStyle = FlatStyle.Flat;
            btnCheck.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnCheck.Location = new Point(0, 316);
            btnCheck.Margin = new Padding(4, 3, 4, 3);
            btnCheck.Name = "btnCheck";
            btnCheck.Size = new Size(82, 90);
            btnCheck.TabIndex = 19;
            btnCheck.UseVisualStyleBackColor = false;
            btnCheck.Click += btnCheck_Click;
            // 
            // FormKeyboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1830, 570);
            Controls.Add(tableKeypad);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            Name = "FormKeyboard";
            StartPosition = FormStartPosition.Manual;
            Text = "Keyboard";
            FormClosing += FormKeyboard_FormClosing;
            Shown += Keyboard_Shown;
            tableKeypad.ResumeLayout(false);
            tableKeypad.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.FlowLayoutPanel panelQuertz;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.TableLayoutPanel tableKeypad;
        private FlowLayoutPanel panelNumBlock;
 
        private Button btnCheck;
        private Button btnAbort;
        private Button btnClear;
        private Button btnRemove;
        private Panel panel1;
        private Button btnPaste;
        private Button btnCopy;
    }
}