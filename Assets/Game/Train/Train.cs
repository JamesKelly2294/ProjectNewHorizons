using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject PlayerPrefab;
    public List<GameObject> StartingTrainCarPrefabs = new List<GameObject>();

    public List<TrainCar> TrainCars = new List<TrainCar>();

    public GameObject ForwardCollider;
    public GameObject AftCollider;

    // Start is called before the first frame update
    void Start()
    {
        var playerGO = Instantiate(PlayerPrefab);
        playerGO.transform.name = "Player";
        playerGO.transform.position = transform.position + new Vector3(0.0f, 0.75f, 0.0f);
        
        for (var i = 0; i < StartingTrainCarPrefabs.Count; i++)
        {
            var trainCarPrefab = StartingTrainCarPrefabs[i];
            var trainCarGO = Instantiate(trainCarPrefab);

            TrainCar tc = trainCarGO.GetComponent<TrainCar>();
            TrainCars.Add(tc);
            tc.transform.parent = transform;

            if (i == 0)
            {
                tc.transform.position = transform.position;
                ForwardCollider.transform.position = tc.forwardLink.transform.position;
                AftCollider.transform.position = tc.aftLink.transform.position;
            }
            else
            {
                var prevCarAftLink = TrainCars[i - 1].aftLink;
                var curCarForwardLink = tc.forwardLink;

                tc.transform.position = TrainCars[i - 1].transform.position + prevCarAftLink.transform.localPosition - curCarForwardLink.transform.localPosition;
                AftCollider.transform.position = tc.aftLink.transform.position;
            }
        }
    }

    public Vector3 ForwardExtent()
    {
        return TrainCars[0].forwardLink.transform.position;
    }

    public Vector3 AftExtent()
    {
        return TrainCars[TrainCars.Count - 1].aftLink.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
