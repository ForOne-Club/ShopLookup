using System;

namespace ForOneToolkit.Core
{
    public enum CtrlState
    {
        None,
        SingleDown,
        SingleClick,
        DoubleDown,
        DoubleClick,
        HoldRelease,
    }

    public class KeyCtrl(Func<bool> press, int cd = 15)
    {
        private int timer;

        // 是否检测双击
        private bool checkDouble;

        /// <summary>
        /// 当前是否按下
        /// </summary>
        public bool Pressing => press.Invoke();

        private bool IsInCoolDown => timer > 0;

        /// <summary>
        /// 当前触发状态，每帧重置
        /// </summary>
        public CtrlState State { get; private set; }

        public bool JustPress { get; private set; }
        public bool JustRelease { get; private set; }

        /// <summary>
        /// 已经按住的时间
        /// </summary>
        public int KeepTime { get; private set; }

        public void Update()
        {
            State = CtrlState.None;
            JustPress = JustRelease = false;
            if (Pressing)
            {
                if (KeepTime++ == 0)
                {
                    if (checkDouble && IsInCoolDown)
                    {
                        State = CtrlState.DoubleDown;
                    }
                    else
                    {
                        State = CtrlState.SingleDown;
                    }
                    JustPress = true;
                    IntoCoolDown();
                }
            }
            else
            {
                if (KeepTime > 0)
                {
                    KeepTime = 0;
                    if (IsInCoolDown)
                    {
                        if (checkDouble)
                        {
                            State = CtrlState.DoubleClick;
                            checkDouble = false;
                            ReSetCoolDown();
                        }
                        else
                        {
                            State = CtrlState.SingleClick;
                            checkDouble = true;
                        }
                    }
                    else
                    {
                        State = CtrlState.HoldRelease;
                    }
                    JustRelease = true;
                }
            }

            if (timer > 0)
            {
                timer--;
                if (timer == 0) checkDouble = false;
            }
        }

        private void IntoCoolDown() => timer = cd;
        private void ReSetCoolDown() => timer = 0;
    }
}