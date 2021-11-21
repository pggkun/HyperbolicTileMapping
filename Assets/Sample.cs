using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hyperbolic.Math;
using Hyperbolic.Tile;

public class Sample : MonoBehaviour
{
    [SerializeField] ChunkObject chunk;
    // Start is called before the first frame update
    void Start()
    {
        chunk.Initialize();
        //Vector3 a = Math.HypotenuseOfFundamentalRegionEndPoint(4,5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
