using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class Enemy : MonoBehaviour
{
    public List<Gun> guns;

    public GameObject target;

    private Train _theTrain;

    // Start is called before the first frame update
    void Start()
    {
        // at the moment, the enemies only attack the train
        _theTrain = FindFirstObjectByType<Train>();
        
        guns = GetComponentsInChildren<Gun>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        ResetFiring();
        AcquireTarget();
        AimAtTarget();
    }

    TrainCar ClosestTrainCar(List<TrainCar> trainCars)
    {
        if(trainCars.Count == 0) { return null; }

        var closestCar = trainCars[0];
        var closestCarDistance = Vector3.SqrMagnitude(transform.position - closestCar.transform.position);
        foreach (var trainCar in trainCars)
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
        foreach (var gun in guns)
        {
            gun.ShouldFireWhenReady = false;
        }
    }

    void AcquireTarget()
    {
        // oh boy, assumptions!
        var side = transform.position.x < _theTrain.transform.position.x ? TrainSide.Left : TrainSide.Right;
        
        var trainCars = _theTrain.TrainCars.Where(e => !e.IsSideDestroyed(side)).ToList();
        if (trainCars.Count == 0)
        {
            trainCars = _theTrain.TrainCars.Where(e => e.IsSideDestroyed(side)).ToList();
        }

        var closestCar = ClosestTrainCar(trainCars);
        target = closestCar.gameObject;
    }

    void AimAtTarget()
    {
        if (target == null) { return; }

        foreach (var gun in guns)
        {
            gun.RequestedPosition = target.transform.position;
            gun.ShouldFireWhenReady = true;
        }
    }
}
