using System;
using System.Collections.Generic;
using System.Text;

namespace HLTBLogger.Utility
{
    public static class Extensions
    {
        #region String extenders
        public static string ZeroIfEmpty(this string s)
        {
            return string.IsNullOrWhiteSpace(s) ? "0" : s;
        }
        #endregion
    }
}
