namespace ReitsKit.UI.Interface;

public interface IHoverToColor
{
    public bool UseHoverChange { get; init; }
    public void HoverToColor(Color? change = null, Color? source = null);
}