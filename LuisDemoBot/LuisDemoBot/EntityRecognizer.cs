using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Chronic;
using Microsoft.Bot.Builder.Luis.Models;

namespace LuisDemoBot
{
    public static class EntityRecognizer
    {
        public static EntityRecommendation FindEntity(IEnumerable<EntityRecommendation> entities, string entityType)
        {
            return entities.FirstOrDefault(e => e.Type == entityType);
        }

        public static IList<EntityRecommendation> FindEntities(IEnumerable<EntityRecommendation> entities, string entityType)
        {
            return entities.Where(e => e.Type == entityType).ToList();
        }

        public static void ParseDateTime(IEnumerable<EntityRecommendation> entities, out DateTime? parsedDate)
        {
            try
            {
                parsedDate = ResolveDateTime(entities);
            }
            catch (Exception)
            {
                parsedDate = null;
            }
        }

        public static void ParseDateTime(string utterance, out DateTime? parsedDate)
        {
            parsedDate = RecognizeTime(utterance);
        }

        public static void ParseNumber(string utterance, out double parsedNumber)
        {
            var numbeRegex = new Regex(@"[+-]?(?:\d+\.?\d*|\d*\.?\d+)");
            var matches = numbeRegex.Matches(utterance);

            if (matches.Count > 0)
            {
                double.TryParse(matches[0].Value, out parsedNumber);
                if (!double.IsNaN(parsedNumber))
                    return;
            }

            parsedNumber = double.NaN;
        }

        public static void ParseNumber(IEnumerable<EntityRecommendation> entities, out double parsedNumber)
        {
            var numberEntities = FindEntities(entities, "builtin.number");

            if (numberEntities != null && numberEntities.Any())
            {
                double.TryParse(numberEntities.First().Entity, out parsedNumber);
                if (!double.IsNaN(parsedNumber))
                    return;
            }

            parsedNumber = double.NaN;
        }

        public static void ParseBoolean(string utterance, out bool? parsedBoolean)
        {
            var boolTrueRegex = new Regex("(?i)^(1|y|yes|yep|sure|ok|true)");
            var boolFalseRegex = new Regex("(?i)^(2|n|no|nope|not|false)");

            var trueMatches = boolTrueRegex.Matches(utterance);
            if (trueMatches.Count > 0)
            {
                parsedBoolean = true;
                return;
            }

            var falseMatches = boolFalseRegex.Matches(utterance);
            if (falseMatches.Count > 0)
            {
                parsedBoolean = true;
                return;
            }

            parsedBoolean = null;
        }

        private static DateTime? ResolveDateTime(IEnumerable<EntityRecommendation> entities)
        {
            DateTime? date = null;
            DateTime? time = null;

            foreach (var entity in entities)
            {
                if (entity.Type.Contains("builtin.datetime") && entity.Resolution.Any())
                {
                    var dateTimeParts = entity.Resolution.First().Value.ToString().Split('T');

                    switch (entity.Resolution.First().Key)
                    {
                        case "date":
                            date = ParseLuisDateString(dateTimeParts[0]);

                            if (date.HasValue)
                            {
                                if (dateTimeParts.Length > 1 && !string.IsNullOrEmpty(dateTimeParts[1]))
                                    time = ParseLuisTimeString(dateTimeParts[1]);
                            }
                            break;
                        case "time":
                            if (entity.Resolution.First().Value.ToString() == "PRESENT_REF")
                            {
                                date = DateTime.Now;
                                time = DateTime.Now;
                            }
                            else
                            {
                                if (dateTimeParts.Length > 1)
                                {
                                    date = ParseLuisDateString(dateTimeParts[0]);
                                    time = ParseLuisTimeString(dateTimeParts[1]);
                                }
                            }
                            break;
                    }
                }

                if (date.HasValue)
                {
                    if (time.HasValue)
                    {
                        return new DateTime(date.Value.Year, date.Value.Month, date.Value.Day,
                            time.Value.Hour, time.Value.Minute, 0);
                    }

                    return date.Value;
                }
                if (time.HasValue)
                {
                    return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day,
                        time.Value.Hour, time.Value.Minute, 0);
                }
            }

            return null;
        }

        private static DateTime? ParseLuisDateString(string value)
        {
            if (value.Contains("XXXX-WXX-"))
            {
                var dayNumber = -1;
                Int32.TryParse(value.Replace("XXXX-WXX-", string.Empty), out dayNumber);
                if (dayNumber > -1)
                    return DateTime.Today.Next(dayNumber);

                return null;
            }
            int year;
            int month;
            int weekNumber;
            int day;

            var dateParts = value.Split('-');

            if (dateParts[0] != "XXXX")
            {
                year = Convert.ToInt16(dateParts[0]);
            }
            else
            {
                year = DateTime.Now.Year;
            }

            if (dateParts[1].Contains("W"))
            {
                if (!dateParts[1].Contains("XX"))
                {
                    weekNumber = Convert.ToInt16(dateParts[1].Substring(1, dateParts[1].Length - 1));
                    return FirstDateOfWeekIso8601(year, weekNumber);
                }
                month = DateTime.Now.Month;
            }
            else
            {
                month = Convert.ToInt16(dateParts[1]);
            }

            if (dateParts[2] != null && dateParts[2] != "XX")
            {
                day = Convert.ToInt16(dateParts[2]);
            }
            else
            {
                day = 1;
            }

            var dateString = string.Format("{0}-{1}-{2}", year, month, day);
            return DateTime.Parse(dateString);
        }

        private static DateTime? ParseLuisTimeString(string value)
        {
            switch (value)
            {
                case "MO":
                    return DateTime.MinValue.AddHours(10);
                case "MI":
                    return DateTime.MinValue.AddHours(12);
                case "AF":
                    return DateTime.MinValue.AddHours(15);
                case "EV":
                    return DateTime.MinValue.AddHours(18);
                case "NI":
                    return DateTime.MinValue.AddHours(20);
                default:
                    var timeParts = value.Split(':');
                    var hours = 0;
                    var minutes = 0;

                    if (timeParts[0] != null)
                    {
                        int.TryParse(timeParts[0], out hours);
                    }

                    if (timeParts[1] != null)
                    {
                        int.TryParse(timeParts[1], out minutes);
                    }

                    var returnDate = DateTime.MinValue;

                    if (hours > 0)
                        returnDate = returnDate.AddHours(hours);

                    if (minutes > 0)
                        returnDate = returnDate.AddMinutes(minutes);

                    return returnDate;
            }
        }

        private static DateTime? RecognizeTime(string utterance)
        {
            try
            {
                var parser = new Parser();
                if (!string.IsNullOrEmpty(utterance))
                {
                    var parsedObj = parser.Parse(utterance);
                    var parsedDateTime = parsedObj?.Start;
                    return parsedDateTime;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DateTime FirstDateOfWeekIso8601(int year, int weekOfYear)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            var firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            var firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

        public static DateTime Next(this DateTime from, int dayOfWeek)
        {
            var start = (int)from.DayOfWeek;
            var target = dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }

        public static TimeSpan? ParseTimeSpanFromDurationEntity(EntityRecommendation entity)
        {
            int? minutes = null;

            if (entity.Type == "builtin.datetimeV2.duration")
            {
                foreach (var vals in entity.Resolution.Values)
                {
                    if (((Newtonsoft.Json.Linq.JArray)vals).First.SelectToken("type").ToString() == "duration")
                    {
                        minutes = (int)((Newtonsoft.Json.Linq.JArray)vals).First.SelectToken("value");
                    }
                }
            }

            if (minutes.HasValue)
            {
                return new TimeSpan(0,0,0, minutes.Value);
            }

            return null;
        }

        public static DateTime? ParseDateTimeFromDurationEntity(EntityRecommendation entity, bool futureDatesOnly = true)
        {
            if (entity.Type == "builtin.datetimeV2.date")
            {
                foreach (var vals in entity.Resolution.Values)
                {
                    foreach (var resolvedDate in (Newtonsoft.Json.Linq.JArray) vals)
                    {
                        if (resolvedDate.SelectToken("type").ToString() == "date")
                        {
                            DateTime parsedDateTime;
                            DateTime.TryParse(resolvedDate.SelectToken("value").ToString(), out parsedDateTime);
                            if (parsedDateTime != DateTime.MinValue &&
                                ((futureDatesOnly && parsedDateTime > DateTime.Now) || (!futureDatesOnly)))
                            {
                                return parsedDateTime;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
