namespace Loupedeck.LupusecXT2PlusPlugin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    using Newtonsoft.Json.Linq;

    // This class contains the plugin-level logic of the Loupedeck plugin.

    public class LupusecXT2PlusPlugin : Plugin
    {
        // Gets a value indicating whether this is an Universal plugin or an Application plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean HasNoApplication => true;

        private static String configfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.lupusecxt2plusplugin.json";

        public static String uri = "";
        public static String user = "";
        public static String password = "";
        public static Boolean ignorecertificationerrors = false;

        // This method is called when the plugin is loaded during the Loupedeck service start-up.
        public override void Load()
        {
            Config cfg = new Config();
            Dictionary<String, String> config = cfg.Read(configfile);
            config.TryGetValue("password", out password);
            config.TryGetValue("uri", out uri);
            config.TryGetValue("user", out user);
            config.TryGetValue("ignorecertificationerrors", out String ignorecert);
            if (ignorecert == "1")
            {
                ignorecertificationerrors = true;
            }
        }

        // This method is called when the plugin is unloaded during the Loupedeck service shutdown.
        public override void Unload()
        {
            uri = "";
            user = "";
            password = "";
        }
    }
}
