using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCoordinator : MonoBehaviour
{
    public List<Boid> boids = new List<Boid>();

    public void Update()
    {
        foreach (var boid in boids)
        {
            boid.Flock(boids);
        }
    }

    // The boid coordinator will ultimately be responsible for creating flocks
    // For now, we will just have boids register themselves with the coordinator on awake
    public void Register(Boid b)
    {

    }
}
