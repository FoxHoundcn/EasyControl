using System;
using System.IO;
using System.Net;
using System.Text;

namespace EasyControl
{
    public static class NetMQServer
    {
        const string ErrorMessage = "MessageError";
        #region html
        static string GetHtml(string url)
        {
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据
            //string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句            
            string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句
            return pageHtml;
        }
        #endregion
        private static string GetServerMessage(string message)
        {
            if (PublicData.LastVersion == ServerState.Offline && !message.Equals(MessageType.GetVersionNC.ToString()))
                return ErrorMessage;
            try
            {
                string url = PublicData.URL + @"/EasyJoy32/MainPage/AccountVerificationNC.php?t=" + message;
                string html = GetHtml(url);
                //-------------------------------------------------------------
                return html;
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog("ServerMessage : " + ex.ToString(), LogType.Error);
                return ErrorMessage;
            }
        }
        public static bool GetProtocolVersion(string protocol, out string version)
        {
            string m1 = GetServerMessage(MessageType.CheckEjoyVersionNC.ToString() + "," + protocol);
            if (!m1.Equals(ErrorMessage))
            {
                version = m1;
                return true;
            }
            version = ErrorMessage;
            return false;
        }
        public static bool GetControlVersion(out string version)
        {
            string m1 = GetServerMessage(MessageType.GetVersionNC.ToString());
            if (!m1.Equals(ErrorMessage))
            {
                string[] ver = m1.Split('_');
                if (ver.Length == 3)
                {
                    byte ver1, ver2, ver3;
                    if (byte.TryParse(ver[0], out ver1) &&
                       byte.TryParse(ver[1], out ver2) &&
                       byte.TryParse(ver[2], out ver3))
                    {
                        if (ver1 == JoyConst.version1 &&
                            ver2 == JoyConst.version2 &&
                            ver3 == JoyConst.version3)
                        {
                            version = "";
                            return true;
                        }
                        else
                        {
                            version = "New Version : " + ver1 + "." + ver2 + "." + ver3;
                            return true;
                        }
                    }
                }
            }
            version = ErrorMessage;
            return false;
        }
        public static bool License(string ver, string mcuID, out string key)
        {
            string m1 = GetServerMessage(MessageType.LicenseNC.ToString() + "," + ver + "," + mcuID);
            switch (m1)
            {
                case ErrorMessage:
                case "AccountError":
                case "KeyIDError":
                case "LicenseFail":
                case "LicenseNewKey":
                    key = m1;
                    return false;
                default:
                    key = m1;
                    return true;
            }
        }
        public static bool CheckPlugin(string ver, out string pluginList)
        {
            string m1 = GetServerMessage(MessageType.CheckPluginNC.ToString() + "," + ver);
            if (!m1.Equals(ErrorMessage))
            {
                pluginList = m1;
                return true;
            }
            pluginList = "";
            return false;
        }
    }
}
