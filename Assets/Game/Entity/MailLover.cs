using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailLover : MonoBehaviour
{
    public PackageType DesiredPackageType = PackageType.camera;
    public int DesiredPackageAmount = 1;

    public PackageRecipientDesireBox PackageRecipientDesireBox;
    public PackageReceiver PackageReceiver;

    [Range(0.0f, 1.0f)]
    public float Desire;

    [Range(0.0f, 60.0f)]
    public float PatienceTimer = 10.0f;

    private float _t = 0.0f;

    bool _alive = true;

    public void Die()
    {
        _alive = false;

        var boid = GetComponent<Boid>();
        if (boid)
        {
            Destroy(boid);
        }

        gameObject.AddComponent<DeathAnimation>();
    }

    // Start is called before the first frame update
    void Awake()
    {
        //var allValues = Enum.GetValues(typeof(PackageType));
        //DesiredPackageType = (PackageType)allValues.GetValue(UnityEngine.Random.Range(0, allValues.Length));
        //DesiredPackageAmount = UnityEngine.Random.Range(1, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_alive)
        {
            return;
        }

        var mailLoverInViewportSpace = Camera.main.WorldToViewportPoint(transform.position);
        var mailLoverIsVisible = mailLoverInViewportSpace.x > 0 &&
            mailLoverInViewportSpace.x < 1.0 &&
            mailLoverInViewportSpace.y > 0 &&
            mailLoverInViewportSpace.y < 1.0;

        if (mailLoverIsVisible && DesiredPackageAmount > 0)
        {
            _t += Time.deltaTime;
        }

        Desire = _t / PatienceTimer;
        
        if (PackageReceiver != null)
        {
            PackageReceiver.WantsPackage = DesiredPackageAmount > 0;
            PackageReceiver.PackageType = DesiredPackageType;
        }

        if (PackageRecipientDesireBox != null)
        {
            PackageRecipientDesireBox.PackageType = DesiredPackageType;
            PackageRecipientDesireBox.Count = DesiredPackageAmount;
            PackageRecipientDesireBox.Desire = Desire;
        }

        if (Desire > 1.0 || DesiredPackageAmount == 0)
        {
            PackageRecipientDesireBox.ShouldEmote = true;
            FlyAway();
        }
    }

    public void FlyAway()
    {
        var boid = GetComponent<Boid>();

        if (boid == null)
        {
            DestroyGOAfterTime(gameObject);
            return;
        }

        var flock = boid.BoidFlock;

        if (flock == null)
        {
            DestroyGOAfterTime(gameObject);
            return;
        }

        var targetPosition = flock.Target.transform.position;

        var newGO = new GameObject("Fly Away Target");
        newGO.transform.parent = flock.Target.transform.parent;
        var newX = targetPosition.x < 0 ? targetPosition.x - 100.0f : targetPosition.x + 100.0f;
        newGO.transform.position = new Vector3(newX, targetPosition.y, targetPosition.z);
        flock.Target = newGO.transform;


        DestroyGOAfterTime(flock.gameObject);
    }

    IEnumerator DestroyGOAfterTime(GameObject go, float time=20.0f)
    {
        yield return new WaitForSeconds(time);

        Destroy(go.gameObject);
    }

    public void OnReceipt()
    {
        DesiredPackageAmount -= 1;
    }
}
