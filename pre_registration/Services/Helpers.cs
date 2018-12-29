using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace pre_registration
{
    public class Helpers
    {
        public static string ConvertStringtoMD5(string strword)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(strword);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
        public static DateTime ConvertStringDateToDateTime(string inputString)
        {
            int[] dateComponents = inputString.Split('.').Select(n => Convert.ToInt32(n)).ToArray();
            return new DateTime(dateComponents[2], dateComponents[1], dateComponents[0]);
        }

        public static DateTime GetDateFromSession(string date)
        {
            try
            {
                var stringDate =date;
                int[] dateComponents = stringDate.Split('.').Select(n => Convert.ToInt32(n)).ToArray();
                return new DateTime(dateComponents[2], dateComponents[1], dateComponents[0]);

            }
            catch (Exception e)
            { 
                return new DateTime();
            }
        }
        public static string getAreaNameDeclination(string areaName, bool onlyAreaName = false)
        {
            if (areaName == "Мингорисполком")
                return "Мингорисполкома";
            else
            {
                string result = areaName;//.Replace("кий", "ого");
                result = result.Replace("район", "");
                result = result.Replace("кий", "кого");
                result = result.Replace("ный", "ного");
                result = result.Replace("кой", "кого");
                if (!onlyAreaName)
                    result = "администрации " + result + " района"; 
                else
                    result = result + " района";  
                return result;

            }
        }
        public static string getAreaNameDeclinationBY(string areaName, bool onlyAreaName = false)
        {
            if (areaName == "Мінгарвыканкам")
                return "Мінгарвыканкама";
            else
            {
                string result = areaName;//.Replace("кий", "ого");
                result = result.Replace("раён", "");
                result = result.Replace("скі", "скага");
                result = result.Replace("ны", "нага");
                result = result.Replace("цкі", "цкага");
                if (!onlyAreaName)
                    result = "адміністрацыі " + result + " раёна";
                else
                    result = result + " раёна";  //result = "Служба «одно окно» администрации " + result + " района";
                return result;

            }
        }
        
        public static string GetCulture(HttpContext context)
        {
            var rqf = context.Features.Get<Microsoft.AspNetCore.Localization.IRequestCultureFeature>();
            // Culture contains the information of the requested culture
            var culture = rqf.RequestCulture.Culture;
            return rqf.RequestCulture.Culture.Name;
        }
    }
}
