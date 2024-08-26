using ReLogic.Graphics;

namespace ForOneToolkit.UI.Interface;

public interface IDrawString
{
    public string Text { get; }
    public DrawStringStyle DrawStringStyle { get; }
    public Color Color { get; }
    public RectangleF DrawRect { get; }
    public Vector2 Scale { get; }
    public DynamicSpriteFont Font { get; }
}

public enum DrawStringStyle
{
    TopLeft,
    TopCenter,
    LeftCenter,
    Center,
    RightCenter,
    BottomLeft,
    BottomCenter,
    BottomRight
}