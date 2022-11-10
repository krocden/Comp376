using System;

public class WallAutomata
{
    public WallState CurrentState => _currentState;
    public TurretState FrontFaceState => _frontFace;
    public TurretState BackFaceState => _backFace;
    public event EventHandler<WallState> StateVisualsChanged;
    public event EventHandler<TurretState> TurretVisualsChanged;

    private WallState _currentState = WallState.Empty;
    private TurretState _frontFace = TurretState.EmptyTurret;
    private TurretState _backFace = TurretState.EmptyTurret;

    public enum WallState
    {
        Empty,
        Plain
    }

    public enum TurretState
    {
        EmptyTurret,
        GunTurret,
        CannonTurret,
        PortalTurret,
        BuffTurret,
        WallTurret,
        SlowTurret
    }

    public void GoToState(WallState newState)
    {
        if (_currentState == newState) return;

        switch (newState)
        {
            case WallState.Empty:
                SetEmptyWall(_currentState);
                break;
            case WallState.Plain:
                SetPlainWall(_currentState);
                break;
        }
        _currentState = newState;
        StateVisualsChanged.Invoke(this, _currentState);
    }

    public void GoToTurretState(bool isFrontFace, TurretState newState)
    {
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

    private void SetPlainWall(WallState previousState)
    {
        //handle any non-visual elements (money down, etc.)
    }

    private void SetEmptyWall(WallState previousState)
    {
        //handle any non-visual elements (money down, etc.)
    }
}
