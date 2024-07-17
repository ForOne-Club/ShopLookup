using Terraria.UI;
using UIElement = ReitsKit.UI.Origin.UIElement;

namespace ReitsKit.UI.Basic;

/// <summary>
/// 物品栏，交互请自己实现，毕竟每个地方功能都不同
/// </summary>
public class UIItemSlot : UIElement
{
    public Item Item;
    public int SlotID;
    public float Scale;
    public Texture2D OverrideTex;

    public UIItemSlot(int itemID, int slotID = 0, float scale = 1f) : this(new Item(itemID), slotID, scale)
    {
    }

    public UIItemSlot(Item item = null, int slotID = 0, float scale = 1f)
    {
        Item = item;
        SlotID = slotID;
        Scale = scale;
        SetSize(new(52 * scale));
    }
    public override void Update()
    {
    }
    public override void Draw(SpriteBatch sb)
    {
        if (IsMouseHover)
        {
            Main.hoverItemName = Item.Name;
            Main.HoverItem = Item.Clone();
        }
        sb.Draw(OverrideTex ?? AssetManager.InvSlot[SlotID].Value, Pos, Color.White);
        float old = Main.inventoryScale;
        Main.inventoryScale = Scale;
        ItemSlot.Draw(sb, ref Item, ItemSlot.Context.ChatItem, FullArea.TopLeft);
        Main.inventoryScale = old;
    }
}