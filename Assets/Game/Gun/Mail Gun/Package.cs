using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Package : MonoBehaviour
{

    public PackageType PackageType = PackageType.automaton;

    public UnityEvent OnReceipt; // Called if the package was received by the right person
    public UnityEvent OnFailure; // Called if the package was not delivered to the right person, or if not delivered at all.

    public float ttl = 5f;
    private Coroutine coroutine;
    private bool canCollide = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        coroutine = StartCoroutine(FailureAfterTime(ttl));
        canCollide = true;
    }

    void OnDisable()
    {
        StopCoroutine(coroutine);
        coroutine = null;
        canCollide = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!canCollide) { return; }

        PackageReceiver r = collision.gameObject.GetComponent<PackageReceiver>();
        if (r != null && r.enabled == true) {
            canCollide = false;
            if (r.TakePackage(PackageType))
            {
                OnReceipt.Invoke();
            } else {
                OnFailure.Invoke();
            }
        }
    }

    IEnumerator FailureAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Hello?");
        OnFailure.Invoke();
    }
}

public enum PackageType
{
    automaton,
    camera,
    gears,
    goggles,
    keyboard,
    masqueradeMask,
    musicBox,
    necklace,
    pocketWatch,
    steamPipe,
}