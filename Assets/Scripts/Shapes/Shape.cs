using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Shape : MonoBehaviour
{
    [field : SerializeField] public Renderer Rend { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }

    [field : SerializeField] public bool isComplex { get; private set; }

    [HideInInspector] public bool canBePlaced = false;
    [HideInInspector] public bool insideOverlap = false;
    [HideInInspector] public bool insideAdjacent = false;
    [HideInInspector] public bool beingPlaced = false;

    //Materials
    private Material validPlaceMaterial;
    private Material invalidPlaceMaterial;
    private Material defaultMaterial;

    //Triggers
    [SerializeField] private OverlapCheckTriggers overlapTriggers;

    [System.Serializable]
    public class BoxcastData
    {
        public Vector3 center;
        public Vector3 size;
    }
    [SerializeField] private BoxcastData[] adjacentBoxcasts;
    [SerializeField] private LayerMask shapeLayerMask;

    public bool debugAdjacentTriggers;


    private void Awake()
    {
        defaultMaterial = Rend.sharedMaterial;
        validPlaceMaterial = Resources.Load("Mat_ValidPlacement", typeof(Material)) as Material;
        invalidPlaceMaterial = Resources.Load("Mat_InvalidPlacement", typeof(Material)) as Material;
    }

    private void Update()
    {
        if(beingPlaced)
        {
            RefreshAdjacency();
            UpdatePlacementStatus();
            UpdateMaterial();
        }
    }

   
    /// <summary>
    /// Toggling the placement of the shape
    /// </summary>
    /// <param name="val"></param>
    public void TogglePlacementMode(bool val)
    {
        beingPlaced = val;        
        beingPlaced = val;

        if(val)
        {
            overlapTriggers.gameObject.layer = LayerMask.NameToLayer("Selected");
        }else
        {
            overlapTriggers.gameObject.layer = LayerMask.NameToLayer("Object");
        }

        UpdateMaterial();
    }

    /// <summary>
    /// Function to call whenever to check whether adjacent shape lies.
    /// </summary>
    public void RefreshAdjacency()
    {      
        if (beingPlaced)
        {
            insideAdjacent = isShapeAdjacent();
        }
    }

    /// <summary>
    /// Used for updating whether the shape place is valid or not
    /// </summary>
    public void UpdatePlacementStatus()
    {
        if (insideAdjacent && !insideOverlap)
            canBePlaced = true;
        else
            canBePlaced = false;
    }

    /// <summary>
    /// Update the Material in Placement Mode, Green and Red
    /// </summary>
    public void UpdateMaterial()
    {
        if(!beingPlaced)
        {
            Rend.sharedMaterial = defaultMaterial;
            return;
        }

        if(canBePlaced)
            Rend.sharedMaterial = validPlaceMaterial;
        else
            Rend.sharedMaterial = invalidPlaceMaterial;
    }

    /// <summary>
    /// Check if shape shares surface with the adjacent shapes 
    /// </summary>
    /// <returns></returns>
    private bool isShapeAdjacent()
    {
        foreach (BoxcastData boxcast in adjacentBoxcasts)
        {
            Collider[] collidersInside = Physics.OverlapBox(boxcast.center + transform.position, boxcast.size * 0.5f, Quaternion.identity, shapeLayerMask);
            if (collidersInside.Length> 0)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Debugging Overlap Boxes for adjacent checks
    /// </summary>
    void OnDrawGizmos()
    {
        if (!debugAdjacentTriggers)
            return;

        Gizmos.color = Color.red;
        foreach (BoxcastData boxcast in adjacentBoxcasts)
        {
            Gizmos.DrawWireCube(boxcast.center + transform.position, boxcast.size);
        }
            
    }

}


