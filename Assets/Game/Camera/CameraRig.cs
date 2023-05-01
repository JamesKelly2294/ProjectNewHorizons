using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRig : MonoBehaviour
{
    public Camera Camera;
    public CameraSway CameraSway;

    public Transform CinematicAngle;
    public Transform CinematicAngleLevelTwo;
    public Transform CinematicAngleLevelThree;
    public Transform GameplayAngle;

    public GameObject blurEffect;
    public Image fullscreenFadeImage;
    public Image fullscreenBlackBackground;

    public bool IsFading { get; private set; }
    public bool IsFadedToBlack { get; private set; }

    private Coroutine _currentCoroutine;

    public void SetToAngle(Transform t)
    {
        CameraSway.enabled = false;
        Camera.transform.position = t.position;
        Camera.transform.rotation = t.rotation;
        CameraSway.enabled = true;
    }

    public void SetToCinematicAngle()
    {
        SetToAngle(CinematicAngle);
    }

    public void SetToGameplayAngle(Level level = Level.One)
    {
        switch (level)
        {
            case Level.Two:
                SetToAngle(CinematicAngleLevelTwo);
                break;
            case Level.Three:
                SetToAngle(CinematicAngleLevelThree);
                break;
            default:
                SetToAngle(GameplayAngle);
                break;
        }
    }

    public void FadeToBlack(float duration=1.0f)
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        IsFading = false;

        _currentCoroutine = StartCoroutine(Fade(Color.black, toBlack: true, time: duration));
    }

    public void FadeFromBlack(float duration = 1.0f)
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        IsFading = false;

        _currentCoroutine = StartCoroutine(Fade(Color.black, toBlack: false, time: duration));
    }

    public void ShowBlackBackground()
    {
        fullscreenBlackBackground.color = Color.black;
        fullscreenBlackBackground.gameObject.SetActive(true);
    }

    public void HideBlackBackground()
    {
        fullscreenBlackBackground.color = Color.clear;
        fullscreenBlackBackground.gameObject.SetActive(false);
    }

    public void FadeToBlackInstant()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        IsFading = false;

        fullscreenFadeImage.color = Color.black;
        fullscreenFadeImage.gameObject.SetActive(true);
        IsFadedToBlack = true;
    }

    public void FadeFromBlackInstant()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        IsFading = false;

        fullscreenFadeImage.color = Color.clear;
        fullscreenFadeImage.gameObject.SetActive(false);
        IsFadedToBlack = false;
    }

    public void ShowBlurInstant()
    {
        blurEffect.SetActive(true);
    }

    public void HideBlurInstant()
    {
        blurEffect.SetActive(false);
    }

    private IEnumerator Fade(Color color, bool toBlack = true, float time = 1.0f)
    {
        IsFading = true;
        float t = 0.0f;
        fullscreenFadeImage.gameObject.SetActive(true);
        Color initialColor = new Color(color.r, color.g, color.b, toBlack ? 0.0f : 1.0f);
        fullscreenFadeImage.color = initialColor;

        if (toBlack)
        {
            while (fullscreenFadeImage.color.a < 0.99)
            {
                t += Time.deltaTime;
                float progress = t / time;
                Color fadeColor = new Color(color.r, color.g, color.b, progress);
                fullscreenFadeImage.color = fadeColor;
                yield return null;
            }
            
            Color finalColor = new Color(color.r, color.g, color.b, 1.0f);
            fullscreenFadeImage.color = finalColor;
        }
        else
        {
            while (fullscreenFadeImage.color.a > 0.01)
            {
                t += Time.deltaTime;
                float progress = t / time;
                Color fadeColor = new Color(color.r, color.g, color.b, 1.0f - progress);
                fullscreenFadeImage.color = fadeColor;
                yield return null;
            }

            Color finalColor = new Color(color.r, color.g, color.b, 0.0f);
            fullscreenFadeImage.color = finalColor;
            fullscreenFadeImage.gameObject.SetActive(false);
        }
        IsFading = false;
    }
}
