using JetBrains.Annotations;
using System;
using System.Collections.Generic;

public class ShopInventoryModel
{
    #region [Dictionaries]
    //sementara stringnya ku jadiin itemnya, ntar ganti ke SO_Item
    private Dictionary<string, int> stockAmount = new(); //jumlah stock peritem
    private Dictionary<string, int> sellPrice = new(); //harga jual
    #endregion

    #region [Event]
    public event Action<string> OnStockChanged;
    #endregion

    #region [Getter Setter]
    public int GetStockCount(string item)
    {
        return stockAmount.ContainsKey(item) ? stockAmount[item] : 0;
    }

    public int GetSellPrice(string item)
    {
        return sellPrice.ContainsKey(item) ? sellPrice[item] : 0; //0 nya ganti ke harga defaultnya kalo dah ada ScriptableObjectnya, item.BasePrice
    }

    public void SetSellPrice(string item, int price)
    {
        sellPrice[item] = price;
    }
    #endregion

    #region [Methods]
    public void AddStock(string item, int amount) //buat beli stock atau kalo misal pembelinya gak jadi beli itu
    {
        stockAmount[item] = GetStockCount(item) + amount;
        OnStockChanged?.Invoke(item);
    }

    public bool TakeItem(string item) //ambil item dari rak
    {
        if (GetStockCount(item) <= 0) return false;

        stockAmount[item]--;
        OnStockChanged?.Invoke(item);
        return true;
    }

    public void ReturnItem(string item)
    {
        AddStock(item, 1);
    }
}
