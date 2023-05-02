using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{

    public ParticleSystem system;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Go()
    {
        Instantiate(system);
        system.gameObject.transform.position = gameObject.transform.position;
        system.gameObject.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0,180,0);
    }
}
