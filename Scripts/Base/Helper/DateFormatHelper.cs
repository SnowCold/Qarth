//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qarth
{
    public class DateFormatHelper
    {
        /// <summary>
        /// 大写一到十
        /// </summary>
        /// <param name="dayNum"></param>
        /// <returns></returns>
        private static string FormatCaseOneToTen(int dayNum)
        {
            switch (dayNum)
            {
                case 1:
                    return TDLanguageTable.Get("One");
                case 2:
                    return TDLanguageTable.Get("Two");
                case 3:
                    return TDLanguageTable.Get("Three");
                case 4:
                    return TDLanguageTable.Get("Four");
                case 5:
                    return TDLanguageTable.Get("Five");
                case 6:
                    return TDLanguageTable.Get("Six");
                case 7:
                    return TDLanguageTable.Get("Seven");
                case 8:
                    return TDLanguageTable.Get("Eight");
                case 9:
                    return TDLanguageTable.Get("Nine");
                case 10:
                    return TDLanguageTable.Get("Ten");
            }
            return string.Empty;
        }

        /// <summary>
        /// 格式化输出  今天，明天， x天后，x天后， 从1开始
        /// </summary>
        /// <param name="dayNum"></param>
        /// <returns></returns>
        public static string FormatDateDayNum(int dayNum)
        {
            if (dayNum == 1)
            {
                return TDLanguageTable.Get("UI_Summon_Today");
            }
            else if (dayNum == 2)
            {
                return TDLanguageTable.Get("UI_Summon_Tomorrow");
            }
            else
            {
                return TDLanguageTable.GetFormat("UI_Summon_Days", FormatCaseDayNum(dayNum - 1));
            }

        }

        /// <summary>
        /// 格式化天，大写 一，二，三，四，十一， 二十, 最大支持99, 从一开始
        /// </summary>
        /// <param name="dayNum"></param>
        /// <returns></returns>
        public static string FormatCaseDayNum(int dayNum)
        {
            int oneDigit = dayNum / 10;
            int twoDigit = dayNum % 10;
            if (dayNum > 10)
            {
                if (oneDigit == 1)
                {
                    return string.Format("{0}{1}", FormatCaseOneToTen(10), FormatCaseOneToTen(twoDigit));
                }
                else
                {
                    if (twoDigit == 0)
                    {
                        return string.Format("{0}{1}", FormatCaseOneToTen(oneDigit), FormatCaseOneToTen(10));
                    }
                    else
                    {
                        return string.Format("{0}{1}", FormatCaseOneToTen(oneDigit), FormatCaseOneToTen(twoDigit));
                    }
                }
            }
            else
            {
                return string.Format("{0}", FormatCaseOneToTen(dayNum));
            }
        }

        /// <summary>
        /// 最大单位是天，输出1位(x天、x小时, x分, x秒）
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatMaxUnitDayOutOne(long timestamp)
        {
            long day = timestamp / 86400;
            long hours = (timestamp % 86400) / 3600;
            long minute = (timestamp % 3600) / 60;
            long second = (timestamp % 60);
            string dayFormat = TDLanguageTable.Get("Day");
            string hoursFormat = TDLanguageTable.Get("Hours");
            string minuteFormat = TDLanguageTable.Get("Minute");
            string secondFormat = TDLanguageTable.Get("Second");
            if (day >= 1)
            {
                // X天
                return string.Format("{0}{1}", day, dayFormat);
            }
            else
            {
                if (hours >= 1)
                {
                    // X小时
                    return string.Format("{0}{1}", hours, hoursFormat);
                }
                else
                {
                    if (minute >= 1)
                    {
                        // X分
                        return string.Format("{0}{1}", minute, minuteFormat);
                    }
                    else
                    {
                        // X秒
                        return string.Format("{0}{1}", second, secondFormat);
                    }
                }
            }
        }

        /// <summary>
        /// 最大单位是天，输出2位(x天y小时、x小时y分、x分y秒）
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatMaxUnitDayOutTwo(long timestamp)
        {
            long day = timestamp / 86400;
            long hours = (timestamp % 86400) / 3600;
            long minute = (timestamp % 3600) / 60;
            long second = (timestamp % 60);
            string dayFormat = TDLanguageTable.Get("Day");
            string hoursFormat = TDLanguageTable.Get("Hours");
            string minuteFormat = TDLanguageTable.Get("Minute");
            string secondFormat = TDLanguageTable.Get("Second");
            if (day >= 1)
            {
                // X天Y小时
                return string.Format("{0}{1}{2}{3}", day, dayFormat, hours, hoursFormat);
            }
            else
            {
                if (hours >= 1)
                {
                    // X小时Y分
                    return string.Format("{0}{1}{2}{3}", hours, hoursFormat, minute, minuteFormat);
                }
                else
                {
                    // X分Y秒
                    return string.Format("{0}{1}{2}{3}", minute, minuteFormat, second, secondFormat);
                }
            }
        }

        /// <summary>
        /// 最大单位是天，输出2位(x天y小时、x小时y分、x分y秒）
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatMaxUnitTimeOutTwo(long timestamp)
        {
            long day = timestamp / 86400;
            long hours = (timestamp % 86400) / 3600;
            long minute = (timestamp % 3600) / 60;
            long second = (timestamp % 60);

            if (day >= 1)
            {
                // X天Y小时
                return string.Format("{0}:{1:D2}", day, hours);
            }
            else
            {
                if (hours >= 1)
                {
                    // X小时Y分
                    return string.Format("{0:D2}:{1:D2}", hours, minute);
                }
                else
                {
                    // X分Y秒
                    return string.Format("{0:D2}:{1:D2}", minute, second);
                }
            }
        }

        /// <summary>
        /// 最大单位是天，
        ///     时间格式(大于1天) ：7天3时6分10秒
        ///     时间格式(小于1天) ：3时6分10秒
        ///     时间格式（小于1小时）：6分10秒
        ///     时间格式（小于1小时）：10秒
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatRemainTime(long timestamp)
        {
            long day = timestamp / 86400;
            long hours = (timestamp % 86400) / 3600;
            long minute = (timestamp % 3600) / 60;
            long second = (timestamp % 60);
            string dayFormat = TDLanguageTable.Get("Day");
            string hoursFormat = TDLanguageTable.Get("HoursEx");
            string minuteFormat = TDLanguageTable.Get("Minute");
            string secondFormat = TDLanguageTable.Get("Second");
            if (day >= 1)
            {
                // a天b时c分d秒
                return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", day, dayFormat, hours, hoursFormat
                    , minute, minuteFormat, second, secondFormat);
            }
            else
            {
                if (hours >= 1)
                {
                    // b时c分d秒
                    return string.Format("{0}{1}{2}{3}{4}{5}", hours, hoursFormat
                        , minute, minuteFormat, second, secondFormat);
                }
                else
                {
                    if (minute >= 1)
                    {
                        // c分d秒
                        return string.Format("{0}{1}{2}{3}", minute, minuteFormat, second, secondFormat);
                    }
                    else
                    {
                        // d秒
                        return string.Format("{0}{1}", second, secondFormat);
                    }
                }
            }
        }


        /// <summary>
        /// 最大单位是天，输出2位(x天y小时、x小时y分、x分y秒）
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatColorMaxUnitDayOutTwo(long timestamp)
        {
            long day = timestamp / 86400;
            long hours = (timestamp % 86400) / 3600;
            long minute = (timestamp % 3600) / 60;
            long second = (timestamp % 60);
            string dayFormat = TDLanguageTable.Get("Day");
            string hoursFormat = TDLanguageTable.Get("Hours");
            string minuteFormat = TDLanguageTable.Get("Minute");
            string secondFormat = TDLanguageTable.Get("Second");
            if (day >= 1)
            {
                // X天Y小时
                return string.Format("[a1ee3e]{0}[-]{1}[a1ee3e]{2}[-]{3}", day, dayFormat, hours, hoursFormat);
            }
            else
            {
                if (hours >= 1)
                {
                    // X小时Y分
                    return string.Format("[a1ee3e]{0}[-]{1}[a1ee3e]{2}[-]{3}", hours, hoursFormat, minute, minuteFormat);
                }
                else
                {
                    // X分Y秒
                    return string.Format("[a1ee3e]{0}[-]{1}[a1ee3e]{2}[-]{3}", minute, minuteFormat, second, secondFormat);
                }
            }
        }

        /// <summary>
        /// 最大单位是天，输出2位(x天y时、x时y分、x分y秒）
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatColorMaxUnitDayOutTwoEx(long timestamp)
        {
            long day = timestamp / 86400;
            long hours = (timestamp % 86400) / 3600;
            long minute = (timestamp % 3600) / 60;
            long second = (timestamp % 60);
            string dayFormat = TDLanguageTable.Get("Day");
            string hoursFormat = TDLanguageTable.Get("HoursEx");
            string minuteFormat = TDLanguageTable.Get("Minute");
            string secondFormat = TDLanguageTable.Get("Second");
            if (day >= 1)
            {
                // X天Y小时
                return string.Format("[a1ee3e]{0}{1}{2}{3}[-]", day, dayFormat, hours, hoursFormat);
            }
            else
            {
                if (hours >= 1)
                {
                    // X小时Y分
                    return string.Format("[a1ee3e]{0}{1}{2}{3}[-]", hours, hoursFormat, minute, minuteFormat);
                }
                else
                {
                    // X分Y秒
                    return string.Format("[a1ee3e]{0}{1}{2}{3}[-]", minute, minuteFormat, second, secondFormat);
                }
            }
        }


        // 输出格式化时间 00:00:00
        public static string FormatTime(int seconds)
        {
            int hour = seconds / 3600;
            int min = (seconds % 3600) / 60;
            int sec = seconds % 60;
            return string.Format("{0}:{1}:{2}", hour.ToString("D2"), min.ToString("D2"), sec.ToString("D2"));
        }
    }
}
