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

    [Header("Turret Costs")]
    [SerializeField] private int plainWallCost = 20;
    [SerializeField] private int gunTurretCost = 30;
    [SerializeField] private int cannonTurretCost = 40;
    [SerializeField] private int portalTurretCost = 180;
    [SerializeField] private int buffTurretCost = 20;
    [SerializeField] private int slowTurretCost = 40;
    [SerializeField] private int barrierWallCost = 80;
    [SerializeField] private float upgradeCostMultiplier = 1.25f;
    [SerializeField] private float refundCostMultiplier = 0.5f;


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
        transform.localScale = new Vector3(10, 0.2f, 0.1f);
        
        _automata.SetCosts(
        plainWallCost,
        gunTurretCost,
        cannonTurretCost,
        portalTurretCost,
        buffTurretCost,
        slowTurretCost,
        barrierWallCost,
        upgradeCostMultiplier,
        refundCostMultiplier);
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

    private void OnGameStateChanged(GameState state)
    {
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

                    if (WrenchMenu.Instance.Selected == WrenchMenu.Instance.PanelNumber - 2)
                    {
                        string upgradeText = Turret.GetUpgradeText(_automata.FrontFaceState, _automata.FrontLevel);
                        string turretPrice = WallAutomata.GetTurretPrice(_automata.FrontFaceState, _automata.FrontLevel).ToString();

                        _text.text += "\n<color=\"green\">" + upgradeText;
                        _text.text += (upgradeText != "Max Level") ? "\n<color=\"red\">Cost: " + turretPrice + "$" : string.Empty;
                    }
                    else if (WrenchMenu.Instance.Selected == WrenchMenu.Instance.PanelNumber - 1)
                    {
                        _text.text += $"\n<color=\"red\">Refund: {WallAutomata.CostRefunded(_automata.FrontFaceState, _automata.FrontLevel, accountForWall: true)}$";
                    }
                    _canvas.transform.localScale = new Vector3(1, 1, 10);
                    _canvas.transform.localPosition = new Vector3(0, -0.2f, -0.51f);
                }
                else
                {
                    _text.text = Turret.GetTurretText(_automata.BackFaceState, _automata.Backlevel);

                    if (WrenchMenu.Instance.Selected == WrenchMenu.Instance.PanelNumber - 2)
                    {
                        string upgradeText = Turret.GetUpgradeText(_automata.FrontFaceState, _automata.FrontLevel);
                        string turretPrice = WallAutomata.GetTurretPrice(_automata.FrontFaceState, _automata.FrontLevel).ToString();

                        _text.text += "\n<color=\"green\">" + upgradeText;
                        _text.text += (upgradeText != "Max Level") ? "\n<color=\"red\">Cost: " + turretPrice + "$" : string.Empty;
                    }
                    else if (WrenchMenu.Instance.Selected == WrenchMenu.Instance.PanelNumber - 1)
                    {
                        _text.text += $"\n<color=\"red\">Refund: {WallAutomata.CostRefunded(_automata.BackFaceState, _automata.Backlevel, accountForWall: true)}$";
                    }
                    _canvas.transform.localScale = new Vector3(-1, 1, 10);
                    _canvas.transform.localPosition = new Vector3(0, -0.2f, 0.51f);
                }
            }

            if (Input.GetMouseButtonDown(0) && _isInteractable)
            {
                if (WrenchMenu.Instance.Selected < WrenchMenu.Instance.PanelNumber - 3)
                {
                    WallAutomata.WallState previousWallState = GetWallState();
                    WallAutomata.TurretState previousTurretState = GetTurretState(_isFacingFrontFace);
                    if (previousWallState == WallAutomata.WallState.Barrier) return;

                    //create tower
                    bool validWall = _automata.GoToState(WallAutomata.WallState.Plain);

                    bool isTurretAlreadyPlaced = _isFacingFrontFace
                        ? _automata.FrontFaceState != WallAutomata.TurretState.EmptyTurret
                        : _automata.BackFaceState != WallAutomata.TurretState.EmptyTurret;

                    if (WrenchMenu.Instance.Selected > 0 && !isTurretAlreadyPlaced && validWall)                    
                        _automata.GoToTurretState(_isFacingFrontFace, (WallAutomata.TurretState)(WrenchMenu.Instance.Selected));

                    VerifyAfterWallBuilt(previousWallState, previousTurretState);
                }
                else if (WrenchMenu.Instance.Selected == WrenchMenu.Instance.PanelNumber - 3)
                {
                    //barrier turret
                    WallAutomata.WallState previousWallState = GetWallState();
                    WallAutomata.TurretState previousTurretState = GetTurretState(_isFacingFrontFace);

                    bool isTurretAlreadyPlaced = _automata.FrontFaceState != WallAutomata.TurretState.EmptyTurret || _automata.BackFaceState != WallAutomata.TurretState.EmptyTurret;

                    if (isTurretAlreadyPlaced) return;

                    _automata.GoToState(WallAutomata.WallState.Barrier);
                    _automata.GoToTurretState(true, WallAutomata.TurretState.BarrierTurret);
                    _automata.GoToTurretState(false, WallAutomata.TurretState.BarrierTurret);

                    VerifyAfterWallBuilt(previousWallState, previousTurretState);
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
        else
        {
            _renderer.sharedMaterial.color = GetWallState() == WallAutomata.WallState.Barrier ? new Color(1, 1, 1, 0.5f) : Color.white;
        }
    }

    private void VerifyAfterWallBuilt(WallAutomata.WallState previousState, WallAutomata.TurretState previousTurretState)
    {
        if (previousState == WallAutomata.WallState.Empty)
        {
            if (tryCalculatePaths.Invoke())
            {
                createNewPaths.Invoke();
            }
            else
            {
                _automata.GoToState(previousState, isFullRefund: true);
                _automata.GoToTurretState(_isFacingFrontFace, previousTurretState, isFullRefund: true);
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

    public WallAutomata.TurretState GetTurretState(bool isFrontFacing)
    {
        return isFrontFacing ? _automata.FrontFaceState : _automata.BackFaceState;
    }

    public void SetEmptyWall()
    {
        _automata.GoToState(WallAutomata.WallState.Empty);
        _automata.GoToTurretState(true, WallAutomata.TurretState.EmptyTurret);
        _automata.GoToTurretState(false, WallAutomata.TurretState.EmptyTurret);
    }

    public void RemoveAnyPortals()
    {
        if (_automata.FrontFaceState == WallAutomata.TurretState.PortalTurret)
            _automata.GoToTurretState(true, WallAutomata.TurretState.EmptyTurret);
        else if (_automata.BackFaceState == WallAutomata.TurretState.PortalTurret)
            _automata.GoToTurretState(false, WallAutomata.TurretState.EmptyTurret);
    }

    private void TurretVisualsChanged(object sender, WallAutomata.TurretState state)
    {
        bool isFrontFacing = !(bool)sender;

        if (isFrontFacing)
        {
            _frontTurret.SetMode(state);
            _frontTurret.SetLevel(1);
        }
        else
        {
            _backTurret.SetMode(state);
            _backTurret.SetLevel(1);
        }
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
