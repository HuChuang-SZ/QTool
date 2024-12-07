using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QTool
{
    public static class VerificationHelper
    {
        public static bool TryGetGroupValue(this string input, string pattern, int groupIndex, out string result)
        {
            var match = Regex.Match(input, pattern);
            if (match.Success)
            {
                result = match.Groups[groupIndex].Value;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public static bool IsPhone(string phone)
        {
            return phone?.Length == 11 && Regex.IsMatch(phone, "^1[3456789]\\d{9}$");
        }

        public static bool IsPhone(long phone)
        {
            return IsPhone(phone.ToString());
        }


        public static string VerifyByAccountName(this string accountName)
        {
            if (string.IsNullOrEmpty(accountName))
            {
                return "账号不能为空。";
            }
            else if (Regex.IsMatch(accountName, "^[a-z0-9]{4,20}$", RegexOptions.IgnoreCase))
            {
                return string.Empty;
            }
            else
            {
                return "账号只允许输入4-32个字符，只支持字母或数字。";
            }
        }

        public static bool IsVerifyCode(string verifyCode)
        {
            return verifyCode?.Length == 6 && Regex.IsMatch(verifyCode, "^\\d{6}$");
        }

        public static bool IsVerifyCode(int verifyCode)
        {
            return verifyCode >= 100000 && verifyCode <= 999999;
        }

        public static bool IsPwd(string password)
        {
            return password?.Length >= 6 && password.Length <= 16 || password != null && Regex.IsMatch(password, "^[a-fA-F0-9]{32}$");
        }

        public static bool IsOsIdentity(string osIdentity)
        {
            return osIdentity != null && Regex.IsMatch(osIdentity, "^[0-9a-f]{32}$");
        }

        public static void Verify(this IDataErrorInfo data)
        {
            var errors = data.Error;
            if (!string.IsNullOrEmpty(errors))
                throw new QException(errors);
        }

        public static bool TryVerify(this IDataErrorInfo data, out string error)
        {
            error = data.Error;
            return string.IsNullOrWhiteSpace(error);
        }


        public static string VerifyByPhone(this string phone)
        {
            if (IsPhone(phone))
            {
                return string.Empty;
            }
            else
            {
                return "请输入正确的手机号。";
            }
        }

        public static string VerifyByPhone(this long phone)
        {
            return VerifyByPhone(phone.ToString());
        }

        public static string VerifyByVerifyCode(this string verifyCode)
        {
            if (IsVerifyCode(verifyCode))
            {
                return string.Empty;
            }
            else
            {
                return "验证码必须是6位数字。";
            }
        }

        public static string VerifyByVerifyCode(this int verifyCode)
        {
            return VerifyByVerifyCode(verifyCode.ToString());
        }

        public static string VerifyByOsIdentity(this string osIdentity)
        {
            if (IsOsIdentity(osIdentity))
            {
                return string.Empty;
            }
            else
            {
                return "系统标识无效。";
            }
        }

        public static string VerifyByPwd(this string password)
        {
            if (IsPwd(password))
            {
                return string.Empty;
            }
            else
            {
                return "密码必须是6-16位字符。";
            }
        }

        public static string VerifyByPwd(string accountPwd, string reAccountPwd)
        {
            var msg = VerifyByPwd(reAccountPwd);
            if (string.IsNullOrEmpty(msg))
            {
                if (accountPwd != reAccountPwd)
                {
                    msg = "两次密码必须一致。";
                }
            }

            return msg;
        }

        public static string VerifyByMD5Pwd(this string password, string title = "密码")
        {
            if (password == null)
            {
                return $"{title}不能为空";
            }
            else if (Regex.IsMatch(password, "^[a-z0-9]{32}$", RegexOptions.IgnoreCase))
            {
                return string.Empty;
            }
            else
            {
                return $"{title}必须是8-16位字符。";
            }
        }

        public static void ThrowException(this string verifyResult)
        {
            if (!string.IsNullOrEmpty(verifyResult))
            {
                throw new QException(verifyResult);
            }
        }

        public static string Required(this string value, string title)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return $"{title}不能为空。";
            }

            return string.Empty;
        }

        public static string VerifyByArgs(this string value, string title, string[] args)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return $"{title}不能为空。";
            }

            var matchList = Regex.Matches(value, @"\[([^\]]+)\]");

            if (matchList.Count > 0)
            {
                var invalidArgs = new List<string>();
                foreach (Match match in matchList)
                {
                    var arg = match.Groups[1].Value;
                    if (!args.Contains(arg))
                    {
                        if (!invalidArgs.Contains(arg))
                            invalidArgs.Add(arg);
                    }
                }

                if (invalidArgs.Count > 0)
                {
                    return $"{title}不支持 {string.Join("、", invalidArgs.Select(a => $"[{a}]"))} 等参数";
                }
            }
            return string.Empty;
        }
    }
}
