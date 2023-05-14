using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrainLevelState {
    Intro,
    EnteringIntroSequence,
    WaitingToEnterGameplay,
    EnteringGameplay,
    Gameplay,
    EnteringOutroSequence,
    WaitingToExitOutro,
}

public enum Level: int
{
    One = 0,
    Two = 1,
    Three = 2,
}

public class TrainLevelManager : MonoBehaviour
{
    public TrainLevelState State = TrainLevelState.Intro;

    // TO HELL WITH SEPARATION OF CONCERNS
    private Train _theTrain;
    private Player _thePlayer;
    private CameraRig _theCameraRig;
    private SteamyGameManager _theSteamGameManager;
    private EntitySquadCoordinator _theSquadCoordinator;
    private GameObject _gameplayGUI;
    private MissionIntroGUI _missionIntroGUI;
    private MissionOutroGUI _missionOutroGUI;
    private Inventory _theInventory;

    // Lol
    public int TrueLevel = 1;
    public Level Level = Level.One;

    public float LevelProgress = 0f;
    public float LevelTime = 60f;
    private float currentLevelTime = 0f;

    public City OriginCity = City.steamerly;
    public City DestinationCity = City.geartonsteamshireville;

    public bool levelOver = false;
    public bool wonLevel = false;
    public bool lostLevel = false;

    private int enemiesKilledThisLevel = 0;
    private int enemiesKilledTotal = 0;

    private int packagesDeliveredThisLevel = 0;
    private int packagesDeliveredTotal = 0;

    private int packagesLostThisLevel = 0;
    private int packagesLostTotal = 0;

    private int moneyEarnedThisLevel = 0;
    private int moneyEarnedTotal = 0;

    // Start is called before the first frame update
    void Start()
    {
        _theTrain = FindFirstObjectByType<Train>();
        _thePlayer = FindFirstObjectByType<Player>();
        _theCameraRig = FindFirstObjectByType<CameraRig>();
        _theSteamGameManager = FindFirstObjectByType<SteamyGameManager>();
        _theSquadCoordinator = FindFirstObjectByType<EntitySquadCoordinator>();
        _missionIntroGUI = FindFirstObjectByType<MissionIntroGUI>(FindObjectsInactive.Include);
        _missionIntroGUI.gameObject.SetActive(false);
        _missionOutroGUI = FindFirstObjectByType<MissionOutroGUI>(FindObjectsInactive.Include);
        _missionOutroGUI.gameObject.SetActive(false);
        _theInventory = FindFirstObjectByType<Inventory>();

        _gameplayGUI = GameObject.FindGameObjectWithTag("GameplayGUI");

        _theSteamGameManager.OriginCity = OriginCity;
        _theSteamGameManager.DestinationCity = DestinationCity;

        if (State == TrainLevelState.Intro)
        {
            ResetState();
            StartCoroutine(PlayIntroSequence());
        }
        else
        {
            _theCameraRig.SetToGameplayAngle(Level);
        }
    }

    private void ResetState()
    {
        currentLevelTime = 0.0f;
        LevelProgress = 0.0f;
        levelOver = false;
        wonLevel = false;
        lostLevel = false;

        enemiesKilledThisLevel = 0;
        packagesDeliveredThisLevel = 0;
        packagesLostThisLevel = 0;
        moneyEarnedThisLevel = 0;

        _theInventory.FillInventoryForLevel(Level);
        _theSquadCoordinator.ResetState();
        _theTrain.ResetState();
    }

    public void EnemyKilled()
    {
        enemiesKilledThisLevel += 1;
        enemiesKilledTotal += 1;
    }

    public void PackageDelivered()
    {
        packagesDeliveredThisLevel += 1;
        packagesDeliveredTotal += 1;
    }

    public void PackageLost()
    {
        packagesLostThisLevel += 1;
        packagesLostTotal += 1;
    }

    public void MoneyEarned(PubSubListenerEvent e)
    {
        moneyEarnedThisLevel += (int)e.value;
        moneyEarnedTotal += (int)e.value;
    }

    private void FireLevelCompletedEvent(bool levelWon)
    {
        JamAnalytics.Instance.FireLevelCompletedEvent(
            levelNumber: (int)Level,
            levelWon: levelWon,
            enemiesKilledTotal: enemiesKilledTotal,
            moneyEarnedTotal: moneyEarnedTotal,
            packagesDeliveredTotal: packagesDeliveredTotal,
            packagesLostTotal: packagesLostTotal,
            enemiesKilledThisLevel: enemiesKilledThisLevel,
            moneyEarnedThisLevel: moneyEarnedThisLevel,
            packagesDeliveredThisLevel: packagesDeliveredThisLevel,
            packagesLostThisLevel: packagesLostThisLevel);
    }

    public void IncrementLevel()
    {
        FireLevelCompletedEvent(true);

        //ahahahahaha, coding!
        if (Level == Level.One)
        {
            Level = Level.Two;
        }
        else if (Level == Level.Two)
        {
            Level = Level.Three;
        }

        TrueLevel += 1;

        OriginCity = NextCity(OriginCity);
        DestinationCity = NextCity(DestinationCity);

        _theSteamGameManager.OriginCity = OriginCity;
        _theSteamGameManager.DestinationCity = DestinationCity;
    }

    // NO TIME TO IMPLEMENT DYNAMIC LOGIC LOLOLOL
    public City NextCity(City city)
    {
        switch (city)
        {
            case City.gearton:
                return City.geartonsteamshireville;
            case City.geartonsteamshireville:
                return City.steamerly;
            case City.steamerly:
                return City.gearton;
            default:
                return City.gearton;
        }
    }

    private float _fadeFromBlackTime = 1.0f;

    IEnumerator PlayOutroSequence()
    {
        State = TrainLevelState.EnteringOutroSequence;

        if (wonLevel)
        {
            _missionOutroGUI.HeaderLabel.text = "J.B. Gearsworth III, Captain";
            _missionOutroGUI.ShipStatusLabel.text = "Hull integrity maintained";
            if(Level == Level.Three)
            {
                _missionOutroGUI.PressToContinueLabel.text = "Press Space To Win";
            }
            else
            {
                _missionOutroGUI.PressToContinueLabel.text = "Press Space For Your Next Delivery";
            }
            AudioManager.Instance.Play("Win");
        }
        else if (lostLevel)
        {
            _missionOutroGUI.HeaderLabel.text = "Brass & Copper Deliveries, Co.";
            _missionOutroGUI.ShipStatusLabel.text = "Hull integrity lost. Captain MIA, assumed deceased - estate invoiced for asset loss.";
            _missionOutroGUI.PressToContinueLabel.text = "Press Space";
            AudioManager.Instance.Play("Lost");
        }

        _missionOutroGUI.CargoRemainingLabel.text = _theInventory.CargoString(_theInventory.PackageInventory);
        _missionOutroGUI.CargoDeliveredLabel.text = _theInventory.CargoString(_theInventory.PackagesDelivered);
        _missionOutroGUI.CargoLostLabel.text = _theInventory.CargoString(_theInventory.PackagesLost);

        _missionOutroGUI.BountiesLabel.text = enemiesKilledThisLevel + " Steam Punks liquidated";

        _missionOutroGUI.PressToContinueLabel.color = Color.clear;

        _theSquadCoordinator.ResetState();

        _thePlayer.enabled = false;
        _gameplayGUI.SetActive(false);
        if (wonLevel)
        {
            _theCameraRig.FadeToBlackInstant();
            _theCameraRig.ShowBlurInstant();
            _theCameraRig.FadeFromBlack(duration: _fadeFromBlackTime);
        }
        else
        {
            _theCameraRig.ShowBlackBackground();
        }
        _missionOutroGUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(_fadeFromBlackTime);

        _missionOutroGUI.PressToContinueLabel.color = Color.white;
        State = TrainLevelState.WaitingToExitOutro;
    }

    IEnumerator PlayOutroToIntroTransition()
    {
        _theCameraRig.FadeToBlack(duration: _fadeFromBlackTime);
        yield return new WaitForSeconds(_fadeFromBlackTime);
        _missionOutroGUI.gameObject.SetActive(false);

        IncrementLevel();
        ResetState();

        yield return StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        State = TrainLevelState.EnteringIntroSequence;

        _missionIntroGUI.OriginLabel.text = SteamyGameManager.CityName(OriginCity);
        _missionIntroGUI.DestinationLabel.text = SteamyGameManager.CityName(DestinationCity);
        _missionIntroGUI.CargoLabel.text = _theInventory.CargoString(_theInventory.PackageInventory);
        _missionIntroGUI.StartLabel.color = Color.clear;

        _theCameraRig.FadeToBlackInstant();

        _thePlayer.enabled = false;
        _gameplayGUI.SetActive(false);
        _theCameraRig.ShowBlurInstant();
        _theCameraRig.SetToCinematicAngle();
        _theCameraRig.FadeFromBlack(duration: _fadeFromBlackTime);
        _missionIntroGUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(_fadeFromBlackTime);

        _missionIntroGUI.StartLabel.color = Color.white;
        State = TrainLevelState.WaitingToEnterGameplay;
    }

    IEnumerator EnterGameplay()
    {
        State = TrainLevelState.EnteringGameplay;

        _theCameraRig.FadeToBlack(duration: _fadeFromBlackTime);
        yield return new WaitForSeconds(_fadeFromBlackTime);

        _theCameraRig.FadeToBlackInstant();
        _theCameraRig.HideBlurInstant();
        _theCameraRig.SetToGameplayAngle(Level);
        _missionIntroGUI.gameObject.SetActive(false);
        _gameplayGUI.SetActive(true);

        _theCameraRig.FadeFromBlack(duration: _fadeFromBlackTime);
        yield return new WaitForSeconds(_fadeFromBlackTime / 2.0f);

        _thePlayer.enabled = true;
        State = TrainLevelState.Gameplay;
    }

    // Update is called once per frame
    void Update()
    {
        if (State == TrainLevelState.Gameplay)
        {
            GameplayTick();
        }
        else if (State == TrainLevelState.WaitingToEnterGameplay)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartCoroutine(EnterGameplay());
            }
        }
        else if (State == TrainLevelState.WaitingToExitOutro)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (wonLevel)
                {
                    if (Level == Level.Three)
                    {
                        var gm = FindFirstObjectByType<GameManager>();
                        if (gm != null)
                        {
                            FireLevelCompletedEvent(true);
                            gm.ShowWinScreen();
                        }
                    }
                    else
                    {
                        State = TrainLevelState.Intro;
                        StartCoroutine(PlayOutroToIntroTransition());
                    }
                }
                else
                {
                    var gm = FindFirstObjectByType<GameManager>();
                    if (gm != null)
                    {
                        FireLevelCompletedEvent(false);
                        gm.ShowLoseScreen();
                    }
                }
            }
        }
    }

    public void UpdateLevelProgress()
    {
        currentLevelTime += Time.deltaTime;
        LevelProgress = Mathf.Clamp01(currentLevelTime / LevelTime);

        _theSteamGameManager.LevelProgress = LevelProgress;
        _theSteamGameManager.CurrentLevelTime = currentLevelTime;
    }

    public void GameplayTick()
    {
        UpdateLevelProgress();
        EvaluateWinLossState();
        if (levelOver)
        {
            StartCoroutine(PlayOutroSequence());
        }
    }

    public void EvaluateWinLossState()
    {
        if (levelOver)
        {
            return;
        }

        if (currentLevelTime > LevelTime)
        {
            levelOver = true;
            wonLevel = true;
        }
        else if (_theTrain.LocomotiveDamageable.CurrentHealth <= 0)
        {
            levelOver = true;
            lostLevel = true;
        }
    }
}
