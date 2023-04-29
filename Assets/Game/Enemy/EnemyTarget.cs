using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    private Vector3 _initialPos;
    private float _initialSeed;
    private MeshRenderer _mr;

    [Range(0.0f, 2.5f)]
    public float TimeScale = 0.25f;

    [Range(0.0f, 5.0f)]
    public float DistanceScale = 1.5f;

    public bool Visualize = false;

    // Start is called before the first frame update
    void Start()
    {
        _mr = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        _initialPos = transform.position;
        _initialSeed = Random.Range(0.0f, 100.0f);
    }

    // Update is called once per frame
    void Update()
    {
        _mr.enabled = Visualize;

        float x = DistanceScale * Mathf.Cos((_initialSeed * _initialSeed) + (TimeScale * Time.time)) * Mathf.Cos(_initialSeed + TimeScale * Time.time);
        float y = 0.0f;
        float z = DistanceScale * Mathf.Sin(_initialSeed + (TimeScale * Time.time));
        transform.position = _initialPos + new Vector3(x, y, z);
    }
}
