using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public GameObject inner;
    public GameObject bulletSpawn;
    public GameObject bulletPrefab;

    public float minRotation = -90;
    public float maxRotation = 90;

    public float RotationSpeed = 2f;
    private Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Point at Cursor, but not instantly to give the gun some weight
        var groundPlane = new Plane(Vector3.up, -1 * inner.transform.position.y);
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitDistance;

        if (groundPlane.Raycast(mouseRay, out hitDistance))
        {
            var lookAtPosition = mouseRay.GetPoint(hitDistance);
            targetRotation = Quaternion.LookRotation(lookAtPosition - inner.transform.position, Vector3.up);
        }

        Vector3 angles = targetRotation.eulerAngles;
        float rotation = angles.y;
        if (rotation > 180) {
            rotation -= 360;
        }

        if (rotation > maxRotation) {
            rotation = maxRotation;
        } else if (rotation < minRotation) {
            rotation = minRotation;
        }

        targetRotation = Quaternion.Euler(angles.x, rotation, angles.z);

        var step = RotationSpeed * 360 * Time.deltaTime;
        inner.transform.rotation = Quaternion.RotateTowards(inner.transform.rotation, targetRotation, step);


        // Shoot when we click
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = bulletSpawn.transform.position;
            bullet.GetComponent<Bullet>().direction = bulletSpawn.transform.position - inner.transform.position;
        }
    }
}
