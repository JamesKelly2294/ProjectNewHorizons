using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidFlock : MonoBehaviour
{
    public bool PerformNeighborDistanceCheck = false;
    public Transform Target;

    public List<Boid> boids = new List<Boid>();

    private BoidCoordinator _bc;

    public void Awake()
    {
        var boidCoordinator = FindFirstObjectByType<BoidCoordinator>();

        if (boidCoordinator != null)
        {
            _bc = boidCoordinator;
            _bc.Register(this);
        }
    }

    private void OnDestroy()
    {
        _bc.Unregister(this);
    }

    public void Flock(List<Boid> allBoids)
    {
        foreach (var boid in boids)
        {
            boid.Flock(this, allBoids);
        }
    }

    public void Register(Boid b)
    {
        if (boids.Contains(b))
        {
            return;
        }

        boids.Add(b);
        _bc.Register(b);
    }

    public void Unregister(Boid b)
    {
        if (!boids.Contains(b))
        {
            return;
        }

        boids.Remove(b);
        _bc.Unregister(b);
    }
}
