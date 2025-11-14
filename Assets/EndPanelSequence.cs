using System.Collections;
using UnityEngine;
using TMPro;

public class EndPanelSequence : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup panelGroup;         // CanvasGroup on EndPanel
    public TextMeshProUGUI endText;        // Text child

    [Header("Timings")]
    public float backgroundFadeDuration = 1.5f;
    public float textFadeDuration = 1f;
    public float textHoldTime = 4f;

    [Header("Start Automatically?")]
    public bool playOnEnable = true;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            StartCoroutine(PlaySequence());
        }
    }

    public void StartSequence()
    {
        // You can call this from another script if you don't want auto start on enable
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // Make sure panel is visible (alpha 0 -> 1)
        yield return StartCoroutine(FadeCanvasGroup(panelGroup, 0f, 1f, backgroundFadeDuration));

        // Show line 1
        yield return StartCoroutine(ShowLine(
            "You tell the child it is loved and not alone and there is nothing to be scared of. " +
            "All of a sudden the room lights up and changes."
        ));

        // Show line 2
        yield return StartCoroutine(ShowLine(
            "It looks quite similar, but yet different. The colours are bright and the sun is shining " +
            "through a window onto a young boy playing on the carpet."
        ));

        // Show line 3 (you can decide if you want this to fade out or stay)
        yield return StartCoroutine(ShowLine(
            "Is this where we have been all along?",
            fadeOutAtEnd: false        // set to true if you want it to fade out as well
        ));

        // If you want to do something after the last line (e.g. load a new scene),
        // you can add that here.
        // Example:
        // yield return new WaitForSeconds(2f);
        // SceneManager.LoadScene("NextSceneName");
    }

    private IEnumerator ShowLine(string line, bool fadeOutAtEnd = true)
    {
        // Set text and start at alpha 0
        endText.text = line;
        Color c = endText.color;
        c.a = 0f;
        endText.color = c;

        // Fade in
        float t = 0f;
        while (t < textFadeDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / textFadeDuration);
            c.a = Mathf.Lerp(0f, 1f, normalized);
            endText.color = c;
            yield return null;
        }

        // Hold
        yield return new WaitForSeconds(textHoldTime);

        if (fadeOutAtEnd)
        {
            // Fade out
            t = 0f;
            while (t < textFadeDuration)
            {
                t += Time.deltaTime;
                float normalized = Mathf.Clamp01(t / textFadeDuration);
                c.a = Mathf.Lerp(1f, 0f, normalized);
                endText.color = c;
                yield return null;
            }
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        float t = 0f;
        group.alpha = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);
            group.alpha = Mathf.Lerp(from, to, normalized);
            yield return null;
        }

        group.alpha = to;
    }
}
