using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region [Subclass]
    [Serializable]
    public class StockEntry
    {
        public SO_Item item;
        public int amount;
    }
    #endregion

    #region [References]
    public static GameManager Instance { get; private set; }

    [SerializeField] private SO_Day[] gameDays;
    [SerializeField] private int startingBalance = 1000000;
    [SerializeField] private ShopController shopController;
    [SerializeField] private OverviewView overview;
    [SerializeField] private StockEntry[] startingStock;    // sementara, buat ngeset stok awalnya, ntar kalo udh jadi mekanik restocknya dihapus aja ini
    #endregion

    #region [Properties]
    public GameState State { get; private set; }
    public int DayNum { get; private set; } = 1;
    public CurrencyModel Currency { get; private set; }
    public ShopInventoryModel Inventory { get; private set; }
    public bool HasNextDay => DayNum < gameDays.Length;
    #endregion

    #region [Events]
    public event Action<GameState> OnStateChanged;
    public event Action OnDayClosed;
    #endregion

    #region [Unity Lifecycle]
    private void Awake()
    {
        Instance = this;

        Currency = new CurrencyModel(startingBalance);
        Inventory = new ShopInventoryModel();

        foreach (StockEntry entry in startingStock) Inventory.AddStock(entry.item, entry.amount);

        shopController.Initialize(Inventory, Currency);
        overview.NextDayRequested += NextDay;
    }

    private void OnDestroy()
    {
        if (overview != null) overview.NextDayRequested -= NextDay;
    }

    private void Start()
    {
        SetState(GameState.Prepare);
    }
    #endregion

    #region [Methods]
    public void OpenShop()
    {
        if (State != GameState.Prepare || !IsValidDay(DayNum)) return;

        SetState(GameState.Open);
        shopController.OpenShop(gameDays[DayNum - 1], CloseShop);
    }

    private void CloseShop()
    {
        SetState(GameState.Close);
        overview.Show(DayNum);
        OnDayClosed?.Invoke();
    }

    public void NextDay()
    {
        if (State != GameState.Close) return;

        DayNum++;
        SetState(GameState.Prepare);
    }

    private bool IsValidDay(int dayNum)
    {
        return dayNum >= 1 && dayNum <= gameDays.Length;
    }

    private void SetState(GameState next)
    {
        State = next;
        OnStateChanged?.Invoke(next);
    }
    #endregion
}