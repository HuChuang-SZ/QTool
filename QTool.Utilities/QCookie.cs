using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public class QCookie
    {

        public string Domain { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        private DateTime? _expires;

        public DateTime? Expires
        {
            get
            {
                return _expires;
            }
            set
            {
                if (value == DateTime.MinValue)
                    _expires = null;
                else
                    _expires = value;
            }
        }


        public bool HttpOnly { get; set; }

        public bool Secure { get; set; }


        public bool Expired()
        {
            if (Expires.HasValue)
            {
                return Expires.Value.ToLocalTime() <= DateTime.Now;
            }

            return false;
        }
    }
}
