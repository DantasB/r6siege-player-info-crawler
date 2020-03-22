using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R6SCrawler
{
    class Utils
    {
        private static readonly int hour   = 24;
        private static readonly int minute = 60;

        private static int CasualDays     { get; set; }
        private static int RankedDays     { get; set; }
        private static int GeneralDays    { get; set; }
        private static int CasualHours    { get; set; }
        private static int RankedHours    { get; set; }
        private static int GeneralHours   { get; set; }
        private static int CasualMinutes  { get; set; }
        private static int RankedMinutes  { get; set; }
        private static int GeneralMinutes { get; set; }

        public static double DateToMinute(string generalDate, string casualDate, string rankedDate)
        {
            ResetTime();

            string [] generalSplitedDate = generalDate.Split(' ');
            string [] casualSplitedDate = casualDate.Split(' ');
            string [] rankedSplitedDate = rankedDate.Split(' ');

            foreach (string element in casualSplitedDate)
            {
                if (element.Contains('d'))
                {
                    CasualDays = int.Parse(element.Split('d')[0])*hour*minute;
                    continue;
                }
                if (element.Contains('h'))
                {
                    CasualHours   = int.Parse(element.Split('h')[0])*minute;
                    continue;
                }
                if (element.Contains('m'))
                {
                    CasualMinutes = int.Parse(element.Split('m')[0]);
                    continue;
                }
            }

            foreach (string element in rankedSplitedDate)
            {
                if (element.Contains('d'))
                {
                    RankedDays = int.Parse(element.Split('d')[0]) * hour * minute;
                    continue;
                }
                if (element.Contains('h'))
                {
                    RankedHours = int.Parse(element.Split('h')[0]) * minute;
                    continue;
                }
                if (element.Contains('m'))
                {
                    RankedMinutes = int.Parse(element.Split('m')[0]);
                    continue;
                }
            }

            foreach (string element in generalSplitedDate)
            {
                if (element.Contains('d'))
                {
                    GeneralDays = int.Parse(element.Split('d')[0]) * hour * minute;
                    continue;
                }
                if (element.Contains('h'))
                {
                    GeneralHours = int.Parse(element.Split('h')[0]) * minute;
                    continue;
                }
                if (element.Contains('m'))
                {
                    GeneralMinutes = int.Parse(element.Split('m')[0]);
                    continue;
                }
            }

            double time = (GeneralDays + GeneralHours + GeneralMinutes) - (CasualDays + CasualHours + CasualMinutes + RankedDays + RankedHours + RankedMinutes);
          
            return time;
        }

        public static double DateToMinute(string date)
        {
            ResetTime();

            string[] splitedDate = date.Split(' ');

            foreach (string element in splitedDate)
            {
                if (element.Contains('d'))
                {
                    RankedDays = int.Parse(element.Split('d')[0]) * hour * minute;
                    continue;
                }
                if (element.Contains('h'))
                {
                    RankedHours = int.Parse(element.Split('h')[0]) * minute;
                    continue;
                }
                if (element.Contains('m'))
                {
                    RankedMinutes = int.Parse(element.Split('m')[0]);
                    continue;
                }
            }

            double time = (RankedDays + RankedHours + RankedMinutes);

            return time;
        }

        private static void ResetTime()
        {
            CasualDays     = 0;
            RankedDays     = 0;
            GeneralDays    = 0;
            CasualHours    = 0;
            RankedHours    = 0;
            GeneralHours   = 0;
            CasualMinutes  = 0;
            RankedMinutes  = 0;
            GeneralMinutes = 0;
        }

        public static int Parse(string actualData, string oldData)
        {
            if (actualData.Contains(","))
            {
               actualData = actualData.Replace(",", string.Empty);
            }

            if (oldData.Contains(","))
            {
                oldData = oldData.Replace(",", string.Empty);
            }

            return int.Parse(actualData) - int.Parse(oldData);
        }

    }
}
