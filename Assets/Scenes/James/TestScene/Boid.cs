using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public void Flock(List<Boid> boids)
    {

    }

    public void Awake()
    {
        var boidCoordinator = FindFirstObjectByType<BoidCoordinator>();
        
        if (boidCoordinator != null)
        {
            boidCoordinator.Register(this);
        }
    }
}
