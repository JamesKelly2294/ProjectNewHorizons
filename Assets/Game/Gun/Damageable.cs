using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [HideInInspector]
    public float MaxHealth;
    public float CurrentHealth = 10f;

    public UnityEvent OnDamage;
    public UnityEvent OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        MaxHealth = CurrentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        if (CurrentHealth <= 0) {
            OnDeath.Invoke();
            enabled = false;
        } else {
            OnDamage.Invoke();
        }
    }

    public void DebugDeath()
    {
        Destroy(gameObject);
    }
}
