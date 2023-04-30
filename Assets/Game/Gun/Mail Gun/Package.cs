using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Package : MonoBehaviour
{

    public PackageType PackageType = PackageType.automaton;

    public UnityEvent OnReceipt; // Called if the package was received by the right person

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        PackageReceiver r = collision.gameObject.GetComponent<PackageReceiver>();
        if (r != null && r.enabled == true) {
            if (r.TakePackage(PackageType))
            {
                OnReceipt.Invoke();
            }
        }
    }
}

public enum PackageType
{
    automaton,
    camera,
    gears,
    goggles,
    keyboard,
    masqueradeMask,
    musicBox,
    necklace,
    pocketWatch,
    steamPipe,
}