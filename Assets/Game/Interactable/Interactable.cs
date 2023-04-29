using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    public bool IsInteracting = false;
    public bool AbleToInteract = false;

    public UnityEvent BeginInteraction;
    public UnityEvent EndInteraction;

    public UnityEvent ShowInteractionPopup;
    public UnityEvent HideInteractionPopup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInteracting(bool isInteracting)
    {
        if (isInteracting != IsInteracting) {
            IsInteracting = isInteracting;
            if (isInteracting) {
                BeginInteraction.Invoke();
            } else {
                EndInteraction.Invoke();
            }
        }
    }

    public void SetAbleToInteract(bool ableToInteract)
    {
        if (ableToInteract != AbleToInteract) {
            AbleToInteract = ableToInteract;
            if (ableToInteract) {
                ShowInteractionPopup.Invoke();
            } else {
                HideInteractionPopup.Invoke();
            }
        }
    }
}
