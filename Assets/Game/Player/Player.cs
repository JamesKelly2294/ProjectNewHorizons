using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool IsInteracting = false;

    public float speed = 10f;
    public GameObject inner;
    Plane plane = new Plane(Vector3.up, 0);

    public AnimationCurve walkVertical;
    public AnimationCurve walkWobble;
    public float walkVerticalTime, walkVerticalTotalTime;
    public float footstepTime;
    public bool alive = true;

    public GameObject cabooseVisual;
    public GameObject normalVisual;

    void PlayFootstepAudio()
    {
        AudioManager.Instance.Play("SFX/PlayerWalk",
            pitchMin: 0.65f, pitchMax: 0.85f,
            volumeMin: 0.4f, volumeMax: 0.4f,
            position: transform.position,
            minDistance: 10, maxDistance: 20);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) { return; }

        Vector3 movement = Vector3.zero;
        if (!IsInteracting)
        {
            Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
            Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;

            if (Input.GetKey("w"))
            {
                movement += camForward;
            }

            if (Input.GetKey("s"))
            {
                movement += -camForward;
            }

            if (Input.GetKey("a"))
            {
                movement += -camRight;
            }

            if (Input.GetKey("d"))
            {
                movement += camRight;
            }
        }

        movement = movement.normalized;
        transform.position += movement * Time.deltaTime * speed;

        // Animate walk
        if (movement != Vector3.zero)
        {
            walkVerticalTime += Time.deltaTime;
            if (walkVerticalTime > walkVerticalTotalTime)
            {
                walkVerticalTime -= walkVerticalTotalTime;
            }

            footstepTime += Time.deltaTime;
            if (footstepTime > (walkVerticalTotalTime / 2.0f) - (walkVerticalTotalTime / 4.0f))
            {
                PlayFootstepAudio();
                footstepTime = -walkVerticalTotalTime / 4.0f;
            }
        }
        else
        {
            // If we are beyond half way, just continue
            if (walkVerticalTime > walkVerticalTotalTime / 2 && walkVerticalTime < walkVerticalTotalTime)
            {
                walkVerticalTime += Time.deltaTime;
                walkVerticalTime = Mathf.Min(walkVerticalTotalTime, walkVerticalTime);

            }
            else if (walkVerticalTime < walkVerticalTotalTime / 2 && walkVerticalTime > 0)
            {
                walkVerticalTime -= Time.deltaTime;
                walkVerticalTime = Mathf.Max(0, walkVerticalTime);
            }
        }
        float innerVerticalOffet = walkVertical.Evaluate(walkVerticalTime / walkVerticalTotalTime);
        inner.transform.localPosition = new Vector3(inner.transform.localPosition.x, innerVerticalOffet, inner.transform.localPosition.z);

        float distance;
        Vector3 cursorLoc = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            cursorLoc = ray.GetPoint(distance);
        }

        if (currentInteractable != null && currentInteractable.IsInteracting)
        {
            //inner.transform.LookAt(currentInteractable.transform);
        }
        else
        {
            cursorLoc = new Vector3(cursorLoc.x, inner.transform.position.y, cursorLoc.z);
            inner.transform.LookAt(cursorLoc);
        }

        Vector3 eulerRotation = inner.transform.rotation.eulerAngles;
        inner.transform.rotation = Quaternion.Euler(new Vector3(eulerRotation.x, eulerRotation.y, walkWobble.Evaluate(walkVerticalTime / walkVerticalTotalTime) * 90));

        CheckForInteractable();
    }

    public void StopInteracting()
    {
        if (currentInteractable == null) { return; }
        currentInteractable.SetInteracting(false, this);
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
