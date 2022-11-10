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
    [SerializeField] private int selected = 0;

    public int Selected => selected;

    private void OnEnable()
    {
        GameStateManager.Instance.onGameStateChanged.AddListener(OnGameStateChanged);
    }
    private void OnDisable()
    {
        GameStateManager.Instance.onGameStateChanged.RemoveListener(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameState state)
    {
        wrenchPanel.SetActive(state == GameState.Building);
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
            selected = Mathf.Clamp(selected + 1, 0, panels.Length - 1);
        else if (Input.mouseScrollDelta.y < 0)
            selected = Mathf.Clamp(selected - 1, 0, panels.Length - 1);

        for (int i = 0; i < panels.Length; i++)
            if (i == selected)
                panels[i].color = new Color(panels[i].color.r, panels[i].color.g, panels[i].color.b, 1);
            else
                panels[i].color = new Color(panels[i].color.r, panels[i].color.g, panels[i].color.b, 0.2f);
    }
}
