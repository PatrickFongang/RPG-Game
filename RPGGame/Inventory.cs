using RPGGame.Items;

namespace RPGGame;

public class Inventory
{
    private List<Item> _items;
    public IReadOnlyList<Item> Items => _items.AsReadOnly();
    public Item? SelectedItem { get; private set; }
    public int SelectedItemIndex { get; private set; }

    public Inventory()
    {
        _items = new List<Item>();
        SelectedItemIndex = 0;
    }

    public void AddItem(Item? item)
    {
        if (item == null) return;
        _items.Add(item);
        if (SelectedItem == null)
        {
            SelectedItem = item;
            SelectedItemIndex = 0;
        }
    }

    public void RemoveItem()
    {
        if (_items.Count == 0) return;

        _items.RemoveAt(SelectedItemIndex);

        if (_items.Count == 0)
        {
            SelectedItem = null;
            SelectedItemIndex = 0;
            return;
        }

        if (SelectedItemIndex >= _items.Count)
        {
            SelectedItemIndex = _items.Count - 1;
        }

        SelectedItem = _items[SelectedItemIndex];
    }

    public void MoveSelectedItemDown()
    {
        if (_items.Count == 0) return;
        SelectedItemIndex = SelectedItemIndex + 1 == _items.Count ? SelectedItemIndex : SelectedItemIndex + 1;
        SelectedItem = _items[SelectedItemIndex];
    }

    public void MoveSelectedItemUp()
    {
        if (_items.Count == 0) return;
        SelectedItemIndex = SelectedItemIndex - 1 < 0 ? 0 : SelectedItemIndex - 1;
        SelectedItem = _items[SelectedItemIndex];
    }
}