namespace Loupedeck.LupusecXT2PlusPlugin
{
    using System;

    using Newtonsoft.Json.Linq;

    public class SetMode : PluginDynamicCommand
    {
        private Boolean _arm = false;

        private readonly String _image0ResourcePath;
        private readonly String _image1ResourcePath;

        public SetMode() : base(displayName: "Set Mode", description: "Sets Mode to Home or Disarm", groupName: "Mode")
        {
            this._image0ResourcePath = EmbeddedResources.FindFile("SetMode0.png");
            this._image1ResourcePath = EmbeddedResources.FindFile("SetMode1.png");
        }
        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            Lupusec.Request lrequest = new Lupusec.Request();

            String data = lrequest.Get("/action/panelCondGet");
            if (data != "")
            {
                JObject json = JObject.Parse(data);
                var mode_a1 = (json.SelectToken("updates.mode_a1")).ToString();

                if (mode_a1 == "{AREA_MODE_2}")
                {
                    this._arm = true;
                    return EmbeddedResources.ReadImage(this._image1ResourcePath);
                }
                if (mode_a1 == "{AREA_MODE_0}")
                {
                    this._arm = false;
                    return EmbeddedResources.ReadImage(this._image0ResourcePath);
                }
            }
            return EmbeddedResources.ReadImage(this._image0ResourcePath);
        }
        protected override void RunCommand(String actionParameter)
        {
            var mode = "2";
            if (this._arm)
            {
                mode = "0";
            }
            Lupusec.Request lrequest = new Lupusec.Request();
            if (lrequest.Set("/action/panelCondPost", ("area=1&mode=" + mode)))
            {
                this.ActionImageChanged();
            }
        }
    }
}
