using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCar : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject RepairSidePrefab;

    public GameObject forwardLink;
    public GameObject aftLink;

    public Transform leftSideSlot;
    public Transform rightSideSlot;

    public TrainUpgrade leftSideUpgradeData;
    public TrainUpgrade rightSideUpgradeData;

    public Damageable leftSideDamageable;
    public Damageable rightSidDamageable;

    public GameObject normalVisuals;
    public GameObject rearVisuals;

    [Header("Runtime")]
    public GameObject leftSideUpgradeGO;
    public GameObject rightSideUpgradeGO;

    private Player _player;

    void Awake()
    {
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        _player = playerGO.GetComponent<Player>();

        SpawnUpgrades();
    }
    
    void SpawnUpgrades()
    {
        leftSideUpgradeGO = SpawnTrainUpgrade(leftSideUpgradeData, leftSideSlot, leftSide: true);
        rightSideUpgradeGO = SpawnTrainUpgrade(rightSideUpgradeData, rightSideSlot, leftSide: false);
    }

    GameObject SpawnTrainUpgrade(TrainUpgrade trainUpgrade, Transform slot, bool leftSide, bool destroyed = false)
    {
        if (trainUpgrade == null)
        {
            return null;
        }
        
        GameObject go = Instantiate(!destroyed ? trainUpgrade.ConstructedPrefab : trainUpgrade.DestroyedPrefab);

        var offset = !destroyed ? trainUpgrade.ConstructedOffset : trainUpgrade.DestroyedOffset;
        if (!leftSide) { offset = new Vector3(offset.x * - 1.0f, offset.y, offset.z); }

        go.transform.parent = transform;
        go.transform.position = slot.transform.position + offset;

        var shouldRotate = (!destroyed ? trainUpgrade.RotateConstructed : trainUpgrade.RotateDestroyed);
        if (shouldRotate)
        {
            var eulerAngles = go.transform.rotation.eulerAngles;
            var slotEulerAngles = slot.rotation.eulerAngles;
            var rotation = go.transform.rotation;
            rotation.eulerAngles = new Vector3(eulerAngles.x, slotEulerAngles.y, eulerAngles.z);

            go.transform.rotation = rotation;
        }

        return go;
    }

    private void DisableUpgradeInteractable(GameObject go)
    {
        if (go == null) { return; }
        var interactable = go.GetComponent<Interactable>();
        if (interactable != null && interactable.IsInteracting)
        {
            interactable.SetInteracting(false, _player);
        }
    }

    public void LeftSideDestroyed()
    {
        DisableUpgradeInteractable(leftSideUpgradeGO);
        Destroy(leftSideUpgradeGO);
        leftSideUpgradeGO = null;

        var spawnedUpgrade = SpawnTrainUpgrade(leftSideUpgradeData, leftSideSlot, leftSide: true, destroyed: true);
        leftSideUpgradeGO = spawnedUpgrade;

        AudioManager.Instance.Play("Gun/Destroyed");

        var repairGO = Instantiate(RepairSidePrefab);
        repairGO.transform.parent = transform;
        repairGO.transform.position = spawnedUpgrade.transform.position;
        var repairInteractable = repairGO.GetComponent<RepairInteractable>();
        repairInteractable.RepairCompleted.AddListener(delegate
        {
            Debug.Log("Left Repair completed!");
            leftSideDamageable.CurrentHealth = leftSideDamageable.MaxHealth;
            leftSideDamageable.enabled = true;

            DisableUpgradeInteractable(leftSideUpgradeGO);
            Destroy(leftSideUpgradeGO);
            leftSideUpgradeGO = null;

            leftSideUpgradeGO = SpawnTrainUpgrade(leftSideUpgradeData, leftSideSlot, leftSide: true, destroyed: false);

            AudioManager.Instance.Play("Gun/Repaired");
        });
    }

    public void RightSideDestroyed()
    {
        DisableUpgradeInteractable(rightSideUpgradeGO);
        Destroy(rightSideUpgradeGO);
        rightSideUpgradeGO = null;

        var spawnedUpgrade = SpawnTrainUpgrade(rightSideUpgradeData, rightSideSlot, leftSide: false, destroyed: true);
        rightSideUpgradeGO = spawnedUpgrade;

        AudioManager.Instance.Play("Gun/Destroyed");

        var repairGO = Instantiate(RepairSidePrefab);
        repairGO.transform.parent = transform;
        repairGO.transform.position = spawnedUpgrade.transform.position;
        var repairInteractable = repairGO.GetComponent<RepairInteractable>();
        repairInteractable.RepairCompleted.AddListener(delegate
        {
            Debug.Log("Right Repair completed!");
            rightSidDamageable.CurrentHealth = rightSidDamageable.MaxHealth;
            rightSidDamageable.enabled = true;

            DisableUpgradeInteractable(rightSideUpgradeGO);
            Destroy(rightSideUpgradeGO);
            rightSideUpgradeGO = null;

            rightSideUpgradeGO = SpawnTrainUpgrade(rightSideUpgradeData, rightSideSlot, leftSide: false, destroyed: false);

            AudioManager.Instance.Play("Gun/Repaired");
        });
    }

    public bool IsSideDestroyed(TrainSide side)
    {
        if (side == TrainSide.Left)
        {
            return leftSideDamageable.CurrentHealth <= 0;
        }
        else
        {
            return rightSidDamageable.CurrentHealth <= 0;
        }
    }
}
