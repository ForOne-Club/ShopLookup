using ReitsKit.UI.Interface;

namespace ReitsKit.UI.Basic;

/// <summary>
/// 模拟原版UI框架的面板部件
/// </summary>
public class UICornerPanel : UIPanel, IHoverToColor
{
    private readonly int cornerSize;
    private readonly int barSize;

    /// <summary>
    /// 描边的颜色，null为不绘制描边
    /// </summary>
    public Color? BorderColor;

    /// <summary>
    /// 描边的纹理
    /// </summary>
    public Texture2D BorderTex { get; init; }

    public bool UseHoverChange { get; init; }

    public UICornerPanel(int cornerSize = 12, int barSize = 4)
    {
        this.cornerSize = cornerSize;
        this.barSize = barSize;
        Tex = AssetManager.VnlBg;
        DrawColor = VnlColor * 0.7f;
        BorderColor = Color.Black;
        BorderTex = AssetManager.VnlBd;
    }

    public void HoverToColor(Color? change = null, Color? source = null)
    {
        if (!UseHoverChange) return;
        OnMouseEnter += _ => BorderColor = change ?? Color.Gold;
        OnMouseLeave += _ => BorderColor = source ?? Color.Black;
    }

    protected override void DrawSelf(SpriteBatch sb)
    {
        sb.DrawCorner(Tex, FullArea, DrawColor ?? Color.White, cornerSize, barSize);
        if (BorderColor == null) return;
        sb.DrawCorner(BorderTex, FullArea, BorderColor.Value, cornerSize, barSize);
    }
}