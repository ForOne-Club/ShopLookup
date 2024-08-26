namespace ForOneToolkit.UI.VisualEditor;

public partial class Editor : UIContainer
{
    internal static string UILabel;
    public override bool AutoLoad => false;
    public override void OnInit()
    {
        ChangeActive();
        UILabel = Label;

        UICornerPanel panel = [];
        panel.SetSize(200, 100);
        panel.SetCenter(0, 0, 0, 0.5f);
        Add(panel);

        // UI3FrameImage
    }
}
