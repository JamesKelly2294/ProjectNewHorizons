using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusBar : MonoBehaviour
{

    public static StatusBar Instance;

    public ProgressBar ProgressBar;
    public TextMeshProUGUI OriginCityNameText;
    public TextMeshProUGUI DestinationCityNameText;

    public TextMeshProUGUI PendingBoxCountText;
    public TextMeshProUGUI DeliveredBoxCountText;
    public TextMeshProUGUI FailedBoxCountText;
    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI VictoryPointsText;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
