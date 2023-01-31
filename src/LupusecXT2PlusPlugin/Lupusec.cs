namespace Loupedeck.LupusecXT2PlusPlugin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;

    using Newtonsoft.Json.Linq;

    public class Lupusec
    {
        private static Int64 gotTokenTicks = 0;
        private static String currentToken = "";

        private const String SmarthomeStatusOn = "{WEB_MSG_PSS_ON}";
        private const String SmarthomeStatusOff = "{WEB_MSG_PSS_OFF}";

        private const String ModeStatusArm = "{AREA_MODE_1}";
        private const String ModeStatusHome = "{AREA_MODE_2}";
        private const String ModeStatusDisarm = "{AREA_MODE_0}";
        public Boolean SetModeA1(String mode)
        {
            Boolean ret = false;
            switch (mode.ToUpper())
            {
                case "HOME":
                    this.SetData("/action/panelCondPost", "area=1&mode=2");
                    ret = true;
                    break;
                case "ARM":
                    this.SetData("/action/panelCondPost", "area=1&mode=1");
                    ret = true;
                    break;
                case "DISARM":
                    this.SetData("/action/panelCondPost", "area=1&mode=0");
                    ret = true;
                    break;
            }
            return ret;
        }

        public Boolean GroupOn(String groupname = "")
        {
            Boolean ret = false;
            var devices = GetSmarthomeDevices(groupname);
            foreach (var device in devices)
            {
                if (device.Status == "on")
                {
                    ret = true;
                }
            }
            return ret;
        }

        public Boolean SmarthomeDevicesOn(String groupname = "")
        {
            Boolean ret = false;
            var devices = this.GetSmarthomeDevices(groupname);
            while (!this.GroupOn(groupname))
            {
                foreach (var device in devices)
                {
                    if (device.Status != "on")
                    {
                        this.SetData("/action/deviceSwitchPSSPost", "id=" + device.Id + "&switch=1");
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                devices = this.GetSmarthomeDevices(groupname);
            }
            ret = this.GroupOn(groupname);
            return ret;
        }
        public Boolean SmarthomeDevicesOff(String groupname = "")
        {
            Boolean ret = false;
            var devices = this.GetSmarthomeDevices(groupname);
            while (this.GroupOn(groupname))
            {
                foreach (var device in devices)
                {
                    if (device.Status != "off")
                    {
                        this.SetData("/action/deviceSwitchPSSPost", "id=" + device.Id + "&switch=0");
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                devices = this.GetSmarthomeDevices(groupname);
            }
            ret = this.GroupOn(groupname);
            return ret;
        }

        public List<(String Name, String Id, String Type, String Status)> GetSmarthomeDevices(String groupname = "")
        {
            var devices = new List<(String Name, String Id, String Type, String Status)> { };
            String data = this.GetData("/action/deviceListPSSGet");
            List<String> devicelist = new List<String> { };
            if (data != "")
            {
                if (groupname != "")
                {
                    Config cfg = new Config();
                    devicelist = cfg.GetDevices(groupname);
                }

                JToken tokens = JObject.Parse(data).SelectToken("pssrows");
                foreach (JToken token in tokens)
                {
                    var name = token.SelectToken("name").ToString();
                    if (groupname != "")
                    {
                        if (!devicelist.Contains(name))
                        {
                            continue;
                        }
                    }
                    var id = token.SelectToken("id").ToString();
                    var type = token.SelectToken("type").ToString();
                    var status = token.SelectToken("status").ToString();
                    String onoff = "off";
                    if (status == SmarthomeStatusOn)
                    {
                        onoff = "on";
                    }
                    devices.Add((name, id, type, onoff));
                }
            }
            return devices;
        }

        public String GetModeA1()
        {
            String data = this.GetData("/action/panelCondGet");
            if (data != "")
            {
                JObject json = JObject.Parse(data);
                var mode_a1 = json.SelectToken("updates.mode_a1").ToString();
                if (mode_a1 == ModeStatusHome)
                {
                    return "HOME";
                }
                else if (mode_a1 == ModeStatusDisarm)
                {
                    return "DISARM";
                }
            }
            return "";
        }


        private Boolean SetData(String command, String data)
        {
            Config cfg = new Config();
            String uri = cfg.GetURI();
            String user = cfg.GetUsername();
            String password = cfg.GetPassword();
            Boolean ignorecertificationerrors = cfg.GetIgnoreCertificatioErrors();

            var token = this.GetToken();
            if (token != "")
            {
                try
                {
                    Byte[] postdata = Encoding.ASCII.GetBytes(data);
                    HttpWebRequest request = WebRequest.Create(uri + command) as HttpWebRequest;
                    request.Credentials = new NetworkCredential(user, password);
                    request.Headers.Add("X-Token", token);
                    request.PreAuthenticate = true;
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = postdata.Length;
                    if (uri.StartsWith("https://"))
                    {
                        if (ignorecertificationerrors)
                        {
                            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                                delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                    System.Security.Cryptography.X509Certificates.X509Chain chain,
                                    System.Net.Security.SslPolicyErrors sslPolicyErrors)
                                {
                                    return true; // **** Always accept
                                };
                        }
                    }
                    Stream requeststream = request.GetRequestStream();
                    requeststream.Write(postdata, 0, postdata.Length);
                    requeststream.Close();

                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        data = reader.ReadToEnd();
                        reader.Close();
                        stream.Close();
                    }
                    else
                    {
                        response.Close();
                        return false;
                    }
                    response.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                    throw ex;
                }
            }
            else
            {
                return false;
            }
        }

        private String GetToken()
        {
            DateTime currentDate = DateTime.Now;
            Int64 elapsedTicks = currentDate.Ticks - gotTokenTicks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            if (elapsedSpan.TotalMinutes > 5)
            {
                String tokendata = this.GetData("/action/tokenGet");
                if (tokendata != "")
                {
                    JObject json = JObject.Parse(tokendata);
                    currentToken = json.SelectToken("message").ToString();
                }
                gotTokenTicks = currentDate.Ticks;
            }
            return currentToken;
        }

        private String GetData(String command)
        {
            Config cfg = new Config();
            String uri = cfg.GetURI();
            String user = cfg.GetUsername();
            String password = cfg.GetPassword();
            Boolean ignorecertificationerrors = cfg.GetIgnoreCertificatioErrors();

            String data = "";
            try
            {
                HttpWebRequest request = WebRequest.Create(uri + command) as HttpWebRequest;
                request.Credentials = new NetworkCredential(user, password);
                request.PreAuthenticate = true;
                if (uri.StartsWith("https://"))
                {
                    if (ignorecertificationerrors)
                    {
                        System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                            delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                System.Security.Cryptography.X509Certificates.X509Chain chain,
                                System.Net.Security.SslPolicyErrors sslPolicyErrors)
                            {
                                return true; // **** Always accept
                            };
                    }
                }
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    data = reader.ReadToEnd();
                    reader.Close();
                    stream.Close();
                }
                response.Close();
                return data;
            }
            catch
            {
                return data;
            }
        }

    }
}
