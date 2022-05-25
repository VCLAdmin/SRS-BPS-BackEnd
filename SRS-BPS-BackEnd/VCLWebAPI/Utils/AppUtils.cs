using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
//using System.Web;
using System.Web.Http;

namespace VCLWebAPI.Utils
{
    public class AppUtils
    {
        public  async Task<HttpResponseMessage> Post<T>(string uriActionString, T modelObject)
        {
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(GetBaseUrl());
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMinutes(15);
                //var token = GetAuthenticationToken();
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(@"Bearer", token);
                var body = new ObjectContent<T>(modelObject, new JsonMediaTypeFormatter());
                var response = await client.PostAsync(uriActionString, body);
                if (!response.IsSuccessStatusCode)
                    throw new HttpResponseException(response);
                return response;
            }
        }

        public static DateTime[] GetDateRange(string dateValue)
        {
            var dates = new DateTime[2];
            if (dateValue == "Year")
            {
                dates = GetDateRange(ApiEnums.FrequencyType.Annually, DateTime.Now);
            }
            else if (dateValue == "Month")
            {
                dates = GetDateRange(ApiEnums.FrequencyType.Monthly, DateTime.Now);
            }
            else if (dateValue == "Week")
            {
                dates = GetDateRange(ApiEnums.FrequencyType.Weekly, DateTime.Now);
            }
            else if (dateValue == "Today")
            {
                dates = GetDateRange(ApiEnums.FrequencyType.Daily, DateTime.Now);
            }
            else
            {
                dates[0] = new DateTime(Int32.Parse(dateValue), 1, 1);
                dates[1] = new DateTime(Int32.Parse(dateValue) + 1, 1, 1);
            }

            return dates;
        }
        public static DateTime[] GetDateRange(ApiEnums.FrequencyType frequency, DateTime dateToCheck)
        {
            DateTime[] result = new DateTime[2];
            DateTime dateRangeBegin = dateToCheck;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0); //One day 
            DateTime dateRangeEnd = DateTime.Today.Add(duration);

            switch (frequency)
            {
                case ApiEnums.FrequencyType.Daily:
                    dateRangeBegin = dateToCheck;
                    dateRangeEnd = dateRangeBegin;
                    break;

                case ApiEnums.FrequencyType.Weekly:
                    dateRangeBegin = dateToCheck.AddDays(-(int)dateToCheck.DayOfWeek);
                    dateRangeEnd = dateToCheck.AddDays(6 - (int)dateToCheck.DayOfWeek);
                    break;

                case ApiEnums.FrequencyType.Monthly:
                    duration = new TimeSpan(DateTime.DaysInMonth(dateToCheck.Year, dateToCheck.Month) - 1, 0, 0, 0);
                    dateRangeBegin = dateToCheck.AddDays((-1) * dateToCheck.Day + 1);
                    dateRangeEnd = dateRangeBegin.Add(duration);
                    break;

                case ApiEnums.FrequencyType.Quarterly:
                    int currentQuater = (dateToCheck.Date.Month - 1) / 3 + 1;
                    int daysInLastMonthOfQuarter = DateTime.DaysInMonth(dateToCheck.Year, 3 * currentQuater);
                    dateRangeBegin = new DateTime(dateToCheck.Year, 3 * currentQuater - 2, 1);
                    dateRangeEnd = new DateTime(dateToCheck.Year, 3 * currentQuater, daysInLastMonthOfQuarter);
                    break;

                case ApiEnums.FrequencyType.Annually:
                    dateRangeBegin = new DateTime(dateToCheck.Year, 1, 1);
                    dateRangeEnd = new DateTime(dateToCheck.Year, 12, 31);
                    break;
            }
            result[0] = dateRangeBegin.Date;
            result[1] = dateRangeEnd.Date.AddDays(1);
            return result;
        }

    }
}