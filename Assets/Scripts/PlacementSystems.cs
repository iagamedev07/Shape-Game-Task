using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Pattern Design for Placement System
/// </summary>
public interface IPlacementState
{
    void Update(GridPlacement gridPlacement);
    void SelectObject(GridPlacement gridPlacement);
    void TogglePlacement(GridPlacement gridPlacement);
    void PlaceObject(GridPlacement gridPlacement);
    void UpdateLevel(GridPlacement gridPlacement, float inputVal);
}

/// <summary>
/// The Idle State, i.e Not the Placing State
/// </summary>
public class IdleState : IPlacementState
{
    public static IdleState Instance { get; } = new IdleState();
    public void Update(GridPlacement gridPlacement) 
    {
        gridPlacement.UpdateSelectionStatus();
    }
    public void SelectObject(GridPlacement gridPlacement) 
    {
        //Go To Placement State
        gridPlacement.SelectShapeInRange();
    }
    public void TogglePlacement(GridPlacement gridPlacement)
    {
        gridPlacement.SetState(PlacingState.Instance);

        gridPlacement.TogglePlacementMode(true);

    }
    public void PlaceObject(GridPlacement gridPlacement) { }
    public void UpdateLevel(GridPlacement gridPlacement, float inputVal) { }
}

/// <summary>
/// The Placement Object State
/// </summary>
public class PlacingState : IPlacementState
{
    public static PlacingState Instance { get; } = new PlacingState();
    public void Update(GridPlacement gridPlacement)
    {
        //Update Position of Selected Object
        gridPlacement.UpdatePlacementPosition();
    }
    public void SelectObject(GridPlacement gridPlacement) { }  
    public void TogglePlacement(GridPlacement gridPlacement)
    {
        gridPlacement.SetState(IdleState.Instance);

        gridPlacement.TogglePlacementMode(false);

    }
    public void PlaceObject(GridPlacement gridPlacement)
    {
        gridPlacement.SetState(IdleState.Instance);
        
        //Place Object
        gridPlacement.PlaceObject();
    }
    public void UpdateLevel(GridPlacement gridPlacement, float inputVal) 
    {
        gridPlacement.ChangeGridLevel(inputVal);
    }
}
