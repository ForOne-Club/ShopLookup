using System;

namespace ForOneToolkit.UI.Scroll
{
    /// <summary>
    /// 水平滚动条
    /// </summary>
    public class UIScrollH : UIBarScroll
    {
        public UIScrollH(UIMovableView view, float? wheelValue, bool drawBorder = false) : base(view, wheelValue,
            drawBorder)
        {
            BarDraw = new UIImage(AssetManager.HScrollInner, false) { SourceRect = new(0, 0, 6, 20) };
            if (drawBorder)
                BorderDraw = new UIImage(AssetManager.HScrollBDorder, false) { SourceRect = new(0, 0, 6, 20) };
            OnUpdate += uie =>
            {
                if (!dragging || oldPos.X.Equals(Mouse.X))
                    return;
                RectangleF i = uie.InsetArea;
                float w = Bar.Width / 2f;
                WaitX = Utils.GetLerpValue(i.Left + w, i.Right - w, Mouse.X);
                oldPos.X = Mouse.X;
            };
            SetMargin(6, 0, 6);
            SetSize(0, 20, 1);
            SetPos(0, -20, 0, 1);
        }

        protected override float Wait
        {
            get => WaitX;
            set => WaitX = value;
        }

        protected override float Real
        {
            get => RealX;
            set => RealX = value;
        }

        protected override float Movable => MovableSize.X;

        public override void UpdateViewDrag(Vector2 offset)
        {
            if (Movable == 0)
                return;
            WaitX -= offset.X / Movable;
        }

        public override void Mapping(ref Vector2 target)
        {
            target.X = Real;
        }

        protected override (Rectangle[], RectangleF[]) GetDrawBarRect(Rectangle r, Vector2 tex, RectangleF i,
            ref RectangleF bar)
        {
            if (Movable > 0)
            {
                float old = bar.Width;
                float now = View.InsetArea.Width / Movable * i.Width;
                float d = now - old;
                if (d > 0.0001f)
                    bar.Width += d * 0.1f;
                else
                    bar.Width = now;
                bar.Width = Math.Min(bar.Width, i.Width);
            }
            else
                bar.Width = i.Width;
            bar.Left = i.Left + Real * (i.Width - bar.Width);
            Rectangle[] scissors = new Rectangle[3];
            RectangleF[] local = new RectangleF[3];
            int s = r.Width, w = (int)tex.X, h = r.Height;
            scissors[0] = r;
            scissors[1] = new(s, 0, w - 2 * s, h);
            scissors[2] = new(w - s, 0, s, h);
            local[0] = i.Order(l: bar.Left - s, w: s);
            local[1] = i.Order(l: bar.Left, w: bar.Width);
            local[2] = i.Order(l: bar.Right, w: s);
            return (scissors, local);
        }

        protected override (Rectangle[], RectangleF[]) GetDrawBorderRect(Rectangle r, Vector2 tex, RectangleF f,
            RectangleF i)
        {
            Rectangle[] scissors = new Rectangle[3];
            RectangleF[] local = new RectangleF[3];
            int s = r.Width, w = (int)tex.X, h = r.Height;
            scissors[0] = r;
            scissors[1] = new(s, 0, w - 2 * s, h);
            scissors[2] = new(w - s, 0, s, h);
            local[0] = f.Order(w: i.Left - f.Left);
            local[1] = i;
            local[2] = f.Order(l: i.Right, w: f.Right - i.Right);
            return (scissors, local);
        }

        public void HorizonFlowLayout(int w, int h)
        {
            View.AutoPos = list =>
            {
                int oriX = View.EdgeX ?? 0, oriY = View.EdgeY ?? 0;
                float x = oriX, y = oriY;
                foreach (var uie in list)
                {
                    if (!y.Equals(oriY) && y + uie.Height > View.InsetArea.Height)
                    {
                        x += uie.Width + w;
                        y = oriY;
                    }
                    uie.SetPos(x, y, cal: true);
                    y += uie.Height + h;
                }
            };
        }
    }
}