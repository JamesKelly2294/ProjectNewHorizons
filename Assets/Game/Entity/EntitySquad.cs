using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoidFlock))]
public class EntitySquad : MonoBehaviour
{
    private BoidFlock _boidFlock;

    // Todo, replace with weighted table
    public GameObject EasyEntityPrefab;
    public GameObject MediumEntityPrefab;

    private List<GameObject> _squadMembers = new List<GameObject>();

    private static int EntitySquadID = 0;
    private int id;

    // Start is called before the first frame update
    void Awake()
    {
        id = EntitySquadID;
        EntitySquadID += 1;
        transform.name = "Entity Squad " + id;

        _boidFlock = GetComponent<BoidFlock>();

        GameObject EntityGOPrefab = Random.Range(0, 1.0f) < 0.5f ? EasyEntityPrefab : MediumEntityPrefab;

        GameObject EntityGO = Instantiate(EntityGOPrefab);
        EntityGO.transform.position = transform.position;
        EntityGO.transform.parent = _boidFlock.transform;
        _squadMembers.Add(EntityGO);

        var damageable = EntityGO.GetComponent<Damageable>();
        damageable.OnDeath.AddListener(delegate
        {
            OnSquadMemberDeath(EntityGO);
        });
    }

    public void OnSquadMemberDeath(GameObject squadMember)
    {
        _squadMembers.Remove(squadMember);
        squadMember.transform.parent = transform.parent;

        if (_squadMembers.Count == 0)
        {
            Debug.Log(transform.name + " destroyed!");

            Destroy(gameObject);
        }
    }

    public void SetTarget(Transform target)
    {
        target.transform.parent = transform;
        _boidFlock.Target = target;
    }
}
