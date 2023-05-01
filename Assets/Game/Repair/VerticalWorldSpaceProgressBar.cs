using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalWorldSpaceProgressBar : MonoBehaviour
{
    [Range(0, 1.0f)]
    public float value;

    public RectTransform outerTransform;
    public RectTransform innerTransform;

    // Update is called once per frame
    void Update()
    {
        if (value < 0) { value = 0.0f; }
        if (value > 1.0f) { value = 1.0f; }

        var x = outerTransform.rect.width;
        var y = outerTransform.rect.height;
        var z = 0.0f;
        innerTransform.localPosition = new Vector3(0, (y * value / 2) - y/2, z);

        innerTransform.sizeDelta = new Vector2(x, y * value);
    }
}
