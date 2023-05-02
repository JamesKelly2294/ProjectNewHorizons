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

    public string CargoString(Dictionary<PackageType, int> packages)
    {
        var str = "";

        foreach (var entry in packages)
        {
            if (entry.Value == 0) { continue; }
            str += entry.Value + "x " + PackageName(entry.Key) + ", ";
        }

        if (str == "") { return str; }

        str = str.Remove(str.Length - 2, 2);
        return str;
    }

    public static Inventory Instance;


    public Dictionary<PackageType, int> PackagesDelivered = new Dictionary<PackageType, int>();
    public Dictionary<PackageType, int> PackagesLost = new Dictionary<PackageType, int>();

    public Dictionary<PackageType, int> PackageInventory = new Dictionary<PackageType, int>();
    public PackageType? SelectionType = null;

    void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        FillInventoryForLevel(Level.One);
    }

    public void PackageWasShot(object package)
    {
        PubSubListenerEvent e = (PubSubListenerEvent)package;
        PackageType packageType = ((Package)e.value).PackageType;
        PackageInventory[packageType] = Mathf.Max(0, PackageInventory[packageType] - 1);

        if (PackageInventory[packageType] <= 0)
        {
            PackageInventory.Remove(packageType);
        }
    }

    public void PackageDelivered(PubSubListenerEvent e)
    {
        var deliveredPackage = e.sender.GetComponent<Package>();

        if (!PackagesDelivered.ContainsKey(deliveredPackage.PackageType))
        {
            PackagesDelivered[deliveredPackage.PackageType] = 1;
        }
        else
        {
            PackagesDelivered[deliveredPackage.PackageType] += 1;
        }
    }

    public void PackageFailedToBeDelivered(PubSubListenerEvent e)
    {
        var lostPackage = e.sender.GetComponent<Package>();

        if (!PackagesLost.ContainsKey(lostPackage.PackageType))
        {
            PackagesLost[lostPackage.PackageType] = 1;
        }
        else
        {
            PackagesLost[lostPackage.PackageType] += 1;
        }
    }

    public void Clear()
    {
        PackagesDelivered.Clear();
        PackagesLost.Clear();
        PackageInventory.Clear();
    }

    public void FillInventoryForLevel(Level level)
    {
        Clear();
        switch (level)
        {
            case Level.Two:
                PackageInventory[PackageType.automaton] = 1;
                PackageInventory[PackageType.gears] = 4;
                PackageInventory[PackageType.camera] = 2;
                PackageInventory[PackageType.keyboard] = 2;
                PackageInventory[PackageType.masqueradeMask] = 2;
                PackageInventory[PackageType.musicBox] = 2;
                PackageInventory[PackageType.necklace] = 2;
                PackageInventory[PackageType.steamPipe] = 4;
                break;

            case Level.Three:
                PackageInventory[PackageType.automaton] = 2;
                PackageInventory[PackageType.gears] = 4;
                PackageInventory[PackageType.camera] = 4;
                PackageInventory[PackageType.goggles] = 4;
                PackageInventory[PackageType.keyboard] = 4;
                PackageInventory[PackageType.masqueradeMask] = 4;
                PackageInventory[PackageType.musicBox] = 4;
                PackageInventory[PackageType.necklace] = 4;
                PackageInventory[PackageType.pocketWatch] = 4;
                PackageInventory[PackageType.steamPipe] = 4;
                break;

            default:
                PackageInventory[PackageType.automaton] = 1;
                PackageInventory[PackageType.gears] = 2;
                PackageInventory[PackageType.steamPipe] = 2;
                PackageInventory[PackageType.goggles] = 4;
                break;
        }
    }
}
