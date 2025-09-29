using System;
using System.ComponentModel.DataAnnotations.Schema;
using PracticalUtilities.CalculationExtensions;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TestScript : MonoBehaviour
{
    public int count = 10000;
    public Vector3[] positions;
    [FormerlySerializedAs("a")] public float fullValue;
    [FormerlySerializedAs("b")] public float separate;
    
    private void Start()
    {
        positions = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            positions[i] = Random.insideUnitSphere;
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
            positions[i] += Vector3.one;
        }

        fullValue = Time.realtimeSinceStartup - time;
    }
    
    private void CheckAddVectorByPartialValue()
    {
        float time = Time.realtimeSinceStartup;
        for (int i = 0; i < count; i++)
        {
            positions[i] = positions[i].FasterAdd(Vector3.one);
        }
        
        separate = Time.realtimeSinceStartup - time;
    }
}
