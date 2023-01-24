﻿namespace Loupedeck.LupusecXT2PlusPlugin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    using Newtonsoft.Json.Linq;

    public class Config
    {
        public Dictionary<String, String> Read(String configfile)
        {
            var configdata = "";
            try
            {
                configdata = System.IO.File.ReadAllText(configfile);
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
            if (configdata != "")
            {
                Dictionary<String, String> cfg = new Dictionary<String, String>();
                JObject configjson = JObject.Parse(configdata);
                cfg.Add("uri", configjson.SelectToken("uri").ToString());
                //uri = configjson.SelectToken("uri").ToString();
                cfg.Add("ignorecertificationerrors", configjson.SelectToken("ignorecertificationerrors").ToString());
                //if (configjson.SelectToken("ignorecertificationerrors").ToString() == "1")
                //{
                //    ignorecertificationerrors = true;
                //}
                cfg.Add("user", configjson.SelectToken("username").ToString());
                //user = configjson.SelectToken("username").ToString();
                String password = configjson.SelectToken("password").ToString();
                if (password.StartsWith("<enc>"))
                {
                    password = password.Substring(5);
                    password = Decode(password);
                    cfg.Add("password", configjson.SelectToken("password").ToString());
                }
                else
                {
                    cfg.Add("password", configjson.SelectToken("password").ToString());
                    this.Write(configfile, cfg);
                }
                return cfg;
            }
            return null;
        }

        private void Write(String configfile, Dictionary<String, String> config)
        {
            try
            {
                config.TryGetValue("password", out String password);
                config.TryGetValue("uri", out String uri);
                config.TryGetValue("user", out String user);
                config.TryGetValue("ignorecertificationerrors", out String ignorecertificationerrors);
                String enc_password = "<enc>" + Encode(password);
                JTokenWriter configwriter = new JTokenWriter();
                configwriter.WriteStartObject();
                configwriter.WritePropertyName("uri");
                configwriter.WriteValue(uri);
                configwriter.WritePropertyName("ignorecertificationerrors");
                configwriter.WriteValue(ignorecertificationerrors);
                configwriter.WritePropertyName("username");
                configwriter.WriteValue(user);
                configwriter.WritePropertyName("password");
                configwriter.WriteValue(enc_password);
                configwriter.WriteEndObject();
                JObject configjsonw = (JObject)configwriter.Token;
                System.IO.File.WriteAllText(configfile, configjsonw.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static String Encode(String Text)
        {
            try
            {
                String Return = null;
                String _key = "xyJcZ47o";
                String privatekey = "dcJF4vES";
                Byte[] privatekeyByte = { };
                privatekeyByte = Encoding.UTF8.GetBytes(privatekey);
                Byte[] _keyByte = { };
                _keyByte = Encoding.UTF8.GetBytes(_key);
                Byte[] inputtextByteArray = System.Text.Encoding.UTF8.GetBytes(Text);
                using (DESCryptoServiceProvider dsp = new DESCryptoServiceProvider())
                {
                    var memstr = new MemoryStream();
                    var crystr = new CryptoStream(memstr, dsp.CreateEncryptor(_keyByte, privatekeyByte), CryptoStreamMode.Write);
                    crystr.Write(inputtextByteArray, 0, inputtextByteArray.Length);
                    crystr.FlushFinalBlock();
                    return Convert.ToBase64String(memstr.ToArray());
                }
                return Return;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private static String Decode(String Text)
        {
            try
            {
                String x = null;
                String _key = "xyJcZ47o";
                String privatekey = "dcJF4vES";
                Byte[] privatekeyByte = { };
                privatekeyByte = Encoding.UTF8.GetBytes(privatekey);
                Byte[] _keyByte = { };
                _keyByte = Encoding.UTF8.GetBytes(_key);
                Byte[] inputtextByteArray = new Byte[Text.Replace(" ", "+").Length];
                //This technique reverses base64 encoding when it is received over the Internet.
                inputtextByteArray = Convert.FromBase64String(Text.Replace(" ", "+"));
                using (DESCryptoServiceProvider dEsp = new DESCryptoServiceProvider())
                {
                    var memstr = new MemoryStream();
                    var crystr = new CryptoStream(memstr, dEsp.CreateDecryptor(_keyByte, privatekeyByte), CryptoStreamMode.Write);
                    crystr.Write(inputtextByteArray, 0, inputtextByteArray.Length);
                    crystr.FlushFinalBlock();
                    return Encoding.UTF8.GetString(memstr.ToArray());
                }
                return x;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
