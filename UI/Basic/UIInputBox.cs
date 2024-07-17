using System;
using Microsoft.Xna.Framework.Input;
using ReitsKit.UI.Interface;
using ReLogic.Graphics;
using ReitsKit.UI.Encapsulation;
using Terraria.GameContent;
using FontData = ReLogic.Graphics.DynamicSpriteFont.SpriteCharacterData;

namespace ReitsKit.UI.Basic;

/// <summary>
/// 文本输入框，注意其本身是一个透明的部件
/// <br/>可以使用已封装好的<see cref="UICornerInput"/>
/// </summary>
public class UIInputBox : UIElement, IDrawString
{
    /// <summary>
    /// 不处于输入状态且框内无已输入内容时显示的提示
    /// </summary>
    public string InputTips { get; init; }

    /// <summary>
    /// 已输入文本
    /// </summary>
    public string Text { get; private set; }

    public DrawStringStyle DrawStringStyle => DrawStringStyle.TopLeft;

    public Color Color { get; set; }

    public RectangleF DrawRect => InsetArea;

    public Vector2 Scale { get; set; }

    /// <summary>
    /// 字符间距
    /// </summary>
    public Vector2 Kerning { get; set; }

    /// <summary>
    /// 输入任意文本时调用，传入已输入的文本
    /// </summary>
    public Action<string> OnInput;

    /// <summary>
    /// 使用清除按钮时调用
    /// </summary>
    public Action OnClear;

    /// <summary>
    /// 文本光标闪烁间隔
    /// </summary>
    public int BlinkTime { get; init; }

    /// <summary>
    /// 是否允许换行
    /// </summary>
    public bool LineBreak;

    public DynamicSpriteFont Font { get; set; }

    /// <summary>
    /// 上下左右键位控制，用于移动文本光标
    /// </summary>
    private readonly KeyCtrl left, right, up, down;

    /// <summary>
    /// 缓存的键位信息，和是否处于输入状态
    /// </summary>
    private bool pl, pr, pu, pd, isEnableIME;

    private const char CursorSym = '|';

    /// <summary>
    /// 文本光标位置索引
    /// </summary>
    private int cursor;

    /// <summary>
    /// 缓存每个文本的定位与大小
    /// </summary>
    private readonly List<(RectangleF local, char t)> text;

    /// <summary>
    /// 文本光标闪烁计时器
    /// </summary>
    private int blink;

    /// <summary>
    /// 当前是否显示文本光标
    /// </summary>
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
                        cursor = Mouse.X > local.Center.X ? i : (i + 1);
                        blink = 0;
                    }
                }
            }
            else
            {
                isEnableIME = true;
                cursor = text.Count;
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
            int cp = cursor;
            string crop = Text[..cp];
            string input = Main.GetInputText(crop, true);
            if (crop != input)
            {
                string remaining = Text[cp..];
                Text = input + remaining;
                cursor = input.Length;
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
                cursor < count ? text[cursor].local.TopLeft : text[^1].local.TopRight;
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

    /// <summary>
    /// 移动光标
    /// </summary>
    private void CheckKeyMoveCursor()
    {
        if (up.State == CtrlState.SingleDown)
        {
            Vector2 nowPos = text[cursor].local.TopLeft;
            int index = -1;
            float dis = 0;
            for (int i = cursor - 1; i > 0; i--)
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
                cursor = index;
                blink = 0;
            }
        }
        if (down.State == CtrlState.SingleDown)
        {
            Vector2 nowPos = text[cursor].local.TopLeft;
            int index = -1;
            float dis = 0;
            int count = text.Count;
            for (int i = cursor + 1; i < count; i++)
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
                cursor = index;
                blink = 0;
            }
        }
        if (left.State is CtrlState.SingleDown or CtrlState.DoubleDown || left.KeepTime >= 30 && left.KeepTime % 7 == 0)
        {
            cursor = Math.Max(0, --cursor);
            blink = 0;
        }
        if (right.State is CtrlState.SingleDown or CtrlState.DoubleDown ||
            right.KeepTime >= 30 && right.KeepTime % 7 == 0)
        {
            cursor = Math.Min(text.Count, ++cursor);
            blink = 0;
        }
    }
}