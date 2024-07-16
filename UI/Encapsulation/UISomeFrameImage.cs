using System;

namespace ReitsKit.UI.Encapsulation
{
    public class UI2FrameImage : UIImage
    {
        /// <summary>
        /// 帧读取偏移
        /// </summary>
        public int FrameOffset;

        public Rectangle Frame { get; protected set; }
        public int OldState { get; protected set; }
        public UI2FrameImage(Texture2D tex) : base(tex, false) => SetSize(tex.Width / 2f, tex.Height);
        public override void OnInit() => ChooseFrame();

        public override void Update()
        {
            if (OldState != IsMouseHover.ToInt())
            {
                Frame = ChooseFrame();
                OldState = IsMouseHover.ToInt();
            }
        }

        protected virtual RectangleF ChooseFrame()
        {
            int i = IsMouseHover ? 1 : 0;
            if (SourceRect == null)
                return new(i * (Width + FrameOffset), 0, Width, Height);
            else
            {
                Rectangle r = SourceRect.Value;
                return new(r.Left + i * (Width + FrameOffset), r.Top, Width, Height);
            }
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            sb.Draw(Tex, FullArea.TopLeft, Frame, Vector2.Zero);
        }
    }

    public class UI3FrameImage : UI2FrameImage
    {
        /// <summary>
        /// 激活条件
        /// </summary>
        public readonly Func<UI3FrameImage, bool> Activator;

        public UI3FrameImage(Texture2D tex, Func<UI3FrameImage, bool> activator) : base(tex)
        {
            Activator = activator;
            SetSize(tex.Width / 3f, tex.Height);
        }

        public override void Update()
        {
            int state = Activator.Invoke(this) ? 2 : IsMouseHover ? 1 : 0;
            if (OldState != state)
            {
                Frame = ChooseFrame(state);
                OldState = state;
            }
        }

        private RectangleF ChooseFrame(int state)
        {
            if (SourceRect == null)
                return new(state * (Width + FrameOffset), 0, Width, Height);
            else
            {
                Rectangle r = SourceRect.Value;
                return new(r.Left + state * (Width + FrameOffset), r.Top, Width, Height);
            }
        }
    }

    public class UIClose(Texture2D tex = null) : UI2FrameImage(tex ?? AssetManager.VnlClose);

    public class UIAdjust(Texture2D tex = null)
        : UI3FrameImage(tex ?? AssetManager.VnlAdjust, x => x is UIAdjust { dragging: true })
    {
        private bool dragging;
        private Vector2 startPos;
        private float minX, minY, maxX, maxY;

        public override void OnInit()
        {
            base.OnInit();
            SetPos(-Width, -Height, 1, 1);
            UIElement pe = Parent;
            minX = pe.Width;
            minY = pe.Height;
            maxX = minX * 2;
            maxY = minY * 2;
            OnLeftJustPress += _ =>
            {
                dragging = true;
                startPos = Main.MouseScreen;
            };
        }

        public override void Update()
        {
            base.Update();
            if (dragging)
            {
                if (!Main.mouseLeft)
                {
                    dragging = false;
                    return;
                }
                Vector2 pos = Mouse;
                UIElement pe = Parent;
                if (!startPos.X.Equals(pos.X))
                {
                    float right = pe.Left + pe.Width;
                    float offset = pos.X - startPos.X;
                    if (CanMove(offset, pos.X, right))
                    {
                        Clamp(ref pe.FullLocation.Width.Absolute, pos.X - startPos.X, minX, maxX);
                        pe.Calculate();
                    }
                }
                if (!startPos.Y.Equals(pos.Y))
                {
                    float bottom = pe.Top + pe.Height;
                    float offset = pos.Y - startPos.Y;
                    if (CanMove(offset, pos.Y, bottom))
                    {
                        Clamp(ref pe.FullLocation.Height.Absolute, pos.Y - startPos.Y, minY, maxY);
                        pe.Calculate();
                    }
                }
                startPos = pos;
            }
        }

        private static bool CanMove(float offset, float mouse, float origin) =>
            (offset > 0 && mouse > origin) || (offset < 0 && mouse < origin);

        private static void Clamp(ref float value, float offset, float min, float max)
        {
            value = Math.Clamp(value + offset, min, max);
        }

        public void SetAdjustRange(float minX, float minY, float maxX, float maxY)
        {
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }
    }
}