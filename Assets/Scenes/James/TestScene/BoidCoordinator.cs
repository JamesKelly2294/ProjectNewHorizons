using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCoordinator : MonoBehaviour
{
    public List<BoidFlock> boidFlocks = new List<BoidFlock>();

    public void Update()
    {
        foreach (var boidFlock in boidFlocks)
        {
            boidFlock.Flock();
        }
    }

    public void Register(BoidFlock b)
    {
        if (boidFlocks.Contains(b))
        {
            return;
        }

        boidFlocks.Add(b);
    }

    public void Unregister(BoidFlock b)
    {
        if (!boidFlocks.Contains(b))
        {
            return;
        }

        boidFlocks.Remove(b);
    }
}
