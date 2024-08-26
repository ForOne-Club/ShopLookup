using Microsoft.Xna.Framework.Input;
using ForOneToolkit.UI.Interface;
using System;

namespace ForOneToolkit.UI.Scroll
{
    public abstract class UIBarScroll : UIScroll
    {
        protected UIBarScroll(UIMovableView view, float? wheelValue, bool drawBorder = false) : base(view, wheelValue)
        {
            OnUpdate += _ =>
            {
                float d = Wait - Real;
                if (Math.Abs(d) > 0.0001f)
                    Real += d * 0.1f;
                else
                    Real = Wait;
            };
        }

        protected RectangleF Bar;
        protected abstract float Wait { get; set; }
        protected abstract float Real { get; set; }
        protected abstract float Movable { get; }

        public override void UpdateScroll(MouseState state)
        {
            if (WheelValue == null || Movable == 0)
                return;
            int offset = state.ScrollWheelValue - OldWheel;
            float wheel = WheelValue.Value;
            if (offset != 0)
            {
                Wait -= Math.Sign(offset) * (wheel > 1 ? (wheel / Movable) : wheel);
                OldWheel = state.ScrollWheelValue;
            }
        }

        protected override void DrawBar(SpriteBatch sb, IDrawTexture bar, float opacity)
        {
            var (scissors, local) = GetDrawBarRect(bar.SourceRect!.Value, bar.Tex.Size(), InsetArea, ref Bar);
            sb.DrawRectByAnyStage(3, bar.Tex, scissors, local, Color.White * /*opacity*/1);
        }

        protected override void DrawBorder(SpriteBatch sb, IDrawTexture border)
        {
            var (scissors, local) = GetDrawBorderRect(border.SourceRect!.Value, border.Tex.Size(), FullArea, InsetArea);
            sb.DrawRectByAnyStage(3, border.Tex, scissors, local);
        }

        /// <summary>
        /// 先计算Bar
        /// <br/>获取用于Bar的绘制信息
        /// </summary>
        /// <param name="r">Bar的裁切</param>
        /// <param name="tex">贴图Size</param>
        /// <param name="i"><see cref="UIElement.InsetArea"/></param>
        /// <param name="bar">内拖条矩形</param>
        protected abstract (Rectangle[], RectangleF[]) GetDrawBarRect(Rectangle r, Vector2 tex, RectangleF i,
            ref RectangleF bar);

        /// <summary>
        /// 获取用于Border的绘制信息
        /// </summary>
        /// <param name="r">Border的裁切</param>
        /// <param name="tex">贴图Size</param>
        /// <param name="f"><see cref="UIElement.FullArea"/></param>
        /// <param name="i"><see cref="UIElement.InsetArea"/></param>
        protected abstract (Rectangle[], RectangleF[]) GetDrawBorderRect(Rectangle r, Vector2 tex, RectangleF f,
            RectangleF i);
    }
}