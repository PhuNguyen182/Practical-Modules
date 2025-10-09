using System;
using System.ComponentModel.DataAnnotations.Schema;
using PracticalUtilities.CalculationExtensions;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using ZLinq;

public class TestScript : MonoBehaviour
{
    public int count = 10000;
    public Vector3Int[] positions;
    [FormerlySerializedAs("a")] public float fullValue;
    [FormerlySerializedAs("b")] public float separate;
    public float getComponentTime;
    public float tryGetComponentTime;
    
    private void Start()
    {
        positions = new Vector3Int[count];
        var x = positions.AsValueEnumerable().Select(x => x.x);
        for (int i = 0; i < count; i++)
        {
            positions[i] = Vector3Int.one;
        }

        if (gameObject != null)
        {
            
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TestGetComponent();
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            TestTryGetComponent();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(getComponentTime < tryGetComponentTime
                ? "getComponentTime value is faster."
                : "tryGetComponentTime component is faster.");
            Debug.Log(tryGetComponentTime / getComponentTime >= 1
                ? tryGetComponentTime / getComponentTime
                : 1 / tryGetComponentTime / getComponentTime);
        }
    }
    
    private void TestGetComponent()
    {
        float time = Time.realtimeSinceStartup;
        for (int i = 0; i < count; i++)
        {
            GetComponent<MessageBrokerExample>();
        }
        getComponentTime = Time.realtimeSinceStartup - time;
    }

    private void TestTryGetComponent()
    {
        float time = Time.realtimeSinceStartup;
        for (int i = 0; i < count; i++)
        {
            TryGetComponent<MessageBrokerExample>(out var x);
        }
        tryGetComponentTime = Time.realtimeSinceStartup - time;
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
