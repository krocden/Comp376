using System;

public class WallAutomata
{
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
        if (_currentState == newState) return false;

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

        if (!CurrencyManager.Instance.SubtractCurrency(5))
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
        switch (newState)
        {
            case TurretState.GunTurret:
                isValidState = CurrencyManager.Instance.SubtractCurrency(5);
                break;
            case TurretState.CannonTurret:
                isValidState = CurrencyManager.Instance.SubtractCurrency(10);
                break;
            case TurretState.BuffTurret:
                isValidState = CurrencyManager.Instance.SubtractCurrency(5);
                break;
            case TurretState.SlowTurret:
                isValidState = CurrencyManager.Instance.SubtractCurrency(5);
                break;
            case TurretState.EmptyTurret:
                CurrencyManager.Instance.AddCurrency(10);
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
        return CurrencyManager.Instance.SubtractCurrency(20);
    }

    private bool SetPlainWall(WallState previousState)
    {
        //handle any non-visual elements (money down, etc.)
        return CurrencyManager.Instance.SubtractCurrency(20);
    }

    private bool SetEmptyWall(WallState previousState)
    {
        CurrencyManager.Instance.AddCurrency(10);
        return true;
        //handle any non-visual elements (money down, etc.)
    }
}
