using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    #region [References]
    [Header("Main Shop Interface")]
    [SerializeField] private ShopShelfSlot[] shopShelves;
    [SerializeField] private CustomerController customerView;
    [SerializeField] private ShopCounter counterZone;

    [Header("Item")]
    [SerializeField] private ItemController itemPrefab;
    [SerializeField] private RectTransform itemLayer;

    [Header("Others")]
    [SerializeField] private DialogueView dialogue;
    [SerializeField] private Button kembalianButton;
    [SerializeField] private int[] nominalDuit = { 500, 1000, 2000, 5000, 10000, 20000 };
    #endregion

    #region [State]
    private SO_Day currentDay;
    private Action dayFinished;
    private ShopInventoryModel inventory;
    private CurrencyModel currency;
    private readonly List<SO_Item> orderedItems = new();

    private bool isOpen;                 
    private bool isServing;             
    private bool isWaitingKembalian;      
    private bool isCustomerDonePurchase;  
    private int pendingTotal;
    private int pendingKembalian;
    #endregion

    #region [Methods]
    public void Initialize(ShopInventoryModel inv, CurrencyModel curr)
    {
        inventory = inv;
        currency = curr;
        counterZone.Initialize(inventory);
        kembalianButton.interactable = false;

        foreach (ShopShelfSlot shelf in shopShelves) shelf.Initialize(inventory, this);
    }

    public void OpenShop(SO_Day day, Action onFinished)
    {
        if (isOpen) return;

        currentDay = day;
        dayFinished = onFinished;
        isOpen = true;
        StartCoroutine(StartDay());
    }

    private IEnumerator StartDay()
    {
        foreach (SO_Customer customer in currentDay.dayCustomer)
            yield return ServeCustomer(customer);

        CloseShop();
    }

    private IEnumerator ServeCustomer(SO_Customer customer)
    {
        customerView.Show(customer);
        yield return dialogue.PlayGreeting(customer);

        BuildOrder(customer);

        if (orderedItems.Count == 0)
        {
            yield return dialogue.PlayEmptyOrder();
            yield return LeaveCustomer();
            yield break;
        }

        yield return dialogue.PlayOrder(orderedItems);

        isCustomerDonePurchase = false;
        isServing = true;
        yield return new WaitUntil(() => isCustomerDonePurchase);

        yield return dialogue.PlayThanks();
        yield return LeaveCustomer();
    }

    private IEnumerator LeaveCustomer()
    {
        dialogue.Close();
        yield return customerView.Leave();
    }

    private void BuildOrder(SO_Customer customer)
    {
        orderedItems.Clear();
        foreach (SO_Item item in customer.customerPurchaseItem)
        {
            if (inventory.GetStockCount(item) > 0) orderedItems.Add(item);
        }
    }

    public void HandleTakeItemFromShelf(SO_Item item)
    {
        if (!isServing || !inventory.TakeItem(item)) return;

        ItemController currentItem = Instantiate(itemPrefab, itemLayer);
        currentItem.Initialize(item, this);
        currentItem.HandleStartDragging();
    }

    public bool HandleExitItemFromCounter(ItemController currentItem)
    {
        if (!isServing) return false;

        counterZone.RemoveItem(currentItem);
        return true;
    }

    public void HandleReleaseItem(ItemController currentItem, Vector2 screenPos)
    {
        if (isServing && counterZone.InsideCounterArea(screenPos))
        {
            counterZone.AddItem(currentItem);
        }
        else
        {
            inventory.ReturnItem(currentItem.Item);
            Destroy(currentItem.gameObject);
        }
    }

    public void HandleCheckout()
    {
        if (!isServing) return;

        if (!counterZone.MatchOrder(orderedItems))
        {
            dialogue.ShowWrongItem();
            return;
        }

        pendingTotal = counterZone.TotalPrice();
        int paid = PaymentFor(pendingTotal);
        pendingKembalian = paid - pendingTotal;

        isServing = false;
        isWaitingKembalian = true;
        kembalianButton.interactable = true;

        dialogue.ShowPayment(paid, pendingKembalian);
    }

    public void HandleGiveKembalian()
    {
        if (!isWaitingKembalian) return;

        isWaitingKembalian = false;
        kembalianButton.interactable = false;
        FinishSale();
    }

    private void FinishSale()
    {
        currency.GainBalance(pendingTotal);

        foreach (ItemController ctrl in counterZone.RemoveAllItem()) Destroy(ctrl.gameObject);

        isCustomerDonePurchase = true;
    }

    private void CloseShop()
    {
        isOpen = false;
        isServing = false;
        isWaitingKembalian = false;
        kembalianButton.interactable = false;

        foreach (ItemController ctrl in counterZone.RemoveAllItem())
        {
            inventory.ReturnItem(ctrl.Item);
            Destroy(ctrl.gameObject);
        }

        dialogue.Close();

        Action done = dayFinished;
        dayFinished = null;
        done?.Invoke();
    }

    private int PaymentFor(int total)
    {
        foreach (int note in nominalDuit)
        {
            if (note >= total) return note;
        }

        int top = nominalDuit[nominalDuit.Length - 1];
        return (total + top - 1) / top * top;
    }
    #endregion
}