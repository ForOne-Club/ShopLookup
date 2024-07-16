using System;

namespace ReitsKit.UI.Origin;

public partial class UIElement : IList<UIElement>
{
    public static RenderTarget2D RTOrigin => UIManager.RTOrigin;
    public static RenderTarget2D RTSwap => UIManager.RTSwap;
    public static Vector2 Mouse => Main.MouseScreen;

    /// <summary>
    /// 绘制对应区域矩形，为null则不绘制
    /// <br/>0 - <see cref="FullArea"/>
    /// <br/>1 - <see cref="InsetArea"/>
    /// <br/>2 - <see cref="ActiveArea"/>
    /// </summary>
    public readonly Color?[] DrawArea = new Color?[3];

    /// <summary>
    /// UI元素 注册or删除 时自动设定
    /// </summary>
    public int SortID { get; private set; }

    public static ref string HoverText => ref UISystem.Manager.HoverText;
    public static ref bool DrawHoverBg => ref UISystem.Manager.DrawHoverBg;

    public bool ContainsPoint(Vector2 point) => ActiveArea.Contains(point);
    public Func<UIElement, RectangleF> OverrideOverFlow;
    public RectangleF OverFlowArea
    {
        get
        {
            RectangleF source = OverrideOverFlow?.Invoke(this) ?? InsetArea;
            return source.Intersect(Parent?.OverFlowArea, out var intersect) ? intersect : source;
        }
    }
    public Func<UIElement, RectangleF> OverrideActiveArea;
    protected virtual RectangleF ActiveArea
    {
        get
        {
            RectangleF source = OverrideActiveArea?.Invoke(this) ?? FullArea;
            return source.Intersect(Parent?.ActiveArea, out var intersect) ? intersect : source;
        }
    }

    public List<UIElement> GetAllChildren(Vector2 point, out HashSet<UIElement> contains)
    {
        List<UIElement> children = [];
        contains = [];
        foreach (var child in this)
        {
            children.AddRange(child.GetAllChildren(point, out var childrenContains));
            foreach (var c in childrenContains)
            {
                contains.Add(c);
            }
        }

        children.Add(this);
        if (ContainsPoint(point))
        {
            if (Interactive && !OtherSourceLock && (Sensitive || !Sensitive && contains.Count == 0))
            {
                contains.Add(this);
            }
        }
        return children;
    }

    /// <summary>
    /// 初始化UI
    /// </summary>
    public virtual void OnInit()
    {
    }

    public void Initialize()
    {
        OnInit();
        FinishInit();
    }

    public virtual void Update()
    {
    }

    protected virtual void DrawSelf(SpriteBatch sb)
    {
    }

    protected virtual void DrawChildren(SpriteBatch sb) => _children.ForEach(x => x.Draw(sb));

    public virtual void Draw(SpriteBatch sb)
    {
        if (Visible)
            DrawSelf(sb);
        if (DrawArea[0] != null)
            sb.DrawRectangle(FullArea, DrawArea[0].Value);
        if (DrawArea[1] != null)
            sb.DrawRectangle(InsetArea, DrawArea[1].Value);
        if (DrawArea[2] != null)
            sb.DrawRectangle(ActiveArea, DrawArea[2].Value);
        if (HiddenOverFlow)
        {
            UISpbState(sb);
            var gd = Main.graphics.GraphicsDevice;
            Rectangle old = gd.ScissorRectangle;
            gd.ScissorRectangle = OverFlowArea;
            DrawChildren(sb);
            UISpbState(sb);
            gd.ScissorRectangle = old;
        }
        else
            DrawChildren(sb);
    }
}