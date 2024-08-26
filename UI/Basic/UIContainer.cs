namespace ForOneToolkit.UI.Basic;

/// <summary>
/// 要开始写一个UI时就继承这个，对标原版的<see cref="Terraria.UI.UIState"/>
/// </summary>
public abstract class UIContainer : UIElement
{
    /// <summary>
    /// 当<see cref="UIManager.TopContainer"/>的返回结果是这个容器时
    /// <br/>在指定矩形列表范围内禁用其他容器的交互
    /// </summary>
    public virtual List<RectangleF> Occupancy => [this[0].FullArea];

    /// <summary>
    /// UI容器的标识符
    /// 用于<see cref="UIManager.TryGetContainer"/>
    /// </summary>
    public virtual string Label => GetType().Name;

    /// <summary>
    /// 是否自动加载
    /// </summary>
    public virtual bool AutoLoad => true;

    /// <summary>
    /// 是否自动初始化<see cref="UIElement.OnInit"/>
    /// </summary>
    public virtual bool AutoInit => true;

    public void Initialize()
    {
        OnInit();
        FinishInit();
        Calculate();
    }
}