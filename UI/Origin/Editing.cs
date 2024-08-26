using System;

namespace ForOneToolkit.UI.Origin
{
    public partial class UIElement
    {
        /// <summary>
        /// 所有在UI开发模式下创建的UI元素均处“编辑中”状态
        /// </summary>
        public bool Editing { get; init; }

        private float focusOpacity;

        public void EditingUpdate()
        {
            focusOpacity += Math.Clamp(IsMouseHover ? 0.1f : -0.1f + focusOpacity, 0, 1);
        }
    }
}
