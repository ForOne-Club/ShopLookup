using ForOneToolkit.UI.Scroll;
using Microsoft.Xna.Framework.Input;
using System;

namespace ForOneToolkit.UI.Basic;

/// <summary>
/// 可动视区，与<see cref="UIScroll"/>进行交互
/// </summary>
public class UIMovableView : UIElement
{
    /// <summary>
    /// 让内部部件自动设置位置的规则
    /// </summary>
    public Action<List<UIElement>> AutoPos;

    /// <summary>
    /// 边缘虚化距离
    /// </summary>
    public int? EdgeX, EdgeY;

    /// <summary>
    /// 可动距离
    /// </summary>
    public Vector2 MovableSize { get; private set; }

    /// <summary>
    /// 是否可以对视区进行绝对像素拖动
    /// </summary>
    public bool CanDrag { get; init; } = true;

    /// <summary>
    /// 绑定的滚动部件
    /// </summary>
    public readonly List<UIScroll> Scrolls;

    /// <summary>
    /// 用于管理内部部件的UI
    /// </summary>
    public readonly UIBottom Inner;

    private bool dragging;
    private bool needReCal;
    private Vector2 oldPos;
    public bool Scrolling => dragging || IsMouseHover;

    public UIMovableView()
    {
        HiddenOverFlow = true;
        OverrideOverFlow = uie => uie.InsetArea;
        Sensitive = true;
        Scrolls = [];
        Inner = [];
        Inner.OverrideActiveArea = uie => uie.Parent.FullArea;
        Inner.SetSize(0, 0, 1, 1);
        Inner.BeforeCalChildren += uie =>
        {
            if (AutoPos == null)
                return true;
            AutoPos?.Invoke(uie);
            return false;
        };
        Inner.AfterCalChildren += CalMoveSize;
        Inner.OnAddAny += (_, _) => needReCal = true;
        base.Add(Inner);
        OnLeftJustPress += _ =>
        {
            if (!CanDrag)
                return;
            dragging = true;
            oldPos = Mouse;
        };
    }

    public override void Update()
    {
        if (needReCal)
        {
            Inner.Calculate();
            needReCal = false;
        }
        CheckDrag();
        Vector2 target = Vector2.Zero;
        Scrolls.ForEach(x => x.Mapping(ref target));
        FullAreaInfo info = Inner.FullLocation;
        Vector2 real = -target * MovableSize;
        if (!info.Left.Absolute.Equals(real.X) || !info.Top.Absolute.Equals(real.Y))
        {
            Inner.SetPos(real, true);
        }
    }

    public void AddScroll(UIScroll scroll) => Scrolls.Add(scroll);
    public new void Add(UIElement uie) => Inner.Add(uie);
    public new void Insert(int index, UIElement uie) => Inner.Insert(index, uie);
    public new bool Remove(UIElement uie) => Inner.Remove(uie);
    public new void RemoveAt(int index) => Inner.RemoveAt(index);
    public new void Clear() => Inner.Clear();
    public new bool Contains(UIElement uie) => Inner.Contains(uie);

    public new void CopyTo(UIElement[] array, int arrayIndex) => Inner.CopyTo(array, arrayIndex);

    public new int Count => Inner.Count;

    public new int IndexOf(UIElement uie) => Inner.IndexOf(uie);

    public new UIElement this[int index]
    {
        get => Inner[index];
        set => Inner[index] = value;
    }

    public void HandleScroll(MouseState state) => Scrolls.ForEach(x => x.UpdateScroll(state));

    private bool CalMoveSize(UIElement self)
    {
        float x = 0, y = 0;
        int eX = EdgeX ?? 0, eY = EdgeY ?? 0;
        foreach (var uie in self)
        {
            float r = uie.FullLocation.Left.Absolute + uie.Width + eX,
                b = uie.FullLocation.Top.Absolute + uie.Height + eY;
            if (x < r)
                x = r;
            if (y < b)
                y = b;
        }
        x = Math.Max(0, x - InsetArea.Width);
        y = Math.Max(0, y - InsetArea.Height);
        MovableSize = new(x, y);
        return false;
    }

    private void CheckDrag()
    {
        if (dragging)
        {
            if (!Main.mouseLeft)
            {
                dragging = false;
                return;
            }
            Vector2 offset = Mouse - oldPos;
            if (offset != Vector2.Zero)
                Scrolls.ForEach(x => x.UpdateViewDrag(offset));
            oldPos = Mouse;
        }
    }
}