using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSway : MonoBehaviour
{
    public float shakeDuration = 0.25f;
    public float shakeAmount = 0.7f;
    public float shootingShakeAmount = 0.025f;
    public float shootingShakeDuration = 0.1f;

    private Vector3 targetPosition;
    private float t;

    private Vector3 originalPos;

    void OnEnable()
    {
        originalPos = transform.localPosition;
        targetPosition = originalPos;
    }

    private bool _playerIsShooting = false;
    public void PlayerShootingStart()
    {
        _playerIsShooting = true;
        ResetShake();
    }

    public void PlayerShootingStop()
    {
        _playerIsShooting = false;
        ResetShake();
    }

    void ResetShake()
    {
        float realShakeAmount = _playerIsShooting ? shootingShakeAmount : shakeAmount;
        targetPosition = originalPos + Random.insideUnitSphere * realShakeAmount;
        t = 0.0f;
    }

    void Update()
    {
        if (Vector3.Distance(transform.localPosition, targetPosition) < 0.0001f)
        {
            ResetShake();
        }

        float realShakeDuration = _playerIsShooting ? shootingShakeDuration : shakeDuration;
        transform.localPosition = Vector3.Slerp(transform.localPosition, targetPosition, t / realShakeDuration);

        t += Time.deltaTime;
    }
}
