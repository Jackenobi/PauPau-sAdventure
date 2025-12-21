using System.Collections;
using TMPro;
using UnityEngine;

public class InteractHintUI : MonoBehaviour
{
    public TMP_Text hintText;

    [Header("Pulse")]
    public float pulseScale = 1.2f;
    public float pulseDuration = 0.15f;

    Coroutine pulseRoutine;
    Vector3 originalScale;

    void Awake()
    {
        originalScale = hintText.transform.localScale;
        hintText.gameObject.SetActive(false);
    }

    public void Show()
    {
        hintText.gameObject.SetActive(true);

        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(Pulse());
    }

    public void Hide()
    {
        hintText.gameObject.SetActive(false);
    }

    IEnumerator Pulse()
    {
        Vector3 targetScale = originalScale * pulseScale;
        float t = 0f;

        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            hintText.transform.localScale =
                Vector3.Lerp(originalScale, targetScale, t / pulseDuration);
            yield return null;
        }

        t = 0f;
        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            hintText.transform.localScale =
                Vector3.Lerp(targetScale, originalScale, t / pulseDuration);
            yield return null;
        }

        hintText.transform.localScale = originalScale;
    }
}
