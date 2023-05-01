using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PackageRecipientDesireBox : MonoBehaviour
{

    public List<ImageOpacityAnimator> chevrons;
    public float Desire;
    public GameObject helpSign;
    public Gradient gradient;

    public Image forgroundImage;
    public Image backgroundFill;
    public GameObject badge;
    public TextMeshProUGUI badgeText;

    public PackageType PackageType;
    public int Count = 1;

    public AnimationCurve switchToEmoteCurve;
    public bool ShouldEmote = false;
    private float currentEmoteCurvePosition = 0f;
    public float emoteCurveTime = 1f;

    public CanvasGroup waitingGroup;
    public CanvasGroup emoteGroup;

    public GameObject happyEmote;
    public GameObject mehEmote;
    public GameObject sadEmote;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Calculate waiting animation
        Desire = Mathf.Max(0f, Mathf.Min(1f, Desire));

        Color color = gradient.Evaluate((Desire * 1.33f) - 0.25f);
        helpSign.SetActive(Desire > 0.75);

        int activeChevrons = (int)(((float) (1 + chevrons.Count)) * Desire) - 1;
        for (int i = 0; i < chevrons.Count; i++)
        {
            Image img = chevrons[i].GetComponent<Image>();
            img.enabled = activeChevrons >= i;
            img.color = new Color(color.r, color.g, color.b, img.color.a);
        }

        Color itemColor = PackageGunBottomBar.GetPackageTypePrimaryColor(PackageType);
        badgeText.text = Count.ToString("N0");
        badge.SetActive(Count > 1);
        backgroundFill.color = new Color(itemColor.r / 4, itemColor.g / 4, itemColor.b / 4, 0.9f);
        forgroundImage.color = itemColor;
        forgroundImage.sprite = PackageGunBottomBar.GetPackageTypeSprite(PackageType);


        // Animate to emote
        if (ShouldEmote) {
            currentEmoteCurvePosition += Time.deltaTime;
        } else {
            currentEmoteCurvePosition -= Time.deltaTime;
        }
        currentEmoteCurvePosition = Mathf.Max(0, Mathf.Min(emoteCurveTime, currentEmoteCurvePosition));
        waitingGroup.alpha = 1 - switchToEmoteCurve.Evaluate(currentEmoteCurvePosition / emoteCurveTime);
        emoteGroup.alpha = switchToEmoteCurve.Evaluate(currentEmoteCurvePosition / emoteCurveTime);

        sadEmote.SetActive(Desire > 0.99);
        mehEmote.SetActive(Desire > 0.75 && Desire <= 0.99);
        happyEmote.SetActive(Desire <= 0.75);
    }
}
