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
    [SerializeField] private TMPro.TMP_Text _text;

    [SerializeField] private Turret _frontTurret;
    [SerializeField] private Turret _backTurret;

    [SerializeField] private GameObject _minimapVisual;
    [SerializeField] private Material[] _wallMaterials;

    private Transform _player;

    private bool _isBeingHovered = false;
    private WallAutomata _automata = new WallAutomata();

    private bool _isFacingFrontFace => Vector3.Dot(transform.forward, PlayerTransform.forward) > 0;

    public Func<bool> tryCalculatePaths;
    public Action createNewPaths;
    private BoxCollider col;

    private bool _isInteractable => _isBeingHovered && Vector3.Distance(transform.position, _player.position) < 20 && GameStateManager.Instance.GetCurrentGameState() == GameState.Planning;


    private void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        col = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        _automata.StateVisualsChanged += VisualsChanged;
        _automata.TurretVisualsChanged += TurretVisualsChanged;
        _automata.TurretUpgraded += TurretUpgraded;
        GameStateManager.Instance.onGameStateChanged.AddListener(OnGameStateChanged);
        //_canvas.worldCamera = Camera.main;
        _canvas.enabled = false;
    }

    private void OnDisable()
    {
        _automata.StateVisualsChanged -= VisualsChanged;
        _automata.TurretVisualsChanged -= TurretVisualsChanged;
        _automata.TurretUpgraded -= TurretUpgraded;
        GameStateManager.Instance.onGameStateChanged.RemoveListener(OnGameStateChanged);
    }

    private void OnMouseEnter()
    {
        _isBeingHovered = true;
    }

    private void OnMouseExit()
    {
        _isBeingHovered = false;
        _renderer.sharedMaterial.color = GetWallState() == WallAutomata.WallState.Barrier ? new Color(1, 1, 1, 0.5f) : Color.white;
        _canvas.enabled = false;
    }

    private void OnGameStateChanged(GameState state) {
        bool isShootingAndEmpty = (state == GameState.Shooting && _automata.CurrentState == WallAutomata.WallState.Empty);

        col.enabled = !isShootingAndEmpty;
        _renderer.enabled = !isShootingAndEmpty;
    }

    private void Update()
    {
        if (_isInteractable)
        {
            _renderer.sharedMaterial.color = GetWallState() == WallAutomata.WallState.Barrier ? new Color(0, 1, 0, 0.5f) : Color.green;

            if (_automata.CurrentState == WallAutomata.WallState.Plain)
            {
                _canvas.enabled = true;

                if (Vector3.Dot(transform.forward, PlayerTransform.forward) > 0)
                {
                    _text.text = Turret.GetTurretText(_automata.FrontFaceState, _automata.FrontLevel);
                    _canvas.transform.localScale = new Vector3(1, 1, 10);
                    _canvas.transform.localPosition = new Vector3(0, 0, -0.51f);
                }
                else
                {
                    _text.text = Turret.GetTurretText(_automata.BackFaceState, _automata.Backlevel);
                    _canvas.transform.localScale = new Vector3(-1, 1, 10);
                    _canvas.transform.localPosition = new Vector3(0, 0, 0.51f);
                }
            }

            if (Input.GetMouseButtonDown(0) && _isInteractable)
            {
                if (WrenchMenu.Instance.Selected < WrenchMenu.Instance.PanelNumber - 3)
                {
                    WallAutomata.WallState previousWallState = GetWallState();
                    if (previousWallState == WallAutomata.WallState.Barrier) return;

                    //create tower
                    _automata.GoToState(WallAutomata.WallState.Plain);

                    bool isTurretAlreadyPlaced = _isFacingFrontFace ? _automata.FrontFaceState != WallAutomata.TurretState.EmptyTurret : _automata.BackFaceState != WallAutomata.TurretState.EmptyTurret;
                    if (WrenchMenu.Instance.Selected > 0 && !isTurretAlreadyPlaced)
                        _automata.GoToTurretState(_isFacingFrontFace, (WallAutomata.TurretState)(WrenchMenu.Instance.Selected));

                    // position the wall in place so the pathfinder algo can look with this new wall
                    // if all the paths are valid we can place the wall
                    // otherwise reset the wall to the empty state

                    VerifyAfterWallBuilt(previousWallState);
                }
                else if (WrenchMenu.Instance.Selected == WrenchMenu.Instance.PanelNumber - 3)
                {
                    //barrier turret
                    WallAutomata.WallState previousWallState = GetWallState();

                    _automata.GoToState(WallAutomata.WallState.Barrier);
                    _automata.GoToTurretState(true, WallAutomata.TurretState.BarrierTurret);
                    _automata.GoToTurretState(false, WallAutomata.TurretState.BarrierTurret);

                    VerifyAfterWallBuilt(previousWallState);
                }
                else if (WrenchMenu.Instance.Selected == WrenchMenu.Instance.PanelNumber - 2)
                {
                    //upgrading
                    _automata.IncrementTurretLevel(_isFacingFrontFace);
                }
                else if (WrenchMenu.Instance.Selected == WrenchMenu.Instance.PanelNumber - 1)
                {
                    //destroy
                    _automata.GoToState(WallAutomata.WallState.Empty);
                    _automata.GoToTurretState(true, WallAutomata.TurretState.EmptyTurret);
                    _automata.GoToTurretState(false, WallAutomata.TurretState.EmptyTurret);
                    createNewPaths.Invoke();
                }
            }
        }
        else {
            _renderer.sharedMaterial.color = GetWallState() == WallAutomata.WallState.Barrier ? new Color(1, 1, 1, 0.5f) : Color.white;
        }
    }

    private void VerifyAfterWallBuilt(WallAutomata.WallState previousState) {
        if (previousState == WallAutomata.WallState.Empty)
        {
            if (tryCalculatePaths.Invoke())
            {
                createNewPaths.Invoke();
            }
            else
            {
                _automata.GoToState(WallAutomata.WallState.Empty);
                _automata.GoToTurretState(_isFacingFrontFace, WallAutomata.TurretState.EmptyTurret);
            }
        }
    }

    private void VisualsChanged(object sender, WallAutomata.WallState state)
    {
        Collider playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();

        switch (state)
        {
            case WallAutomata.WallState.Empty:
                _canvas.enabled = false;
                transform.localScale = new Vector3(transform.localScale.x, 0.2f, transform.localScale.z);
                transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
                _renderer.sharedMaterial = _wallMaterials[0];
                _renderer.sharedMaterial.color = new Color(1, 1, 1, 1);
                GetComponent<Collider>().isTrigger = false;
                _minimapVisual.SetActive(false);
                break;
            case WallAutomata.WallState.Barrier:
                _canvas.enabled = false;
                transform.localScale = new Vector3(transform.localScale.x, 10, transform.localScale.z);
                transform.localPosition = new Vector3(transform.localPosition.x, 5, transform.localPosition.z);
                _renderer.sharedMaterial = _wallMaterials[1];
                _renderer.sharedMaterial.color = new Color(1, 1, 1, 0.5f);
                GetComponent<Collider>().isTrigger = true;
                _minimapVisual.SetActive(true);
                break;
            default:
                transform.localScale = new Vector3(transform.localScale.x, 10, transform.localScale.z);
                transform.localPosition = new Vector3(transform.localPosition.x, 5, transform.localPosition.z);
                _renderer.sharedMaterial = _wallMaterials[0];
                _renderer.sharedMaterial.color = new Color(1, 1, 1, 1f);
                GetComponent<Collider>().isTrigger = false;
                _minimapVisual.SetActive(true);
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
        _automata.GoToTurretState(true, WallAutomata.TurretState.EmptyTurret);
        _automata.GoToTurretState(false, WallAutomata.TurretState.EmptyTurret);
    }

    private void TurretVisualsChanged(object sender, WallAutomata.TurretState state)
    {
        bool isFrontFacing = !(bool)sender;

        if (isFrontFacing)
            _frontTurret.SetMode(state);
        else
            _backTurret.SetMode(state);
    }

    private void TurretUpgraded(object sender, int lvl)
    {
        bool isFrontFacing = !(bool)sender;

        if (isFrontFacing)
            _frontTurret.SetLevel(lvl);
        else
            _backTurret.SetLevel(lvl);
    }
}
