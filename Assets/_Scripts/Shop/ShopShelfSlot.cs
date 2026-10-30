using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopShelfSlot : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private SO_Item itemData;
    [SerializeField] private Image itemImage;   

    private ShopInventoryModel inventory;
    private ShopController shopController;

    public void Initialize(ShopInventoryModel inv, ShopController shop)
    {
        inventory = inv;
        shopController = shop;
        inventory.OnStockChanged += HandleStockChanged;
        Refresh();
    }

    private void OnDestroy()
    {
        if (inventory != null) inventory.OnStockChanged -= HandleStockChanged;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        shopController.HandleTakeItemFromShelf(itemData);
    }

    private void HandleStockChanged(SO_Item data)
    {
        if (data == itemData) Refresh();
    }

    void Refresh()
    {
        int count = inventory.GetStockCount(itemData);
        itemImage.enabled = count > 0;
        itemImage.sprite = itemData.icon;
    }
}