using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopCounter : MonoBehaviour
{
    [SerializeField] private RectTransform counterZone;
    [SerializeField] private TMP_Text counterText;

    private ShopInventoryModel inventory;
    private readonly List<ItemController> itemControllers = new();

    public void Initialize(ShopInventoryModel inv)
    {
        inventory = inv;
        RefreshCounterText();
    }

    public bool InsideCounterArea(Vector2 screenPos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(counterZone, screenPos, null);
    }

    public void AddItem(ItemController ctrl)
    {
        if (itemControllers.Contains(ctrl)) return;
        itemControllers.Add(ctrl);
        RefreshCounterText();
    }

    public void RemoveItem(ItemController ctrl)
    {
        if (!itemControllers.Remove(ctrl)) return;
        RefreshCounterText();
    }

    public List<ItemController> RemoveAllItem()
    {
        var all = new List<ItemController>(itemControllers);
        itemControllers.Clear();
        RefreshCounterText();
        return all;
    }

    public int TotalPrice()
    {
        int total = 0;
        foreach (ItemController ctrl in itemControllers)
        {
            total += inventory.GetSellPrice(ctrl.Item);
        }
        return total;
    }

    public bool MatchOrder(List<SO_Item> order)
    {
        if (itemControllers.Count != order.Count) return false;

        var remaining = new List<SO_Item>();
        
        foreach (ItemController ctrl in itemControllers)
        {
            remaining.Add(ctrl.Item);
        }

        foreach (SO_Item item in order)
        {
            if (!remaining.Remove(item)) return false;
        }
        return true;
    }

    private void RefreshCounterText()
    {
        counterText.text = "Total: " + TotalPrice();
    }
}