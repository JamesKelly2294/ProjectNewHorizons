using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainUpgrade", menuName = "Train Upgrade", order = 2)]
public class TrainUpgrade : ScriptableObject
{
    public float Cost = 100;
    public bool Automatable = false;

    public GameObject ConstructedPrefab;
    public GameObject DestroyedPrefab;

    public Vector3 ConstructedOffset;
    public Vector3 DestroyedOffset;

    public bool RotateConstructed = true;
    public bool RotateDestroyed = true;
}