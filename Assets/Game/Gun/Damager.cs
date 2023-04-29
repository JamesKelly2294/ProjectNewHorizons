using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{

    public float Damage = 1f;

    public UnityEvent OnDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Damageable d = collision.gameObject.GetComponent<Damageable>();
        if (d != null && d.enabled == true) {
            d.TakeDamage(Damage);
            OnDamage.Invoke();
        }
    }
}
