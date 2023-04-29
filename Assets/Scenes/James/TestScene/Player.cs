using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Range(0.0f, 100.0f)]
    public float MaxSpeed = 10.0f;

    //[Range(0.0f, 100.0f)]
    //public float Acceleration = 1.0f;

    //[Range(0.0f, 100.0f)]
    //public float DecelerationMultiplier = 0.1f;

    Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movementDir = Vector3.zero;

        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;

        if (Input.GetKey(KeyCode.W))
        {
            movementDir += camForward;
        } else if (Input.GetKey(KeyCode.S))
        {
            movementDir -= camForward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            movementDir -= camRight;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movementDir += camRight;
        }

        _rb.velocity = movementDir.normalized * MaxSpeed;

        //if (movementDir == Vector3.zero)
        //{
        //    if (_rb.velocity.sqrMagnitude > 0.1)
        //    {
        //        _rb.velocity -= (_rb.velocity * DecelerationMultiplier * Time.deltaTime);
        //    }
        //    else
        //    {
        //        _rb.velocity = Vector3.zero;
        //    }
        //}
        //else
        //{
        //    // moving, accelerate to target
        //    _rb.velocity += (movementDir.normalized * Acceleration * Time.deltaTime);
        //    _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, MaxSpeed);
        //}
    }
}
