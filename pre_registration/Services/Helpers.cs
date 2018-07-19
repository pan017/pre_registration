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
        public static string getAreaNameDeclination(string areaName)
        {
            if (areaName == "Мингорисполком")
                return "Мингорисполкома";
            else
            {
                string result = areaName;//.Replace("кий", "ого");
                result = result.Replace("район", "");
                result = result.Replace("кий", "ого");
                result = result.Replace("ный", "ного");
                result = result.Replace("кой", "кого");
                result = "администрации " + result + " района";
                return result;
            }
        }
    }
}
