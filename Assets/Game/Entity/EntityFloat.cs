using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFloat : MonoBehaviour
{
    [Range(0.0f, 2.5f)]
    public float cycleTime = 1.0f;

    [Range(0.0f, 2.5f)]
    public float floatDistance = 1.0f;

    private float _t;
    private Vector3 _initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        _initialPosition = transform.localPosition;
        _t = Random.Range(0, cycleTime);
    }

    // Update is called once per frame
    void Update()
    {
        _t += Time.deltaTime;

        var x = transform.localPosition.x;
        var yOffset = floatDistance * Mathf.Sin(_t / cycleTime);
        var y = _initialPosition.y + yOffset;
        var z = transform.localPosition.z;

        transform.localPosition = new Vector3(x, y, z);
    }
}
