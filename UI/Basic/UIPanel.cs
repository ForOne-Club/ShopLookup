using ForOneToolkit.UI.Interface;
using Terraria.GameContent;

namespace ForOneToolkit.UI.Basic;

/// <summary>
/// 最基础的部件，作为底层面板用
/// <br/>使用3x3式绘制，将纹理等分为9小块，四角不变
/// <br/>横纵拉伸，中心平铺，以此自适应部件大小
/// </summary>
public class UIPanel : UIBottom
{
    public Texture2D Tex;
    public Color? DrawColor;
    public float Opacity;

    protected override void DrawSelf(SpriteBatch sb)
    {
        Rectangle rec = FullArea;
        int dis = Tex.Width / 3;
        Rectangle[] coords = Rect3X3(dis, dis);
        Vector2 size = new(Tex.Width / 6f);
        Vector2 halfX = new(dis / 2f, 0), halfY = new(0, dis / 2f);
        sb.Draw(TextureAssets.MagicPixel.Value, rec, null, (DrawColor ?? Color.White) * Opacity);
        sb.Draw(Tex, new RectangleF(rec.TopLeft() - halfY, rec.Width, dis), coords[1], Color.White);
        sb.Draw(Tex, new RectangleF(rec.TopLeft() - halfX, dis, rec.Height), coords[3], Color.White);
        sb.Draw(Tex, new RectangleF(rec.TopRight() - halfX, dis, rec.Height), coords[5], Color.White);
        sb.Draw(Tex, new RectangleF(rec.BottomLeft() - halfY, rec.Width, dis), coords[7], Color.White);
        sb.Draw(Tex, rec.TopLeft(), coords[0], size);
        sb.Draw(Tex, rec.TopRight(), coords[2], size);
        sb.Draw(Tex, rec.BottomLeft(), coords[6], size);
        sb.Draw(Tex, rec.BottomRight(), coords[8], size);
    }
}