using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class Encipher
    {
        private readonly static byte[] SecretKey;
        static Encipher()
        {
            SecretKey = Convert.FromBase64String("+lb8h9IwK+upkzmZK1RazVms12y3CLHstebBilmPDwK6GbhIQaMovHDq1xrVWJmj4/V0ljiJ/fGlENHrbqGSGoQ23hGbEYu1hBv7zTCWFl1NHqZjGnjJMBzGozHSPE3PV72WnnmIB9FqvxIdjjDBDvlznlD6L/8Lba711W1a40fNLHVzAHjjxRbU+VtmrKHsBZU3Xp791Ci9ijHLkjJkENvKT2K7TQ6BkNk5JTakpcAMCM/YIoSp0FdMtXVGGDLkcOh/nF0/MbiHts7Ew6/yGeVxvopji+5WIH4LbpiYO3enVbVNJAlizdcXUWu25QynWZuQ1FqogeT6KXveNMIf7ntWHW76pfXcD/GyTDqCoDtyYxX8CGYc/tPQUvuNhJgU92LwX8eerbOls8Vp9TWqeIlAbNCtI+UUOR6aKVP6TopYnlTXkkBz1LVhV1SgswmeJtpxTRfIZtR92yaoA4NgqmU6EVbg0uZJzJcaTMuQc4YV+wN5iABykDeAWrYZy9AyAKzvZqKzvlPzKIGArRoI7Gefkwd/5yJlx4EeYHgvLwDG6EsfoQ3jYj2whWB/8uIceG1OejfJshiKip4hlF7bxPmAucTP6EtavDHKz2LB5dWeAecK4LXJ/TUzlRdvws3f1MBm/jkdTY8i9L0G/+K1yKTJgpJ20ONEt7yPm1Iz6/lj6vuiW+ecgLRY6Oz3w/5Tfc1zpT87aAPcIUd8jrE87gnEohTnE2y8LQLeHuj3TfS+Ox6FwEW5J4L3xTmgH/ECG/niMiDpa2lgtng9XUoa5dRSvlKTty7EC3qS704yHA4Q3ALvFRrT1NC1JArMe8o33OBlTWCebmTLHielV3saLZpqLYFHK9C5fuqC3ZIj94+wWhJuzc6tcW2eI/bzLnxUmvZvYk1I8LEQq851HJHRpyaX9P1prGDLvLwUqAfZxlBuLkKbtJVBaGlhXiH/OFXczXzQWzl+6+6WpKWRNEepNuV+B+1ONvORVYyTB00/y4RmN9uLCGoW70NUBQ/vsDMhtyc7gTBnjpeIPwat6+OgV9ghKvxsHS57aBiq+TDjoaVZcU8mA0pT/qlDA8lkaQNfs603zoU3tZr5vxgYvZRmpZ5gVHmTelONzixSKRpkF/8USkWDq9IgIby1LEMxO0EV3V0rAxSKzDlZqgBZ5It/lKcZA3NO4/nLQDK83k0PW5LqPUu8ORTqin4LKQHSxj0wNpibGDjNxvpkYMLize3sSU5tbqdAwluTL3j1Q7n3+Hn0i6i1dENaHXZRWNhRB72QH9+zZViW8HM8COi/14NAbWdJ7UFFN9ULE1Aw7mqCQSi7ZCPaRJMhXps0eGtamkXzEOpwCBl7S+cJp1EHZQEJuQ==");
        }


        public static string Encrypt(string inputVal)
        {
            var bytes = Encoding.UTF8.GetBytes(inputVal);
            Encrypt(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static void Encrypt(byte[] bytes)
        {
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] += SecretKey[i % SecretKey.Length];
            }
        }

        public static void Decrypt(byte[] bytes)
        {
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] -= SecretKey[i % SecretKey.Length];
            }
        }

        public static string Decrypt(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            Decrypt(bytes);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
