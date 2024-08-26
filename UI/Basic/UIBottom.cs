namespace ForOneToolkit.UI.Basic;

/// <summary>
/// 一个完全透明的UI部件
/// </summary>
public class UIBottom : UIElement
{
    /// <summary>
    /// 是否允许拖动
    /// </summary>
    public bool CanDrag;
    private bool dragging;
    private Vector2 origin;

    public UIBottom()
    {
        OnLeftJustPress += _ => StartDrag();
        OnLeftJustRelease += _ => dragging = false;
    }

    public override void Update()
    {
        if (dragging)
        {
            Vector2 offset = Mouse - origin;
            if (offset != Vector2.Zero)
            {
                UIElement uie = this;
                uie >>= offset;
            }

            origin = Mouse;
        }
    }

    private void StartDrag()
    {
        if (!CanDrag) return;
        origin = Mouse;
        dragging = true;
    }
}