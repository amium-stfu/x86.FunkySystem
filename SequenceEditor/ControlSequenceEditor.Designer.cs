namespace FunkySystem.SequenceEditor
{
    partial class ControlSequenceEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            treeView1 = new TreeView();
            TableCodeExporer = new TableLayoutPanel();
            splitContainer3 = new SplitContainer();
            tableEditor = new TableLayoutPanel();
            panelEditor = new Panel();
            GridViewDiagnosticOutput = new DataGridView();
            imageList1 = new ImageList(components);
            tableLayoutPanel1 = new TableLayoutPanel();
            panelControl = new Panel();
            btnSave = new FunkySystem.Controls.ButtonWithIcon();
            btnReload = new FunkySystem.Controls.ButtonWithIcon();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            tableEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)GridViewDiagnosticOutput).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            panelControl.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(13, 73);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer3);
            splitContainer1.Size = new Size(1209, 657);
            splitContainer1.SplitterDistance = 221;
            splitContainer1.TabIndex = 0;
            splitContainer1.SplitterMoved += splitContainer1_SplitterMoved;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(treeView1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(TableCodeExporer);
            splitContainer2.Size = new Size(221, 657);
            splitContainer2.SplitterDistance = 305;
            splitContainer2.TabIndex = 0;
            // 
            // treeView1
            // 
            treeView1.BackColor = Color.FromArgb(224, 224, 224);
            treeView1.BorderStyle = BorderStyle.None;
            treeView1.Dock = DockStyle.Fill;
            treeView1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            treeView1.Location = new Point(0, 0);
            treeView1.Margin = new Padding(0);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(221, 305);
            treeView1.TabIndex = 0;
            // 
            // TableCodeExporer
            // 
            TableCodeExporer.BackColor = Color.FromArgb(224, 224, 224);
            TableCodeExporer.ColumnCount = 2;
            TableCodeExporer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            TableCodeExporer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            TableCodeExporer.Dock = DockStyle.Fill;
            TableCodeExporer.Location = new Point(0, 0);
            TableCodeExporer.Name = "TableCodeExporer";
            TableCodeExporer.RowCount = 2;
            TableCodeExporer.RowStyles.Add(new RowStyle(SizeType.Percent, 11.8863049F));
            TableCodeExporer.RowStyles.Add(new RowStyle(SizeType.Percent, 88.11369F));
            TableCodeExporer.Size = new Size(221, 348);
            TableCodeExporer.TabIndex = 0;
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(tableEditor);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(GridViewDiagnosticOutput);
            splitContainer3.Size = new Size(984, 657);
            splitContainer3.SplitterDistance = 508;
            splitContainer3.TabIndex = 0;
            // 
            // tableEditor
            // 
            tableEditor.ColumnCount = 2;
            tableEditor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableEditor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableEditor.Controls.Add(panelEditor, 0, 0);
            tableEditor.Dock = DockStyle.Fill;
            tableEditor.Location = new Point(0, 0);
            tableEditor.Name = "tableEditor";
            tableEditor.RowCount = 1;
            tableEditor.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableEditor.Size = new Size(984, 508);
            tableEditor.TabIndex = 0;
            // 
            // panelEditor
            // 
            panelEditor.BackColor = Color.FromArgb(224, 224, 224);
            panelEditor.Dock = DockStyle.Fill;
            panelEditor.Location = new Point(0, 0);
            panelEditor.Margin = new Padding(0);
            panelEditor.Name = "panelEditor";
            panelEditor.Size = new Size(50, 508);
            panelEditor.TabIndex = 0;
            // 
            // GridViewDiagnosticOutput
            // 
            GridViewDiagnosticOutput.BackgroundColor = Color.WhiteSmoke;
            GridViewDiagnosticOutput.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            GridViewDiagnosticOutput.Dock = DockStyle.Fill;
            GridViewDiagnosticOutput.Location = new Point(0, 0);
            GridViewDiagnosticOutput.Name = "GridViewDiagnosticOutput";
            GridViewDiagnosticOutput.Size = new Size(984, 145);
            GridViewDiagnosticOutput.TabIndex = 0;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            tableLayoutPanel1.Controls.Add(splitContainer1, 1, 1);
            tableLayoutPanel1.Controls.Add(panelControl, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1235, 733);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // panelControl
            // 
            panelControl.BackColor = Color.FromArgb(224, 224, 224);
            panelControl.Controls.Add(btnSave);
            panelControl.Controls.Add(btnReload);
            panelControl.Dock = DockStyle.Fill;
            panelControl.Location = new Point(13, 3);
            panelControl.Margin = new Padding(3, 3, 3, 0);
            panelControl.Name = "panelControl";
            panelControl.Size = new Size(1209, 67);
            panelControl.TabIndex = 1;
            // 
            // btnSave
            // 
            btnSave.BorderColor = Color.Transparent;
            btnSave.ButtonIcon = "fa:floppy-disk:black";
            btnSave.ButtonText = "";
            btnSave.DefaultBackColor = Color.Empty;
            btnSave.Dock = DockStyle.Left;
            btnSave.GridScale = 5;
            btnSave.HoverColor = Color.Transparent;
            btnSave.IconSizeFactor = 0.6D;
            btnSave.LedMargin = 2;
            btnSave.LedSelectColor = Color.Red;
            btnSave.LedSize = 15;
            btnSave.LedVisible = false;
            btnSave.Location = new Point(68, 0);
            btnSave.Margin = new Padding(0);
            btnSave.Name = "btnSave";
            btnSave.SelectColor = Color.Orange;
            btnSave.Selected = false;
            btnSave.ShortcutText = "Ctrl+X";
            btnSave.SignalValue = "Unknown";
            btnSave.Size = new Size(67, 67);
            btnSave.TabIndex = 1;
            btnSave.Text = "buttonWithIcon1";
            btnSave.Click += btnSave_Click;
            // 
            // btnReload
            // 
            btnReload.BorderColor = Color.Transparent;
            btnReload.ButtonIcon = "fa:rotate-left:Black";
            btnReload.ButtonText = "";
            btnReload.DefaultBackColor = Color.Empty;
            btnReload.Dock = DockStyle.Left;
            btnReload.GridScale = 5;
            btnReload.HoverColor = Color.Transparent;
            btnReload.IconSizeFactor = 0.6D;
            btnReload.LedMargin = 2;
            btnReload.LedSelectColor = Color.Red;
            btnReload.LedSize = 15;
            btnReload.LedVisible = false;
            btnReload.Location = new Point(0, 0);
            btnReload.Margin = new Padding(0);
            btnReload.Name = "btnReload";
            btnReload.SelectColor = Color.Orange;
            btnReload.Selected = false;
            btnReload.ShortcutText = "Ctrl+X";
            btnReload.SignalValue = "Unknown";
            btnReload.Size = new Size(68, 67);
            btnReload.TabIndex = 0;
            btnReload.Text = "buttonWithIcon1";
            btnReload.LeftClicked += btnReload_LeftClicked;
            btnReload.Click += btnReload_Click;
            // 
            // ControlSequenceEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "ControlSequenceEditor";
            Size = new Size(1235, 733);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            tableEditor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)GridViewDiagnosticOutput).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            panelControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private SplitContainer splitContainer3;
        private TableLayoutPanel tableEditor;
        private TreeView treeView1;
        private TableLayoutPanel TableCodeExporer;
        private DataGridView GridViewDiagnosticOutput;
        private ImageList imageList1;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panelControl;
        private Controls.ButtonWithIcon btnReload;
        private Controls.ButtonWithIcon btnSave;
        private Panel panelEditor;
    }
}
