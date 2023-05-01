using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public Camera Camera;
    public CameraSway CameraSway;

    public Transform CinematicAngle;
    public Transform GameplayAngle;

    public void SetToAngle(Transform t)
    {
        CameraSway.enabled = false;
        Camera.transform.position = t.position;
        Camera.transform.rotation = t.rotation;
        CameraSway.enabled = true;
    }

    public void SetToCinematicAngle()
    {
        SetToAngle(CinematicAngle);
    }

    public void SetToGameplayAngle()
    {
        SetToAngle(GameplayAngle);
    }
}
