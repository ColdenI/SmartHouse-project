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
    public partial class MStuffForm : Form
    {

        private MainForm.Module module;
        private MainForm mainForm;

        private SHa.Time nowTimeInModule = SHa.Time.Zero;




        

        public MStuffForm(MainForm.Module _module)
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



        }

        private void MLempForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            timer1.Stop();
        }



        
    }
}
