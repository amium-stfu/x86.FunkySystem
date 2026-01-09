
using FunkySystem.Core;
using System.Diagnostics;

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

        private async void button1_Click(object sender, EventArgs e)
        {
            //FormSignalPool signalPoolForm = new FormSignalPool();
            //signalPoolForm.Show();
            await SequenceManager.LoadSequences();

            foreach (var item in SequenceManager.Sequences) 
            { 
             Debug.WriteLine(item.Key);
                foreach (var seq in item.Value) 
                {
                    Debug.WriteLine($"   {seq.Key} ");
                   

                    if(seq.Value is FunkySequence)
                        Debug.WriteLine($"{seq.Value.Name} is valid");

                }

            }

           
        }


        private void btnAddTest_Click(object sender, EventArgs e)
        {

        }

    }
}
