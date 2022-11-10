using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallSegment : MonoBehaviour
{
    public Transform PlayerTransform { get; set; }

    [SerializeField] private Renderer _renderer;
    [SerializeField] private Canvas _canvas;

    [SerializeField] private Turret _frontTurret;
    [SerializeField] private Turret _backTurret;

    private Transform _player;

    private bool _isBeingHovered = false;
    private WallAutomata _automata = new WallAutomata();

    private bool _isFacingFrontFace => Vector3.Dot(transform.forward, PlayerTransform.forward) > 0;

    public Func<bool> tryCalculatePaths;
    public Action createNewPaths;

    private bool _isInteractable => _isBeingHovered && Vector3.Distance(transform.position, _player.position) < 20 && GameStateManager.Instance.GetCurrentGameState() == GameState.Building;


    private void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
    }

    private void OnEnable()
    {
        _automata.StateVisualsChanged += VisualsChanged;
        _automata.TurretVisualsChanged += TurretVisualsChanged;
        //_canvas.worldCamera = Camera.main;
        _canvas.enabled = false;
    }

    private void OnDisable()
    {
        _automata.StateVisualsChanged -= VisualsChanged;
        _automata.TurretVisualsChanged -= TurretVisualsChanged;
    }

    private void OnMouseEnter()
    {
        _isBeingHovered = true;
    }

    private void OnMouseExit()
    {
        _isBeingHovered = false;
        _renderer.material.color = Color.white;
        _canvas.enabled = false;
    }

    private void Update()
    {
        if (_isInteractable)
        {
            _renderer.material.color = Color.green;

            if (_automata.CurrentState == WallAutomata.WallState.Plain)
            {
                _canvas.enabled = true;

                if (Vector3.Dot(transform.forward, PlayerTransform.forward) > 0)
                {
                    _canvas.transform.localScale = new Vector3(1, 1, 10);
                    _canvas.transform.localPosition = new Vector3(0, 0, -0.51f);
                }
                else
                {
                    _canvas.transform.localScale = new Vector3(-1, 1, 10);
                    _canvas.transform.localPosition = new Vector3(0, 0, 0.51f);
                }
            }

            if (Input.GetMouseButtonDown(0) && _isInteractable)
            {
                if (WrenchMenu.Instance.Selected < 6)
                {
                    //create tower
                    _automata.GoToState(WallAutomata.WallState.Plain);
                    _automata.GoToTurretState(_isFacingFrontFace, (WallAutomata.TurretState)(WrenchMenu.Instance.Selected + 1));

                    // position the wall in place so the pathfinder algo can look with this new wall
                    // if all the paths are valid we can place the wall
                    // otherwise reset the wall to the empty state
                    if (tryCalculatePaths.Invoke())
                        createNewPaths.Invoke();
                    else
                        _automata.GoToState(WallAutomata.WallState.Empty);
                }
                else if (WrenchMenu.Instance.Selected == 6)
                {
                    //upgrading
                }
                else if (WrenchMenu.Instance.Selected == 7)
                {
                    //destroy
                    _automata.GoToState(WallAutomata.WallState.Empty);
                    createNewPaths.Invoke();
                }
            }
        }
        else { 
        _renderer.material.color = Color.white;
        }
    }

    private void VisualsChanged(object sender, WallAutomata.WallState state)
    {
        switch (state)
        {
            case WallAutomata.WallState.Empty:
                _canvas.enabled = false;
                transform.localScale = new Vector3(transform.localScale.x, 0.2f, transform.localScale.z);
                transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
                break;
            default:
                transform.localScale = new Vector3(transform.localScale.x, 10, transform.localScale.z);
                transform.localPosition = new Vector3(transform.localPosition.x, 5, transform.localPosition.z);
                break;
        }
    }

    public WallAutomata.WallState GetWallState()
    {
        return _automata.CurrentState;
    }
    public void SetEmptyWall()
    {
        _automata.GoToState(WallAutomata.WallState.Empty);
    }

    private void TurretVisualsChanged(object sender, WallAutomata.TurretState state)
    {
        bool isFrontFacing = !(bool)sender;

        if (isFrontFacing)
            _frontTurret.SetMode(state);
        else
            _backTurret.SetMode(state);
    }
}
