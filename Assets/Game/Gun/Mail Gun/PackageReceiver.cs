using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PackageReceiver : MonoBehaviour
{
    public bool GenerateRandomPackageOnAwake = false;
    public bool WantsPackage = false;
    public PackageType PackageType = PackageType.automaton;

    public UnityEvent OnReceipt; // Called when they get the package they wanted

    private void Awake()
    {
        if (GenerateRandomPackageOnAwake)
        {
            var allValues = Enum.GetValues(typeof(PackageType));
            PackageType = (PackageType) allValues.GetValue(UnityEngine.Random.Range(0, allValues.Length));
        }
    }

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
            OnReceipt.Invoke();
            return true;
        }

        return false;
    }

    public void SetWantsPackage(bool wants)
    {
        WantsPackage = wants;
    }

    public void DebugDeath()
    {
        Destroy(gameObject);
    }
}
