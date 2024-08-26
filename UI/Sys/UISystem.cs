using Terraria.UI;

namespace ForOneToolkit.UI.Sys;

public class UISystem : ModSystem
{
    public static UIManager Manager { get; private set; }
    internal static Vector2 Resolution { get; private set; } = Vector2.Zero;
    internal static RectangleF Screen { get; private set; }

    public override void Load()
    {
        AssetManager.Load();
        Manager = new();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        base.UpdateUI(gameTime);
        Manager.Update();
        if (!Resolution.X.Equals(Main.screenWidth) || !Resolution.Y.Equals(Main.screenHeight))
        {
            Resolution = new(Main.screenWidth, Main.screenHeight);
            Screen = new(0, 0, Resolution);
            Manager.Calculate();
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        layers.Insert(layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text")), new LegacyGameInterfaceLayer(
            Mod.Name + ":RUISystem",
            () =>
            {
                SpriteBatch sb = Main.spriteBatch;
                Manager.Draw(sb);
                return true;
            },
            InterfaceScaleType.UI)
        );
    }
}