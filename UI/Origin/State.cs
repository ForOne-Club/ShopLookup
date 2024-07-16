namespace ReitsKit.UI.Origin;

public partial class UIElement
{
    public bool Active { get; private set; } = true;
    public bool Visible { get; private set; } = true;
    public bool Interactive { get; private set; } = true;
    public bool OtherSourceLock { get; private set; }
    public bool NeedRemove { get; private set; }
    public bool IsMouseHover { get; private set; }
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