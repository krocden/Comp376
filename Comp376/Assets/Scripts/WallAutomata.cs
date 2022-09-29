using System;

public class WallAutomata
{
    public WallState CurrentState => _currentState;
    public event EventHandler<WallState> StateVisualsChanged;

    private WallState _currentState = WallState.Empty;

    public enum WallState
    {
        Empty,
        Plain,
        Turret,
        Portal
    }

    public void GoToState(WallState newState)
    {
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

    private void SetPlainWall(WallState previousState)
    {
        //handle any non-visual elements (money down, etc.)
    }

    private void SetEmptyWall(WallState previousState)
    {
        //handle any non-visual elements (money down, etc.)
    }
}
