using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VATClient
{
    internal static class FormClearing   //FromClearing.ClearAllFormControls(this);
    {
        public static void ClearAllFormControls(Form form)
        {
            foreach (Control control in form.Controls)
            {
                if (control is TextBox)
                {
                    TextBox txtbox = (TextBox)control;
                    txtbox.Text = string.Empty;
                }
                else if(control is CheckBox)
                {
                    CheckBox chkbox = (CheckBox)control;
                    chkbox.Checked = false;
                }
                else if (control is RadioButton)
                {
                    RadioButton rdbtn = (RadioButton)control;
                    rdbtn.Checked = false;
                }
                else if (control is DateTimePicker)
                {
                    DateTimePicker dtp = (DateTimePicker)control;
                    dtp.Value = DateTime.Now;
                }
                else if (control is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)control;
                    comboBox.SelectedIndex = -1;
                }
            }
        }
    }

    internal static class FormHelper     // FormHelper.ResetFields(this);
    {
        public static void ResetFields(Control form)
        {
            foreach (Control ctrl in form.Controls)
            {
                if (ctrl.Controls.Count > 0)
                    ResetFields(ctrl);
                Reset(ctrl);
            }
        }
        private static void Reset(Control ctrl)
        {
            if (ctrl is TextBox)
            {
                TextBox tb = (TextBox)ctrl;
                if (tb != null)
                {
                    tb.ResetText();
                }
            }
            else if (ctrl is ComboBox)
            {
                ComboBox dd = (ComboBox)ctrl;
                if (dd != null)
                {
                    dd.SelectedIndex = 0;
                }
            }
            else if (ctrl is RadioButton)
            {
                RadioButton radioButton = (RadioButton)ctrl;
                if (radioButton.Checked)
                {
                    radioButton.Checked = false;
                }
            }
            else if (ctrl is ListBox)
            {
                ListBox listBox = (ListBox)ctrl;
                if (listBox.Items.Count>0)
                {
                    listBox.Items.Clear();
                }
            }

            else if (ctrl is DateTimePicker)
            {
                DateTimePicker dateTimePicker= (DateTimePicker)ctrl;
                if (dateTimePicker.Value.ToString() !=string.Empty)
                {
                    dateTimePicker.Value = DateTime.Today;
                }
            }
        }
    }
}
