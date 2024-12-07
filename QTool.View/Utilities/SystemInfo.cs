using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QTool.View.Utilities
{

    public class SystemInfo
    {
        public SystemInfo()
        {
            OS = GetOperatingSystem();
            Identity = GetIdentity(OS);
            Version = Application.ResourceAssembly.GetName().Version.ToString();
        }

        public string Identity { get; }

        public string OS { get; }

        public string Version { get; }

        /// <summary>
        /// 获得CPU编号
        /// </summary>
        /// <returns></returns>
        public string GetCpuIDs()
        {
            return JsonStr(GetProperties("select ProcessorId,Name from Win32_Processor"));
        }

        /// <summary>
        /// 主板编号
        /// </summary>
        /// <returns></returns>
        public string GetBoardIDs()
        {
            return JsonStr(GetProperties("select SerialNumber,Product from Win32_BaseBoard"));
        }

        public string GetOperatingSystem()
        {
            var datas = GetProperties("select Caption,OSArchitecture,CSName,RegisteredUser from Win32_OperatingSystem");
            return string.Join(" ", datas["Caption"], datas["OSArchitecture"], datas["CSName"], datas["RegisteredUser"]);
        }

        private NameValueCollection GetProperties(string queryString)
        {
            var mos = new ManagementObjectSearcher(queryString);
            var moc = mos.Get();
            NameValueCollection datas = new NameValueCollection();
            foreach (ManagementObject mo in moc)
            {
                datas.Add(GetProperties(mo));
            }
            return datas;
        }

        private NameValueCollection GetProperties(ManagementObject mo)
        {
            NameValueCollection datas = new NameValueCollection();
            foreach (var item in mo.Properties)
            {
                datas.Add(item.Name, item.Value?.ToString());
            }
            return datas;
        }

        public string JsonStr(NameValueCollection datas)
        {
            var sortDatas = new SortedSet<string>();
            foreach (var key in datas.AllKeys)
            {
                var value = datas[key];
                sortDatas.Add(string.Join("=", key, value ?? ""));
            }
            return string.Join("&", sortDatas);
        }

        private string GetIdentity(string os)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QTool");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            string file = Path.Combine(path, "identity.dat");

            SystemIdentity systemIdentity = null;
            string identity = GlobalHelper.GetMD5(string.Join(Environment.NewLine, GetCpuIDs(), GetBoardIDs(), os));
            try
            {
                if (File.Exists(file))
                {
                    systemIdentity = JsonConvert.DeserializeObject<SystemIdentity>(new RSAHelper().Decrypt(File.ReadAllText(file, Encoding.UTF8)));
                }
            }
            catch { }

            if (systemIdentity != null)
            {
                if (string.Compare(identity, systemIdentity.Identity, true) != 0)
                {
                    systemIdentity = null;
                }
            }

            if (systemIdentity == null)
            {
                systemIdentity = new SystemIdentity()
                {
                    Guid = Guid.NewGuid(),
                    Identity = identity,
                };
                File.WriteAllText(file, new RSAHelper().Encrypt(JsonConvert.SerializeObject(systemIdentity)));
            }

            return GlobalHelper.GetMD5(JsonConvert.SerializeObject(systemIdentity));
        }

        class SystemIdentity
        {
            public Guid Guid { get; set; }

            public string Identity { get; set; }
        }
    }
}
