using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class HideOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}