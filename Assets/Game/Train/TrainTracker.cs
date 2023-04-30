using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTracker : MonoBehaviour
{
    private Train _theTrain;

    // Start is called before the first frame update
    void Start()
    {
        _theTrain = FindFirstObjectByType<Train>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 sumVector = new Vector3(0f, 0f, 0f);

        foreach (Transform child in _theTrain.transform)
        {
            sumVector += child.position;
        }

        Vector3 groupCenter = sumVector / _theTrain.transform.childCount;
        transform.position = new Vector3(groupCenter.x, transform.position.y, groupCenter.z);
    }
}
