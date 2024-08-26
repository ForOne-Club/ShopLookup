namespace ForOneToolkit.UI.Interface;

public interface IHoverToColor
{
    /// <summary>
    /// 是否自动附加覆盖变化
    /// </summary>
    public bool UseHoverChange { get; init; }

    /// <summary>
    /// 覆盖变化的实现
    /// </summary>
    /// <param name="change"></param>
    /// <param name="source"></param>
    public void HoverToColor(Color? change = null, Color? source = null);
}