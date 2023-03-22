using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Smart_House
{
    public class SHa
    {
        public struct Time
        {
            public uint Min { set; get; }
            public uint Hour { set; get; }

            public Time(uint hour, uint min)
            {
                this.Hour = hour;
                this.Min = min;
            }

            public string Format(string sep = ":")
            {
                if (this.Min < 10) return this.Hour.ToString() + sep + "0" + this.Min.ToString();
                else return this.Hour.ToString() + sep + this.Min.ToString();
            }
            public void Set(uint hour, uint min)
            {
                this.Min = min;
                this.Hour = hour;
            }

            public static Time Zero = new Time(0, 0);
        }

        public struct Schedule
        {
            public int Type { get; set; }
            public int IndexDay { get; set; }
            public Time startTime { get; set; }
            public Time endTime { get; set; }

        }





        public static void NumericUpDownSelect (ref NumericUpDown min, ref NumericUpDown hour)
        {
            if (min.Value >= min.Maximum)
            {
                min.Value = 0;
                hour.Value++;         
            }
            if (hour.Value >= hour.Maximum) hour.Value = 0;
        }
    }
}
