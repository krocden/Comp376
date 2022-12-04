using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CurrencyManager : MonoBehaviour
{
    public const int STARTING_CASH = 100;
    public int totalCurrencyEarned = STARTING_CASH;

    public static CurrencyManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _currency = STARTING_CASH;
        OnCurrencyChanged = new UnityEvent<int>();
    }

    public int Currency { get => _currency; private set => _currency = value; }
    public UnityEvent<int> OnCurrencyChanged;

    private int _currency;

    public void Start() => OnCurrencyChanged.Invoke(_currency);

    public void AddCurrency(int toAdd) {
        _currency += toAdd;
        totalCurrencyEarned += toAdd;
        OnCurrencyChanged.Invoke(_currency);
        //play SFX for add
    }

    public bool SubtractCurrency(int toRemove, bool checkEnoughCurrency = true)
    {
        //return false if cannot afford transaction
        int newCurrency = _currency - toRemove;
       
        if(!checkEnoughCurrency)
            totalCurrencyEarned -= toRemove;

        if (checkEnoughCurrency && newCurrency < 0)
        {
            NotificationManager.Instance.PlayStandardNotification(NotificationType.NotEnoughCurrency);
            return false;
        }

        _currency -= toRemove;

        if (_currency < 0)
            _currency = 0;

        OnCurrencyChanged.Invoke(_currency);
        return true;

        //play SFX for removal
    }
}
