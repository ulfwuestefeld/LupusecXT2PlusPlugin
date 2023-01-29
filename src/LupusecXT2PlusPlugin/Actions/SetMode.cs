namespace Loupedeck.LupusecXT2PlusPlugin
{
    using System;

    public class SetMode : PluginDynamicCommand
    {
        private Boolean _arm = false;

        private readonly String _image0ResourcePath;
        private readonly String _image1ResourcePath;
        private readonly String _image2ResourcePath;

        public SetMode() : base(displayName: "Set Mode", description: "Sets Mode to Home or Disarm", groupName: "Mode")
        {
            this._image0ResourcePath = EmbeddedResources.FindFile("SetMode0.png");
            this._image1ResourcePath = EmbeddedResources.FindFile("SetMode1.png");
            this._image2ResourcePath = EmbeddedResources.FindFile("SetMode2.png");
        }
        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            Lupusec lrequest = new Lupusec();
            String mode_a1 = lrequest.GetModeA1();
            if (mode_a1 != "")
            {
                if (mode_a1 == "HOME")
                {
                    this._arm = true;
                    return EmbeddedResources.ReadImage(this._image2ResourcePath);
                }
                if (mode_a1 == "ARM")
                {
                    this._arm = true;
                    return EmbeddedResources.ReadImage(this._image1ResourcePath);
                }
                if (mode_a1 == "DISARM")
                {
                    this._arm = false;
                    return EmbeddedResources.ReadImage(this._image0ResourcePath);
                }
            }
            return EmbeddedResources.ReadImage(this._image0ResourcePath);
        }
        protected override void RunCommand(String actionParameter)
        {
            var mode = "HOME";
            if (this._arm)
            {
                mode = "DISARM";
            }
            Lupusec lrequest = new Lupusec();
            if (lrequest.SetModeA1(mode))
            {
                this.ActionImageChanged();
            }
        }
    }
}
