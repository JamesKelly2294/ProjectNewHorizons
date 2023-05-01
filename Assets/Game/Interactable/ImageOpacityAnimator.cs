using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageOpacityAnimator : MonoBehaviour
{

    public Image img;
    public AnimationCurve curve;
    public float total = 1f;
    public float current = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        current += Time.deltaTime;
        if (current > total) {
            current -= total;
        }

        img.color = new Color(img.color.r, img.color.g, img.color.b, curve.Evaluate(current / total));
    }
}
