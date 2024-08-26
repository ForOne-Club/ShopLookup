using ForOneToolkit.UI.Interface;

namespace ForOneToolkit.UI.Basic;

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
    public RectangleF DrawRect { get; set; }
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public Vector2 Scale { get; set; } = Vector2.One;

    public UIImage(Texture2D tex, bool autoSize = true, DrawTextureStyle style = DrawTextureStyle.Default)
    {
        Tex = tex;
        if (autoSize)
        {
            SetSize(tex.Size());
        }
        ChangeDrawStyle(style);
    }
    public void ChangeDrawStyle(DrawTextureStyle style)
    {
        switch (style)
        {
            case DrawTextureStyle.Full:
                SourceRect = null;
                DrawRect = FullArea;
                return;
            case DrawTextureStyle.HorizonFull:
                Scale = new(DrawRect.Width / Tex.Width, 1);
                break;
            case DrawTextureStyle.VerticalFull:
                Scale = new(1, DrawRect.Height / Tex.Height);
                break;
            case DrawTextureStyle.FromCenter:
                DrawRect = new(FullArea.Center, 0, 0);
                Origin = Tex.Size() / 2f;
                break;
        }
    }
    protected override void DrawSelf(SpriteBatch sb) => sb.Draw(this);
    protected override void CalculateSelf()
    {
        base.CalculateSelf();
        ChangeDrawStyle(DrawTextureStyle);
    }

    /// <summary>
    /// 改变纹理
    /// </summary>
    /// <param name="tex"></param>
    /// <param name="reSize">是否将部件大小设为纹理大小（会立刻重新计算该部件）</param>
    public void ChangeTex(Texture2D tex, bool reSize = true)
    {
        Tex = tex;
        if (reSize)
        {
            SetSize(tex.Size());
            Calculate();
        }
        ChangeDrawStyle(DrawTextureStyle);
    }
}