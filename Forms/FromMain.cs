
namespace FunkySystem
{
    public partial class FormMain : Form
    {

        public FormMain()
        {
            InitializeComponent();
        }

        private void TableRoot_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormSignalPool signalPoolForm = new FormSignalPool();
            signalPoolForm.Show();
        }


        private void btnAddTest_Click(object sender, EventArgs e)
        {

        }

    }
}
