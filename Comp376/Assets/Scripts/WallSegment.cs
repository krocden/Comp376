using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallSegment : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    private bool _isBeingHovered = false;
    private WallAutomata _automata = new WallAutomata();

    public Func<bool> tryCalculatePaths;
    public Action createNewPaths;

    private void OnEnable()
    {
        _automata.StateVisualsChanged += VisualsChanged;
    }

    private void OnDisable()
    {
        _automata.StateVisualsChanged -= VisualsChanged;
    }

    private void OnMouseEnter()
    {
        _isBeingHovered = true;
        _renderer.material.color = Color.green;
    }

    private void OnMouseExit()
    {
        _isBeingHovered = false;
        _renderer.material.color = Color.white;
    }

    private void Update()
    {
        if (_isBeingHovered && Input.GetKeyDown(KeyCode.E))
        {
            if (_automata.CurrentState == WallAutomata.WallState.Empty)
            {
                // position the wall in place so the pathfinder algo can look with this new wall
                _automata.GoToState(WallAutomata.WallState.Plain);


                // if all the paths are valid we can place the wall
                // otherwise reset the wall to the empty state
                if (tryCalculatePaths.Invoke())
                    createNewPaths.Invoke();
                else
                    _automata.GoToState(WallAutomata.WallState.Empty);
            }
            else
            {
                // update the paths when the wall is removed
                _automata.GoToState(WallAutomata.WallState.Empty);
                createNewPaths.Invoke();
            }
        }
    }

    private void VisualsChanged(object sender, WallAutomata.WallState state)
    {
        switch (state)
        {
            case WallAutomata.WallState.Plain:
                transform.localScale = new Vector3(transform.localScale.x, 10, transform.localScale.z);
                transform.localPosition = new Vector3(transform.localPosition.x, 5, transform.localPosition.z);
                break;
            case WallAutomata.WallState.Empty:
                transform.localScale = new Vector3(transform.localScale.x, 0.2f, transform.localScale.z);
                transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
                break;

        }
    }

    public WallAutomata.WallState GetWallState()
    {
        return _automata.CurrentState;
    }
}
