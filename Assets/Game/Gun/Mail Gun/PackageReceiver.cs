using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PackageReceiver : MonoBehaviour
{
    public bool WantsPackage = false;
    public PackageType PackageType = PackageType.cat;

    public UnityEvent OnReceipt; // Called when they get the package they wanted

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TakePackage(PackageType type)
    {
        if (WantsPackage && PackageType == type)
        {
            WantsPackage = false;
            OnReceipt.Invoke();
            return true;
        }

        return false;
    }

    public void DebugDeath()
    {
        Destroy(gameObject);
    }
}
