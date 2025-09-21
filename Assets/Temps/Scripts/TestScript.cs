using System;
using PracticalUtilities.CalculationExtensions;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public int[] indexex;
    
    private void Start()
    {
        SpriteRenderer obj;
        transform.TryGetChildComponent(out obj, indexex);
        Debug.Log(obj.name);
    }
}
