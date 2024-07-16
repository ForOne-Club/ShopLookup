using System;
using System.Linq;
using System.Net.Mime;
using Microsoft.Xna.Framework.Input;
using ReitsKit.Helper;
using ReitsKit.UI.Interface;
using ReLogic.Graphics;
using ReLogic.Localization.IME;
using ReLogic.OS;
using Steamworks;
using Terraria.GameContent;
using FontData = ReLogic.Graphics.DynamicSpriteFont.SpriteCharacterData;

namespace ReitsKit.UI.Basic;

public class UIInputBox : UIElement, IDrawString
{
    public string InputTips { get; init; }

    public string Text { get; private set; }

    public DrawStringStyle DrawStringStyle => DrawStringStyle.TopLeft;

    public Color Color { get; set; }

    public RectangleF DrawRect => InsetArea;

    public Vector2 Scale { get; set; }

    /// <summary>
    /// 字符间距
    /// </summary>
    public Vector2 Kerning { get; set; }

    public Action<string> OnInput;
    public Action OnClear;
    public int BlinkTime { get; init; }
    public bool LineBreak;

    public DynamicSpriteFont Font { get; set; }
    private KeyCtrl left, right, up, down;
    private bool pl, pr, pu, pd, isEnableIME;
    private const char CursorSym = '|';
    private int Cursor;
    private Point CursorPos;
    private List<(RectangleF local, char t)> text;
    private int blink;

    private bool Blink
    {
        get
        {
            blink = ++blink % BlinkTime;
            return blink < BlinkTime / 2;
        }
    }

    public UIInputBox()
    {
        left = new(() => pl);
        right = new(() => pr);
        up = new(() => pu);
        down = new(() => pd);
        text = [];
        BlinkTime = 60;
        LineBreak = true;
    }

    public UIInputBox(string inputTips = "", Color? color = null) : this()
    {
        InputTips = inputTips;
        Scale = Vector2.One;
        Color = color ?? Color.Black;
        Font = FontAssets.MouseText.Value;
        Text = string.Empty;
    }

    public override void OnInit()
    {
        OnLeftJustPress += _ =>
        {
            if (isEnableIME)
            {
                int count = text.Count;
                for (int i = 0; i < count; i++)
                {
                    RectangleF local = text[i].local;
                    if (local.Contains(Mouse))
                    {
                        Cursor = Mouse.X > local.Center.X ? i : (i + 1);
                        blink = 0;
                    }
                }
            }
            else
            {
                isEnableIME = true;
                Cursor = text.Count;
                blink = 0;
            }
        };
        OnMouseOverInteract += _ => isEnableIME = false;
    }

    public override void Update()
    {
        KeyboardState state = Keyboard.GetState();
        pl = state.IsKeyDown(Keys.Left);
        pr = state.IsKeyDown(Keys.Right);
        pu = state.IsKeyDown(Keys.Up);
        pd = state.IsKeyDown(Keys.Down);
        left.Update();
        right.Update();
        up.Update();
        down.Update();
        CheckKeyMoveCursor();
    }

    protected override void DrawSelf(SpriteBatch sb)
    {
        Vector2 zero = Vector2.Zero;
        foreach (var (local, t)in text)
        {
            Vector2 pos = local.TopLeft;
            FontData d = Font.GetCharacterData(t);
            sb.Draw(d.Texture, pos + InsetArea.TopLeft + Font.GetCharacterData(t).Kerning.Z * Vector2.UnitY, d.Glyph,
                Color, 0, zero,
                Scale, 0, 0);
        }

        int count = text.Count;
        if (isEnableIME)
        {
            Terraria.GameInput.PlayerInput.WritingText = true;
            Main.instance.HandleIME();
            int cp = Cursor;
            string crop = Text[..cp];
            string input = Main.GetInputText(crop, true);
            if (crop != input)
            {
                string remaining = Text[cp..];
                Text = input + remaining;
                Cursor = input.Length;
                text.Clear();
                RectangleF rect = InsetArea;
                Vector2 pos = Vector2.Zero;
                foreach (var t in Text)
                {
                    FontData d = Font.GetCharacterData(t);
                    float w = (d.Glyph.Width + d.Kerning.X) * Scale.X + Kerning.X;
                    if (LineBreak && pos.X + w > rect.Right)
                    {
                        pos.X = rect.Left;
                        pos.Y += Font.LineSpacing * Scale.Y;
                    }
                    text.Add((new(pos, w, Font.LineSpacing), t));
                    pos.X += w;
                }
                count = text.Count;
            }
            Vector2 local = count == 0 ? Vector2.Zero :
                Cursor < count ? text[Cursor].local.TopLeft : text[^1].local.TopRight;
            var gd = Main.graphics.GraphicsDevice;
            Rectangle old = gd.ScissorRectangle;
            gd.ScissorRectangle = UISystem.Screen;
            UISpbState(sb);
            Main.instance.DrawWindowsIMEPanel(local + Vector2.UnitY * Font.LineSpacing);
            UISpbState(sb);
            gd.ScissorRectangle = old;
            if (Blink)
            {
                FontData d = Font.GetCharacterData(CursorSym);
                sb.Draw(d.Texture, local + InsetArea.TopLeft, d.Glyph, Color, 0, zero, Scale, 0, 0);
            }
        }
        else if (count == 0)
        {
            sb.DrawString(Font, InputTips, InsetArea.TopLeft, Color * 0.5f);
        }
    }

    /// <summary>
    /// 清除文本
    /// </summary>
    /// <param name="doInput">是否（以空文本）调用一次输入事件</param>
    public void ClearText(bool doInput = true)
    {
        Text = "";
        text.Clear();
        if (doInput) OnInput?.Invoke(string.Empty);
        OnClear?.Invoke();
        isEnableIME = false;
        blink = 0;
    }

    private void CheckKeyMoveCursor()
    {
        if (up.State == CtrlState.SingleDown)
        {
            Vector2 nowPos = text[Cursor].local.TopLeft;
            int index = -1;
            float dis = 0;
            for (int i = Cursor - 1; i > 0; i--)
            {
                RectangleF local = text[i].local;
                if (local.Top.Equals(nowPos.Y)) continue;
                float d = local.TopLeft.Distance(nowPos);
                if (dis == 0 || d < dis)
                {
                    index = i;
                    dis = d;
                }
            }
            if (index > -1)
            {
                Cursor = index;
                blink = 0;
            }
        }
        if (down.State == CtrlState.SingleDown)
        {
            Vector2 nowPos = text[Cursor].local.TopLeft;
            int index = -1;
            float dis = 0;
            int count = text.Count;
            for (int i = Cursor + 1; i < count; i++)
            {
                RectangleF local = text[i].local;
                if (local.Top.Equals(nowPos.Y)) continue;
                float d = local.TopLeft.Distance(nowPos);
                if (dis == 0 || d < dis)
                {
                    index = i;
                    dis = d;
                }
            }
            if (index > -1)
            {
                Cursor = index;
                blink = 0;
            }
        }
        if (left.State is CtrlState.SingleDown or CtrlState.DoubleDown || left.KeepTime >= 30 && left.KeepTime % 7 == 0)
        {
            Cursor = Math.Max(0, --Cursor);
            blink = 0;
        }
        if (right.State is CtrlState.SingleDown or CtrlState.DoubleDown || right.KeepTime >= 30 && right.KeepTime % 7 == 0)
        {
            Cursor = Math.Min(text.Count, ++Cursor);
            blink = 0;
        }
    }
}