using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FunkySystem.Controls
{
    public class RadioButtonFlowPanel : FlowLayoutPanel
    {

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SelectColor { get; set; } = Color.Orange;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DefaultColor { get; set; } = Color.LightGray;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ButtonBackColor { get; set; } = Color.LightGray;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ButtonForeColor { get; set; } = Color.Black;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ButtonBorderColor { get; set; } = Color.DarkGray;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ButtonWidth { get; set; } = 250;



        public RadioButtonFlowPanel()
        {
 
            this.FlowDirection = FlowDirection.LeftToRight;
            this.WrapContents = false;
            this.Size = new Size(300, 30);
        }

        void reset()
        {
            foreach (Control ctrl in this.Controls)
            {
                if( ctrl is RadioButtonEx)
                    ((RadioButtonEx)ctrl).Selected = false;
            }
        }

        int index = 0;


        public void Remove(RadioButtonEx rb)
        {
            this.Controls.Remove(rb);
        }
        public RadioButtonEx AddRadioButton(string text,string name, System.Action onSelect, Object attechment = null)
        {
            index++;
            RadioButtonEx rb = new RadioButtonEx();
            rb.Name = name;
            if (attechment != null)
                rb.Tag = attechment;
            rb.DisplayText = text;
            rb.NumberText = (index).ToString();
            rb.Margin = new Padding(1,1,1,0);
            rb.BackColor = this.ButtonBackColor;
            rb.ForeColor = this.ButtonForeColor;
            rb.BorderColor = this.ButtonBorderColor;
            rb.SelectColor = this.SelectColor;
            rb.DefaultColor = this.DefaultColor;
            rb.ShowMenuButton = false;
            rb.Selected = false;
            if (FlowDirection == FlowDirection.TopDown)
            {
                rb.Height = this.Width;
                rb.Width = ButtonWidth;
            }
            else
            {
                rb.Height = this.Height;
                rb.Width = ButtonWidth;
            }

            rb.Width = ButtonWidth;
            rb.Clicked += (s, e) =>
            {
                reset();
                rb.Selected = true;
                onSelect?.Invoke();
            };

            this.Controls.Add(rb);
            return rb;      
        }

        public void SetSelected(RadioButtonEx rb)
        {
            
            rb.Selected = true;
           
        }


    }
}
