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
    public partial class MainForm : Form
    {
        // struct Modeule for sava and work a data
        public struct Module
        {
            public string Name { get; set; }
            public string IP { get; set; }
            public string SCode { get; set; }
            public string Type { get; set; }

            public Module(string name, string type, string ip, string s_code)
            {
                this.Name = name;
                this.IP = ip;
                this.Type = type;
                this.SCode = s_code;
            }

            public string FormatList()
            {
                return this.Name + " - " + this.IP;
            }
            

            public string GetAllToSting()
            {
                return Name + " - " + IP + " - " + Type + " - " + SCode;
            }
        }


        public List<Module> modules = new List<Module>();
        private int DoubleModuleListSelect = -1;
        public static Module NModule;


        public MainForm()
        {
            InitializeComponent();
        }

        public static string GetValueFromItem(string str, string item)
        {
            string itemS = "<" + item + ">",
                   itemE = "</" + item + ">";
            string res = "";

            if (!str.Contains(itemS) && !str.Contains(itemE)) return null;

            try
            {
                res = str.Split(new[] { itemS }, StringSplitOptions.None)[1].Split(new[] { itemE }, StringSplitOptions.None)[0];
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

            return res;
        }

        private void LoadAndRangeModuleData()
        {
            string _str = Properties.Settings.Default.ModuleData;
            Console.WriteLine(_str);
            int countItems = 0;
            
            /*  **Format Data**       
               
              <MData>count items</MData>
              
              <item-{index}>
                   <name>value</name>
                   <ip>value</ip>
                   <s_code>value</s_code>
                   <type>value</type>
              </item-{index}>
            
            */
            
            if (GetValueFromItem(_str, "MData") != null) countItems = Convert.ToInt32(GetValueFromItem(_str, "MData"));
            else 
            {
                DialogResult message = MessageBox.Show(
                    "Ошибка чтения сохранений!\n" +
                    "Попробовать переписать сохранения?\n" +
                    "Приведёт к потере данных!\n\n" +
                    "Если вы впервые запустили программу, то всё хорошо.", 
                    "Error!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (message == DialogResult.Yes)
                {
                    ResetModuleData();
                }
                else
                {
                    Application.Exit();
                }
            }

            if (countItems > 0)
            {
                for (int i = 1; i <= countItems; i++)
                {
                    string str = GetValueFromItem(_str, "mo-" + i.ToString());
                    if (str != null)
                        modules.Add(new Module(name: GetValueFromItem(str, "name"), type: GetValueFromItem(str, "type"), ip: GetValueFromItem(str, "ip"), s_code: GetValueFromItem(str, "s_code")));
                }               
            }

        }

        public void SavaModuleData()
        {
            string strForWrite = "";

            strForWrite += "<MData>" + modules.Count.ToString() + "</MData>";

            if (modules.Count > 0)
                for (int i = 1; i <= modules.Count; i++)
                {
                    strForWrite += "<mo-" + i.ToString() + ">";

                    strForWrite += "<name>" + modules[i - 1].Name + "</name>";
                    strForWrite += "<type>" + modules[i - 1].Type + "</type>";
                    strForWrite += "<s_code>" + modules[i - 1].SCode + "</s_code>";
                    strForWrite += "<ip>" + modules[i - 1].IP + "</ip>";

                    strForWrite += "</mo-" + i.ToString() + ">";
                }

            Properties.Settings.Default.ModuleData = strForWrite;
            Properties.Settings.Default.Save();
            Console.WriteLine("Save data.");
            Console.WriteLine(strForWrite);
        }

        public void ResetModuleData()
        {
            Console.WriteLine("Reset data.");
            //Properties.Settings.Default.ModuleData = "<MData>0</MData>";
            //Properties.Settings.Default.Save();
            modules.Clear();
            MessageBox.Show("Произведён сброс данных.", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Exit();
        }

        private string[] GetRangeFormatForList()
        {
            string[] strArray = new string[modules.Count];
            for (int i = 0; i < modules.Count; i++)
                strArray[i] = modules[i].FormatList();

            return strArray;
        }

        public void ModuleList_Update()
        {
            moduleList.Items.Clear();
            moduleList.Items.AddRange(GetRangeFormatForList());
        }

        public string CheckVersionApp()
        {
            try
            {
                using (var wb = new WebClient())
                {
                    string response = wb.DownloadString("https://" + "akylinandrej.wixsite.com/colden-i/key-passwd-rz6az-uz5q9-0z294-wgyb9-0t2q5-6dqy7-yy1pw-uwifm-taub2-gjkso-ghmo3-195mg-09na0-xzpd9");
                    string newVersion = response.Split(new[] { "smart_house_win_app\\" }, StringSplitOptions.None)[1].Split(new[] { "\\" }, StringSplitOptions.None)[0];
                    Console.WriteLine(response);
                    if (newVersion != Properties.Settings.Default.version)
                        MessageBox.Show("Найдена новая версия!", "Smart House - Обновление!", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                }
            }
            catch (System.Net.WebException)
            {

            }
            return "";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Height = Properties.Settings.Default.Height;
            Width = Properties.Settings.Default.Width;
            //CheckVersionApp();
            LoadAndRangeModuleData();
            ModuleList_Update();

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Width = Width;
            Properties.Settings.Default.Height = Height;
            Properties.Settings.Default.Save();
            SavaModuleData();
            
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            AddModuleForm addModuleForm = new AddModuleForm();
            addModuleForm.Owner = this;
            addModuleForm.ShowDialog();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.Owner = this;
            settingsForm.Show();
        }

        private void удалитьМодульToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (moduleList.SelectedIndex >= 0)
            {
                modules.RemoveAt(moduleList.SelectedIndex);
                SavaModuleData();
                ModuleList_Update();
            }
            else MessageBox.Show("Выберите модуль!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void изменитьToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (moduleList.SelectedIndex >= 0)
            {
                if (toolStripTextBox3.Text.Length > 4)
                {
                    modules[moduleList.SelectedIndex] = new Module(toolStripTextBox3.Text, modules[moduleList.SelectedIndex].Type, modules[moduleList.SelectedIndex].IP, modules[moduleList.SelectedIndex].SCode);
                    SavaModuleData();
                    ModuleList_Update();
                }
                else MessageBox.Show("Имя слишком короткое.", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else MessageBox.Show("Выберите модуль!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button_openWorkModule(object sender, EventArgs e)
        {
            if (moduleList.SelectedIndex >= 0)
            {
                openWorkModule();
            }
            else MessageBox.Show("Выберите модуль!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void openWorkModule()
        {
            Console.WriteLine(modules[moduleList.SelectedIndex].GetAllToSting());
            //NModule = modules[moduleList.SelectedIndex];
            openModuleWindow(modules[moduleList.SelectedIndex]);
        }

        public bool isModuleOnCorrectTypeIP(Module _module)
        {
            this.Text = "Smart House - (подключение)";
            this.Enabled = false;
            try
            {
                string _type = "";
                using (var wb = new WebClient())
                {
                    _type = wb.DownloadString("http://" + _module.IP + "/test");
                }
                if (_type == _module.Type) return true;
                else return false;
            }
            catch (System.Net.WebException)
            {
                return false;
            }
        }

        public void openModuleWindow(Module _module)
        {
            switch (_module.Type)
            {
                case "socket":
                    if (isModuleOnCorrectTypeIP(_module))
                    {
                        MSocketForm mLempForm = new MSocketForm(_module);
                        mLempForm.Owner = this;
                        this.Enabled = true;
                        mLempForm.Show();
                    }
                    else MessageBox.Show("Ошибка соединения! Невозможно установить связь с устройством. Проверьте устройство. Перезагрузите устройство. Если ошибка остаётся напишите нам.", "Smart House - Модуль", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "lemp":
                    if (isModuleOnCorrectTypeIP(_module)) 
                    {
                        MLempForm mLempForm = new MLempForm(_module);
                        mLempForm.Owner = this;
                        this.Enabled = true;
                        mLempForm.Show(); 
                    }
                    else MessageBox.Show("Ошибка соединения! Невозможно установить связь с устройством. Проверьте устройство. Перезагрузите устройство. Если ошибка остаётся напишите нам.", "Smart House - Модуль", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "rgb_lamp":
                    MessageBox.Show("нет окна!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "temp_c":
                    MessageBox.Show("нет окна!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case "temp_m":
                    if (isModuleOnCorrectTypeIP(_module))
                    {
                        MTempMForm mLempForm = new MTempMForm(_module);
                        mLempForm.Owner = this;
                        this.Enabled = true;
                        mLempForm.Show();
                    }
                    else MessageBox.Show("Ошибка соединения! Невозможно установить связь с устройством. Проверьте устройство. Перезагрузите устройство. Если ошибка остаётся напишите нам.", "Smart House - Модуль", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                
            }

            this.Enabled = true;
            this.Text = "Smart House";
        }

        private void moduleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (moduleList.SelectedIndex >= 0 && DoubleModuleListSelect == moduleList.SelectedIndex)
            {            
                DoubleModuleListSelect = -1;
                openWorkModule();
            }
            DoubleModuleListSelect = moduleList.SelectedIndex;

        }

        private void информацияОМодулеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (moduleList.SelectedIndex >= 0)
            {
                MessageBox.Show("Информация о модуле\n" +
                                "\n   Имя:\t\t" + modules[moduleList.SelectedIndex].Name +
                                "\n   IP:\t\t" + modules[moduleList.SelectedIndex].IP +
                                "\n   Тип:\t\t" + modules[moduleList.SelectedIndex].Type +
                                "\n   Код защиты:\t" + modules[moduleList.SelectedIndex].SCode,
                                "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else MessageBox.Show("Выберите модуль!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.Show();
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ip = this.toolStripTextBox1.Text;


            if (moduleList.SelectedIndex >= 0)
            {
                try { 
                    if (ip.Split('.').Length == 4 && Convert.ToInt32(ip.Split('.')[0]) < 256 && Convert.ToInt32(ip.Split('.')[1]) < 256 && Convert.ToInt32(ip.Split('.')[2]) < 256 && Convert.ToInt32(ip.Split('.')[3]) < 256)
                    {

                        this.Text = "Smart House - (подключение)";
                        this.Enabled = false;

                        using (var wb = new WebClient())
                        {
                            string response = "";
                            string _type = "";
                            try
                            {
                                response = wb.DownloadString("http://" + ip + "/security_code_check?s_code=" + modules[moduleList.SelectedIndex].SCode);
                                _type = wb.DownloadString("http://" + ip + "/test");
                            }
                            catch (System.Net.WebException)
                            {
                                MessageBox.Show("IP адрес неверен или модуль неисправен!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            if (_type != modules[moduleList.SelectedIndex].Type) MessageBox.Show("Тип модуля неверен!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else if (response == "s_code_incorrect") MessageBox.Show("Код защиты неверен!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            if (response == "s_code_correct" && _type == modules[moduleList.SelectedIndex].Type)
                            {
                                modules[moduleList.SelectedIndex] = new Module(modules[moduleList.SelectedIndex].Name, modules[moduleList.SelectedIndex].Type, ip, modules[moduleList.SelectedIndex].SCode);
                                SavaModuleData();
                                ModuleList_Update();
                            }

                        }
                    }
                    else MessageBox.Show("IP адрес неверен!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (FormatException)
                {
                    MessageBox.Show("IP адрес неверен!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else MessageBox.Show("Выберите модуль!", "Smart House - Настойки", MessageBoxButtons.OK, MessageBoxIcon.Error);



            this.Enabled = true;
            this.Text = "Smart House";

        }

        private void toolStripTextBox3_Click(object sender, EventArgs e)
        {
            toolStripTextBox3.Text = "";
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
            toolStripTextBox1.Text = "";
        }
    }
}
