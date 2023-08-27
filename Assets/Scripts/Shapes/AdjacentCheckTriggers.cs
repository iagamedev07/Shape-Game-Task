using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// OBSOLETE : Earlier approach for using triggers instead of OverlapBox
/// </summary>
public class AdjacentCheckTriggers : MonoBehaviour
{
    private Shape shape;
    private int touchCounter = 0;

    private void Awake()
    {
        shape = GetComponentInParent<Shape>();
    }

    public void ResetPreferences()
    {
        touchCounter = 0;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shape"))
        {
            Debug.Log("Inside Adjacent");
            touchCounter++;
            shape.insideAdjacent = true;
        }
    }

    

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shape"))
        {            
            touchCounter--;

            if (touchCounter == 0)
            {
                shape.insideAdjacent = false;
                Debug.Log("Outside Adjacent");
            }
        }
       
    }
    

    /// <summary>
    /// Obsolete! Used to check with one box collider but fails in case of complex shapes
    /// </summary>
    /// <param name="normal"></param>
    /// <returns></returns>
    private bool CheckForCornerAdjecency(Vector3 normal)
    {
        const float epsilon = 1e-6f;
        int nonZeroCount = (Mathf.Abs(normal.x) > epsilon ? 1 : 0) + (Mathf.Abs(normal.y) > epsilon ? 1 : 0) + (Mathf.Abs(normal.z) > epsilon ? 1 : 0);
        return nonZeroCount >= 2;
    }

}
