/*using System;

namespace ReitsKit.UI.Encapsulation
{
    public class UIDropDownList<T> : UICornerPanel
    {
        public readonly UICornerPanel showArea, expandArea;
        public readonly UIContainerPanel expandView;
        private readonly VerticalScrollbar scroll;
        public readonly UIImage expandButton;
        public readonly Func<T, T> clone;
        public UIElement lockUI;
        public float ButtonXoffset;

        public T? ShowUIE
        {
            get
            {
                BaseUIElement? value = showArea.ChildrenElements.FirstOrDefault(x => true, null);
                return value == null ? null : (value as T);
            }
        }

        public T? FirstUIE
        {
            get
            {
                BaseUIElement? value = expandView.InnerUIE.FirstOrDefault(x => true, null);
                return value == null ? null : (value as T);
            }
        }

        public bool Expanding { get; private set; }

        public UIDropDownList(BaseUIElement parent, BaseUIElement lockUI, Func<T, T> clone)
        {
            showArea = new(0, 0, opacity: 1);
            showArea.Info.HiddenOverflow = true;
            showArea.Info.RightMargin.Pixel = 40;
            showArea.Events.OnLeftSingleDown += evt => Expand();
            showArea.BorderHoverToGold();
            showArea.ReDraw = sb =>
            {
                showArea.DrawSelf(sb);
                Rectangle rec = showArea.HitBox();
                Texture2D tex = Expanding ? AssetLoader.Fold : AssetLoader.Unfold;
                Vector2 origin = tex.Size() / 2f;
                Vector2 pos = rec.TopRight() + new Vector2(-origin.X - ButtonXoffset, rec.Height / 2f);
                sb.SimpleDraw(tex, pos, null, origin);
            };
            parent.OnAddedBy(showArea);

            expandArea = new(0, 0, opacity: 1);
            expandArea.Info.SetMargin(10);
            expandArea.Events.UnLeftDown += evt =>
            {
                if (!showArea.Info.IsMouseHover && Expanding)
                    Expand();
            };
            expandArea.Info.IsVisible = false;
            parent.OnAddedBy(expandArea);

            expandView = new();
            expandView.SetSize(-30, 0, 1, 1);
            expandArea.OnAddedBy(expandView);

            scroll = new(52, true, false);
            scroll.Info.Left.Pixel += 10;
            expandView.SetVerticalScrollbar(scroll);
            expandArea.OnAddedBy(scroll);

            this.lockUI = lockUI;
            this.clone = clone;

            ButtonXoffset = 10;
        }

        public void Expand()
        {
            expandArea.Info.IsVisible = Expanding = !Expanding;
            lockUI?.LockInteract(!Expanding);
        }

        public void SetWhellPixel(int pixel) => scroll.WheelPixel = pixel;

        public void ChangeShowElement(T uie)
        {
            showArea.RemoveAll();
            showArea.OnAddedBy(clone(uie));
        }

        public void ChangeShowElement(int index)
        {
            if (expandView.InnerUIE.IndexInRange(index))
            {
                ChangeShowElement(expandView.InnerUIE[index] as T);
            }
        }

        public void AddElement(T uie)
        {
            uie.Events.OnLeftSingleDown += evt => ChangeShowElement(uie);
            expandView.AddElement(uie);
        }

        public void ClearAllElements()
        {
            showArea.RemoveAll();
            expandView.ClearAllElements();
        }
    }
}*/