namespace ReitsKit.UI.Origin;

public partial class UIElement
{
    /// <summary>
    /// UI定位结构
    /// </summary>
    public struct UILocal(float absolute = 0, float relative = 0)
    {
        public static readonly UILocal Empty = new();

        /// <summary>
        /// 相对值
        /// </summary>
        public float Relative = relative;

        /// <summary>
        /// 绝对值
        /// </summary>
        public float Absolute = absolute;

        /// <summary>
        /// 设置UI的定位数据
        /// </summary>
        /// <param name="absolute">绝对值</param>
        /// <param name="relative">相对值</param>
        public void Set(float? absolute = null, float? relative = null)
        {
            if (absolute != null)
                Absolute = absolute.Value;
            if (relative != null)
                Relative = relative.Value;
        }

        /// <summary>
        /// 获取对于传入数据的绝对定位
        /// </summary>
        /// <param name="start">上级起点</param>
        /// <param name="length">上级长度</param>
        /// <returns></returns>
        public readonly float Get(float start, float length) => (int)(start + length * Relative + Absolute);

        public static UILocal operator +(UILocal left, UILocal right) =>
            new(left.Absolute + right.Absolute, left.Relative + right.Relative);

        public static UILocal operator -(UILocal left, UILocal right) =>
            new(left.Absolute - right.Absolute, left.Relative - right.Relative);
    }

    /// <summary>
    /// UI定位信息
    /// </summary>
    public struct FullAreaInfo()
    {
        public UILocal Left = UILocal.Empty;
        public UILocal Top = UILocal.Empty;
        public UILocal Width = UILocal.Empty;
        public UILocal Height = UILocal.Empty;
        public readonly Vector2 Absolute => new(Width.Absolute, Height.Absolute);

        public void SetSize(float w, float h, float wp = 0, float hp = 0)
        {
            Width.Set(w, wp);
            Height.Set(h, hp);
        }

        public void SetPos(float l, float t, float lp = 0, float tp = 0)
        {
            Left.Set(l, lp);
            Top.Set(t, tp);
        }

        public Vector2 GetPosition(RectangleF depend)
        {
            float left = Left.Get(depend.Left, depend.Width);
            float top = Top.Get(depend.Top, depend.Height);
            return new(left, top);
        }

        public Vector2 GetSize(RectangleF depend)
        {
            float width = Width.Get(0, depend.Width);
            float height = Height.Get(0, depend.Height);
            return new(width, height);
        }

        public RectangleF LocateWith(RectangleF depend) => new(GetPosition(depend), GetSize(depend));
    }

    public struct InsetAreaInfo()
    {
        public UILocal Left = UILocal.Empty;
        public UILocal Right = UILocal.Empty;
        public UILocal Top = UILocal.Empty;
        public UILocal Bottom = UILocal.Empty;

        public RectangleF LocateWith(RectangleF depend)
        {
            float left = Left.Get(depend.Left, depend.Width);
            float right = depend.Right - Right.Get(0, depend.Width);
            float top = Top.Get(depend.Top, depend.Height);
            float bottom = depend.Bottom - Bottom.Get(0, depend.Height);
            return RectangleF.New(left, top, right, bottom);
        }

        public void SetMargin(float? l = null, float? t = null, float? r = null, float? b = null)
        {
            if (l != null)
                Left.Absolute = l.Value;
            if (t != null)
                Top.Absolute = t.Value;
            if (r != null)
                Right.Absolute = r.Value;
            if (b != null)
                Bottom.Absolute = b.Value;
        }
    }

    /// <summary>
    /// UI交互区定位
    /// </summary>
    public FullAreaInfo FullLocation = new();

    /// <summary>
    /// UI内部交互区定位
    /// </summary>
    public InsetAreaInfo InsetLocation = new();

    /// <summary>
    /// UI交互区
    /// </summary>
    public RectangleF FullArea { get; private set; }

    /// <summary>
    /// UI内切交互区
    /// </summary>
    public RectangleF InsetArea { get; private set; }

    public float Left => InitDone ? FullArea.Left : FullLocation.Left.Absolute;
    public float Top => InitDone ? FullArea.Top : FullLocation.Top.Absolute;
    public float Width => InitDone ? FullArea.Width : FullLocation.Width.Absolute;
    public float Height => InitDone ? FullArea.Height : FullLocation.Height.Absolute;
    public float Right => InitDone ? FullArea.Right : (FullLocation.Left.Absolute + FullLocation.Width.Absolute);
    public float Bottom => InitDone ? FullArea.Bottom : (FullLocation.Top.Absolute + FullLocation.Height.Absolute);
    public Vector2 Size => InitDone ? FullArea.Size : new(FullLocation.Width.Absolute, FullLocation.Height.Absolute);
    public Vector2 Pos => FullArea.Pos;
    public Vector2 Center => FullArea.Center;

    public void Calculate()
    {
        if (PreCalSelf(this))
            CalculateSelf();
        if (PreCalChildren(this))
            CalculateChildren();

        if (PostCalSelf(this))
            CalculateSelf();
        if (PostCalChildren(this))
            CalculateChildren();
    }

    /// <summary>
    /// 将定位信息计算到实际坐标
    /// </summary>
    protected virtual void CalculateSelf()
    {
        RectangleF depend = Parent?.InsetArea ?? UISystem.Screen;
        FullArea = FullLocation.LocateWith(depend);
        InsetArea = InsetLocation.LocateWith(FullArea);
    }

    protected virtual void CalculateChildren() => _children.ForEach(x => x.Calculate());

    /// <summary>
    /// 设置大小
    /// </summary>
    /// <param name="w">绝对宽度</param>
    /// <param name="h">绝对高度</param>
    /// <param name="wp">相对宽度</param>
    /// <param name="hp">相对高度</param>
    /// <param name="cal">是否立即执行坐标计算</param>
    public void SetSize(float w, float h, float wp = 0, float hp = 0, bool cal = false)
    {
        FullLocation.SetSize(w, h, wp, hp);
        if (cal)
            Calculate();
    }

    /// <summary>
    /// 设置大小
    /// </summary>
    /// <param name="size"></param>
    /// <param name="cal"></param>
    public void SetSize(Vector2 size, bool cal = false) => SetSize(size.X, size.Y, cal: cal);

    /// <summary>
    /// 设置位置
    /// </summary>
    /// <param name="l">绝对横值</param>
    /// <param name="t">绝对纵值</param>
    /// <param name="lp">相对横值</param>
    /// <param name="tp">相对纵值</param>
    /// <param name="cal">是否立即执行坐标计算</param>
    public void SetPos(float l, float t, float lp = 0, float tp = 0, bool cal = false)
    {
        FullLocation.SetPos(l, t, lp, tp);
        if (cal)
            Calculate();
    }

    /// <summary>
    /// 设置位置——左上角
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="cal"></param>
    public void SetPos(Vector2 pos, bool cal = false) => SetPos(pos.X, pos.Y, cal: cal);

    /// <summary>
    /// 设置位置——中心
    /// <br/>在<see cref="Initialize"/>之前将使用Size的绝对值
    /// </summary>
    /// <param name="x">绝对横值</param>
    /// <param name="y">绝对纵值</param>
    /// <param name="xp">相对横值</param>
    /// <param name="yp">相对纵值</param>
    /// <param name="cal">是否立即执行坐标计算</param>
    public void SetCenter(float x, float y, float xp = 0.5f, float yp = 0.5f, bool cal = false)
    {
        Vector2 half = Size / 2f;
        FullLocation.Left.Set(x - half.X, xp);
        FullLocation.Top.Set(y - half.Y, yp);
        if (cal)
            Calculate();
    }

    /// <summary>
    /// 设置内边距
    /// </summary>
    /// <param name="l">绝对左边距</param>
    /// <param name="t">绝对上边距</param>
    /// <param name="r">绝对右边距</param>
    /// <param name="b">绝对下边距</param>
    /// <param name="cal">是否立即执行坐标计算</param>
    public void SetMargin(float? l = null, float? t = null, float? r = null, float? b = null, bool cal = false)
    {
        InsetLocation.SetMargin(l, t, r, b);
        if (cal)
            Calculate();
    }

    /// <summary>
    /// 快速设置内边距
    /// </summary>
    /// <param name="p">绝对四向内边距</param>
    /// <param name="cal">是否立即执行坐标计算</param>
    public void SetPadding(float p, bool cal = false) => SetMargin(p, p, p, p, cal);
}