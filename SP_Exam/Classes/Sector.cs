using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SP_Exam.Classes
{
    internal class Sector
    {
        public int Number { get; set; }
        public List<Fan> Fans { get; set; }
        public bool IsQueueToFill { get; set; }

        public Sector(int num) 
        {
            Number = num;
            Fans = new List<Fan>();
            IsQueueToFill = true;
        }

        public void UpdateFields(/*ListBox listBox*/)
        {
            while (Fans.Count > 0)
            {
                Monitor.Enter(this);
                try
                {
                    if (Fans.Count > 0) 
                    {
                        Fans.Remove(Fans[0]);
                    }
                }
                finally
                {
                    Monitor.Exit(this);
                }
                //Thread.Sleep(100);
            }
        }
    }
}
