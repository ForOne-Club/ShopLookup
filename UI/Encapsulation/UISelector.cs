namespace ForOneToolkit.UI.Encapsulation
{
    public class UISelector : UI3FrameImage
    {
        public int MaxSelected { get; set; }
        public bool Selecting { get; private set; }
        private readonly List<UISelector> another;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="another">同属的选择器</param>
        public UISelector(Texture2D iconTex, Texture2D tex, List<UISelector> another) : base(tex, null)
        {
            Activator = _ => Selecting;
            this.another = another;
            UIImage icon = new(iconTex)
            {
                DrawTextureStyle = Interface.DrawTextureStyle.FromCenter,
            };
            icon.SetCenter(0, 0, 0.5f, 0.5f);
            Add(icon);
            Sensitive = true;
            OnLeftSingleDown += _ => CheckSeleted();
        }
        private void CheckSeleted()
        {
            if (!Selecting)
            {
                if (another != null)
                {
                    if (another.Count > 1)
                    {
                        int actived = 0;
                        foreach (UISelector selector in another)
                        {
                            if (selector.Selecting)
                            {
                                if (++actived == MaxSelected)
                                {
                                    return;
                                }
                            }
                        }
                    }
                    else if (another.Count == 1)
                    {
                        foreach (UISelector selector in another)
                        {
                            if (selector != this)
                            {
                                selector.Selecting = false;
                            }
                        }
                    }
                }
                Selecting = true;
            }
            else
                Selecting = false;
        }
    }
}
