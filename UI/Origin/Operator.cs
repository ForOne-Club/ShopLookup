namespace ReitsKit.UI.Origin;

public partial class UIElement
{
    /// <summary>
    /// 将容器的位置加上绝对值后立刻重新定位
    /// </summary>
    /// <param name="uie"></param>
    /// <param name="abs"></param>
    /// <returns></returns>
    public static UIElement operator >>(UIElement uie, Vector2 abs)
    {
        ref var info = ref uie.FullLocation;
        info.Left.Absolute += abs.X;
        info.Top.Absolute += abs.Y;
        uie.Calculate();
        return uie;
    }
    public static void Move(UIElement uie, Vector2 abs) => uie >>= abs;

    /// <summary>
    /// 将容器的位置加上绝对值后立刻重新定位
    /// </summary>
    /// <param name="uie"></param>
    /// <param name="abs"></param>
    /// <returns></returns>
    public static UIElement operator >>(UIElement uie, float abs)
    {
        uie >>= new Vector2(abs);
        return uie;
    }

    /// <summary>
    /// 将容器的位置减去绝对值后立刻重新定位
    /// </summary>
    /// <param name="uie"></param>
    /// <param name="rel"></param>
    /// <returns></returns>
    public static UIElement operator <<(UIElement uie, Vector2 rel)
    {
        ref var info = ref uie.FullLocation;
        info.Left.Relative += rel.X;
        info.Top.Relative += rel.Y;
        uie.Calculate();
        return uie;
    }

    /// <summary>
    /// 将容器的位置减去绝对值后立刻重新定位
    /// </summary>
    /// <param name="uie"></param>
    /// <param name="rel"></param>
    /// <returns></returns>
    public static UIElement operator <<(UIElement uie, float rel)
    {
        uie <<= new Vector2(rel);
        return uie;
    }
}