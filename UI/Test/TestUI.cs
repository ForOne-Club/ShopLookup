using ForOneToolkit.UI.Encapsulation;
using ForOneToolkit.UI.Scroll;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;

namespace ForOneToolkit.UI.Test;

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

    public override bool AutoLoad => false;

    public override void OnInit()
    {
        UICornerPanel bg = [];
        bg.SetSize(300, 300);
        bg.CanDrag = true;
        bg.HoverToColor(Color.Gold, Color.Black);
        bg.SetPadding(10);
        Add(bg);

        UIMovableView view = [];

        UIDropDownList<UIText> ddl = new(40, 5, Clone, out var expand, view);
        ddl.SetSize(0, 30, 1);
        ddl.SetMargin(5, 3);
        bg.Add(ddl);

        expand.SetSize(0, -40, 1, 1);
        expand.SetPos(0, 40);

        for (int i = 0; i < 10; i++)
        {
            UIText t = new(i);
            t.HoverToColor();
            ddl.Add(t);
        }

        view.SetSize(-30, -70, 1, 1);
        view.SetPos(0, 40);
        bg.Add(view);

        UIScrollV vs = new(view, 62, true);
        vs.FullLocation.Top.Absolute += 40;
        vs.FullLocation.Height.Absolute -= 70;
        bg.Add(vs);
        view.AddScroll(vs);

        UIScrollH hs = new(view, null, true);
        hs.FullLocation.Width.Absolute -= 70;
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

    private static UIText Clone(UIText x) => new(x.Text, x.MaxWidth);
}