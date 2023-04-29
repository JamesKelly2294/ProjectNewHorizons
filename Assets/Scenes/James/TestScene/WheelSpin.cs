using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpin : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float rotationSpeedRPS = 1.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(new Vector3(rotationSpeedRPS * 360.0f * Time.fixedDeltaTime, 0.0f, 0.0f), Space.Self);
    }
}
