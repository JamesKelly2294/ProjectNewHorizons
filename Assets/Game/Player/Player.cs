using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Range(0.0f, 100.0f)]
    public float MaxSpeed = 10.0f;

    //[Range(0.0f, 100.0f)]
    //public float Acceleration = 1.0f;

    //[Range(0.0f, 100.0f)]
    //public float DecelerationMultiplier = 0.1f;

    Rigidbody _rb;

    public bool IsInteracting = false;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsInteracting) 
        {
            Vector3 movementDir = Vector3.zero;

            Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
            Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;

            if (Input.GetKey(KeyCode.W))
            {
                movementDir += camForward;
            } else if (Input.GetKey(KeyCode.S))
            {
                movementDir -= camForward;
            }

            if (Input.GetKey(KeyCode.A))
            {
                movementDir -= camRight;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                movementDir += camRight;
            }

            _rb.velocity = movementDir.normalized * MaxSpeed;
        }

        CheckForInteractable();
    }

    Interactable currentInteractable;
    void CheckForInteractable() {

        // Find the interactables around the player, find the closest in the process too
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 2f);
        List<Interactable> currentlyIntersectedInteractables = new List<Interactable>();
        Interactable closestInteractable = null;
        float closestInteractableDistance = 1000f;
        foreach (var collision in colliders) {
            Interactable i = collision.gameObject.GetComponent<Interactable>();
            if (i != null && i.enabled == true) {
                currentlyIntersectedInteractables.Add(i);
                float distance = Vector3.Distance(collision.gameObject.transform.position, gameObject.transform.position);
                if (closestInteractable == null || distance < closestInteractableDistance) {
                    closestInteractable = i;
                    closestInteractableDistance = distance;
                }
            }
        }
        
        // Set the current interactable that we are going to target, and let it know what we are wanting to do
        if (closestInteractable != currentInteractable) {
            if (currentInteractable != null) {
                if (currentInteractable.IsInteracting) {
                    currentInteractable.SetInteracting(false, this);
                }

                currentInteractable.SetAbleToInteract(false);
            }

            currentInteractable = closestInteractable;

            if (currentInteractable != null) {
                currentInteractable.SetAbleToInteract(true);
            }
        }

        // Enter or exit interacting with an interactable
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E)) 
        {
            currentInteractable.SetInteracting(!currentInteractable.IsInteracting, this);
        }
    }
}
