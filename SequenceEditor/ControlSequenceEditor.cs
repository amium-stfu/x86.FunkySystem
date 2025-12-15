using FunkySystem.Core;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using FunkySystem.Devices;
using FunkySystem.Roslyn;

namespace FunkySystem.SequenceEditor
{
    public partial class ControlSequenceEditor : UserControl
    {
        DocumentEditor Editor = new DocumentEditor();
        public ControlSequenceEditor()
        {


            InitializeComponent();
            Editor.Dock = DockStyle.Fill;
            tableEditor.Controls.Add(Editor, 1, 0);

            InitGridDiagnostics();

            treeView1.NodeMouseClick += TreeView1_NodeMouseClick;


        }
        BookNode LastSelectedNode = null;
        public BookNode SelectedCodeNode = null;
        private async void TreeView1_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Node is BookNode)
                {

                    BookNode? node = e.Node as BookNode;

                    if (node == null) return;
                    if (node.Type != NodeType.Code)
                    {
                        return;
                    }


                    if (node.Type == NodeType.Code)
                    {
                        if (LastSelectedNode != null)
                        {
                            LastSelectedNode.ForeColor = Theme.GridForeColor;
                        }

                        SelectedCodeNode = e.Node as BookNode;
                        SelectedCodeNode.ForeColor = Theme.IsDark ? Color.Plum : Color.DarkOrchid;
                        try
                        {
                            Editor.Target = SelectedCodeNode.Tag as CodeDocument;
                            Editor.ApplyLightTheme();

                            Editor.Text = Editor.Target.Code;
                            await Editor.UpdateRoslyn("Selected Code Node Changed");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error loading code document: " + ex.Message);
                        }

                        //     View.SetTarget("Mouse click", SelectedCodeNode.Editor);


                        LastSelectedNode = SelectedCodeNode;
                    }
                }

            }
        }

        public void InitTree()
        {
            string uri = System.IO.Path.Combine(Program.SettingsDirectory, "Sequences");

            BookNode rootNode = new BookNode("Sequences", NodeType.Book);
            rootNode.Tag = uri;
            treeView1.Nodes.Add(rootNode);
            foreach (string dir in System.IO.Directory.GetDirectories(uri))
            {
                BookNode dirNode = new BookNode(System.IO.Path.GetFileName(dir), NodeType.Folder);
                dirNode.Tag = dir;
                rootNode.Nodes.Add(dirNode);
                foreach (string file in System.IO.Directory.GetFiles(dir, "*.cs"))
                {

                    BookNode fileNode = new BookNode(System.IO.Path.GetFileName(file), NodeType.Code);
                    fileNode.Tag = FunkyCore.Roslyn.GetCodeDocument(file);

                    dirNode.Nodes.Add(fileNode);
                }
            }
        }


        private void InitGridDiagnostics()
        {

            GridViewDiagnosticOutput.DataSource = Editor.Output;
            GridViewDiagnosticOutput.DataBindingComplete += (s, e) =>
            {
                GridViewDiagnosticOutput.Columns["Page"].Width = 0;
                GridViewDiagnosticOutput.Columns["Class"].Width = 0;
                GridViewDiagnosticOutput.Columns["Position"].Width = 0;
                GridViewDiagnosticOutput.Columns["Length"].Width = 0;
                GridViewDiagnosticOutput.Columns["Type"].Width = 0;


                GridViewDiagnosticOutput.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            };

            GridViewDiagnosticOutput.AllowUserToResizeColumns = false;
            GridViewDiagnosticOutput.AllowUserToAddRows = false;
            GridViewDiagnosticOutput.RowHeadersVisible = false;
            GridViewDiagnosticOutput.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            GridViewDiagnosticOutput.MultiSelect = false;
            GridViewDiagnosticOutput.ReadOnly = true;
            GridViewDiagnosticOutput.BackgroundColor = Color.Red;
            GridViewDiagnosticOutput.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            GridViewDiagnosticOutput.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            GridViewDiagnosticOutput.ColumnHeadersVisible = true;
            GridViewDiagnosticOutput.RowHeadersVisible = false;
            GridViewDiagnosticOutput.Dock = DockStyle.Fill;
            GridViewDiagnosticOutput.AllowUserToAddRows = false;
            GridViewDiagnosticOutput.AllowUserToDeleteRows = false;
            GridViewDiagnosticOutput.AllowUserToOrderColumns = true;
            GridViewDiagnosticOutput.AllowUserToResizeColumns = false;
            GridViewDiagnosticOutput.BackgroundColor = System.Drawing.Color.LightGray;
            GridViewDiagnosticOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            GridViewDiagnosticOutput.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            GridViewDiagnosticOutput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            GridViewDiagnosticOutput.ColumnHeadersVisible = false;
            GridViewDiagnosticOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            GridViewDiagnosticOutput.Location = new System.Drawing.Point(46, 25);
            GridViewDiagnosticOutput.Margin = new System.Windows.Forms.Padding(0);
            GridViewDiagnosticOutput.Name = "dataGridOutput";
            GridViewDiagnosticOutput.Size = new System.Drawing.Size(832, 115);
            GridViewDiagnosticOutput.TabIndex = 0;
            GridViewDiagnosticOutput.ScrollBars = System.Windows.Forms.ScrollBars.None;
            GridViewDiagnosticOutput.CellFormatting += (s, e) =>
            {
                if (GridViewDiagnosticOutput.Columns[e.ColumnIndex].Name == "Type")
                {
                    string severity = e.Value?.ToString();
                    switch (severity)
                    {
                        case "Error":
                            GridViewDiagnosticOutput.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Tomato;
                            break;
                        case "Warning":
                            GridViewDiagnosticOutput.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                            break;
                        case "Info":
                            GridViewDiagnosticOutput.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightBlue;
                            break;
                    }
                }
            };
            GridViewDiagnosticOutput.CellClick += (s, e) => Task.Run(async () =>
            {
                if (e.RowIndex >= 0)
                {
                    string pos = GridViewDiagnosticOutput.Rows[e.RowIndex].Cells["Position"].Value.ToString();
                    int length = Convert.ToInt32(GridViewDiagnosticOutput.Rows[e.RowIndex].Cells["Length"].Value);
                    if (Editor.InvokeRequired)
                    {
                        Editor.Invoke(new System.Action(() =>
                        {
                            int p = Convert.ToInt32(pos);
                            Editor.SelectRange(p, length);
                        }));
                    }
                    else
                    {
                        int p = Convert.ToInt32(pos);
                        Editor.SelectRange(p, length);
                    }
                }
            });

        }

        private void btnReload_LeftClicked(object sender, EventArgs e)
        {
            InitTree();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Editor.Target.SaveToFile();
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }
    }

    public enum NodeType
    {
        Page,
        SubCode,
        Book,
        Program,
        Settings,
        Folder,
        Code
    }


    public class BookNode : System.Windows.Forms.TreeNode
    {
        public NodeType Type { get; set; }

        public DocumentEditor Editor;
        public BookNode(string name, NodeType type) : base(name)
        {
            Type = type;
            Name = name;

            if (Type == NodeType.SubCode)
            {
                Text = name.Split('.')[1];
            }

            if (Type == NodeType.Page)
            {
                Text = name.Split('.')[0];
            }



        }
    }
}
