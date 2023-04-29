using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidFlock : MonoBehaviour
{
    public bool PerformNeighborDistanceCheck = false;

    public List<Boid> boids = new List<Boid>();

    public void Awake()
    {
        var boidCoordinator = FindFirstObjectByType<BoidCoordinator>();

        if (boidCoordinator != null)
        {
            boidCoordinator.Register(this);
        }
    }

    public void Flock()
    {
        foreach (var boid in boids)
        {
            boid.Flock(boids);
        }
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
