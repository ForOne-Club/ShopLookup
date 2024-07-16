using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using System.Linq;
using System.Text;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI.Chat;
using UIElement = ReitsKit.UI.Origin.UIElement;

namespace ReitsKit.UI.Sys;

public class UIManager
{
    public static RenderTarget2D RTOrigin { get; private set; }
    public static RenderTarget2D RTSwap { get; private set; }
    public string HoverText = string.Empty;
    public bool DrawHoverBg;
    public int HoverTextMaxWidth;

    /// <summary>
    /// UI容器更新顺序
    /// </summary>
    private readonly List<UIContainer> callOrder;

    private readonly List<UIElement> needRemoves;

    /// <summary>
    /// 存放所有UI容器
    /// </summary>
    private readonly Dictionary<(Mod, string label), UIContainer> containers;

    private readonly KeyCtrl mouseLeft = new(() => Main.mouseLeft);
    private readonly KeyCtrl mouseMiddle = new(() => Main.mouseMiddle);
    private readonly KeyCtrl mouseRight = new(() => Main.mouseRight);

    public UIManager()
    {
        containers = [];
        callOrder = [];
        needRemoves = [];
        Main.QueueMainThreadAction(() =>
        {
            var gd = Main.graphics.GraphicsDevice;
            int w = Main.screenWidth, h = Main.screenHeight;
            RTOrigin = RTSwap = new(gd, w, h);
        });
        foreach (var mod in ModLoader.Mods)
        {
            foreach (var c in mod.GetType().Assembly.GetTypes()
                         .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(UIContainer))))
            {
                UIContainer ui = (UIContainer)Activator.CreateInstance(c);
                string label = ui?.Label;
                if (label == null)
                {
                    StringBuilder error = new StringBuilder("UI ")
                        .Append('<')
                        .Append(c.Name)
                        .Append('>')
                        .Append(" from ")
                        .Append(mod.Name)
                        .Append(" has no unique label");
                    throw new Exception(error.ToString());
                }

                if (!ui.AutoLoad)
                    continue;
                ui.SetSize(0, 0, 1, 1);
                if (ui.AutoInit)
                    ui.Initialize();
                containers.Add((mod, ui.Label), ui);
                callOrder.Add(ui);
            }
        }
    }

    private IEnumerable<Action<UIElement>> TryDoMouseEvent()
    {
        if (mouseLeft.JustPress) yield return x => x.LeftJustPress();
        if (mouseLeft.JustRelease) yield return x => x.LeftJustRelease();
        if (mouseRight.JustPress) yield return x => x.RightJustPress();
        if (mouseRight.JustRelease) yield return x => x.RightJustRelease();
        if (mouseMiddle.JustPress) yield return x => x.MiddleJustPress();
        if (mouseMiddle.JustRelease) yield return x => x.MiddleJustRelease();
        yield return mouseLeft.State switch
        {
            CtrlState.SingleDown => x => x.LeftSingleDown(),
            CtrlState.SingleClick => x => x.LeftSingleClick(),
            CtrlState.DoubleDown => x => x.LeftDoubleDown(),
            CtrlState.DoubleClick => x => x.LeftDoubleClick(),
            CtrlState.HoldRelease => x => x.LeftHoldRelease(),
            _ => null
        };
        yield return mouseRight.State switch
        {
            CtrlState.SingleDown => x => x.RightSingleDown(),
            CtrlState.SingleClick => x => x.RightSingleClick(),
            CtrlState.DoubleDown => x => x.RightDoubleDown(),
            CtrlState.DoubleClick => x => x.RightDoubleClick(),
            CtrlState.HoldRelease => x => x.RightHoldRelease(),
            _ => null
        };
        yield return mouseMiddle.State switch
        {
            CtrlState.SingleDown => x => x.MiddleSingleDown(),
            CtrlState.SingleClick => x => x.MiddleSingleClick(),
            CtrlState.DoubleDown => x => x.MiddleDoubleDown(),
            CtrlState.DoubleClick => x => x.MiddleDoubleClick(),
            CtrlState.HoldRelease => x => x.MiddleHoldRelease(),
            _ => null
        };
    }

    /// <summary>
    /// 获取UI容器
    /// </summary>
    /// <param name="mod">你的Mod实例</param>
    /// <param name="label">目标UI容器的唯一标识符</param>
    /// <param name="container"></param>
    /// <returns></returns>
    public bool TryGetContainer(Mod mod, string label, out UIContainer container) =>
        containers.TryGetValue((mod, label), out container);


    public void AddToRemove(UIElement uie) => needRemoves.Add(uie);

    public UIContainer TopContainer => callOrder.FirstOrDefault(x => x.Active, null);

    /// <summary>
    /// 置顶目标UI容器
    /// </summary>
    /// <param name="mod"></param>
    /// <param name="label"></param>
    /// <returns></returns>
    public bool SetTop(Mod mod, string label)
    {
        if (!TryGetContainer(mod, label, out UIContainer c))
        {
            StringBuilder report = new StringBuilder("Not found the UI with Key (")
                .Append(mod.Name)
                .Append(", ")
                .Append(label)
                .Append(')');
            Main.NewText(report);
            return false;
        }

        SetTop(c);
        return true;
    }

    public void ReSet()
    {
        foreach (var c in callOrder)
        {
            c.Clear();
            c.Initialize();
            c.Calculate();
        }
    }

    private void SetTop(UIContainer c)
    {
        callOrder.Remove(c);
        callOrder.Insert(0, c);
    }

    /// <summary>
    /// 计算所有容器
    /// </summary>
    internal void Calculate() => callOrder.ForEach(x => x.Calculate());

    internal void Update()
    {
        HoverText = string.Empty;
        HoverTextMaxWidth = -1;
        DrawHoverBg = false;
        if (callOrder.Count == 0)
            return;
        mouseLeft.Update();
        mouseRight.Update();
        mouseMiddle.Update();
        UIContainer changeToTop = null, top = TopContainer;
        Vector2 screen = Main.MouseScreen;
        bool mouseAny = mouseLeft.State > 0 || mouseRight.State > 0 || mouseMiddle.State > 0;
        bool hoverAny = false;
        foreach (var c in callOrder)
        {
            bool occupancy = c != top && top?.Occupancy?.Any(x => x.Contains(screen)) == true;
            List<UIElement> children = c.GetAllChildren(screen, out var hover);
            foreach (UIElement uie in children.Where(uie => uie is not UIContainer))
            {
                uie.Update();
                uie.OnUpdate?.Invoke(uie);
                if (!occupancy && hover.Contains(uie))
                {
                    hoverAny = true;
                    if (!uie.IsMouseHover)
                    {
                        uie.ChangeMouseHover();
                        uie.MouseEnter();
                    }

                    uie.MouseHover();
                }
                else if (uie.IsMouseHover)
                {
                    uie.ChangeMouseHover();
                    uie.MouseLeave();
                }
            }

            if (hoverAny)
            {
                ((UIMovableView)hover.LastOrDefault(x => x is UIMovableView, null))?.HandleScroll(Mouse.GetState());
                if (mouseAny && changeToTop == null)
                {
                    changeToTop = c;
                    var mouseEvents = TryDoMouseEvent().ToArray();
                    foreach (var uie in hover)
                    {
                        foreach (var mouseEvent in mouseEvents)
                        {
                            mouseEvent?.Invoke(uie);
                        }
                    }
                    foreach (var uie in children.Except(hover))
                    {
                        uie.MouseOverInteract();
                    }
                }
            }
        }

        if (hoverAny)
        {
            Main.LocalPlayer.mouseInterface = true;
            PlayerInput.LockVanillaMouseScroll("ReitsKit");
        }

        foreach (var uie in needRemoves)
        {
            uie.Parent.Remove(uie);
        }

        needRemoves.Clear();

        if (changeToTop != null && changeToTop != callOrder[0])
        {
            SetTop(changeToTop);
        }
    }

    internal void Draw(SpriteBatch sb)
    {
        if (callOrder.Count == 0)
            return;
        var gd = Main.graphics.GraphicsDevice;
        UISpbState(sb);

        // 保存原UI画布（其实只是不清除）
        RenderTargetBinding[] originalRT2Ds = gd.GetRenderTargets();
        RenderTargetUsage lastRTUsage = gd.PresentationParameters.RenderTargetUsage;
        gd.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

        // 切换画布
        gd.SetRenderTarget(RTOrigin);
        gd.Clear(Color.Transparent);

        // 绘制所有容器
        for (int i = callOrder.Count - 1; i >= 0; i--)
        {
            callOrder[i].Draw(sb);
        }

        UISpbState(sb, false);

        gd.SetRenderTargets(originalRT2Ds);
        sb.Draw(RTOrigin, Vector2.Zero, Color.White);

        UISpbState(sb);
        gd.PresentationParameters.RenderTargetUsage = lastRTUsage;
        DrawHoverText(sb);
    }

    private void DrawHoverText(SpriteBatch sb)
    {
        if (HoverText == string.Empty)
            return;
        DynamicSpriteFont font = FontAssets.MouseText.Value;
        Vector2 pos = Main.MouseScreen + new Vector2(16);
        if (HoverTextMaxWidth > 0)
            HoverText = font.CreateWrappedText(HoverText, HoverTextMaxWidth);
        Vector2 size = ChatManager.GetStringSize(font, HoverText, Vector2.One);
        float overX = pos.X + size.X + 20 - Main.screenWidth;
        float overY = pos.Y + size.Y + 10 - Main.screenHeight;
        if (overX > 0)
            pos.X -= overX;
        if (overY > 0)
            pos.Y -= overY;
        RectangleF drawRec = new(pos, size.X + 20, size.Y + 10);
        if (DrawHoverBg)
        {
            sb.DrawCorner(AssetManager.VnlBd, drawRec, Color.Black, 12, 4);
            sb.DrawCorner(AssetManager.VnlBg, drawRec, VnlColor, 12, 4);
            drawRec += 10;
        }

        ChatManager.DrawColorCodedStringWithShadow(sb, font, HoverText, drawRec.TopLeft,
            Color.White, 0, Vector2.Zero, Vector2.One);
    }
}