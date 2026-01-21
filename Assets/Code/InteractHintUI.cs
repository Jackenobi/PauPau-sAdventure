using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractHintUI : MonoBehaviour
{
    public Image hintImage;

    [Header("Pulse")]
    public float pulseScale = 1.2f;
    public float pulseDuration = 0.15f;

    Coroutine pulseRoutine;
    Vector3 originalScale;

    void Awake()
    {
        originalScale = hintImage.transform.localScale;
        hintImage.gameObject.SetActive(false);
    }

    public void Show()
    {
        hintImage.gameObject.SetActive(true);

        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(Pulse());
    }

    public void Hide()
    {
        hintImage.gameObject.SetActive(false);
    }

    IEnumerator Pulse()
    {
        Vector3 targetScale = originalScale * pulseScale;
        float t = 0f;

        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            hintImage.transform.localScale =
                Vector3.Lerp(originalScale, targetScale, t / pulseDuration);
            yield return null;
        }

        t = 0f;
        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            hintImage.transform.localScale =
                Vector3.Lerp(targetScale, originalScale, t / pulseDuration);
            yield return null;
        }

        hintImage.transform.localScale = originalScale;
    }
}
