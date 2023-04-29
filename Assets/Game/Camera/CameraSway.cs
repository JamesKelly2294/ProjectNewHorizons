using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSway : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.
    public float shakeDuration = 0.25f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;



    Vector3 targetPosition;
    float t;

    Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
        targetPosition = originalPos;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) < 0.0001f)
        {
            targetPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            t = 0.0f;
        }

        transform.position = Vector3.Slerp(transform.position, targetPosition, t / shakeDuration);

        t += Time.deltaTime;
    }
}
