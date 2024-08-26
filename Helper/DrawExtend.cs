using ForOneToolkit.UI.Interface;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ForOneToolkit.Helper;

public static class DrawExtend
{
    public static readonly Color VnlColor = new(63, 82, 151) /* 0.7f*/;

    /// <summary>
    /// 包含 End 和 Begin
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="useMatrix"></param>
    /// <param name="useShader"></param>
    public static void UISpbState(SpriteBatch sb, bool useMatrix = true, bool useShader = false)
    {
        sb.End();
        sb.Begin(useShader ? SpriteSortMode.Immediate : SpriteSortMode.Deferred, BlendState.AlphaBlend,
            SamplerState.AnisotropicClamp,
            DepthStencilState.None, CullNoneAndScissor, null, useMatrix ? Main.UIScaleMatrix : Matrix.Identity);
    }

    public static void Draw(this SpriteBatch sb, Texture2D tex, Vector2 pos, Rectangle? scissors = null,
        Vector2? origin = null, Vector2? scale = null, float rot = 0, Color? color = null) =>
        sb.Draw(tex, pos, scissors, color ?? Color.White, rot, origin ?? Vector2.Zero, scale ?? Vector2.One, 0, 0);

    public static void DrawLine(this SpriteBatch sb, Vector2 start, Vector2 end,
        Color? color = null, float width = 2f, bool toScreen = false)
    {
        Vector2 toTar = end - start;
        Vector2 center = (start + end) / 2;
        if (toScreen)
        {
            toTar -= Main.screenPosition;
            center -= Main.screenPosition;
        }

        sb.Draw(TextureAssets.MagicPixel.Value, center, new(0, 0, 1, 1), color ?? Color.White,
            toTar.ToRotation(), new Vector2(0.5f), new Vector2(toTar.Length(), width), 0, 0);
    }

    public static void DrawRectangle(this SpriteBatch sb, RectangleF rect, Color? color = null, float width = 2f,
        bool toScreen = false)
    {
        if (toScreen)
            rect -= Main.screenPosition;
        Vector2 topLeft = rect.TopLeft,
            topRight = rect.TopRight,
            bottomLeft = rect.BottomLeft,
            bottomRight = rect.BottomRight;
        sb.DrawLine(topLeft, topRight, color, width);
        sb.DrawLine(topRight, bottomRight, color, width);
        sb.DrawLine(topLeft, bottomLeft, color, width);
        sb.DrawLine(bottomLeft, bottomRight, color, width);
    }

    public static void DrawCorner(this SpriteBatch sb, Texture2D texture, Rectangle rec, Color color, int cornerSize,
        int barSize)
    {
        Point point = new(rec.X, rec.Y);
        Point point2 = new(point.X + rec.Width - cornerSize, point.Y + rec.Height - cornerSize);
        int width = point2.X - point.X - cornerSize;
        int height = point2.Y - point.Y - cornerSize;
        sb.Draw(texture, new Rectangle(point.X, point.Y, cornerSize, cornerSize),
            new Rectangle(0, 0, cornerSize, cornerSize), color);
        sb.Draw(texture, new Rectangle(point2.X, point.Y, cornerSize, cornerSize),
            new Rectangle(cornerSize + barSize, 0, cornerSize, cornerSize), color);
        sb.Draw(texture, new Rectangle(point.X, point2.Y, cornerSize, cornerSize),
            new Rectangle(0, cornerSize + barSize, cornerSize, cornerSize), color);
        sb.Draw(texture, new Rectangle(point2.X, point2.Y, cornerSize, cornerSize),
            new Rectangle(cornerSize + barSize, cornerSize + barSize, cornerSize, cornerSize), color);
        sb.Draw(texture, new Rectangle(point.X + cornerSize, point.Y, width, cornerSize),
            new Rectangle(cornerSize, 0, barSize, cornerSize), color);
        sb.Draw(texture, new Rectangle(point.X + cornerSize, point2.Y, width, cornerSize),
            new Rectangle(cornerSize, cornerSize + barSize, barSize, cornerSize), color);
        sb.Draw(texture, new Rectangle(point.X, point.Y + cornerSize, cornerSize, height),
            new Rectangle(0, cornerSize, cornerSize, barSize), color);
        sb.Draw(texture, new Rectangle(point2.X, point.Y + cornerSize, cornerSize, height),
            new Rectangle(cornerSize + barSize, cornerSize, cornerSize, barSize), color);
        sb.Draw(texture, new Rectangle(point.X + cornerSize, point.Y + cornerSize, width, height),
            new Rectangle(cornerSize, cornerSize, barSize, barSize), color);
    }

    public static void Draw(this SpriteBatch sb, IDrawTexture info)
    {
        Texture2D tex = info.Tex;
        if (tex == null)
            return;
        RectangleF drawRect = info.DrawRect;
        Rectangle? source = info.SourceRect;
        Color color = info.Color;
        if (info.DrawTextureStyle == DrawTextureStyle.Full)
        {
            sb.Draw(tex, drawRect, source, color);
            return;
        }
        Vector2 drawPos = drawRect.TopLeft;
        float rot = info.Rot;
        Vector2 origin = info.Origin;
        Vector2 scale = info.Scale;
        sb.Draw(tex, drawPos, source, origin, scale, rot, color * info.Opacity);
    }

    public static void Draw(this SpriteBatch sb, IDrawString info, Vector2 size)
    {
        RectangleF area = info.DrawRect;
        Vector2 drawPos = Vector2.Zero, origin = Vector2.Zero;
        switch ((int)info.DrawStringStyle)
        {
            case 0:
                drawPos = area.TopLeft;
                break;
            case 1:
                drawPos = area.TopCenter;
                origin.X = size.X / 2f;
                break;
            case 2:
                drawPos = area.TopRight;
                origin.X = size.X;
                break;
            case 3:
                drawPos = area.LeftCenter;
                origin.Y = size.Y / 2f;
                break;
            case 4:
                drawPos = area.Center;
                origin = size / 2f;
                break;
            case 5:
                drawPos = area.RightCenter;
                origin.X = size.X;
                origin.Y = size.Y / 2f;
                break;
            case 6:
                drawPos = area.BottomLeft;
                origin.Y = size.Y;
                break;
            case 7:
                drawPos = area.BottomCenter;
                origin.X = size.X / 2f;
                origin.Y = size.Y;
                break;
            case 8:
                drawPos = area.BottomRight;
                origin = size;
                break;
        }

        ChatManager.DrawColorCodedStringWithShadow(sb, info.Font, info.Text, drawPos, info.Color, 0,
            origin, info.Scale, -1, 1.5f);
    }

    public static void DrawRectByAnyStage(this SpriteBatch sb, int stage, Texture2D tex, Rectangle[] scissors,
        RectangleF[] locals, Color? color = null)
    {
        Color c = color ?? Color.White;
        for (int i = 0; i < stage; i++)
        {
            sb.Draw(tex, locals[i], scissors[i], c);
        }
    }
}