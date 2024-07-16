using ReitsKit.UI.Interface;

namespace ReitsKit.UI.Basic;

public class UICornerPanel : UIPanel, IHoverToColor
{
    public readonly int CornerSize;
    public readonly int BarSize;
    public Color? BorderColor;
    public Texture2D BorderTex { get; init; }
    public bool UseHoverChange { get; init; }

    public UICornerPanel(int cornerSize = 12, int barSize = 4)
    {
        CornerSize = cornerSize;
        BarSize = barSize;
        Tex = AssetManager.VnlBg;
        DrawColor = VnlColor * 0.7f;
        BorderColor = Color.Black;
        BorderTex = AssetManager.VnlBd;
    }

    public void HoverToColor(Color? change = null, Color? source = null)
    {
        OnMouseEnter += _ => BorderColor = change ?? Color.Gold;
        OnMouseLeave += _ => BorderColor = source ?? Color.Black;
    }

    protected override void DrawSelf(SpriteBatch sb)
    {
        sb.DrawCorner(Tex, FullArea, DrawColor ?? Color.White, CornerSize, BarSize);
        if (BorderColor == null) return;
        sb.DrawCorner(BorderTex, FullArea, BorderColor.Value, CornerSize, BarSize);
    }
}