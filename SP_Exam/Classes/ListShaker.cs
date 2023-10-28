using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_Exam.Classes
{
    internal static class ListShaker
    {
        public static void MakeMixList<t>(IList<t> list)
        {
            Random r = new Random();

            SortedList<int, t> mixedList = new SortedList<int, t>();
            foreach (t item in list)
                mixedList.Add(r.Next(), item);

            list.Clear();
            for (int i = 0; i < mixedList.Count; i++)
            {
                list.Add(mixedList.Values[i]);
            }
            mixedList.Clear();  
        }
    }
}
