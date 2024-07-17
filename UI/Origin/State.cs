namespace ReitsKit.UI.Origin;

public partial class UIElement
{
    /// <summary>
    /// 处于活动状态（影响更新与交互）
    /// </summary>
    public bool Active { get; private set; } = true;

    /// <summary>
    /// 处于可视状态（不影响更新与交互）
    /// </summary>
    public bool Visible { get; private set; } = true;

    /// <summary>
    /// 处于可交互状态
    /// </summary>
    public bool Interactive { get; private set; } = true;

    /// <summary>
    /// 处于被其他原因禁用交互状态
    /// </summary>
    public bool OtherSourceLock { get; private set; }

    /// <summary>
    /// 为防止更改迭代变量问题，统一用这个标记要删除的部件，会在下一帧统一删除
    /// </summary>
    public bool NeedRemove { get; private set; }

    /// <summary>
    /// 处于鼠标覆盖状态
    /// </summary>
    public bool IsMouseHover { get; private set; }

    /// <summary>
    /// 已完成初始化
    /// </summary>
    public bool InitDone { get; private set; }

    /// <summary>
    /// 是否在绘制子元素时启用溢出隐藏
    /// </summary>
    public bool HiddenOverFlow { get; init; }

    /// <summary>
    /// 其上有子元素也不影响事件判定
    /// </summary>
    public bool Sensitive { get; init; }

    public bool ChangeActive()
    {
        Active = !Active;
        ActiveChange();
        return Active;
    }

    public bool ChangeVisible()
    {
        Visible = !Visible;
        VisibleChange();
        return Visible;
    }

    public bool ChangeInteractive()
    {
        Interactive = !Interactive;
        InteractiveChange();
        return Interactive;
    }

    public void LockByOther()
    {
        OtherSourceLock = true;
        OtherSourceLockChange();
        foreach (var uie in this)
        {
            uie.LockByOther();
        }
    }

    public void UnlockByOther(bool only = false)
    {
        OtherSourceLock = false;
        OtherSourceLockChange();
        if (only) return;
        foreach (var uie in this)
        {
            uie.UnlockByOther();
        }
    }

    public void WaitingFoRemove()
    {
        NeedRemove = true;
        UISystem.Manager.AddToRemove(this);
        NeedRemoveChange();
    }

    public void ChangeMouseHover() => IsMouseHover = !IsMouseHover;

    public void FinishInit()
    {
        InitDone = true;
        foreach (var uie in this)
        {
            uie.FinishInit();
        }
    }
}