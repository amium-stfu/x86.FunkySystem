using FunkySystem.Core;
using FunkySystem.Helpers;
using FunkySystem.Signals;
using FunkySystem.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FunkySystem.UI
{
    public partial class UserControlPage : UserControl
    {
        public FormMain View;

       
        public UserControlPage()
        {
            InitializeComponent();
      
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string PageText
        {
            get => lblPageText.Text;
            set => lblPageText.Text = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string PageNumber
        {
            get => lblPageNumber.Text;
            set => lblPageNumber.Text = value;
        }

        bool hideMenuButton = false;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool HideMenuButton
        {
            get => hideMenuButton;
            set
            {
                hideMenuButton = value;
                btnAdd.Visible = !hideMenuButton;
                Invalidate();
            }
        }

        private void lblPageNumber_Click(object sender, EventArgs e)
        {
            SelectView();


            //foreach (Control ctrl in Parent.Controls)
            //{
            //    if (ctrl is UserControlPage page)
            //    {
            //        page.Deselect();
            //    }
            //}

            //lblPageNumber.BackColor = Color.Orange;
            //AmiumScripter.Root.Main.ShowPageView(View);
            //Debug.WriteLine("Selected Page " + View.Name);
            //UIEditor.CurrentPageName = View.Name;
        }

        public void Deselect()
        {
            lblPageNumber.BackColor = Color.LightGray;
        }

        public void SelectView()
        {
            //foreach (Control ctrl in Parent.Controls)
            //{
            //    if (ctrl is UserControlPage page)
            //    {
            //        page.Deselect();
            //    }
            //}

            //lblPageNumber.BackColor = Color.Orange;
            //AmiumScripter.Root.Main.ShowPageView(View);
            //Debug.WriteLine("Selected Page " + View.Name);
            //UIEditor.CurrentPageName = View.Name;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
          
        }
    }
}
