using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class Enemy : MonoBehaviour
{
    public List<Gun> guns;

    public GameObject target;

    [Range(0, 60.0f)]
    public float ShootTime = 7.0f; // how long shouldFireWhenReady should be true
    [Range(0, 60.0f)]
    public float ShootCooldown = 2.0f; // how long shouldFireWhenReady should be false
    private bool _isShooting = true;
    private float _shootTimer;
    private float _cooldownTimer;

    private Train _theTrain;
    private float _randomSeed;

    public bool _alive = true;

    // Start is called before the first frame update
    void Start()
    {
        // at the moment, the enemies only attack the train
        _theTrain = FindFirstObjectByType<Train>();
        _randomSeed = Random.Range(0, 1.0f);

        _shootTimer = Random.Range(0, ShootTime);

        guns = GetComponentsInChildren<Gun>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (_alive)
        {
            ResetFiring();
            AcquireTarget();
            AimAtTarget();
        }
    }

    public void Die()
    {
        _alive = false;

        var boid = GetComponent<Boid>();
        if (boid)
        {
            Destroy(boid);
        }

        foreach (var gun in guns)
        {
            gun.ShouldFireWhenReady = false;
            gun.enabled = false;
        }

        gameObject.AddComponent<DeathAnimation>();
    }

    IEnumerator DestroyAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Destroy(gameObject);
    }

    GameObject ClosestTarget(List<GameObject> targets)
    {
        if(targets.Count == 0) { return null; }

        var closestCar = targets[0];
        var closestCarDistance = Vector3.SqrMagnitude(transform.position - closestCar.transform.position);
        foreach (var trainCar in targets)
        {
            var newDistance = Vector3.SqrMagnitude(transform.position - trainCar.transform.position);
            if (newDistance < closestCarDistance)
            {
                closestCar = trainCar;
                closestCarDistance = newDistance;
            }
        }

        return closestCar;
    }

    void ResetFiring()
    {
        if (_isShooting)
        {
            _shootTimer -= Time.deltaTime;
        }
        else
        {
            _cooldownTimer -= Time.deltaTime;
        }

        if (_isShooting && _shootTimer < 0)
        {
            _isShooting = false;
            _cooldownTimer = ShootCooldown;
        } else if (!_isShooting && _cooldownTimer < 0)
        {
            _isShooting = true;
            _shootTimer = ShootTime;
        }

        foreach (var gun in guns)
        {
            gun.ShouldFireWhenReady = false;
        }
    }

    void AcquireTarget()
    {
        // oh boy, assumptions!
        var side = transform.position.x < _theTrain.transform.position.x ? TrainSide.Left : TrainSide.Right;
        
        var trainCars = _theTrain.TrainCars
            .Where(e => !e.IsSideDestroyed(side))
            .Select(e => e.gameObject)
            .Append(_theTrain.LocomotiveDamageable.gameObject)
            .ToList();

        var closestCar = ClosestTarget(trainCars);
        if (closestCar != null)
        {
            target = closestCar.gameObject;
        }
        else
        {
            target = null;
        }
    }

    void AimAtTarget()
    {
        if (target == null || !_isShooting) { return; }

        var anyGunShooting = false;
        foreach (var gun in guns)
        {
            gun.RequestedPosition = target.transform.position + Vector3.forward * Mathf.Sin(2.0f * Time.time + _randomSeed) * 1.5f;

            var gunPosInViewportSpace = Camera.main.WorldToViewportPoint(gun.transform.position);
            var shouldFireWhenReady = gunPosInViewportSpace.x > 0 &&
                gunPosInViewportSpace.x < 1.0 &&
                gunPosInViewportSpace.y > 0 &&
                gunPosInViewportSpace.y < 1.0;

            gun.ShouldFireWhenReady = shouldFireWhenReady;
            anyGunShooting |= gun.ShouldFireWhenReady;
        }
    }
}
