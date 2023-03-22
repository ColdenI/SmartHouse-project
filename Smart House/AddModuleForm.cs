using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace Smart_House
{
    public partial class AddModuleForm : Form
    {
        public AddModuleForm()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e)
        {
            string name = textBox2.Text;
            string s_code = textBox1.Text;
            string ip = numericUpDown1.Value.ToString() + "." + numericUpDown4.Value.ToString() + "." + numericUpDown3.Value.ToString() + "." + numericUpDown2.Value.ToString();
            string type = comboBox1.Text;

            if (name.Length < 4) MessageBox.Show("Имя слишком короткое.", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!comboBox1.Items.Contains(type)) MessageBox.Show("Тип не найден.", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                using (var wb = new WebClient())
                {
                    string response = "";
                    string _type = "";
                    try
                    {
                        response = wb.DownloadString("http://" + ip + "/security_code_check?s_code=" + s_code);
                        _type = wb.DownloadString("http://" + ip + "/test");
                    }
                    catch (System.Net.WebException)
                    {
                        MessageBox.Show("IP адрес неверен или модуль неисправен!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (_type != type) MessageBox.Show("Тип модуля неверен!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (response == "s_code_incorrect") MessageBox.Show("Код защиты неверен!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (response == "s_code_correct" && _type == type)
                    {
                        MainForm main = this.Owner as MainForm;
                        main.modules.Add(new MainForm.Module(name, type, ip, s_code));
                        Console.Beep(2000, 150);
                        Console.Beep(1500, 150);
                        main.SavaModuleData();
                        main.ModuleList_Update();
                    }

                }
            }

        }

        private void AddModuleForm_Load(object sender, EventArgs e)
        {
            MainForm main = this.Owner as MainForm;
            Location = main.Location;
        }
    }
}
