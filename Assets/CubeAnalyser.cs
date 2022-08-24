using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAnalyser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var mesh = GetComponent<MeshFilter>().mesh;

        mesh.Optimize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
