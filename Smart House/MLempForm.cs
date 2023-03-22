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
    public partial class MLempForm : Form
    {
        private MainForm.Module module;
        private MainForm mainForm;

        private bool isOnLamp = false;
        private int Mode = 0;
        private bool isAuto = false;
        private bool isAutoOff = false;
        private bool isAutoOffTick = true;
        private ulong isAutoOffTime = 0;
        private ulong timeOffIs = 0;
        private SHa.Time nowTimeInModule = SHa.Time.Zero;

        private int alarmType = 0;
        private SHa.Time alarmTime = SHa.Time.Zero;
        private int alarmIndexDay = 0;
        private bool alarmIsBlinkLamp = true;
        

        public MLempForm(MainForm.Module _module)
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


            if (isOnLamp)
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

            if (Mode != 0)
            {
                button2.Text = "Вкл";
                button2.BackColor = Color.FromArgb(128, 255, 128);
                button2.ForeColor = Color.FromArgb(0, 64, 0);
            }
            else
            {
                button2.Text = "Выкл";
                button2.BackColor = Color.FromArgb(255, 192, 192);
                button2.ForeColor = Color.FromArgb(220, 0, 0);
            }

            string groupText_ = "";
            if (Mode == 0) groupText_ = "Режимы:";
            else if (Mode > 100 && Mode < 110) groupText_ = "Режим (Цвет):";
            else groupText_ = "Режим (" + listBox1.Items[Mode - 1].ToString().Split(new[] { ". "}, StringSplitOptions.None)[1] + "):";       
            if (groupText_ != groupBox1.Text) groupBox1.Text = groupText_;

            if (checkBox1.Checked != isAuto) checkBox1.Checked = isAuto;

            Color Bd = Color.FromArgb(192, 192, 255);
            Color Td = Color.FromArgb(0, 0, 192);
            Color Ba = Color.FromArgb(192, 255, 255);
            Color Ta = Color.FromArgb(0, 220, 0);

            if (isAutoOffTick != isAutoOff || true)
            {
                isAutoOffTick = isAutoOff;
                if (!isAutoOff)
                {
                    button6.BackColor = Bd;
                    button6.ForeColor = Td;
                    button7.BackColor = Bd;
                    button7.ForeColor = Td;
                    button8.BackColor = Bd;
                    button8.ForeColor = Td;
                    button9.BackColor = Ba;
                    button9.ForeColor = Ta;
                }
                else
                {
                    if (isAutoOffTime == 900000)
                    {
                        button6.BackColor = Ba;
                        button6.ForeColor = Ta;
                        button7.BackColor = Bd;
                        button7.ForeColor = Td;
                        button8.BackColor = Bd;
                        button8.ForeColor = Td;
                        button9.BackColor = Bd;
                        button9.ForeColor = Td;
                    }
                    else if (isAutoOffTime == 1800000)
                    {
                        button6.BackColor = Bd;
                        button6.ForeColor = Td;
                        button7.BackColor = Ba;
                        button7.ForeColor = Ta;
                        button8.BackColor = Bd;
                        button8.ForeColor = Td;
                        button9.BackColor = Bd;
                        button9.ForeColor = Td;
                    }
                    else if (isAutoOffTime == 2700000)
                    {
                        button6.BackColor = Bd;
                        button6.ForeColor = Td;
                        button7.BackColor = Bd;
                        button7.ForeColor = Td;
                        button8.BackColor = Ba;
                        button8.ForeColor = Ta;
                        button9.BackColor = Bd;
                        button9.ForeColor = Td;
                    }

                }
            }

            if (label5.Text != nowTimeInModule.Format()) label5.Text = nowTimeInModule.Format();

            if (label6.Text.Split(new[] { "Тип: " }, StringSplitOptions.None)[1] != comboBox3.Items[alarmType].ToString()) label6.Text = "Тип: " + comboBox3.Items[alarmType].ToString();

            string _alarmIsBlinkLamp = "";
            if (alarmIsBlinkLamp) _alarmIsBlinkLamp = "Сейчас: Мигает";
            else _alarmIsBlinkLamp = "Сейчас: Не мигает";

            if (label7.Text != _alarmIsBlinkLamp) label7.Text = _alarmIsBlinkLamp;

            if (label8.Text != "Установлен на: " + alarmTime.Format()) label8.Text = "Установлен на: " + alarmTime.Format();

            string _alarmDayIndex = "";
            if (alarmType == 2) _alarmDayIndex = "Установлен на: " + comboBox4.Items[alarmIndexDay].ToString();
            else _alarmDayIndex = "Установлен на: (другой тип)";

            if (label9.Text != _alarmDayIndex) label9.Text = _alarmDayIndex;


        }


        private void button3_Click(object sender, EventArgs e)
        {
            WebSend("/mode/set?arg=" + new Random().Next(1, listBox1.Items.Count) + "&s_code=" + module.SCode);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isOnLamp) WebSend("/key/set?arg=0&s_code=" + module.SCode);
            else WebSend("/key/set?arg=1&s_code=" + module.SCode);
            UpdateData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Mode == 0) WebSend("/mode/set?arg=" + new Random().Next(1, listBox1.Items.Count) + "&s_code=" + module.SCode);
            else WebSend("/mode/set?arg=0&s_code=" + module.SCode);
            UpdateData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Items.Contains(comboBox1.Text))
            {
                WebSend("/color/set?arg=" + (comboBox1.SelectedIndex + 1).ToString() + "&s_code=" + module.SCode);
                UpdateData();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                WebSend("/mode/set?arg=" + (listBox1.SelectedIndex + 1).ToString() + "&s_code=" + module.SCode);
                UpdateData();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) WebSend("/mode/auto/set?arg=1&s_code=" + module.SCode);
            else WebSend("/mode/auto/set?arg=0&s_code=" + module.SCode);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            WebSend("/bri/set?arg=" + trackBar1.Value + "&s_code=" + module.SCode);
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
            isOnLamp = ToBool(data.Split(new[] { "\\" }, StringSplitOptions.None)[1]);
            isAuto = ToBool(data.Split(new[] { "\\" }, StringSplitOptions.None)[2]);
            isAutoOff = ToBool(data.Split(new[] { "\\" }, StringSplitOptions.None)[3]);
            isAutoOffTime = Convert.ToUInt64(data.Split(new[] { "\\" }, StringSplitOptions.None)[4]);
            timeOffIs = Convert.ToUInt64(data.Split(new[] { "\\" }, StringSplitOptions.None)[5]);
            nowTimeInModule = new SHa.Time(Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[6]), Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[7]));
            alarmType = Convert.ToInt32(data.Split(new[] { "\\" }, StringSplitOptions.None)[8]);
            alarmTime = new SHa.Time(Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[9]), Convert.ToUInt16(data.Split(new[] { "\\" }, StringSplitOptions.None)[10]));
            alarmIndexDay = Convert.ToInt32(data.Split(new[] { "\\" }, StringSplitOptions.None)[11]);
            alarmIsBlinkLamp = ToBool(data.Split(new[] { "\\" }, StringSplitOptions.None)[12]);

        }

        private void MLempForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            timer1.Stop();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            WebSend("/timer/set?time=15&s_code=" + module.SCode);
            UpdateData();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            WebSend("/timer/set?time=30&s_code=" + module.SCode);
            UpdateData();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            WebSend("/timer/set?time=-1&s_code=" + module.SCode);
            UpdateData();
        }

        private void button8_Click(object sender, EventArgs e)
        {

            WebSend("/timer/set?time=45&s_code=" + module.SCode);
            UpdateData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Вы уверенны что хотите произвести сброс до зводских настроек?\nВсе данные дудут удалены!", "Smart House - Модуль - Сброс", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                WebSend("/redat?s_code=" + module.SCode);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            WebSend("/schedule/time_zone/set?time=" + (comboBox2.SelectedIndex + 1).ToString() + "&s_code=" + module.SCode);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown2, ref numericUpDown1);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            SHa.NumericUpDownSelect(ref numericUpDown2, ref numericUpDown1);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (!comboBox3.Items.Contains(comboBox3.Text))
            {
                MessageBox.Show("Выберите тип!", "Smart House - Модуль", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!comboBox4.Items.Contains(comboBox4.Text) && comboBox3.Items.IndexOf(comboBox3.Text) == 2)
            {
                MessageBox.Show("Выберите день недели!", "Smart House - Модуль", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string _isBlinkLamp = "1";
            if (checkBox2.Checked) _isBlinkLamp = "1"; else _isBlinkLamp = "0";

            WebSend(
                "/schedule/set" +
                "?type=" + comboBox3.Items.IndexOf(comboBox3.Text) +
                "&is_inv_lamp=" + _isBlinkLamp +
                "&f_hour=" + numericUpDown1.Value.ToString() +
                "&f_min=" + numericUpDown2.Value.ToString() +
                "&day_index=" + comboBox4.Items.IndexOf(comboBox4.Text) +
                "&s_code=" + module.SCode
                );
        }

        private void button11_Click(object sender, EventArgs e)
        {
            WebSend(
                "/schedule/set" +
                "?type=-1" +
                "&s_code=" + module.SCode
                );
        }
    }
}
