using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotspot : MonoBehaviour
{
    [Header("Texts")]
    [TextArea] public string interactText = "…"; // shown if requirement not met
    [TextArea] public string doneText = "";       // shown when step already completed
    [TextArea] public string successText = "";    // shown the first time success runs


    [Header("Requires (to perform action)")]
    public string requiredItem = "";   // checks inventory (no selection needed)
    public string requiredFlag = "";   // must be true to allow action

    [Header("Done State (blocks action, shows doneText)")]
    public string doneIfFlag = "";     // if this flag is true, action is replaced by doneText
    public bool openPanelWhenDone = false; // optionally still open the panel when done

    [Header("On Success (when action runs)")]
    public string setFlag = "";        // e.g. "candleLit", "hasCode", etc.
    public string giveItem = "";       // e.g. "Key"
    public Sprite giveItemIcon;        // icon for given item (you’re using this approach)
    public GameObject openPanel;       // show close-up / code panel
    public Image swapTarget;           // optional art swap
    public Sprite swapSprite;
    public List<GameObject> enableOnSuccess = new List<GameObject>();  // e.g., KeyBtn
    public List<GameObject> disableOnSuccess = new List<GameObject>(); // e.g., DarkOverlay
    public bool hideOnSuccess = false; // hide this hotspot after success
    public bool oneShot = true;        // do action only once (but see “meaningful change” rule)

    bool used = false;

    void Start()
    {
        // ensure a single listener (prevents double-fire -> “already done” spam)
        var btn = GetComponent<Button>();
        if (btn)
        {
            btn.onClick.RemoveListener(OnClick);
            btn.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        // If a "done" flag is set, we’re in the completed state: show doneText (and optionally open panel)
        if (!string.IsNullOrEmpty(doneIfFlag) && GameManager.I.Get(doneIfFlag))
        {
            if (openPanelWhenDone && openPanel) openPanel.SetActive(true);
            if (!string.IsNullOrEmpty(doneText)) DialogueManager.I.Say(doneText);
            return;
        }

        // For strict one-shot hotspots that have already run (and have no explicit done flag)
        if (oneShot && used)
        {
            if (!string.IsNullOrEmpty(doneText)) DialogueManager.I.Say(doneText);
            return;
        }

        // Requirement gate: flag
        if (!string.IsNullOrEmpty(requiredFlag) && !GameManager.I.Get(requiredFlag))
        {
            if (!string.IsNullOrEmpty(interactText)) DialogueManager.I.Say(interactText);
            return;
        }

        // Requirement gate: item present in inventory (no selection required)
        if (!string.IsNullOrEmpty(requiredItem) && !GameManager.I.Has(requiredItem))
        {
            if (!string.IsNullOrEmpty(interactText)) DialogueManager.I.Say(interactText);
            return;
        }

        Success(consumeItem: !string.IsNullOrEmpty(requiredItem));
    }

    void Success(bool consumeItem)
    {
        bool didMeaningfulChange = false;

        if (!string.IsNullOrEmpty(setFlag))
        {
            GameManager.I.Set(setFlag);
            didMeaningfulChange = true;
        }

        if (!string.IsNullOrEmpty(giveItem))
        {
            GameManager.I.Give(giveItem, giveItemIcon);
            didMeaningfulChange = true;
        }


        if (swapTarget && swapSprite)
        {
            swapTarget.sprite = swapSprite;
            var c = swapTarget.color; c.a = 1f; swapTarget.color = c;
            didMeaningfulChange = true;
        }

        if (enableOnSuccess != null && enableOnSuccess.Count > 0)
        {
            foreach (var go in enableOnSuccess) if (go) go.SetActive(true);
            didMeaningfulChange = true;
        }
        if (disableOnSuccess != null && disableOnSuccess.Count > 0)
        {
            foreach (var go in disableOnSuccess) if (go) go.SetActive(false);
            didMeaningfulChange = true;
        }

        if (openPanel) openPanel.SetActive(true); // opening a panel alone is NOT “meaningful”

        if (consumeItem)
        {
            GameManager.I.Consume(requiredItem);
            didMeaningfulChange = true;
        }

        // Say a one-time success line (e.g., for wall symbols)
        if (!string.IsNullOrEmpty(successText))
            DialogueManager.I.Say(successText);

        // Mark used ONLY if something actually changed (opening a panel doesn’t lock it)
        if (oneShot && didMeaningfulChange)
        {
            used = true;
            if (hideOnSuccess) gameObject.SetActive(false);
        }
    }
}
