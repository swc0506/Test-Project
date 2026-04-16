using System.Collections.Generic;

namespace Core
{
    public static class FormatUtils
    {
        private static readonly List<string> SizeUnits = new List<string>() {"B", "KB", "MB", "GB", "TB"};
        private static readonly List<string> TimeUnits = new List<string>() {"ms", "s", "m", "h"};

        public static string GetByteSizeText(long size)
        {
            int index = 0;
            double value = size;
            while (value >= 1024 && index < SizeUnits.Count - 1)
            {
                value /= 1024;
                index++;
            }

            return string.Format("{0:F2} {1}", value, SizeUnits[index]);
        }

        public static string GetMSTimeText(long ms)
        {
            int index = 0;
            if (ms < 1000)
            {
                return string.Format("{0} {1}", ms, TimeUnits[index]);
            }

            index++;
            double value = ms / 1000f;
            if (value < 60)
            {
                return string.Format("{0:F2} {1}", value, TimeUnits[index]);
            }

            while (value >= 60 && index < TimeUnits.Count - 1)
            {
                value /= 60;
                index++;
            }

            return string.Format("{0:F2} {1}", value, TimeUnits[index]);
        }
    }
}