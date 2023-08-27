using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapCheckTriggers : MonoBehaviour
{
    private Shape shape;
    private int touchCounter = 0;

    private void Awake()
    {
        shape = GetComponentInParent<Shape>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (!shape.beingPlaced)
            return;

        if (other.CompareTag("Shape"))
        {
            touchCounter++;
            shape.insideOverlap = true;
        }        
    }
   
    private void OnTriggerExit(Collider other)
    {
        
        if (!shape.beingPlaced)
            return;

        if (other.CompareTag("Shape"))
        {
            touchCounter--;

            if (touchCounter == 0)
            {
                shape.insideOverlap = false;
            }
        }
        
    }
    
}
