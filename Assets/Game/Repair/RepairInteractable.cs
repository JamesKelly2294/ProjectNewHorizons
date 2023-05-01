using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RepairInteractable : MonoBehaviour
{
    public VerticalWorldSpaceProgressBar ProgressBar;

    [Range(0, 1.0f)]
    public float Progress;

    [Range(0, 60.0f)]
    public float TimeToRepair = 7.5f;

    public UnityEvent RepairCompleted;

    private bool _isBeingRepaired;

    private float _repairProgress;
    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _repairProgress = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isBeingRepaired) { return; }

        _repairProgress += Time.deltaTime;
        ProgressBar.value = (_repairProgress / TimeToRepair);

        if (_repairProgress > TimeToRepair)
        {
            CompleteRepair();
        }
    }

    public void CompleteRepair()
    {
        EndControl(_player);
        RepairCompleted.Invoke();

        Destroy(gameObject);
    }

    public void BeginControl(Player player)
    {
        _player = player;
        enabled = true;
        player.IsInteracting = true;
        _isBeingRepaired = true;
    }

    public void EndControl(Player player)
    {
        _player = player;
        player.IsInteracting = false;
        enabled = false;
        _isBeingRepaired = false;
    }
}
