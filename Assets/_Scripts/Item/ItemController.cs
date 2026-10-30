using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemController : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image itemSprite;

    public SO_Item Item { get; private set; }
    private ShopController shopController;
    private bool isDragging;

    public void Initialize(SO_Item itemData, ShopController shop)
    {
        Item = itemData;
        shopController = shop;
        itemSprite.sprite = Item.icon;
    }

    public void HandleStartDragging()
    {
        transform.position = Mouse.current.position.ReadValue();
        transform.SetAsLastSibling();
        itemSprite.raycastTarget = false;
        isDragging = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!shopController.HandleExitItemFromCounter(this)) return;
        HandleStartDragging();
    }

    private void Update()
    {
        if (!isDragging) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        transform.position = mousePos;

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
            itemSprite.raycastTarget = true;   
            shopController.HandleReleaseItem(this, mousePos);
        }
    }
}