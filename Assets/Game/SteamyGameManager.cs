using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamyGameManager : MonoBehaviour
{

    public float LevelProgress = 0f;
    public float LevelTime = 60f;
    private float currentLevelTime = 0f;

    public City OriginCity = City.steamerly;
    public City DestinationCity = City.geartonsteamshireville;

    public int VictoryPoints = 0;
    public int Money = 0;

    public int DeliveredBoxCount = 0;
    public int FailedBoxCount = 0;

    private Train _theTrain;

    // Start is called before the first frame update
    void Start()
    {
        _theTrain = FindFirstObjectByType<Train>();
    }

    // Update is called once per frame
    void Update()
    {   
        currentLevelTime += Time.deltaTime;
        LevelProgress = currentLevelTime / LevelTime;
        if (LevelProgress > 1) {

            // Just loop for now...
            currentLevelTime -= LevelTime;
        }

        // Calculate total package inventory
        int PackageInventory = 0;
        if (Inventory.Instance != null) {
            foreach (var item in Inventory.Instance.PackageInventory) {
                PackageInventory += item.Value;
            }
        }

        StatusBar sb = StatusBar.Instance;
        if (sb != null) {

            // City progress bar
            sb.ProgressBar.progress = Mathf.Max(Mathf.Min(LevelProgress, 1f), 0f);
            sb.OriginCityNameText.text = SteamyGameManager.CityName(OriginCity);
            sb.DestinationCityNameText.text = SteamyGameManager.CityName(DestinationCity);

            // Current Info Display
            sb.PendingBoxCountText.text = PackageInventory.ToString("N0");
            sb.DeliveredBoxCountText.text = DeliveredBoxCount.ToString("N0");
            sb.FailedBoxCountText.text = FailedBoxCount.ToString("N0");
            sb.MoneyText.text = Money.ToString("N0");
            sb.VictoryPointsText.text = VictoryPoints.ToString("N0");
        }

        CheckGameState();
    }

    public static string CityName(City city)
    {
        switch (city)
        {
            case City.gearton:                  return "Gearton";
            case City.steamerly:                return "Steamerly";
            case City.geartonsteamshireville:   return "Gearsteamshire"; // Gear-Stem-sure
            default:                            return "Unknown";
        }
    }

    public void PackageDelivered(PubSubListenerEvent e)
    {
        DeliveredBoxCount += 1;
        VictoryPoints += 1;
        AudioManager.Instance.Play("Package/Delivered");
        Money += PackageGunBottomBar.GetPackageTypeMonitaryValue(e.sender.GetComponent<Package>().PackageType);
    }

    public void PackageFailedToBeDelivered()
    {
        FailedBoxCount += 1;
        AudioManager.Instance.Play("Package/Failed");
    }

    public void CheckGameState()
    {
        if (_theTrain.LocomotiveDamageable.CurrentHealth <= 0)
        {
            BeginLoss();
        }
    }

    public void BeginLoss()
    {
        var gm = FindFirstObjectByType<GameManager>();

        if (gm == null)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            gm.ShowLoseScreen();
        }
    }
}

public enum City
{
    gearton,
    steamerly,
    geartonsteamshireville
}