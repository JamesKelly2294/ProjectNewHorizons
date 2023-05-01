using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Boid : MonoBehaviour
{
    [Range(0.0f, 100.0f)]
    public float MaxSpeed = 10.0f;

    [Range(0.0f, 100.0f)]
    public float MaxSteeringForce = 10.0f;

    [Range(0.0f, 100.0f)]
    public float DesiredSeparation = 10.0f;

    [Range(0.0f, 100.0f)]
    public float NeighborRadius = 50.0f; // boids within this range are considered for calculations

    [Range(0.0f, 10.0f)]
    public float TargetReachedRange = 1.0f;

    [Header("Weighting")]
    [Range(0.0f, 5.0f)]
    public float SeparationWeight = 1.5f;

    [Range(0.0f, 5.0f)]
    public float AlignmentWeight = 1.0f;

    [Range(0.0f, 5.0f)]
    public float CohesionWeight = 1.0f;

    [Range(0.0f, 5.0f)]
    public float TargetWeight = 1.5f;

    private Rigidbody _rb;

    private Vector3 _boidAcceleration = Vector3.zero;
    private Vector3 _boidVelocity = Vector3.zero;

    private BoidFlock _myFlock;

    public BoidFlock BoidFlock
    {
        get
        {
            return _myFlock;
        }
    }

    private bool ShouldPerformNeighborDistanceCheck
    {
        get
        {
            if (_myFlock != null)
            {
                return _myFlock.PerformNeighborDistanceCheck;
            }
            else
            {
                return true;
            }
        }
    }

    private Transform TargetTransform
    {
        get
        {
            if (_myFlock != null)
            {
                return _myFlock.Target;
            }
            else
            {
                return null;
            }
        }
    }

    public void Start()
    {
        _rb = GetComponent<Rigidbody>();

        var boidFlock = GetComponentInParent<BoidFlock>();

        if (boidFlock != null)
        {
            boidFlock.Register(this);
            _myFlock = boidFlock;
        }
    }

    private void OnDestroy()
    {
        if (_myFlock != null)
        {
            _myFlock.Unregister(this);
        }
    }

    public void FixedUpdate()
    {
        // Update velocity
        _boidVelocity += (_boidAcceleration * Time.fixedDeltaTime);
        _boidVelocity = Vector3.ClampMagnitude(_boidVelocity, MaxSpeed);

        // Update position
        _rb.MovePosition(transform.position + (_boidVelocity * Time.fixedDeltaTime));
    }

    public void Flock(BoidFlock boidFlock, List<Boid> allBoids)
    {
        _boidAcceleration = Vector3.zero;

        // Calculate forces
        Vector3 separation = Separate(allBoids); // don't get too close to any boid
        Vector3 alignment = Align(boidFlock.boids); // try to maintain the same direction as nearby boids
        Vector3 cohesion = Cohere(boidFlock.boids); // cluster up with nearby boids
        Vector3 target = Target(TargetTransform); // move toward target if it exists

        // Arbitrary weighting
        separation *= SeparationWeight;
        alignment *= AlignmentWeight;
        cohesion *= CohesionWeight;
        target *= TargetWeight;

        // Apply forces
        ApplyForce(separation);
        ApplyForce(alignment);
        ApplyForce(cohesion);
        ApplyForce(target);
    }


    private void ApplyForce(Vector3 force)
    {
        _boidAcceleration += force;
    }

    private Vector3 Target(Transform targetTransform)
    {
        if (targetTransform == null) { return Vector3.zero; }

        // For our purposes, ignore y of target
        Vector3 targetPosition = targetTransform.position;
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

        Vector3 steeringForce = Vector3.zero;
        var distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > 0.001f)
        {
            steeringForce = Seek(targetPosition);

            if (distance < TargetReachedRange)
            {
                steeringForce *= (distance / TargetReachedRange);
            }
        }
        return steeringForce;
    }

    private Vector3 Separate(List<Boid> boids)
    {
        Vector3 steeringForce = Vector3.zero;

        int boidsTooCloseCount = 0;
        foreach(var other in boids)
        {
            if (other == this) { continue; }
            float distance = Vector3.Distance(transform.position, other.transform.position);

            if (distance < DesiredSeparation)
            {
                // We are too close!
                Vector3 difference = transform.position - other.transform.position;
                difference.Normalize();
                difference = difference / distance; // Weight by distance
                steeringForce += difference;

                boidsTooCloseCount += 1;
            }
        }

        // Calculate the average
        if (boidsTooCloseCount > 0)
        {
            steeringForce = steeringForce / boidsTooCloseCount;
        }

        // Calculate the steering force!
        // Real Steering Force = Desired Steering Force - Velocity
        if (steeringForce.sqrMagnitude > 0)
        {
            steeringForce = steeringForce.normalized * MaxSpeed;
            steeringForce -= _boidVelocity;
            steeringForce = Vector3.ClampMagnitude(steeringForce, MaxSteeringForce);
        }

        return steeringForce;
    }

    private Vector3 Align(List<Boid> boids)
    {
        Vector3 totalVelocity = Vector3.zero;
        int neighboringBoidsCount = 0;

        foreach (var other in boids)
        {
            if (other == this) { continue; }

            bool neighborIsIncluded = true;
            if (ShouldPerformNeighborDistanceCheck)
            {
                float distance = Vector3.Distance(transform.position, other.transform.position);
                neighborIsIncluded = distance < NeighborRadius;
            }

            if (neighborIsIncluded)
            {
                totalVelocity += other._boidVelocity;
                neighboringBoidsCount += 1;
            }
        }

        Vector3 avgVelocity = Vector3.zero;
        if (neighboringBoidsCount > 0)
        {
            avgVelocity = totalVelocity / neighboringBoidsCount;
        }

        // Calculate the steering force!
        // Real Steering Force = Desired Steering Force - Velocity
        Vector3 steeringForce = Vector3.zero;
        if (avgVelocity.sqrMagnitude > 0)
        {
            steeringForce = avgVelocity.normalized * MaxSpeed;
            steeringForce -= _boidVelocity;
            steeringForce = Vector3.ClampMagnitude(steeringForce, MaxSteeringForce);
        }

        return steeringForce;
    }

    private Vector3 Cohere(List<Boid> boids)
    {
        Vector3 totalPosition = Vector3.zero;
        int neighboringBoidsCount = 0;

        foreach (var other in boids)
        {
            if (other == this) { continue; }

            bool neighborIsIncluded = true;

            if (ShouldPerformNeighborDistanceCheck)
            {
                float distance = Vector3.Distance(transform.position, other.transform.position);
                neighborIsIncluded = distance < NeighborRadius;
            }

            if (neighborIsIncluded)
            {
                totalPosition += other.transform.position;
                neighboringBoidsCount += 1;
            }
        }

        Vector3 avgPosition = Vector3.zero;
        Vector3 steeringForce = Vector3.zero;
        if (neighboringBoidsCount > 0)
        {
            avgPosition = totalPosition / neighboringBoidsCount;
            steeringForce = Seek(avgPosition);
        }

        return steeringForce;
    }

    private Vector3 Seek(Vector3 targetPosition)
    {
        // Pointing from current position to target
        Vector3 steeringForce = targetPosition - transform.position;

        // Calculate the steering force!
        // Real Steering Force = Desired Steering Force - Velocity
        steeringForce = steeringForce.normalized * MaxSpeed;
        steeringForce -= _boidVelocity;
        steeringForce = Vector3.ClampMagnitude(steeringForce, MaxSteeringForce);

        return steeringForce;
    }
}
