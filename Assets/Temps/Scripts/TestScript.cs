using System;
using System.ComponentModel.DataAnnotations.Schema;
using PracticalUtilities.CalculationExtensions;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TestScript : MonoBehaviour
{
    public int count = 10000;
    public Vector3Int[] positions;
    [FormerlySerializedAs("a")] public float fullValue;
    [FormerlySerializedAs("b")] public float separate;
    
    private void Start()
    {
        positions = new Vector3Int[count];
        for (int i = 0; i < count; i++)
        {
            positions[i] = Vector3Int.one;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CheckAddVectorByFullValue();
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            CheckAddVectorByPartialValue();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(separate < fullValue
                ? "Separate value is faster than full component."
                : "Full component is faster than separate value.");
            Debug.Log((fullValue - separate)/fullValue * 100);
        }
    }

    private void CheckAddVectorByFullValue()
    {
        float time = Time.realtimeSinceStartup;
        for (int i = 0; i < count; i++)
        {
            positions[i] += Vector3Int.one;
        }

        fullValue = Time.realtimeSinceStartup - time;
    }
    
    private void CheckAddVectorByPartialValue()
    {
        float time = Time.realtimeSinceStartup;
        for (int i = 0; i < count; i++)
        {
            positions[i] = positions[i].FasterAdd(1, 1, 1);
        }
        
        separate = Time.realtimeSinceStartup - time;
    }
}
