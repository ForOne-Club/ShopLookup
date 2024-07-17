namespace ReitsKit.UI.Interface;

/// <summary>
/// 实现某种缓存的绘制（意义不明
/// </summary>
public interface IDrawTexture
{
    public Texture2D Tex { get; }
    public DrawTextureStyle DrawTextureStyle { get; }
    public Color Color { get; }
    public float Opacity { get; }
    public float Rot { get; }
    public Rectangle? SourceRect { get; }
    public RectangleF DrawRect { get; }
    public Vector2 Origin { get; }
    public Vector2 Scale { get; }
}

public enum DrawTextureStyle
{
    Default,
    Full,
    HorizonFull,
    VerticalFull,
    FromCenter
}