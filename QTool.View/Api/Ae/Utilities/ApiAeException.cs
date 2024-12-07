using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Api.Ae
{
    public class ApiAeException : ApiException
    {
        public ApiErrorCode Code { get; }

        public ApiAeException(string message)
            : this(ApiErrorCode.UnKnown, message)
        {

        }

        public ApiAeException(ApiErrorCode code, string message)
            : base(message)
        {
            Code = code;
        }

        public ApiAeException(ApiErrorCode code)
            : this(code, code.ToDisplayName())
        {

        }

        public ApiAeException(JObject jObject)
            : this(jObject.GetError())
        {
        }
    }


    public enum ApiErrorCode
    {
        /// <summary>
        /// 令牌过期
        /// </summary>
        [Display("令牌过期，请重新登录。")]
        TOKEN_EXOIRED,

        /// <summary>
        /// Session过期
        /// </summary>
        [Display("Session过期，请重新登录。")]
        SESSION_EXPIRED,

        /// <summary>
        /// 非法请求
        /// </summary>
        [Display("非法请求，请重新登录。")]
        ILLEGAL_ACCESS,

        /// <summary>
        /// 服务内部故障
        /// </summary>
        [Display("服务内部故障，请重新登录。")]
        SERVICE_INNER_FAULT,

        /// <summary>
        /// 登录店铺与绑定店铺不一致
        /// </summary>
        [Display("登录店铺与绑定店铺不一致，请重新登录。")]
        NON_CURRENT_STORE,

        /// <summary>
        /// 未知错误代码
        /// </summary>
        [Display("令牌过期，请重新登录。")]
        UNKNOWN_FAIL_CODE,


        [Display("超过最大限制")]
        ExceedMaxLimit,

        /// <summary>
        /// 未知异常
        /// </summary>
        [Display("未知异常，请联系客服。")]
        UnKnown,

        Error,
    }
}
