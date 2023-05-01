using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public static Inventory Instance;

    public Dictionary<PackageType, int> PackageInventory = new Dictionary<PackageType, int>();
    public PackageType? SelectionType = null;

    void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        PackageInventory[PackageType.automaton]         = 4;
        PackageInventory[PackageType.gears]             = 3;
        PackageInventory[PackageType.camera]            = 2;
        PackageInventory[PackageType.goggles]           = 12;
        PackageInventory[PackageType.keyboard]          = 2;
        PackageInventory[PackageType.masqueradeMask]    = 3;
        PackageInventory[PackageType.musicBox]          = 5;
        PackageInventory[PackageType.necklace]          = 6;
        PackageInventory[PackageType.pocketWatch]       = 4;
        PackageInventory[PackageType.steamPipe]         = 3;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PackageWasShot(object package)
    {
        PubSubListenerEvent e = (PubSubListenerEvent)package;
        PackageType packageType = ((Package)e.value).PackageType;
        PackageInventory[packageType] = Mathf.Max(0, PackageInventory[packageType] - 1);

        if (PackageInventory[packageType] <= 0) {
            PackageInventory.Remove(packageType);
        }
    }
}
