using System;
using System.Collections.Generic;

public class ShopInventoryModel
{
    #region [Dictionaries]
    //sementara stringnya ku jadiin itemnya, ntar ganti ke SO_Item
    private Dictionary<SO_Item, int> stockAmount = new(); //jumlah stock peritem
    private Dictionary<SO_Item, int> sellPrice = new(); //harga jual
    #endregion

    #region [Event]
    public event Action<SO_Item> OnStockChanged;
    #endregion

    #region [Getter Setter]
    public int GetStockCount(SO_Item item)
    {
        return stockAmount.ContainsKey(item) ? stockAmount[item] : 0;
    }

    public int GetSellPrice(SO_Item item)
    {
        return sellPrice.TryGetValue(item, out int price) ? price : item.baseSellPrice;
    }

    public void SetSellPrice(SO_Item item, int price)
    {
        sellPrice[item] = price;
    }
    #endregion

    #region [Methods]
    public void AddStock(SO_Item item, int amount) //buat beli stock atau kalo misal pembelinya gak jadi beli itu
    {
        stockAmount[item] = GetStockCount(item) + amount;
        OnStockChanged?.Invoke(item);
    }

    public bool TakeItem(SO_Item item) //ambil item dari rak
    {
        if (GetStockCount(item) <= 0) return false;

        stockAmount[item]--;
        OnStockChanged?.Invoke(item);
        return true;
    }

    public void ReturnItem(SO_Item item)
    {
        AddStock(item, 1);
    }
    #endregion
}
