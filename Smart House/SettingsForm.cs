using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Smart_House
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Удалить все данные о сохранённых ранее модулях?", "Smart House - Настройки", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                MainForm main = this.Owner as MainForm;
                main.ResetModuleData();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TimeAutoUpdateData = Convert.ToInt32(numericUpDown1.Value);
            Properties.Settings.Default.Save();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = Properties.Settings.Default.TimeAutoUpdateData;
            checkBox1.Checked = Properties.Settings.Default.IsFATemp;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IsFATemp = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
