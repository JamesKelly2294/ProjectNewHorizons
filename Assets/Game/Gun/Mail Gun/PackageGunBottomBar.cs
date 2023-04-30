using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PackageGunBottomBar : MonoBehaviour
{

    public GameObject ListLocation;
    public GameObject ListItemPrefab;
    public GameObject NoPackagesListItemPrefab;
    public TextMeshProUGUI SelectedItemText;
    public int Selection = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Update selection
        if (Input.GetKeyDown(KeyCode.A) || Input.GetAxis("Mouse ScrollWheel") < 0) {
            Selection -= 1;
        } else if (Input.GetKeyDown(KeyCode.D) || Input.GetAxis("Mouse ScrollWheel") > 0) {
            Selection += 1;
        }

        // Delete the existing elements in ListLocation
        int childCount = ListLocation.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject child = ListLocation.transform.GetChild(i).gameObject;
            UnityEngine.Object.Destroy(child);
        }

        // Get the inventory
        Dictionary<PackageType, int> inventory = Inventory.Instance.PackageInventory;
        
        // Exit early if we don't have anything to do
        if (inventory.Count <= 0) {
            Inventory.Instance.SelectionType = null;
            Instantiate(NoPackagesListItemPrefab, ListLocation.transform);
            SelectedItemText.text = "No Packages Left!";
            return;
        }

        Selection = (inventory.Keys.Count + Selection) % (inventory.Keys.Count);

        // Add new ones.
        int current = 0;
        foreach (var item in inventory)
        {
            GameObject newItem = Instantiate(ListItemPrefab, ListLocation.transform);
            PackageBarListItem listItem = newItem.GetComponent<PackageBarListItem>();
            listItem.selectionOutline.enabled = current == Selection;
            listItem.badgeText.text = "" + item.Value;
            listItem.badge.SetActive(item.Value > 1);
            listItem.icon.color = PackageGunBottomBar.GetPackageTypePrimaryColor(item.Key);
            listItem.selectionOutline.color = PackageGunBottomBar.GetPackageTypePrimaryColor(item.Key);

            if (current == Selection) {
                SelectedItemText.text = PackageGunBottomBar.GetPackageTypeString(item.Key);
                Inventory.Instance.SelectionType = item.Key;
            }

            current++;
        }
    }

    public static string GetPackageTypeString(PackageType packageType)
    {
        switch (packageType)
        {
            case PackageType.automaton:         return "Automaton";
            case PackageType.camera:            return "Camera";
            case PackageType.gears:             return "Gears";
            case PackageType.goggles:           return "Goggles";
            case PackageType.keyboard:          return "Keyboard";
            case PackageType.masqueradeMask:    return "Masquerade Mask";
            case PackageType.musicBox:          return "Music Box";
            case PackageType.necklace:          return "Necklace";
            case PackageType.pocketWatch:       return "Pocket Watch";
            case PackageType.steamPipe:         return "Steam Pipe";
            default:                            return "Unknown";
        }
    }

    public static Color GetPackageTypePrimaryColor(PackageType packageType)
    {
        switch (packageType)
        {
            case PackageType.automaton:         return Color.HSVToRGB(0.0f, 1, 1);
            case PackageType.camera:            return Color.HSVToRGB(0.1f, 1, 1);
            case PackageType.gears:             return Color.HSVToRGB(0.2f, 1, 1);
            case PackageType.goggles:           return Color.HSVToRGB(0.3f, 1, 1);
            case PackageType.keyboard:          return Color.HSVToRGB(0.4f, 1, 1);
            case PackageType.masqueradeMask:    return Color.HSVToRGB(0.5f, 1, 1);
            case PackageType.musicBox:          return Color.HSVToRGB(0.6f, 1, 1);
            case PackageType.necklace:          return Color.HSVToRGB(0.7f, 1, 1);
            case PackageType.pocketWatch:       return Color.HSVToRGB(0.8f, 1, 1);
            case PackageType.steamPipe:         return Color.HSVToRGB(0.9f, 1, 1);
            default:                            return Color.HSVToRGB(1.0f, 1, 1);
        }
    }

    public static int GetPackageTypeMonitaryValue(PackageType packageType)
    {
        switch (packageType)
        {
            case PackageType.automaton:         return   100_000;
            case PackageType.camera:            return    10_000;
            case PackageType.gears:             return        50;
            case PackageType.goggles:           return        50;
            case PackageType.keyboard:          return       100;
            case PackageType.masqueradeMask:    return       500;
            case PackageType.musicBox:          return       200;
            case PackageType.necklace:          return     4_000;
            case PackageType.pocketWatch:       return     2_000;
            case PackageType.steamPipe:         return       100;
            default:                            return         0;
        }
    }
}
