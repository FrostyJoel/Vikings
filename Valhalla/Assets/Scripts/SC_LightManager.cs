using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_LightManager : MonoBehaviour
{
    public static SC_LightManager single;

    private void Awake()
    {
        single = this;
    }

    public Mesh sphereMesh;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
