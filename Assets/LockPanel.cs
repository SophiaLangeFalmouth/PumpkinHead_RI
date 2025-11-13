using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockPanel : MonoBehaviour
{
    [Header("Gate")]
    public string requiredFlag = "hasCode";   // must be true to allow input

    [Header("Slots (left to right)")]
    public Image slot1;
    public Image slot2;
    public Image slot3;

    [Header("Symbol sprites to reveal")]
    public Sprite symbolA;
    public Sprite symbolB;
    public Sprite symbolC;

    [Header("Expected order (A=0, B=1, C=2)")]
    public List<int> correctOrder = new List<int> { 0, 1, 2 };

    [Header("On success")]
    public string successFlag = "drawerUnlocked";
    public GameObject[] enableOnSuccess;   // e.g., KeyBtn
    public GameObject[] disableOnSuccess;

    // runtime
    private readonly List<int> input = new List<int>(3);
    private bool initialized = false; // only clear the very first time panel is opened

    void OnEnable()
    {
        // Persist state between opens. Only clear the very first time.
        if (!initialized)
        {
            input.Clear();
            ClearSlot(slot1);
            ClearSlot(slot2);
            ClearSlot(slot3);
            initialized = true;
        }
    }

    // Invisible hitboxes call these:
    public void ClickA() => OnSymbol(0, symbolA);
    public void ClickB() => OnSymbol(1, symbolB);
    public void ClickC() => OnSymbol(2, symbolC);

   private void OnSymbol(int id, Sprite sprite)
    {
        // Player hasn't seen the wall yet
        if (!GameManager.I.Get(requiredFlag))
        {
            DialogueManager.I.Say("I wonder what the right symbols are.");
            return;
        }

        // 🔊 play drawer button sound when pressing any symbol
        if (SFXManager.I) SFXManager.I.Play("drawer_button");

        // Already filled 3 slots
        if (input.Count >= 3) return;

        // Fill next slot (become visible only when set)
        Image target = (input.Count == 0) ? slot1 : (input.Count == 1) ? slot2 : slot3;
        ShowSlot(target, sprite);

        input.Add(id);

        if (input.Count == 3)
            Check();
    }


    private void Check()
{
    bool ok =
        input.Count == 3 &&
        input[0] == correctOrder[0] &&
        input[1] == correctOrder[1] &&
        input[2] == correctOrder[2];

    if (ok)
    {
        GameManager.I.Set(successFlag);
        DialogueManager.I.Say("The drawer clicks. Something falls down.", 3.0f);

        // 🔊 play the drop sound right when the drawer unlocks
        if (SFXManager.I) SFXManager.I.Play("key_drop");   // <- this is the one you asked for
        // (optional) also play an unlock thunk if you have one:
        // if (SFXManager.I) SFXManager.I.Play("drawer_unlock");

        if (enableOnSuccess != null)
            foreach (var go in enableOnSuccess) if (go) go.SetActive(true);
        if (disableOnSuccess != null)
            foreach (var go in disableOnSuccess) if (go) go.SetActive(false);

        // do not auto-close panel
    }
    else
    {
        DialogueManager.I.Say("That didn’t work.");
        if (SFXManager.I) SFXManager.I.Play("lockpanel_wrong");
        Invoke(nameof(ResetSlots), 0.45f);
    }
}

    private void ResetSlots()
    {
        input.Clear();
        ClearSlot(slot1);
        ClearSlot(slot2);
        ClearSlot(slot3);
    }

    private void ClearSlot(Image slot)
    {
        if (!slot) return;
        slot.sprite = null;
        var c = slot.color; c.a = 0f; slot.color = c; // fully transparent when empty
    }

    private void ShowSlot(Image slot, Sprite s)
    {
        if (!slot) return;
        slot.sprite = s;
        var c = slot.color; c.a = 1f; slot.color = c; // visible when filled
    }
}

