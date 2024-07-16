using ReitsKit.UI.Interface;

namespace ReitsKit.UI.Basic;

public class UIImage : UIElement, IDrawTexture
{
    /// <summary>
    /// 设置时自动更新容器大小为贴图大小
    /// </summary>
    public Texture2D Tex { get; private set; }

    public DrawTextureStyle DrawTextureStyle { get; set; }
    public Color Color { get; set; } = Color.White;
    public float Opacity { get; set; } = 1;
    public float Rot { get; set; }
    public Rectangle? SourceRect { get; set; }
    public RectangleF DrawRect => FullArea;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public Vector2 Scale { get; set; } = Vector2.One;

    public UIImage(Texture2D tex, bool autoSize = true)
    {
        Tex = tex;
        if (autoSize)
        {
            SetSize(tex.Size());
        }
    }

    protected override void DrawSelf(SpriteBatch sb) => sb.Draw(this);

    public void ChangeTex(Texture2D tex, bool reSize = true)
    {
        Tex = tex;
        if (reSize)
        {
            SetSize(tex.Size());
            Calculate();
        }
    }
}