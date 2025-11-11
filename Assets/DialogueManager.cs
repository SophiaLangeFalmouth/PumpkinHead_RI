using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager I;  // simple singleton
    public GameObject panel;
    public Text text;

    void Awake() => I = this;

    public void Say(string message, float duration = 2f)
    {
        Debug.Log("[Dialogue] " + message);        // helps you see calls in Console

        if (panel == null || text == null) return;

        panel.SetActive(true);
        // make sure it renders on top of other panels
        panel.transform.SetAsLastSibling();

        text.text = message;

        CancelInvoke(nameof(Hide));
        Invoke(nameof(Hide), duration);
    }

    void Hide()
    {
        if (panel) panel.SetActive(false);
    }
}
