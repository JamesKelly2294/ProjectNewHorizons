using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCoordinator : MonoBehaviour
{
    public List<BoidFlock> boidFlocks = new List<BoidFlock>();
    public List<Boid> boids = new List<Boid>();

    public void Update()
    {
        foreach (var boidFlock in boidFlocks)
        {
            boidFlock.Flock(boids);
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

    public void Register(Boid b)
    {
        if (boids.Contains(b))
        {
            return;
        }

        boids.Add(b);
    }

    public void Unregister(Boid b)
    {
        if (!boids.Contains(b))
        {
            return;
        }

        boids.Remove(b);
    }
}
