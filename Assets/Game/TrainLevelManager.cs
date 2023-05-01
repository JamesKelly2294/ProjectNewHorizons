using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrainLevelState {
    Intro,
    EnteringIntroSequence,
    WaitingToEnterGameplay,
    EnteringGameplay,
    Gameplay,
    Outro,
}

public class TrainLevelManager : MonoBehaviour
{
    public TrainLevelState State = TrainLevelState.Intro;

    // TO HELL WITH SEPARATION OF CONCERNS
    private Train _theTrain;
    private Player _thePlayer;
    private CameraRig _theCameraRig;
    private SteamyGameManager _theSteamGameManager;
    private GameObject _gameplayGUI;
    private MissionIntroGUI _missionIntroGUI;
    private Inventory _theInventory;

    public float LevelProgress = 0f;
    public float LevelTime = 60f;
    private float currentLevelTime = 0f;

    public City OriginCity = City.steamerly;
    public City DestinationCity = City.geartonsteamshireville;

    // Start is called before the first frame update
    void Start()
    {
        _theTrain = FindFirstObjectByType<Train>();
        _thePlayer = FindFirstObjectByType<Player>();
        _theCameraRig = FindFirstObjectByType<CameraRig>();
        _theSteamGameManager = FindFirstObjectByType<SteamyGameManager>();
        _missionIntroGUI = FindFirstObjectByType<MissionIntroGUI>(FindObjectsInactive.Include);
        _missionIntroGUI.gameObject.SetActive(false);
        _theInventory = FindFirstObjectByType<Inventory>();

        _gameplayGUI = GameObject.FindGameObjectWithTag("GameplayGUI");

        _theSteamGameManager.OriginCity = OriginCity;
        _theSteamGameManager.DestinationCity = DestinationCity;

        if (State == TrainLevelState.Intro)
        {
            StartCoroutine(PlayIntroSequence());
        }
    }

    private float _fadeFromBlackTime = 2.5f;

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
    }

    public void UpdateLevelProgress()
    {
        currentLevelTime += Time.deltaTime;
        LevelProgress = currentLevelTime / LevelTime;
        if (LevelProgress > 1)
        {

            // Just loop for now...
            currentLevelTime -= LevelTime;
        }

        _theSteamGameManager.LevelProgress = LevelProgress;
        _theSteamGameManager.CurrentLevelTime = currentLevelTime;
    }

    public void GameplayTick()
    {
        UpdateLevelProgress();

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
