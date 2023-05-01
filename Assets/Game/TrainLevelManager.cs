using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrainLevelState {
    Intro = 0,
    Gameplay = 1,
    Outtro = 2,
}

public class TrainLevelManager : MonoBehaviour
{
    public TrainLevelState State = TrainLevelState.Intro;

    private Train _theTrain;

    // Start is called before the first frame update
    void Start()
    {
        _theTrain = FindFirstObjectByType<Train>();
    }

    // Update is called once per frame
    void Update()
    {

        CheckGameState();
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
