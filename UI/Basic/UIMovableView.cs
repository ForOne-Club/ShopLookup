using Microsoft.Xna.Framework.Input;
using ReitsKit.UI.Scroll;
using System;

namespace ReitsKit.UI.Basic;

public class UIMovableView : UIElement
{
    public Action<List<UIElement>> AutoPos;
    public int? EdgeX, EdgeY;
    public Vector2 MovableSize;
    public bool CanDrag { get; init; } = true;
    private bool dragging;
    private bool needReCal;
    private Vector2 oldPos;
    public readonly List<UIScroll> Scrolls;
    public readonly UIBottom Inner;
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
        (this as UIElement).Add(Inner);
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

    public void HandleScroll(MouseState state) => Scrolls.ForEach(x => x.UpdateScroll(state));

    private bool CalMoveSize(UIElement self)
    {
        float x = 0, y = 0;
        int eX = EdgeX ?? 0, eY = EdgeY ?? 0;
        foreach (var uie in self)
        {
            float r = uie.FullLocation.Left.Absolute + uie.Width + eX, b = uie.FullLocation.Top.Absolute + uie.Height + eY;
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