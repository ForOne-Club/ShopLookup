using System.Collections;
using Terraria.Social.WeGame;
using Terraria.UI;

namespace ReitsKit.UI.Origin;

public partial class UIElement
{
    public UIElement Parent { get; private set; }
    private readonly List<UIElement> _children = [];

    public IEnumerator<UIElement> GetEnumerator() => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public static implicit operator List<UIElement>(UIElement uie) => uie._children;

    public bool AllowRegister(UIElement uie) => uie is { Parent: null } && !_children.Contains(uie);

    public bool AllowDelete(UIElement uie) => uie == null || uie.Parent != this;

    public void Add(UIElement uie)
    {
        if (!AllowRegister(uie)) return;
        uie!.Parent = this;
        uie.SortID = _children.Count;
        uie.OnInit();
        _children.Add(uie);
        AddAny(uie);
        uie.AddBy();
    }

    public void Insert(int index, UIElement uie)
    {
        if (!AllowRegister(uie)) return;
        uie!.SortID = index;
        uie.OnInit();
        _children.Insert(index, uie);
        AddAny(uie);
        uie.AddBy();
    }

    public bool Remove(UIElement uie)
    {
        if (!AllowDelete(uie)) return false;
        RemoveAny(uie);
        uie!.RemoveBy();
        uie.Parent = null;
        bool remove = _children.Remove(uie);
        if (remove)
        {
            int i = 0;
            foreach (var left in _children)
            {
                left.SortID = i++;
            }
        }
        return remove;
    }

    public void RemoveAt(int index)
    {
        UIElement r = _children[index];
        RemoveAny(r);
        r!.RemoveBy();
        _children.RemoveAt(index);
        int i = 0;
        foreach (var uie in _children)
        {
            uie.SortID = i++;
        }
    }

    public void Clear()
    {
        foreach (var uie in _children)
        {
            RemoveAny(uie);
            uie.RemoveBy();
            uie.Parent = null;
        }
        _children.Clear();
    }

    public bool Contains(UIElement uie) => _children.Contains(uie);

    public void CopyTo(UIElement[] array, int arrayIndex) => _children.CopyTo(array, arrayIndex);

    public int Count => _children.Count;

    public bool IsReadOnly => true;

    public int IndexOf(UIElement uie) => _children.IndexOf(uie);

    public UIElement this[int index]
    {
        get => _children[index];
        set => _children[index] = value;
    }
}