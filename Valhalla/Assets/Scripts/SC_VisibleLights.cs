using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SC_VisibleLights : MonoBehaviour
{
    public Light[] LightSource { get { return GetComponentsInChildren<Light>(); } }

    private void Start()
    {
        //MeshFilter meshFilt = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        //MeshRenderer meshRend = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        //meshFilt.mesh = SC_LightManager.single.sphereMesh;

        foreach (Light light in LightSource)
        {
            light.enabled = false;
        }
    }
    

    private void OnBecameVisible()
    {
        foreach (Light light in LightSource)
        {
            light.enabled = true;
        }
    }

    private void OnBecameInvisible()
    {
        foreach (Light light in LightSource)
        {
            light.enabled = false;
        }
    }
}
