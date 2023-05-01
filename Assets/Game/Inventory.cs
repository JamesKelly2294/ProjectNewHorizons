using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static string PackageName(PackageType package)
    {
        switch (package)
        {
            case PackageType.automaton: return "Automaton";
            case PackageType.gears: return "Gears";
            case PackageType.camera: return "Camera";
            case PackageType.goggles: return "Goggles";
            case PackageType.keyboard: return "Keyboard";
            case PackageType.masqueradeMask: return "Masquerade Mask";
            case PackageType.musicBox: return "Music Box";
            case PackageType.necklace: return "Necklace";
            case PackageType.pocketWatch: return "Pocket Watch";
            case PackageType.steamPipe: return "Steam Pipe";
            default: return "Unknown";
        }
    }

    public string CargoString()
    {
        var str = "";

        foreach (var entry in PackageInventory)
        {
            if (entry.Value == 0) { continue; }
            str += entry.Value + "x " + PackageName(entry.Key) + ", ";
        }

        if (str == "") { return str; }

        str = str.Remove(str.Length - 2, 2);
        return str;
    }

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
