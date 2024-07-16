using System;

namespace ReitsKit.UI.Encapsulation;

public class UICornerInput : UICornerPanel
{
    public readonly UIInputBox Input;
    public Action<string> OnInput => Input.OnInput;
    public Action OnClear => Input.OnClear;

    public UICornerInput(string inputTips = "")
    {
        Input = new(inputTips);
        Sensitive = true;
    }

    public override void OnInit()
    {
        DrawColor = Color.White;
        Input.LineBreak = false;
        Input.SetPos(5, 5);
        Input.SetSize(-40, 0, 1, 1);
        Add(Input);

        UIClose clear = [];
        clear.DrawArea[0] = Color.Red;
        clear.SetCenter(-clear.Width - 10, 0, 1);
        clear.OnLeftSingleDown += _ => ClearText();
        Add(clear);
    }

    public void ClearText(bool doInput = true) => Input.ClearText(doInput);
}