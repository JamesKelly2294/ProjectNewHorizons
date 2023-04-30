using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public GameObject inner;
    public GameObject barrel;
    public GameObject bulletSpawn;
    public GameObject bulletPrefab;
    public GameObject GunOperator;
    public GameObject GunOperatorPosition;
    public PackageType PackageType; // Used if the bullet is actually a package...

    public Vector3 RequestedPosition;
    public bool ShouldFireWhenReady = false;

    public float minRotation = -90;
    public float maxRotation = 90;

    public float RotationSpeed = 2f;

    public AnimationCurve FireTimeCurve; // How long to wait before shooting the next shot
    private float currentFireTimeCurvePosition = 0;
    public float MaxFireTimeCurveTime = 5f;
    private float currentFireTimeValue = 0f;
    public AnimationCurve RecoilCurve;
    private float recoilTime = 1;
    public ParticleSystem MuzzleFlashParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        RequestedPosition = transform.position + transform.forward;
    }

    // Update is called once per frame
    void Update()
    {

        // If there is an operator, put them on the controls
        if (GunOperator != null)
        {
            GunOperator.gameObject.transform.position = GunOperatorPosition.gameObject.transform.position;
            GunOperator.gameObject.transform.rotation = GunOperatorPosition.gameObject.transform.rotation;
        }

        // Limit gun to point within range
        Vector3 angles = Quaternion.LookRotation(RequestedPosition - inner.transform.position, Vector3.up).eulerAngles;
        float rotation = angles.y - gameObject.transform.rotation.eulerAngles.y;
        if (rotation > 180) {
            rotation -= 360;
        }

        if (rotation > maxRotation) {
            rotation = maxRotation;
        } else if (rotation < minRotation) {
            rotation = minRotation;
        }

        var step = RotationSpeed * 360 * Time.deltaTime;
        Quaternion targetRotation = Quaternion.Euler(angles.x, rotation + gameObject.transform.rotation.eulerAngles.y, angles.z);
        inner.transform.rotation = Quaternion.RotateTowards(inner.transform.rotation, targetRotation, step);

        // Continue count down if we are firing
        if (currentFireTimeValue > 0) {
            currentFireTimeValue = Mathf.Max(0, currentFireTimeValue - Time.deltaTime);
        }

        // Shoot when we click
        if (currentFireTimeValue == 0 && ShouldFireWhenReady)
        {
            GameObject bullet = ObjectPooler.Instance.GetPooledObject(bulletPrefab.name);
            ObjectPooler.Instance.ReturnObjectToPoolAfterDelay(bullet, 5f);
            bullet.transform.position = bulletSpawn.transform.position;
            bullet.transform.rotation = bulletSpawn.transform.rotation;
            bullet.SetActive(true);
            bullet.GetComponent<Bullet>().Fire((bulletSpawn.transform.position - barrel.transform.position).normalized);

            // Set the package type if the bullet is a package.
            Package p = bullet.GetComponent<Package>();
            if (p != null)
            {
                p.PackageType = PackageType;
            }
            

            // Reset the value to rate of fire
            currentFireTimeValue = FireTimeCurve.Evaluate(currentFireTimeCurvePosition);
            recoilTime = currentFireTimeValue;
            Debug.Log(currentFireTimeCurvePosition);

            MuzzleFlashParticleSystem.Play();
            GetComponent<AudioSource>().Play();
        }

        // Update the barrel position to account for recoil
        barrel.transform.localPosition = new Vector3(0, 0, RecoilCurve.Evaluate(1f - (currentFireTimeValue / recoilTime)));

        if (ShouldFireWhenReady) {
            currentFireTimeCurvePosition = Mathf.Min(MaxFireTimeCurveTime, currentFireTimeCurvePosition + Time.deltaTime);
        } else {
            currentFireTimeCurvePosition = Mathf.Max(0, currentFireTimeCurvePosition - Time.deltaTime);
        }
    }
}
