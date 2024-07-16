using System;

namespace ReitsKit.UI.Origin;

public partial class UIElement
{
    #region MouseEvent

    public delegate void UIMouseEvent(UIElement uie);

    public event UIMouseEvent OnLeftJustPress;
    public event UIMouseEvent OnLeftSingleDown;
    public event UIMouseEvent OnLeftSingleClick;
    public event UIMouseEvent OnLeftDoubleDown;
    public event UIMouseEvent OnLeftDoubleClick;
    public event UIMouseEvent OnLeftHoldRelease;
    public event UIMouseEvent OnLeftJustRelease;

    public event UIMouseEvent OnRightJustPress;
    public event UIMouseEvent OnRightSingleDown;
    public event UIMouseEvent OnRightSingleClick;
    public event UIMouseEvent OnRightDoubleDown;
    public event UIMouseEvent OnRightDoubleClick;
    public event UIMouseEvent OnRightHoldRelease;
    public event UIMouseEvent OnRightJustRelease;

    public event UIMouseEvent OnMiddleJustPress;
    public event UIMouseEvent OnMiddleSingleDown;
    public event UIMouseEvent OnMiddleSingleClick;
    public event UIMouseEvent OnMiddleDoubleDown;
    public event UIMouseEvent OnMiddleDoubleClick;
    public event UIMouseEvent OnMiddleHoldRelease;
    public event UIMouseEvent OnMiddleJustRelease;

    public void LeftJustPress() => OnLeftJustPress?.Invoke(this);
    public void LeftSingleDown() => OnLeftSingleDown?.Invoke(this);
    public void LeftSingleClick() => OnLeftSingleClick?.Invoke(this);
    public void LeftDoubleDown() => OnLeftDoubleDown?.Invoke(this);
    public void LeftDoubleClick() => OnLeftDoubleClick?.Invoke(this);
    public void LeftHoldRelease() => OnLeftHoldRelease?.Invoke(this);
    public void LeftJustRelease() => OnLeftJustRelease?.Invoke(this);
    public void RightJustPress() => OnRightJustPress?.Invoke(this);
    public void RightSingleDown() => OnRightSingleDown?.Invoke(this);
    public void RightSingleClick() => OnRightSingleClick?.Invoke(this);
    public void RightDoubleDown() => OnRightDoubleDown?.Invoke(this);
    public void RightDoubleClick() => OnRightDoubleClick?.Invoke(this);
    public void RightHoldRelease() => OnRightHoldRelease?.Invoke(this);
    public void RightJustRelease() => OnRightJustRelease?.Invoke(this);
    public void MiddleJustPress() => OnMiddleJustPress?.Invoke(this);
    public void MiddleSingleDown() => OnMiddleSingleDown?.Invoke(this);
    public void MiddleSingleClick() => OnMiddleSingleClick?.Invoke(this);
    public void MiddleDoubleDown() => OnMiddleDoubleDown?.Invoke(this);
    public void MiddleDoubleClick() => OnMiddleDoubleClick?.Invoke(this);
    public void MiddleHoldRelease() => OnMiddleHoldRelease?.Invoke(this);
    public void MiddleJustRelease() => OnMiddleJustRelease?.Invoke(this);

    public event UIMouseEvent OnLeftOverInteract;
    public event UIMouseEvent OnRightOverInteract;
    public event UIMouseEvent OnMiddleOverInteract;
    public void LeftOverInteract() => OnLeftOverInteract?.Invoke(this);
    public void RightOverInteract() => OnRightOverInteract?.Invoke(this);
    public void MiddleOverInteract() => OnMiddleOverInteract?.Invoke(this);

    public event UIMouseEvent OnMouseEnter;
    public event UIMouseEvent OnMouseLeave;
    public event UIMouseEvent OnMouseHover;
    public event UIMouseEvent OnMouseOverInteract;
    public void MouseOverInteract() => OnMouseOverInteract?.Invoke(this);

    public void MouseEnter() => OnMouseEnter?.Invoke(this);
    public void MouseLeave() => OnMouseLeave?.Invoke(this);
    public void MouseHover() => OnMouseHover?.Invoke(this);

    #endregion

    #region CalculateEvent

    public delegate bool UICalculate(UIElement uie);

    public event UICalculate BeforeCalSelf;
    public event UICalculate AfterCalSelf;
    public event UICalculate BeforeCalChildren;
    public event UICalculate AfterCalChildren;

    /// <summary>
    /// 在计算坐标之前调用
    /// </summary>
    /// <param name="uie"></param>
    /// <returns>是否进行坐标计算</returns>
    public bool PreCalSelf(UIElement uie) => BeforeCalSelf?.Invoke(uie) ?? true;

    /// <summary>
    /// 在计算完坐标后调用
    /// </summary>
    /// <param name="uie"></param>
    /// <returns>是否再次计算坐标</returns>
    public bool PostCalSelf(UIElement uie) => AfterCalSelf?.Invoke(uie) ?? false;

    public bool PreCalChildren(UIElement uie) => BeforeCalChildren?.Invoke(uie) ?? true;
    public bool PostCalChildren(UIElement uie) => AfterCalChildren?.Invoke(uie) ?? false;

    #endregion

    #region DependEvent

    public delegate void UIDependChange(UIElement orig, UIElement depend);

    /// <summary>
    /// 自身被注册时
    /// </summary>
    public event UIDependChange OnAddedBy;

    /// <summary>
    /// 自身被移除时
    /// </summary>
    public event UIDependChange OnRemovedBy;

    /// <summary>
    /// 被添加元素时
    /// </summary>
    public event UIDependChange OnAddAny;

    /// <summary>
    /// 移除元素时
    /// </summary>
    public event UIDependChange OnRemoveAny;

    /// <summary>
    /// 自身注册时调用
    /// </summary>
    public void AddBy() => OnAddedBy?.Invoke(this, Parent);

    /// <summary>
    /// 自身被移除时调用
    /// </summary>
    public void RemoveBy() => OnRemovedBy?.Invoke(this, Parent);

    /// <summary>
    /// 被添加元素时调用
    /// </summary>
    /// <param name="add"></param>
    public void AddAny(UIElement add) => OnAddAny?.Invoke(this, add);

    /// <summary>
    /// 移除元素时调用
    /// </summary>
    /// <param name="remove"></param>
    public void RemoveAny(UIElement remove) => OnRemoveAny?.Invoke(this, remove);

    #endregion

    #region StateEvent

    public delegate void UIStateChange(UIElement uie, bool nowState);

    public event UIStateChange OnActiveChange;
    public event UIStateChange OnVisibleChange;
    public event UIStateChange OnInteractiveChange;
    public event UIStateChange OnOtherSourceLockChange;
    public event UIStateChange OnNeedRemoveChange;
    public void ActiveChange() => OnActiveChange?.Invoke(this, Active);
    public void VisibleChange() => OnVisibleChange?.Invoke(this, Visible);
    public void InteractiveChange() => OnInteractiveChange?.Invoke(this, Interactive);
    public void OtherSourceLockChange() => OnOtherSourceLockChange?.Invoke(this, OtherSourceLock);
    public void NeedRemoveChange() => OnNeedRemoveChange?.Invoke(this, NeedRemove);

    #endregion

    public Action<UIElement> OnUpdate;
}