namespace Loupedeck.LupusecXT2PlusPlugin
{
    using System;

    public class SetSmarthomeGroup : PluginDynamicCommand
    {
        private readonly String _image0ResourcePath = EmbeddedResources.FindFile("Off.png");
        private readonly String _image1ResourcePath = EmbeddedResources.FindFile("On.png");

        public SetSmarthomeGroup() : base(displayName: "Control smarthome groups", description: "Controls smarthome groups", groupName: "Smarthome")
        {
            this.MakeProfileAction($"text;Group");
            this._image0ResourcePath = EmbeddedResources.FindFile("Off.png");
            this._image1ResourcePath = EmbeddedResources.FindFile("On.png");
        }

        /*private LupusecXT2PlusPlugin _plugin;
        public SetSmarthomeGroup() : base() => this.MakeProfileAction($"text;Group");
        protected override Boolean OnLoad()
        {
            this._plugin = base.Plugin as LupusecXT2PlusPlugin;
            this.DisplayName = "Control smarthome groups";
            this.Description = "Controls smarthome groups";
            this.GroupName = "Smarthome";
            return !(this._plugin is null) && base.OnLoad();
        }*/
        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            Lupusec lrequest = new Lupusec();
            if (lrequest.GroupOn(actionParameter))
            {
                return EmbeddedResources.ReadImage(this._image1ResourcePath);
            }
            return EmbeddedResources.ReadImage(this._image0ResourcePath);
        }
        protected override void RunCommand(String actionParameter)
        {
            Lupusec lrequest = new Lupusec();
            if (!lrequest.GroupOn(actionParameter))
            {
                if (lrequest.SmarthomeDevicesOn(actionParameter))
                {
                    this.ActionImageChanged();
                }
            }
            else if (lrequest.GroupOn(actionParameter))
            {
                if (lrequest.SmarthomeDevicesOff(actionParameter))
                {
                    this.ActionImageChanged();
                }
            }
        }
    }
}
