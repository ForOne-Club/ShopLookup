using ReitsKit.UI.Interface;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ReitsKit.UI.Basic;

/// <summary>
///
/// </summary>
public class UIText : UIElement, IDrawString
{
    public string Text { get; private set; }

    public DrawStringStyle DrawStringStyle { get; set; }

    public Color Color { get; set; } = Color.White;

    public RectangleF DrawRect => FullArea;

    public Vector2 Scale { get; set; } = Vector2.One;

    public Vector2 TextSize { get; private set; }
    public DynamicSpriteFont Font { get; private set; } = FontAssets.MouseText.Value;
    public int MaxWidth { get; private set; } = -1;

    public UIText(string text = "", int maxWidth = -1, bool autoSize = true)
    {
        if (maxWidth > -1)
        {
            text = Font.CreateWrappedText(text, maxWidth);
        }

        Text = text;
        if (autoSize)
        {
            SetSizeFromText();
        }
    }

    protected override void DrawSelf(SpriteBatch sb) => sb.Draw(this, TextSize);

    /// <summary>
    /// 改变文本
    /// </summary>
    /// <param name="text"></param>
    /// <param name="maxWidth">会根据给定的最大宽度自动换行</param>
    /// <param name="reSize">是否将部件大小设为文本大小（会立即重新计算部件）</param>
    public void ChangeText(string text = "", int maxWidth = -1, bool reSize = true)
    {
        if (maxWidth > -1)
        {
            text = Font.CreateWrappedText(text, maxWidth);
        }

        Text = text;
        if (reSize)
        {
            SetSizeFromText();
            Calculate();
        }
    }

    public void ChangeFont(DynamicSpriteFont font, bool reSize = true)
    {
        Font = font;
        if (MaxWidth > -1)
        {
            Text = Font.CreateWrappedText(Text, MaxWidth);
        }

        if (reSize)
        {
            SetSizeFromText();
            Calculate();
        }
    }

    public void SetSizeFromText()
    {
        TextSize = Font.MeasureString(Text) * Scale;
        SetSize(TextSize);
    }
}