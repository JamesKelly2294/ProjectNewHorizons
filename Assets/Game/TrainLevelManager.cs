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

    public float LevelProgress = 0f;
    public float LevelTime = 60f;
    private float currentLevelTime = 0f;

    public City OriginCity = City.steamerly;
    public City DestinationCity = City.geartonsteamshireville;

    public bool levelOver = false;
    public bool wonLevel = false;
    public bool lostLevel = false;

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

        ResetState();
        if (State == TrainLevelState.Intro)
        {
            StartCoroutine(PlayIntroSequence());
        }
    }

    private void ResetState()
    {
        currentLevelTime = 0.0f;
        levelOver = false;
        wonLevel = false;
        lostLevel = false;
        _theSquadCoordinator.ResetState();
    }

    private float _fadeFromBlackTime = 2.5f;

    IEnumerator PlayOutroSequence()
    {
        State = TrainLevelState.EnteringOutroSequence;

        if (wonLevel)
        {
            _missionOutroGUI.HeaderLabel.text = "J.B. Gearsworth III, Captain";
            _missionOutroGUI.ShipStatusLabel.text = "Hull integrity maintained";
            _missionOutroGUI.PressToContinueLabel.text = "Press Space For Your Next Delivery";
        }
        else if (lostLevel)
        {
            _missionOutroGUI.HeaderLabel.text = "Brass & Copper Deliveries, Co.";
            _missionOutroGUI.ShipStatusLabel.text = "Hull integrity lost. Captain MIA, assumed deceased - estate invoiced for asset loss.";
            _missionOutroGUI.PressToContinueLabel.text = "Press Space";
        }

        _missionOutroGUI.PressToContinueLabel.color = Color.clear;

        ResetState();
        _thePlayer.enabled = false;
        _gameplayGUI.SetActive(false);
        if (wonLevel)
        {
            _theCameraRig.FadeToBlackInstant();
            _theCameraRig.ShowBlurInstant();
            _theCameraRig.SetToCinematicAngle();
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
        yield return StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        State = TrainLevelState.EnteringIntroSequence;

        _missionIntroGUI.OriginLabel.text = SteamyGameManager.CityName(OriginCity);
        _missionIntroGUI.DestinationLabel.text = SteamyGameManager.CityName(DestinationCity);
        _missionIntroGUI.CargoLabel.text = _theInventory.CargoString();
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
        _theCameraRig.SetToGameplayAngle();
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
                    State = TrainLevelState.Intro;
                    StartCoroutine(PlayOutroToIntroTransition());
                }
                else
                {
                    var gm = FindFirstObjectByType<GameManager>();
                    gm.ShowLoseScreen();
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
