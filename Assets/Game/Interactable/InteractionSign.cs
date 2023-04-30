using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSign : MonoBehaviour
{

    public Color ForgroundColor;
    public Color BackgroundColor;
    public Sprite Icon;

    public Color ExitForground;
    public Color ExitBackground;
    public Sprite ExitSprite;

    public Image image;
    public Image panel;

    // Start is called before the first frame update
    void Start()
    {
        SwitchBackToNormal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToExitSign()
    {
        image.sprite = ExitSprite;
        image.color = ExitForground;
        panel.color = ExitBackground;
    }

    public void SwitchBackToNormal()
    {
        image.sprite = Icon;
        image.color = ForgroundColor;
        panel.color = BackgroundColor;
    }
}
