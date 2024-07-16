using Microsoft.Xna.Framework.Input;
using ReitsKit.UI.Encapsulation;
using ReitsKit.UI.Scroll;
using Terraria.GameInput;

namespace ReitsKit.UI.Test;

public class TestPlayer : ModPlayer
{
    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        var state = Keyboard.GetState();
        if (state.IsKeyDown(Keys.LeftControl) && state.IsKeyDown(Keys.C))
        {
            UISystem.Manager.ReSet();
        }
    }
}

public class TestUI : UIContainer
{
    public override List<RectangleF> Occupancy => null;

    public override bool AutoLoad => true;

    public override void OnInit()
    {
        UICornerPanel bg = [];
        bg.SetSize(300, 300);
        bg.CanDrag = true;
        bg.HoverToColor(Color.Gold, Color.Black);
        bg.SetPadding(10);
        Add(bg);

        UIMovableView view = [];
        view.SetSize(-30, -30, 1, 1);
        bg.Add(view);

        UIScrollV vs = new(view, 62, true);
        vs.FullLocation.Height.Absolute -= 30;
        bg.Add(vs);
        view.AddScroll(vs);

        UIScrollH hs = new(view, null, true);
        hs.FullLocation.Width.Absolute -= 30;
        bg.Add(hs);
        view.AddScroll(hs);

        UIAdjust adjust = [];
        bg.Add(adjust);

        for (int i = 0; i < 200; i++)
        {
            UIItemSlot slot = new(i);
            slot.SetPos(62 * (i % 20), 62 * (i / 20));
            view.Add(slot);
        }
    }
}