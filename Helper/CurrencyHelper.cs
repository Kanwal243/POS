using System;
using EyeHospitalPOS.Models;

namespace EyeHospitalPOS.Helper
{
    public static class CurrencyHelper
    {
        public static string GetCurrencySymbol(string? currencyString)
        {
            if (string.IsNullOrEmpty(currencyString)) return "$";
            var start = currencyString.IndexOf('(');
            var end = currencyString.IndexOf(')');
            if (start != -1 && end != -1 && end > start + 1)
            {
                return currencyString.Substring(start + 1, end - start - 1);
            }
            return currencyString;
        }

        public static string GetCurrencySymbol(Organization? org)
        {
            return GetCurrencySymbol(org?.Currency);
        }
    }
}
