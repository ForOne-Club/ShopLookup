using Microsoft.Xna.Framework.Input;
using ReitsKit.UI.Interface;
using System;

namespace ReitsKit.UI.Scroll;

public abstract class UIScroll : UIElement
{
    /// <summary>
    /// 绑定的可动视区
    /// </summary>
    public UIMovableView View { get; init; }

    /// <summary>
    /// 设置贴图和裁剪矩形
    /// </summary>
    public IDrawTexture BarDraw { get; init; }

    /// <summary>
    /// 设置贴图和裁剪矩形
    /// </summary>
    public IDrawTexture BorderDraw { get; init; }

    public Vector2 MovableSize => View.MovableSize;
    public float? WheelValue;
    protected bool dragging;
    protected Vector2 oldPos;
    protected int OldWheel;
    private float opacity;
    private Vector2 _waitLocal;


    public float WaitX
    {
        get => _waitLocal.X;
        set => _waitLocal.X = Math.Clamp(value, 0, 1);
    }

    public float WaitY
    {
        get => _waitLocal.Y;
        set => _waitLocal.Y = Math.Clamp(value, 0, 1);
    }

    private Vector2 _realLocal;

    public float RealX
    {
        get => _realLocal.X;
        protected set => _realLocal.X = Math.Clamp(value, 0, 1);
    }

    public float RealY
    {
        get => _realLocal.Y;
        protected set => _realLocal.Y = Math.Clamp(value, 0, 1);
    }

    protected UIScroll(UIMovableView view, float? wheelValue)
    {
        View = view;
        WheelValue = wheelValue;
        OnUpdate += _ =>
        {
            bool use = IsMouseHover || View.Scrolling || dragging;
            opacity = Math.Clamp(opacity + 0.05f * (use ? 1 : -1), 0, 1);
            if (!Main.mouseLeft)
                dragging = false;
            if (!use)
                OldWheel = Microsoft.Xna.Framework.Input.Mouse.GetState().ScrollWheelValue;
        };
        OnLeftJustPress += _ => dragging = true;
    }

    protected sealed override void DrawSelf(SpriteBatch sb)
    {
        if (BorderDraw != null)
            DrawBorder(sb, BorderDraw);
        DrawBar(sb, BarDraw, opacity);
    }

    /// <summary>
    /// 使用鼠标拖动视区进行绝对像素移动，更新WaitLocal
    /// </summary>
    /// <param name="offset"></param>
    public abstract void UpdateViewDrag(Vector2 offset);

    /// <summary>
    /// 在此将RealLocal映射到视区上，已将结果反向
    /// </summary>
    /// <param name="target"></param>
    public abstract void Mapping(ref Vector2 target);

    /// <summary>
    /// 传入滚动状态，更新WaitLocal
    /// </summary>
    /// <param name="state"></param>
    public abstract void UpdateScroll(MouseState state);

    protected abstract void DrawBorder(SpriteBatch sb, IDrawTexture border);
    protected abstract void DrawBar(SpriteBatch sb, IDrawTexture bar, float opacity);
}