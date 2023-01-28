namespace Loupedeck.LupusecXT2PlusPlugin.Lupusec
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    using Newtonsoft.Json.Linq;

    public class Request
    {
        private static long gotTokenTicks = 0;
        private static String currentToken = "";
        public Boolean Set(String command, String data)
        {
            return SetData(command, data);
        }

        public String Get(String command)
        {
            return GetData(command);
        }

        private Boolean SetData(String command, String data)
        {
            var token = GetToken();
            if (token != "")
            {
                try
                {
                    Byte[] postdata = Encoding.ASCII.GetBytes(data);
                    HttpWebRequest request = WebRequest.Create(LupusecXT2PlusPlugin.uri + command) as HttpWebRequest;
                    request.Credentials = new NetworkCredential(LupusecXT2PlusPlugin.user, LupusecXT2PlusPlugin.password);
                    request.Headers.Add("X-Token", token);
                    request.PreAuthenticate = true;
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = postdata.Length;
                    if (LupusecXT2PlusPlugin.uri.StartsWith("https://"))
                    {
                        if (LupusecXT2PlusPlugin.ignorecertificationerrors)
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

        private string GetToken()
        {
            DateTime currentDate = DateTime.Now;
            long elapsedTicks = currentDate.Ticks - gotTokenTicks;
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
            String data = "";
            try
            {
                HttpWebRequest request = WebRequest.Create(LupusecXT2PlusPlugin.uri + command) as HttpWebRequest;
                request.Credentials = new NetworkCredential(LupusecXT2PlusPlugin.user, LupusecXT2PlusPlugin.password);
                request.PreAuthenticate = true;
                if (LupusecXT2PlusPlugin.uri.StartsWith("https://"))
                {
                    if (LupusecXT2PlusPlugin.ignorecertificationerrors)
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
            catch (Exception ex)
            {
                return data;
            }
        }

    }
}
