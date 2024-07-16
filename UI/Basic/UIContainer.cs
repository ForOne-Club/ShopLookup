using System.Collections.Generic;

namespace ReitsKit.UI.Basic;

public abstract class UIContainer : UIElement
{
    /// <summary>
    /// 当<see cref="UIManager.TopContainer"/>的返回结果是这个容器时
    /// <br/>在指定矩形列表范围内禁用其他容器的交互
    /// </summary>
    public abstract List<RectangleF> Occupancy { get; }

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
}