using System.Text.RegularExpressions;

namespace Core
{
    public static class RegexUtils
    {
        private static readonly Regex UrlPattern =
            new Regex(@"^(http|https)://+[^/]*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsUrlPath(string value)
        {
            return UrlPattern.IsMatch(value);
        }
        
        public static string MatchUrl(string value)
        {
            MatchCollection mc = UrlPattern.Matches(value);
            if (mc.Count == 1)
            {
                return mc[0].Value;
            }
            return string.Empty;
        }

        public static string ReplaceUrlPath(string value, string replaceValue)
        {
            MatchCollection mc = UrlPattern.Matches(value);
            if (mc.Count == 1)
            {
                return value.Replace(mc[0].Value + "/", replaceValue);
            }

            return string.Empty;
        }

        public static string MatchUrlPath(string value)
        {
            return ReplaceUrlPath(value, string.Empty);
        }

        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumber(string value)
        {
            return Regex.IsMatch(value, @"^[0-9]*$");
        }

        /// <summary>
        /// 是否是手机号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(string value)
        {
            return Regex.IsMatch(value, @"^[1]+[3,4,5,6,7,8]+\d{9}");
        }

        public static bool IsIp(string value)
        {
            string ipv4Pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            string ipv6Pattern = @"^(([0-9a-fA-F]{1,4}:){7}([0-9a-fA-F]{1,4}))$";
        
            return Regex.IsMatch(value, ipv4Pattern) || Regex.IsMatch(value, ipv6Pattern);
        }

        public static string Unescape(string value)
        {
            return Regex.Unescape(value);
        }
    }
}