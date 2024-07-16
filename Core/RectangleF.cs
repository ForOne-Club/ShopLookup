using static System.Math;

namespace ReitsKit.Core;

public struct RectangleF(float l = 0, float t = 0, float w = 0, float h = 0)
{
    public static readonly RectangleF Empty = new();

    #region 属性

    public float Left { get; set; } = l;
    public float Top { get; set; } = t;
    public float Width { get; set; } = w;
    public float Height { get; set; } = h;

    public float Right
    {
        readonly get => Left + Width;
        set => Width = value - Left;
    }

    public float Bottom
    {
        readonly get => Top + Height;
        set => Height = value - Top;
    }

    public readonly Vector2 TopLeft => new(Left, Top);
    public readonly Vector2 TopCenter => new(Left + Width / 2f, Top);
    public readonly Vector2 TopRight => new(Left + Width, Top);
    public readonly Vector2 LeftCenter => new(Left, Top + Height / 2f);
    public readonly Vector2 RightCenter => new(Right, Top + Height / 2f);
    public readonly Vector2 BottomLeft => new(Left, Top + Height);
    public readonly Vector2 BottomCenter => new(Left + Width / 2f, Bottom);
    public readonly Vector2 BottomRight => new(Left + Width, Top + Height);

    public Vector2 Pos
    {
        get => TopLeft;
        set
        {
            Left = value.X;
            Top = value.Y;
        }
    }

    public Vector2 Center
    {
        get => TopLeft + Size / 2;
        set => Pos = value - Size / 2;
    }

    public Vector2 Size
    {
        readonly get => new(Width, Height);
        set
        {
            Width = value.X;
            Height = value.Y;
        }
    }

    #endregion

    #region 构造函数

    public RectangleF(Vector2 pos, float width, float h) : this(pos.X, pos.Y, width, h)
    {
    }

    public RectangleF(float x, float y, Vector2 size) : this(x, y, size.X, size.Y)
    {
    }


    public RectangleF(Vector2 pos, Vector2 size) : this(pos.X, pos.Y, size.X, size.Y)
    {
    }

    #endregion

    #region 静态构造方法

    public static RectangleF New(float l, float t, float r, float b) => new(l, t, r - l, b - t);
    public static RectangleF New(Vector2 tl, float r, float b) => New(tl.X, tl.Y, r, b);
    public static RectangleF New(float l, float t, Vector2 br) => New(l, t, br.X, br.Y);
    public static RectangleF New(Vector2 tl, Vector2 br) => New(tl.X, tl.Y, br.X, br.Y);

    #endregion

    #region 操作符重载

    public static implicit operator Rectangle(RectangleF self) => self.CeilingToRect();
    public static implicit operator RectangleF(Rectangle self) => new(self.X, self.Y, self.Width, self.Height);

    /// <summary>
    /// 操作pos
    /// </summary>
    /// <param name="self"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static RectangleF operator +(RectangleF self, Vector2 offset)
    {
        self.Pos += offset;
        return self;
    }

    public static RectangleF operator +(RectangleF self, float offset)
    {
        self.Pos += new Vector2(offset);
        return self;
    }

    /// <summary>
    /// 操作pos
    /// </summary>
    /// <param name="self"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static RectangleF operator -(RectangleF self, Vector2 offset)
    {
        self.Pos -= offset;
        return self;
    }

    /// <summary>
    /// 操作size
    /// </summary>
    /// <param name="self"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static RectangleF operator >>(RectangleF self, Vector2 size)
    {
        self.Size += size;
        return self;
    }

    /// <summary>
    /// 操作size
    /// </summary>
    /// <param name="self"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static RectangleF operator <<(RectangleF self, Vector2 size)
    {
        self.Size -= size;
        return self;
    }

    /// <summary>
    /// 整体放大
    /// </summary>
    /// <param name="self"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static RectangleF operator *(RectangleF self, Vector2 scale)
    {
        self.Pos *= scale;
        self.Size *= scale;
        return self;
    }

    public static RectangleF operator *(RectangleF self, Matrix matrix)
    {
        self.Pos = Vector2.Transform(self.Pos, matrix);
        self.Size = Vector2.Transform(self.Size, matrix);
        return self;
    }

    /// <summary>
    /// 整体缩小
    /// </summary>
    /// <param name="self"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static RectangleF operator /(RectangleF self, Vector2 scale)
    {
        self.Left /= scale.X;
        self.Width /= scale.X;
        self.Top /= scale.Y;
        self.Height /= scale.Y;
        return self;
    }

    public static bool operator ==(RectangleF l, RectangleF r) =>
        l.Left.Equals(r.Left) && l.Top.Equals(r.Top) && l.Width.Equals(r.Width) && l.Height.Equals(r.Height);

    public static bool operator !=(RectangleF l, RectangleF r) =>
        !l.Left.Equals(r.Left) || !l.Top.Equals(r.Top) || !l.Width.Equals(r.Width) || !l.Height.Equals(r.Height);

    #endregion

    #region 实例方法

    public readonly bool Contains(Vector2 v) => Between(Left, v.X, Right) && Between(Top, v.Y, Bottom);

    public readonly bool Contains(RectangleF f) => Between(Left, f.Left, Right) && Between(Left, f.Right, Right)
                                                                       && Between(Top, f.Top, Bottom) &&
                                                                       Between(Top, f.Bottom, Bottom);

    public readonly bool Intersect(RectangleF? f, out RectangleF intersect)
    {
        if (f == null)
        {
            intersect = this;
            return false;
        }

        var target = f.Value;
        float newLeft = Max(Left, target.Left);
        float newTop = Max(Top, target.Top);
        float newRight = Min(Right, target.Right);
        float newBottom = Min(Bottom, target.Bottom);
        intersect = New(newLeft, newTop, newRight, newBottom);
        return intersect.Size != Vector2.Zero;
    }

    public readonly Rectangle CeilingToRect() => new((int)Left, (int)Top, (int)Ceiling(Width), (int)Ceiling(Height));
    public RectangleF ModifySelf(float l = 0, float t = 0, float w = 0, float h = 0)
    {
        Left += l;
        Top += t;
        Width += w;
        Height += h;
        return this;
    }

    public RectangleF Modify(float l = 0, float t = 0, float w = 0, float h = 0)
    {
        return new()
        {
            Left = Left += l,
            Top = Top += t,
            Width = Width += w,
            Height = Height += h,
        };
    }

    public RectangleF Order(float? l = null, float? t = null, float? w = null, float? h = null)
    {
        return new()
        {
            Left = l ?? Left,
            Top = t ?? Top,
            Width = w ?? Width,
            Height = h ?? Height,
        };
    }
    public RectangleF OrderSelf(float? l = null, float? t = null, float? w = null, float? h = null)
    {
        Left = l ?? Left;
        Top = t ?? Top;
        Width = w ?? Width;
        Height = h ?? Height;
        return this;
    }

    public readonly RectangleF Clamp(float? l = null, float? t = null, float? r = null, float? b = null)
    {
        RectangleF n = this;
        if (l != null)
        {
            n.Left = Max(n.Left, l.Value);
        }
        if (t != null)
        {
            n.Top = Max(n.Top, t.Value);
        }
        if (r != null)
        {
            n.Right = Min(n.Right, r.Value);
        }
        if (b != null)
        {
            n.Bottom = Min(n.Bottom, b.Value);
        }
        return n;
    }
    public RectangleF ClampSelf(float? l = null, float? t = null, float? r = null, float? b = null)
    {
        if (l != null)
        {
            Left = Max(Left, l.Value);
        }
        if (t != null)
        {
            Top = Max(Top, t.Value);
        }
        if (r != null)
        {
            Left = Min(Left, r.Value - Width);
        }
        if (b != null)
        {
            Top = Min(Top, b.Value - Height);
        }
        return this;
    }

    public readonly override bool Equals(object obj)
    {
        if (obj is RectangleF f)
        {
            return this == f;
        }

        return false;
    }

    public readonly override int GetHashCode() => CeilingToRect().GetHashCode();

    #endregion
}