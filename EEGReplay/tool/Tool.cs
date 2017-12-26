using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGReplay
{
    class Tool
    {
        public static String getTimeStringBySeconds(int seconds)
        {
            StringBuilder sb = new StringBuilder();

            int hour = seconds / 3600;
            int minute = (seconds - hour * 3600) / 60;
            int second = seconds - hour * 3600 - minute * 60;
            sb.Append(hour.ToString().PadLeft(2, '0')).Append(":").Append(minute.ToString().PadLeft(2, '0')).Append(":").Append(second.ToString().PadLeft(2, '0'));

            return sb.ToString();
        }
    }
}
