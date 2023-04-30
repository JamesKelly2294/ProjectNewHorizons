using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    public bool IsInteracting = false;
    public bool AbleToInteract = false;

    public UnityEvent<Player> BeginInteraction;
    public UnityEvent<Player> EndInteraction;

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

    public void SetInteracting(bool isInteracting, Player player)
    {
        if (isInteracting != IsInteracting) {
            IsInteracting = isInteracting;
            if (isInteracting) {
                BeginInteraction.Invoke(player);
            } else {
                EndInteraction.Invoke(player);
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
