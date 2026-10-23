using System;

public class CurrencyModel 
{
    public int Balance { get; private set; }
    public event Action<int> OnBalanceChanged;

    public CurrencyModel(int startingBalance)
    {
        Balance = startingBalance;
    }

    public bool SpendBalance(int amount) //method buat beli stock/nafkah pokok yg pake duit kita
    {
        if (amount <= 0 || amount > Balance) return false;

        Balance -= amount;
        OnBalanceChanged?.Invoke(amount);
        return true;
    }

    public void GainBalance(int amount)
    {
        Balance += amount;
        OnBalanceChanged?.Invoke(amount);
    }
}
