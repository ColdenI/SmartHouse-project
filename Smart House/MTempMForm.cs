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
    public partial class MTempMForm : Form
    {
        public struct rgb
        {
            public Color color;
            public rgb(byte r, byte g, byte b)
            {
                color = Color.FromArgb(r, g, b);
            }
        }


        private MainForm.Module module;
        private MainForm mainForm;

        //private SHa.Time nowTimeInModule = SHa.Time.Zero;

        private rgb[] colors = {    new rgb(5, 34, 255),
                                    new rgb(0, 61, 255),
                                    new rgb(0, 79, 255),
                                    new rgb(0, 94, 255),
                                    new rgb(0, 107, 255),
                                    new rgb(0, 118, 255),
                                    new rgb(0, 128, 255),
                                    new rgb(0, 138, 255),
                                    new rgb(0, 146, 255),
                                    new rgb(0, 154, 255),
                                    new rgb(0, 161, 255),
                                    new rgb(0, 168, 255),
                                    new rgb(0, 175, 255),
                                    new rgb(0, 181, 255),
                                    new rgb(0, 187, 255),
                                    new rgb(0, 192, 255),
                                    new rgb(0, 198, 255),
                                    new rgb(0, 203, 247),
                                    new rgb(0, 209, 231),
                                    new rgb(0, 214, 215),
                                    new rgb(0, 219, 198),
                                    new rgb(0, 224, 180),
                                    new rgb(0, 229, 163),
                                    new rgb(0, 234, 145),
                                    new rgb(0, 238, 127),
                                    new rgb(0, 242, 109),
                                    new rgb(0, 246, 90),
                                    new rgb(0, 249, 70),
                                    new rgb(0, 252, 47),
                                    new rgb(81, 255, 5),
                                    new rgb(81, 255, 5),
                                    new rgb(87, 246, 5),
                                    new rgb(93, 237, 5),
                                    new rgb(99, 229, 4),
                                    new rgb(105, 220, 4),
                                    new rgb(111, 211, 4),
                                    new rgb(117, 202, 4),
                                    new rgb(123, 193, 4),
                                    new rgb(129, 185, 4),
                                    new rgb(135, 176, 3),
                                    new rgb(141, 167, 3),
                                    new rgb(147, 158, 3),
                                    new rgb(153, 149, 3),
                                    new rgb(159, 141, 3),
                                    new rgb(165, 132, 3),
                                    new rgb(171, 123, 2),
                                    new rgb(177, 114, 2),
                                    new rgb(183, 106, 2),
                                    new rgb(189, 97, 2),
                                    new rgb(195, 88, 2),
                                    new rgb(201, 79, 2),
                                    new rgb(207, 70, 1),
                                    new rgb(213, 62, 1),
                                    new rgb(219, 53, 1),
                                    new rgb(225, 44, 1),
                                    new rgb(231, 35, 1),
                                    new rgb(237, 26, 1),
                                    new rgb(243, 18, 0),
                                    new rgb(249, 9, 0),
                                    new rgb(255, 0, 0)
        };

        private string temp = "", hum = "";


        public MTempMForm(MainForm.Module _module)
        {
            InitializeComponent();
            this.module = _module;
           
        }

        private void MLempForm_Load(object sender, EventArgs e)
        {
            this.mainForm = this.Owner as MainForm;
            Text = module.Name + " (" + module.IP + ") - Smart House";
            Location = mainForm.Location;
            timer1.Enabled = true;
            timer1.Interval = Properties.Settings.Default.TimeAutoUpdateData;
            UpdateData();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateData();
        }

        public static bool ToBool(string val)
        {
            if (val == "0") return false;
            else return true;
        }

        private void WebSend(string url)
        {
            try
            {
                using (var wb = new WebClient())
                {
                    string url_ = "http://" + module.IP + url;
                    wb.DownloadString(url_);
                }
            }
            catch (System.Net.WebException)
            {
                MessageBox.Show("Ошибка соединения! Невозможно установить связь с устройством. Проверьте устройство. Перезагрузите устройство. Если ошибка остаётся напишите нам.", "Smart House - Модуль", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void UpdateData()
        {
            try
            {
                backgroundWorker1.RunWorkerAsync();
            }
            catch (System.InvalidOperationException)
            {

            }


            string str = temp.Split(new[] { "." }, StringSplitOptions.None)[0];
            float _temp = 0;

            try { 
                int _index = Convert.ToInt32(str);


                if (Properties.Settings.Default.IsFATemp)
                {
                    _temp = (_index * 1.8f) + 32;
                    if (label1.Text != _temp.ToString() + " °F") label1.Text = _temp.ToString() + " °F";
                }
                else
                {
                    _temp = _index;
                    if (label1.Text != _temp.ToString() + " °C") label1.Text = _temp.ToString() + " °C";
                }


                if (_index >= colors.Length - 1) _index = colors.Length - 1;
                if (_index < 0) _index *= -1;

                label1.ForeColor = colors[_index].color;
            }
            catch (System.FormatException) { }


            
            if (label2.Text != "Влажность: " + hum.ToString() + " %") label2.Text = "Влажность: " + hum.ToString() + " %";

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string data = "";
            try
            {
                using (var wb = new WebClient())
                {
                    data = wb.DownloadString("http://" + module.IP + "/get_data?s_code=" + module.SCode);
                }
            }
            catch (System.Net.WebException)
            {
                MessageBox.Show("Ошибка соединения! Невозможно установить связь с устройством. Проверьте устройство. Перезагрузите устройство. Если ошибка остаётся напишите нам.", "Smart House - Модуль", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }

            Console.WriteLine(data);
            try
            {
                temp = data.Split(new[] { "\\" }, StringSplitOptions.None)[0];
                hum = data.Split(new[] { "\\" }, StringSplitOptions.None)[1];

            } catch (System.FormatException){ }


        }

        private void MLempForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            timer1.Stop();
        }


        
    }
}
