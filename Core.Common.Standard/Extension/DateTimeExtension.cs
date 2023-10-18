using System;
using System.Globalization;

namespace KY.Core
{
    public static class DateTimeExtension
    {
        public static int GetWeek(this DateTime day, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(day, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
        }

        public static int GetNumberOfWeeksInMonth(this DateTime date)
        {
            int weeks = 0;
            int month = date.Month;
            DateTime currentDate = new DateTime(date.Year, month, 1);
            while (currentDate.Month == month)
            {
                weeks++;
                currentDate = currentDate.AddDays(7);
            }
            return weeks;
        }

        public static int GetNumberOfWorkdays(this DateTime from, DateTime to)
        {
            int days = 0;
            var date = from;
            while (date.Date <= to.Date)
            {
                if (!date.IsWeekend())
                {
                    days++;
                }
                date = date.AddDays(1);
            }
            return days;
        }

        public static DateTime GetFirstDayOfWeek(this DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetFirstDayOfWeek(dayInWeek, defaultCultureInfo);
        }

        public static DateTime GetFirstDayOfWeek(this DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            return firstDayInWeek;
        }

        public static DateTime GetFirstDayOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new(year, 1, 1);
            DateTime firstThursday = jan1.AddDays(DayOfWeek.Thursday - jan1.DayOfWeek);
            int firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            int weekNumber = weekOfYear;
            if (firstWeek == 1)
            {
                weekNumber -= 1;
            }
            DateTime result = firstThursday.AddDays(weekNumber * 7);
            return result.AddDays(-3);
        }

        public static DateTime GetFirstDayOfWeekInThisMonth(this DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return Max(GetFirstDayOfWeek(dayInWeek, defaultCultureInfo), dayInWeek.GetFirstDayOfMonth());
        }

        public static DateTime GetLastDayOfWeek(this DateTime dayInWeek)
        {
            return dayInWeek.GetFirstDayOfWeek().AddDays(6);
        }

        public static DateTime GetLastDayOfWeek(this DateTime dayInWeek, CultureInfo cultureInfo)
        {
            return dayInWeek.GetFirstDayOfWeek(cultureInfo).AddDays(6);
        }

        public static DateTime GetLastDayOfWeekInThisMonth(this DateTime dayInWeek)
        {
            return Min(dayInWeek.GetFirstDayOfWeek().AddDays(6), dayInWeek.GetLastDayOfMonth());
        }

        public static DateTime GetFirstDayOfMonth(this DateTime dayInMonth)
        {
            return new DateTime(dayInMonth.Year, dayInMonth.Month, 1);
        }

        public static DateTime GetLastDayOfMonth(this DateTime dayInMonth)
        {
            return new DateTime(dayInMonth.Year, dayInMonth.Month, DateTime.DaysInMonth(dayInMonth.Year, dayInMonth.Month), 23, 59, 59, 999);
        }

        public static DateTime Min(DateTime left, DateTime right)
        {
            return left < right ? left : right;
        }

        public static DateTime Max(DateTime left, DateTime right)
        {
            return left > right ? left : right;
        }

        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        public static int GetMonths(this DateTime start, DateTime end)
        {
            if (start < end)
            {
                return end.GetMonths(start);
            }
            return (start.Year - end.Year) * 12 + start.Month - end.Month;
        }
    }
}
