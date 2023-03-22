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
    public partial class MSocketForm : Form
    {

        private MainForm.Module module;
        private MainForm mainForm;

        private SHa.Time nowTimeInModule = SHa.Time.Zero;
        private bool isOn = false;
        private int Mode = 1;
        private bool isInvert = false;
        private bool isFlag = false;
        private int eeDelay = 1;

        private SHa.Schedule schedule1, schedule2, schedule3;





        public MSocketForm(MainForm.Module _module)
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


            if (isOn)
            {
                button1.Text = "Вкл";
                button1.BackColor = Color.FromArgb(128, 255, 128);
                button1.ForeColor = Color.FromArgb(0, 64, 0);
            }
            else
            {
                button1.Text = "Выкл";
                button1.BackColor = Color.FromArgb(255, 192, 192);
                button1.ForeColor = Color.FromArgb(220, 0, 0);
            }

            if (groupBox1.Text != "Режим (" + comboBox1.Items[Mode - 1].ToString() + "):") groupBox1.Text = "Режим (" + comboBox1.Items[Mode - 1].ToString() + "):";

            if (groupBox2.Text != "Задержка таймера (" + listBox1.Items[eeDelay - 1].ToString() + "):") groupBox2.Text = "Задержка таймера (" + listBox1.Items[eeDelay - 1].ToString() + "):";

            if (checkBox1.Checked != isFlag) checkBox1.Checked = isFlag;
            if (checkBox2.Checked != isInvert) checkBox2.Checked = isInvert;

            if (label1.Text != nowTimeInModule.Format()) label1.Text = nowTimeInModule.Format();

            if (label3.Text != "Тип: " + comboBox4.Items[schedule1.Type].ToString()) label3.Text = "Тип: " + comboBox4.Items[schedule1.Type].ToString();
            if (label14.Text != "Тип: " + comboBox7.Items[schedule2.Type].ToString()) label14.Text = "Тип: " + comboBox7.Items[schedule2.Type].ToString();
            if (label19.Text != "Тип: " + comboBox9.Items[schedule3.Type].ToString()) label19.Text = "Тип: " + comboBox9.Items[schedule3.Type].ToString();

            if (label6.Text != "Сейчас: " + schedule1.startTime.Format() + " - " + schedule1.endTime.Format()) label6.Text = "Сейчас: " + schedule1.startTime.Format() + " - " + schedule1.endTime.Format();
            if (label13.Text != "Сейчас: " + schedule2.startTime.Format() + " - " + schedule2.endTime.Format()) label13.Text = "Сейчас: " + schedule2.startTime.Format() + " - " + schedule2.endTime.Format();
            if (label18.Text != "Сейчас: " + schedule3.startTime.Format() + " - " + schedule3.endTime.Format()) label18.Text = "Сейчас: " + schedule3.startTime.Format() + " - " + schedule3.endTime.Format();

            if (schedule1.Type == 2) if (label7.Text != "Сейчас: " + comboBox5.Items[schedule1.IndexDay]) label7.Text = "Сейчас: " + comboBox5.Items[schedule1.IndexDay]; else { } else label7.Text = "Сейчас: (другой тип)";
            if (schedule2.Type == 2) if (label12.Text != "Сейчас: " + comboBox6.Items[schedule2.IndexDay]) label12.Text = "Сейчас: " + comboBox5.Items[schedule2.IndexDay]; else { } else label12.Text = "Сейчас: (другой тип)";
            if (schedule3.Type == 2) if (label17.Text != "Сейчас: " + comboBox8.Items[schedule3.IndexDay]) label17.Text = "Сейчас: " + comboBox8.Items[schedule3.IndexDay]; else { } else label17.Text = "Сейчас: (другой тип)";

        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (Mode == 1)
                if (isOn) WebSend("/key/0?s_code=" + module.SCode);
                else WebSend("/key/1?s_code=" + module.SCode);
            else
                MessageBox.Show("Выберите другой режим!", "Smart House - Модуль", MessageBoxButtons.OK, MessageBoxIcon.Error);

            UpdateData();
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

            Mode = Convert.ToInt32(data.Split(new[] { "\\" }, StringSplitOptions.None)[0]);
            isOn = ToBool(data.Split(new[] { "\\" }, StringSplitOptions.None)[1]);
            eeDelay = Convert.ToInt32(data.Split(new[] { "\\" }, StringSplitOptions.None)[2]);
            isFlag = ToBool(data.Split(new[] { "\\" }, StringSplitOptions.None)[3]);
            isInvert = ToBool(data.Split(new[] { "\\" }, StringSplitOptions.None)[4]);
            // time zone - index [5]
            schedule1.Type      = Convert.ToInt32(data.Split(new[] { "\\" }, StringSplitOptions.None)[6]);
            schedule2.Type      = Convert.ToInt32(data.Split(new[] { "\\" }, StringSplitOptions.None)[7]);
            schedule3.Type      = Convert.ToInt32(data.Split(new[] { "\\" }, StringSplitOptions.None)[8]);
            schedule1.IndexDay  = Convert.ToInt32(data.Split(new[] { "\\" }, StringSplitOptions.None)[9]);
            schedule2.IndexDay  = Convert.ToInt32(data.Split(new[] { "\\" }, StringSplitOptions.None)[10]);
            schedule3.IndexDay  = Convert.ToInt32(data.Split(new[] { "\\" }, StringSplitOptions.None)[11]);
            schedule1.startTime = new SHa.Time(Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[12]), Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[15]));
            schedule2.startTime = new SHa.Time(Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[13]), Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[16]));
            schedule3.startTime = new SHa.Time(Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[14]), Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[17]));
            schedule1.endTime   = new SHa.Time(Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[18]), Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[21]));
            schedule2.endTime   = new SHa.Time(Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[19]), Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[22]));
            schedule3.endTime   = new SHa.Time(Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[20]), Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[23]));
            nowTimeInModule     = new SHa.Time(Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[24]), Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[25]));

        }

        private void MLempForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            timer1.Stop();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0: WebSend("/mode/s?s_code=" + module.SCode); break;
                case 1: WebSend("/mode/t?s_code=" + module.SCode); break;
                case 2: WebSend("/mode/m?s_code=" + module.SCode); break;
                case 3: WebSend("/mode/sh?s_code=" + module.SCode); break;
            }
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
                switch (listBox1.SelectedIndex)
                {
                    case 0: WebSend("/timer_delay/m5?s_code=" + module.SCode); break;
                    case 1: WebSend("/timer_delay/m10?s_code=" + module.SCode); break;
                    case 2: WebSend("/timer_delay/m30?s_code=" + module.SCode); break;
                    case 3: WebSend("/timer_delay/h1?s_code=" + module.SCode); break;
                    case 4: WebSend("/timer_delay/h2?s_code=" + module.SCode); break;
                    case 5: WebSend("/timer_delay/h3?s_code=" + module.SCode); break;
                    case 6: WebSend("/timer_delay/h5?s_code=" + module.SCode); break;
                    case 7: WebSend("/timer_delay/h8?s_code=" + module.SCode); break;
                    case 8: WebSend("/timer_delay/h10?s_code=" + module.SCode); break;
                    case 9: WebSend("/timer_delay/h12?s_code=" + module.SCode); break;
                    case 10: WebSend("/timer_delay/h17?s_code=" + module.SCode); break;
                    case 11: WebSend("/timer_delay/h24?s_code=" + module.SCode); break;
                }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) WebSend("/modul/flag/1?s_code=" + module.SCode);
            else WebSend("/modul/flag/0?s_code=" + module.SCode);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetSchedule("1", ref comboBox4, ref comboBox5, ref numericUpDown1, ref numericUpDown2, ref numericUpDown3, ref numericUpDown4);
        }

        private void SetSchedule(string indexSch, ref ComboBox type, ref ComboBox day, ref NumericUpDown f_hour, ref NumericUpDown f_min, ref NumericUpDown t_hour, ref NumericUpDown t_min)
        {
            if (!type.Items.Contains(type.Text))
            {
                MessageBox.Show("Выберите тип!", "Smart House - Модуль", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!day.Items.Contains(day.Text) && type.Items.IndexOf(type.Text) == 2)
            {
                MessageBox.Show("Выберите день недели!", "Smart House - Модуль", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            WebSend(
                "/schedule/set/" + indexSch +               
                "?type=" + type.Items.IndexOf(type.Text) +
                "&f_hour=" + f_hour.Value.ToString() +
                "&f_min=" + f_min.Value.ToString() +
                "&t_hour=" + t_hour.Value.ToString() +
                "&t_min=" + t_min.Value.ToString() +
                "&day_index=" + day.Items.IndexOf(day.Text) +
                "&s_code=" + module.SCode
                );
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown2, ref numericUpDown1);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown2, ref numericUpDown1);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown4, ref numericUpDown3);
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown4, ref numericUpDown3);
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown8, ref numericUpDown7);
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown6, ref numericUpDown5);
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown8, ref numericUpDown7);
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown6, ref numericUpDown5);
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown12, ref numericUpDown11);
        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown12, ref numericUpDown11);
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown10, ref numericUpDown9);
        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown10, ref numericUpDown9);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetSchedule("2", ref comboBox7, ref comboBox6, ref numericUpDown7, ref numericUpDown8, ref numericUpDown5, ref numericUpDown6);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetSchedule("3", ref comboBox9, ref comboBox8, ref numericUpDown11, ref numericUpDown12, ref numericUpDown9, ref numericUpDown10);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) WebSend("/modul/invert/1?s_code=" + module.SCode);
            else WebSend("/modul/invert/0?s_code=" + module.SCode);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            WebSend("/schedule/set_time_zone?time=" + (comboBox3.SelectedIndex + 1).ToString() + "&s_code=" + module.SCode);
        }

    }
}
