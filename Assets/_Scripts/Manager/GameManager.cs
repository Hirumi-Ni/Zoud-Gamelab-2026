using System;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class GameManager : MonoBehaviour
{
    #region [References]
    public static GameManager Instance { get; private set; }

    [SerializeField] private SO_Day[] gameDays;
    [SerializeField] private int startingBalance = 1000000; //1jt
    [SerializeField] private ShopController shopController;
    #endregion

    #region [Properties]
    public GameState State { get; private set; }
    public int DayNum { get; private set; } = 1;
    public CurrencyModel Currency { get; private set; }
    public ShopInventoryModel Inventory { get; private set; }

    #endregion

    #region [Event]
    public event Action<GameState> OnStateChanged;
    public event Action OnDayClosed;
    #endregion

    #region [Unity Lifecycle]
    private void Awake()
    {
        Instance = this;

        Currency = new(startingBalance);
        Inventory = new();
    }

    private void Start()
    {
        //shopController.

        SetState(GameState.Prepare);
    }
    #endregion

    #region [Methods]
    void SetState(GameState next)
    {
        State = next;
        OnStateChanged?.Invoke(next);
    }

    public void OpenShop()
    {
        SetState(GameState.Open);
        shop.Run(days[DayNumber - 1], CloseShop);
    }

    void CloseShop()
    {
        SetState(GameState.Close);
        DayClosed?.Invoke(DayNumber);
    }
    #endregion
}
