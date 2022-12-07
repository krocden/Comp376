using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WrenchMenu : MonoBehaviour
{
    public static WrenchMenu Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] private GameObject wrenchPanel;
    [SerializeField] private Image[] panels;
    [SerializeField] private TMPro.TMP_Text costText;
    [SerializeField] private int selected = 0;

    public int Selected => selected;
    public int PanelNumber => panels.Length;

    private void Start()
    {
        wrenchPanel.SetActive(GameStateManager.Instance.GetCurrentGameState() == GameState.Planning);
        GameStateManager.Instance.onGameStateChanged.AddListener(OnGameStateChanged);
    }
    private void OnDestroy()
    {
        GameStateManager.Instance.onGameStateChanged.RemoveListener(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameState state)
    {
        wrenchPanel.SetActive(state == GameState.Planning);
        costText.gameObject.SetActive(state == GameState.Planning);
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
            UpdateSelected(true);
        else if (Input.mouseScrollDelta.y < 0)
            UpdateSelected(false);

        for (int i = 0; i < panels.Length; i++)
            if (i == selected)
                panels[i].color = new Color(panels[i].color.r, panels[i].color.g, panels[i].color.b, 1);
            else
                panels[i].color = new Color(panels[i].color.r, panels[i].color.g, panels[i].color.b, 0.2f);
    }

    private void UpdateSelected(bool isPos)
    {
        if (isPos)
        {
            selected++;
            if (selected > panels.Length - 1) selected = 0;

            costText.text = (Selected < PanelNumber - 3) ? "Cost: " + (WallAutomata.GetTurretPrice((WallAutomata.TurretState)(WrenchMenu.Instance.Selected)) + WallAutomata.PlainWallCost).ToString() : string.Empty;
        }
        else {
            selected--;
            if (selected < 0) selected = panels.Length - 1;

            costText.text = (Selected < PanelNumber - 3) ? "Cost: " + (WallAutomata.GetTurretPrice((WallAutomata.TurretState)(WrenchMenu.Instance.Selected)) + WallAutomata.PlainWallCost).ToString() : string.Empty;
        }


        if (Selected < PanelNumber - 3)
        {
            costText.text = $"Cost: {WallAutomata.GetTurretPrice((WallAutomata.TurretState)(Selected)) + WallAutomata.PlainWallCost}$";
        }
        else if (Selected == PanelNumber - 3)
            costText.text = $"Cost: {WallAutomata.GetWallPrice(WallAutomata.WallState.Barrier)}$";
        else
            costText.text = string.Empty;
    }
}
