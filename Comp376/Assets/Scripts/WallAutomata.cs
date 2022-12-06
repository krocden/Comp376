using System;
using UnityEngine;

public class WallAutomata
{
    private int _plainWallCost;
    private int _gunTurretCost;
    private int _cannonTurretCost;
    private int _portalTurretCost;
    private int _buffTurretCost;
    private int _slowTurretCost;
    private int _barrierWallCost;
    private float _upgradeCostMultiplier;
    private float _refundCostMultiplier;

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

    public bool GoToState(WallState newState)
    {
        if (_currentState == newState) return true;

        bool isValidState = true;
        switch (newState)
        {
            case WallState.Empty:
                isValidState = SetEmptyWall(_currentState);
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
        int price = Mathf.RoundToInt(GetTurretPrice(turret) * Mathf.Pow(_upgradeCostMultiplier, currentLevel));
        if (!CurrencyManager.Instance.SubtractCurrency(price))
            return false;

        if (isFrontFace)
            _frontTurretLevel++;
        else
            _backTurretLevel++;

        TurretUpgraded.Invoke(isFrontFace, currentLevel + 1);
        return true;
    }

    public void GoToTurretState(bool isFrontFace, TurretState newState)
    {
        bool isValidState = true;
        int cost = GetTurretPrice(newState);
        switch (newState)
        {
            case TurretState.EmptyTurret:
                int level = isFrontFace ? _frontTurretLevel : _backTurretLevel;
                int baseCost = isFrontFace ? GetTurretPrice(_frontFace) : GetTurretPrice(_backFace);
                float totalCost = 0;
                for (int i = 0; i < level; i++)
                    totalCost += baseCost * Mathf.Pow(_upgradeCostMultiplier, i);
                int costRefunded = Mathf.RoundToInt(totalCost * _refundCostMultiplier);
                if (isFrontFace)
                    _frontTurretLevel = 1;
                else
                    _backTurretLevel = 1;
                CurrencyManager.Instance.AddCurrency(costRefunded);
                break;
            default:
                isValidState = CurrencyManager.Instance.SubtractCurrency(cost);
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

    private bool SetEmptyWall(WallState previousState)
    {
        int costRefunded = Mathf.RoundToInt(GetWallPrice(previousState) * _refundCostMultiplier);
        CurrencyManager.Instance.AddCurrency(costRefunded);
        return true;
        //handle any non-visual elements (money down, etc.)
    }

    private int GetWallPrice(WallState wallState)
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

    private int GetTurretPrice(TurretState turretState)
    {
        switch (turretState)
        {
            case TurretState.EmptyTurret:
                return 0;
            case TurretState.GunTurret:
                return _gunTurretCost;
            case TurretState.CannonTurret:
                return _cannonTurretCost;
            case TurretState.PortalTurret:
                return _portalTurretCost;
            case TurretState.BuffTurret:
                return _buffTurretCost;
            case TurretState.SlowTurret:
                return _slowTurretCost;
            case TurretState.BarrierTurret:
                return 0;
        }
        // not going to happen
        return -99999;
    }
}
