using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlledGun : MonoBehaviour
{
    private Gun gun;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // Start is called before the first frame update
    void Start()
    {
        gun = GetComponent<Gun>();
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Point at Cursor
        var groundPlane = new Plane(Vector3.up, -1 * transform.position.y);
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitDistance;

        if (groundPlane.Raycast(mouseRay, out hitDistance))
        {
            gun.RequestedPosition = mouseRay.GetPoint(hitDistance);
        }

        gun.ShouldFireWhenReady = Input.GetMouseButton(0);
    }

    public void BeginControl(Player player)
    {
        enabled = true;
        player.IsInteracting = true;
        gun.GunOperator = player.gameObject;
        originalPosition = player.gameObject.transform.position;
        originalRotation = player.gameObject.transform.rotation;
    }

    public void EndControl(Player player)
    {
        player.IsInteracting = false;
        player.gameObject.transform.position = originalPosition;
        player.gameObject.transform.rotation = originalRotation;
        gun.RequestedPosition = gun.gameObject.transform.position + (100f * gun.gameObject.transform.forward);
        gun.ShouldFireWhenReady = false;
        gun.GunOperator = null;
        enabled = false;
    }
}
