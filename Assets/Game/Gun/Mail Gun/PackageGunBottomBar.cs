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

            Color itemColor = PackageGunBottomBar.GetPackageTypePrimaryColor(item.Key);
            listItem.icon.color = itemColor;
            listItem.icon.sprite = PackageGunBottomBar.GetPackageTypeSprite(item.Key);
            listItem.selectionOutline.color = itemColor;
            listItem.backgroundFill.color = new Color(itemColor.r / 4, itemColor.g / 4, itemColor.b / 4, 0.9f);

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
            case PackageType.automaton:         return Color.HSVToRGB(0.1f, 0.8f, 1);
            case PackageType.camera:            return Color.HSVToRGB(0.0f, 0.8f, 1);
            case PackageType.gears:             return Color.HSVToRGB(0.2f, 0.8f, 1);
            case PackageType.goggles:           return Color.HSVToRGB(0.3f, 0.8f, 1);
            case PackageType.keyboard:          return Color.HSVToRGB(0.4f, 0.8f, 1);
            case PackageType.masqueradeMask:    return Color.HSVToRGB(0.5f, 0.8f, 1);
            case PackageType.musicBox:          return Color.HSVToRGB(0.6f, 0.8f, 1.5f);
            case PackageType.necklace:          return Color.HSVToRGB(0.7f, 0.8f, 1.5f);
            case PackageType.pocketWatch:       return Color.HSVToRGB(0.8f, 0.8f, 1);
            case PackageType.steamPipe:         return Color.HSVToRGB(0.9f, 0.8f, 1);
            default:                            return Color.HSVToRGB(1.0f, 0.8f, 1);
        }
    }

    public static int GetPackageTypeMonitaryValue(PackageType packageType)
    {
        switch (packageType)
        {
            case PackageType.automaton:         return    50_000;
            case PackageType.camera:            return    20_000;
            case PackageType.gears:             return      5000;
            case PackageType.goggles:           return      2500;
            case PackageType.keyboard:          return     15000;
            case PackageType.masqueradeMask:    return     35000;
            case PackageType.musicBox:          return     17500;
            case PackageType.necklace:          return     12500;
            case PackageType.pocketWatch:       return      8500;
            case PackageType.steamPipe:         return      1000;
            default:                            return         0;
        }
    }

    public static Sprite GetPackageTypeSprite(PackageType packageType)
    {
        switch (packageType)
        {
            case PackageType.automaton:         return  SteamyGameManager.Instance.automatonSprite;
            case PackageType.camera:            return  SteamyGameManager.Instance.cameraSprite;
            case PackageType.gears:             return  SteamyGameManager.Instance.gearsSprite;
            case PackageType.goggles:           return  SteamyGameManager.Instance.gogglesSprite;
            case PackageType.keyboard:          return  SteamyGameManager.Instance.keyboardSprite;
            case PackageType.masqueradeMask:    return  SteamyGameManager.Instance.masqueradeMaskSprite;
            case PackageType.musicBox:          return  SteamyGameManager.Instance.musicBoxSprite;
            case PackageType.necklace:          return  SteamyGameManager.Instance.necklaceSprite;
            case PackageType.pocketWatch:       return  SteamyGameManager.Instance.pocketWatchSprite;
            case PackageType.steamPipe:         return  SteamyGameManager.Instance.steamPipeSprite;
            default:                            return  SteamyGameManager.Instance.automatonSprite;
        }
    }
}
