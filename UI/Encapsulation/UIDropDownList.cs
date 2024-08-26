using ForOneToolkit.UI.Scroll;
using System;

namespace ForOneToolkit.UI.Encapsulation
{
    public class UIDropDownList<T> : UICornerPanel where T : UIElement
    {
        public readonly UICornerPanel ExpandArea;
        public readonly UIMovableView ExpandView;
        public readonly Func<T, T> Clone;
        public readonly List<UIElement> Locks;
        private bool expanding;
        public T ShowUI => this[0] as T;

        public UIDropDownList(float? wheelValue, float spaceY, Func<T, T> clone, out UICornerPanel expand,
            params UIElement[] locks)
        {
            Clone = clone;
            this.Locks = [..locks];
            ExpandArea = expand = [];
            ExpandArea.ChangeActive();
            ExpandArea.OnLeftOverInteract += _ => Expand(false);
            ExpandView = [];
            ExpandView.Inner.OnAddAny += (_, uie) => uie.OnLeftJustPress += u => ChangeShow(u.SortID);
            ExpandArea.Add(ExpandView);
            HoverToColor();
            OnLeftJustPress += _ => Expand(true);

            UIScrollV sc = new(ExpandView, wheelValue);
            sc.VerticalLineBreak(spaceY);
            ExpandArea.Add(sc);
            ExpandView.AddScroll(sc);

            OnAddedBy += (_, p) =>
            {
                ChangeShow();
                p.Add(ExpandArea);
            };
        }

        public void ChangeShow(int index = 0)
        {
            base.Clear();
            if (Count > 0)
            {
                base.Add(Clone.Invoke(this[0] as T));
            }
            Calculate();
            Expand(false);
        }

        public void ChangeShow(T uie, bool needClone)
        {
            base.Clear();
            base.Add(needClone ? Clone.Invoke(uie) : uie);
            Expand(false);
        }

        public void Expand(bool expand)
        {
            if (expand == expanding) return;
            expanding = !expanding;
            ExpandArea.ChangeActive();
            if (expanding) { Locks.ForEach(x => x.LockByOther()); }
            else Locks.ForEach(x => x.UnlockByOther());
        }

        public new void Add(UIElement uie) => ExpandView.Add(uie);
        public new void Insert(int index, UIElement uie) => ExpandView.Insert(index, uie);
        public new bool Remove(UIElement uie) => ExpandView.Remove(uie);
        public new void RemoveAt(int index) => ExpandView.RemoveAt(index);
        public new void Clear() => ExpandView.Clear();
    }
}