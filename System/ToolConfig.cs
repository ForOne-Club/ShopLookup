using ForOneToolkit.UI.VisualEditor;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ForOneToolkit.System
{
    public class ToolConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        internal static ToolConfig Ins { get; private set; }
        public override void OnLoaded() => Ins = this;

        [DefaultValue(false)]
        public bool UIMasterMode;
        public override void OnChanged()
        {
            if (UISystem.Manager?.TryGetContainer(Mod, Editor.UILabel, out UIContainer editor) == true)
            {
                if (editor.Active != UIMasterMode)
                {
                    editor.ChangeActive();
                }
            }
        }
    }
}
