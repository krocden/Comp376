using System;
using UnityEngine;

public class WallAutomata
{
    public static int PlainWallCost => _plainWallCost;

    private static int _plainWallCost;
    private static int _gunTurretCost;
    private static int _cannonTurretCost;
    private static int _portalTurretCost;
    private static int _buffTurretCost;
    private static int _slowTurretCost;
    private static int _barrierWallCost;
    private static float _upgradeCostMultiplier;
    private static float _refundCostMultiplier;

    public void SetCosts(
        int plainCost,
        int gunCost,
        int cannonCost,
        int portalCost,
        int buffCost,
        int slowCost,
        int barrierCost,
        float upgradeMulti,
        float refundMulti)
    {
        _plainWallCost = plainCost;
        _gunTurretCost = gunCost;
        _cannonTurretCost = cannonCost;
        _portalTurretCost = portalCost;
        _buffTurretCost = buffCost;
        _slowTurretCost = slowCost;
        _barrierWallCost = barrierCost;
        _upgradeCostMultiplier = upgradeMulti;
        _refundCostMultiplier = refundMulti;
    }


    public WallState CurrentState => _currentState;
    public TurretState FrontFaceState => _frontFace;
    public TurretState BackFaceState => _backFace;
    public int FrontLevel => _frontTurretLevel;
    public int Backlevel => _backTurretLevel;
    public event EventHandler<WallState> StateVisualsChanged;
    public event EventHandler<TurretState> TurretVisualsChanged;
    public event EventHandler<int> TurretUpgraded;

    private WallState _currentState = WallState.Empty;
    private TurretState _frontFace = TurretState.EmptyTurret;
    private TurretState _backFace = TurretState.EmptyTurret;
    private int _frontTurretLevel = 1;
    private int _backTurretLevel = 1;

    public enum WallState
    {
        Empty,
        Plain,
        Barrier
    }

    public enum TurretState
    {
        EmptyTurret,
        GunTurret,
        CannonTurret,
        PortalTurret,
        BuffTurret,
        SlowTurret,
        BarrierTurret
    }

    public bool GoToState(WallState newState, bool isFullRefund = false)
    {
        if (_currentState == newState) return true;

        bool isValidState = true;
        switch (newState)
        {
            case WallState.Empty:
                isValidState = SetEmptyWall(_currentState, isFullRefund);
                break;
            case WallState.Plain:
                isValidState = SetPlainWall(_currentState);
                break;
            case WallState.Barrier:
                isValidState = SetBarrierWall(_currentState);
                break;
        }

        if (!isValidState) return false;

        _currentState = newState;
        StateVisualsChanged.Invoke(this, _currentState);
        return true;
    }

    public bool IncrementTurretLevel(bool isFrontFace)
    {
        int currentLevel = isFrontFace ? _frontTurretLevel : _backTurretLevel;
        TurretState turret = isFrontFace ? _frontFace : _backFace;

        int maxLevel;
        switch (turret) {
            case TurretState.EmptyTurret:
            case TurretState.PortalTurret:
                maxLevel = 1;
                break;
            default:
                maxLevel = 3;
                break;
        }

        if (currentLevel >= maxLevel)
            return false;
        int price = GetTurretPrice(turret, currentLevel);
        if (!CurrencyManager.Instance.SubtractCurrency(price))
            return false;

        if (isFrontFace)
            _frontTurretLevel++;
        else
            _backTurretLevel++;

        TurretUpgraded.Invoke(isFrontFace, currentLevel + 1);
        return true;
    }

    public static int CostRefunded(TurretState state, int level, bool isFullRefund = false) {
        int baseCost = GetTurretPrice(state);
        float totalCost = 0;
        for (int i = 0; i < level; i++)
            totalCost += baseCost * Mathf.Pow(_upgradeCostMultiplier, i);
        return Mathf.RoundToInt(totalCost * (isFullRefund ? 1 : _refundCostMultiplier));
    }

    public void GoToTurretState(bool isFrontFace, TurretState newState, bool isFullRefund = false)
    {
        bool isValidState = true;
        switch (newState)
        {
            case TurretState.EmptyTurret:
                int costRefunded = isFrontFace ? CostRefunded(_frontFace, _frontTurretLevel, isFullRefund) : CostRefunded(_backFace, _backTurretLevel, isFullRefund);
                if (isFrontFace)
                    _frontTurretLevel = 1;
                else
                    _backTurretLevel = 1;
                CurrencyManager.Instance.AddCurrency(costRefunded);
                break;
            default:
                isValidState = CurrencyManager.Instance.SubtractCurrency(GetTurretPrice(newState));
                break;

        }

        if (!isValidState) return;


        if (isFrontFace)
        {
            if (_frontFace == newState) return;
            _frontFace = newState;
            TurretVisualsChanged.Invoke(true, newState);
        }
        else
        {
            if (_backFace == newState) return;
            _backFace = newState;
            TurretVisualsChanged.Invoke(false, newState);
        }
    }
    private bool SetBarrierWall(WallState previousState)
    {
        int amountToRefund = 0;
        if (_currentState == WallState.Plain)
        {
            amountToRefund = Mathf.RoundToInt(GetWallPrice(_currentState) * _refundCostMultiplier);
        }
        int cost = GetWallPrice(WallState.Barrier);
        return CurrencyManager.Instance.SubtractCurrency(cost - amountToRefund);
    }

    private bool SetPlainWall(WallState previousState)
    {
        //handle any non-visual elements (money down, etc.)
        int cost = GetWallPrice(WallState.Plain);
        return CurrencyManager.Instance.SubtractCurrency(cost);
    }

    private bool SetEmptyWall(WallState previousState, bool isFullRefund = false)
    {
        int costRefunded = Mathf.RoundToInt(GetWallPrice(previousState) * (isFullRefund ? 1 : _refundCostMultiplier));
        CurrencyManager.Instance.AddCurrency(costRefunded);
        return true;
        //handle any non-visual elements (money down, etc.)
    }

    public static int GetWallPrice(WallState wallState)
    {
        switch (wallState)
        {
            case WallState.Empty:
                return 0;
            case WallState.Plain:
                return _plainWallCost;
            case WallState.Barrier:
                return _barrierWallCost;
        }
        // not going to happen
        return -99999;
    }

    public static int GetTurretPrice(TurretState turretState, int currentLevel = 0)
    {
        float price = 0;

        switch (turretState)
        {
            case TurretState.EmptyTurret:
                price = 0; break;
            case TurretState.GunTurret:
                price = _gunTurretCost; break;
            case TurretState.CannonTurret:
                price = _cannonTurretCost; break;
            case TurretState.PortalTurret:
                price = _portalTurretCost; break;
            case TurretState.BuffTurret:
                price = _buffTurretCost; break;
            case TurretState.SlowTurret:
                price = _slowTurretCost; break;
            case TurretState.BarrierTurret:
                price = 0; break;
        }

        if (currentLevel > 0)
            price *= Mathf.Pow(_upgradeCostMultiplier, currentLevel);

        return Mathf.RoundToInt(price);
    }
}