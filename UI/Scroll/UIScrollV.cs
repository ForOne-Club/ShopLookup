using System;

namespace ReitsKit.UI.Scroll
{
    public class UIScrollV : UIBarScroll
    {
        public UIScrollV(UIMovableView view, float? wheelValue, bool drawBorder = false) : base(view, wheelValue,
            drawBorder)
        {
            BarDraw = new UIImage(AssetManager.VScrollInner, false) { SourceRect = new(0, 0, 20, 6) };
            if (drawBorder)
                BorderDraw = new UIImage(AssetManager.VScrollBorder, false) { SourceRect = new(0, 0, 20, 6) };
            OnUpdate += uie =>
            {
                if (!dragging || oldPos.Y.Equals(Mouse.Y))
                    return;
                RectangleF i = uie.InsetArea;
                float h = Bar.Height / 2f;
                WaitY = Utils.GetLerpValue(i.Top + h, i.Bottom - h, Mouse.Y);
                oldPos.Y = Mouse.Y;
            };
            SetMargin(0, 6, 0, 6);
            SetSize(20, 0, 0, 1);
            SetPos(-20, 0, 1);
        }

        protected override float Wait
        {
            get => WaitY;
            set => WaitY = value;
        }

        protected override float Real
        {
            get => RealY;
            set => RealY = value;
        }

        protected override float Movable => MovableSize.Y;


        public override void UpdateViewDrag(Vector2 offset)
        {
            if (Movable == 0)
                return;
            WaitY -= offset.Y / Movable;
        }

        public override void Mapping(ref Vector2 target)
        {
            target.Y = Real;
        }

        protected override (Rectangle[], RectangleF[]) GetDrawBarRect(Rectangle r, Vector2 tex, RectangleF i,
            ref RectangleF bar)
        {
            if (Movable > 0)
            {
                float old = bar.Height;
                float now = View.InsetArea.Height / (View.InsetArea.Height + Movable) * i.Height;
                float d = now - old;
                if (d > 0.0001f)
                    bar.Height += d * 0.1f;
                else
                    bar.Height = now;
                bar.Height = Math.Min(bar.Height, i.Height);
            }
            else
                bar.Height = i.Height;
            bar.Top = i.Top + Real * (i.Height - bar.Height);
            Rectangle[] scissors = new Rectangle[3];
            RectangleF[] local = new RectangleF[3];
            int s = r.Height, w = r.Width, h = (int)tex.Y;
            scissors[0] = r;
            scissors[1] = new(0, s, w, h - 2 * s);
            scissors[2] = new(0, h - s, w, s);
            local[0] = i.Order(t: bar.Top - s, h: s);
            local[1] = i.Order(t: bar.Top, h: bar.Height);
            local[2] = i.Order(t: bar.Bottom, h: s);
            return (scissors, local);
        }

        protected override (Rectangle[], RectangleF[]) GetDrawBorderRect(Rectangle r, Vector2 tex, RectangleF f,
            RectangleF i)
        {
            Rectangle[] scissors = new Rectangle[3];
            RectangleF[] local = new RectangleF[3];
            int s = r.Height, w = r.Width, h = (int)tex.Y;
            scissors[0] = r;
            scissors[1] = new(0, s, w, h - 2 * s);
            scissors[2] = new(0, h - s, w, s);
            local[0] = f.Order(h: i.Top - f.Top);
            local[1] = i;
            local[2] = f.Order(t: i.Bottom, h: f.Bottom - i.Bottom);
            return (scissors, local);
        }

        public void VerticalFlowLayout(int w, int h)
        {
            View.AutoPos = list =>
            {
                int oriX = View.EdgeX ?? 0, oriY = View.EdgeY ?? 0;
                float x = oriX, y = oriY;
                foreach (var uie in list)
                {
                    if (!x.Equals(oriX) && x + uie.Width > View.InsetArea.Width)
                    {
                        x = oriX;
                        y += uie.Height + h;
                    }
                    uie.SetPos(x, y, cal: true);
                    x += uie.Width + w;
                }
            };
        }
    }
}