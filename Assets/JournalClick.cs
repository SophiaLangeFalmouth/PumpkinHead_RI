using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalClick : MonoBehaviour
{
    [Header("Flags")]
    public string scrapsFlag = "scrapsComplete";       // must be true to progress
    public string visualFlag = "journalVisualComplete"; // set after swapping art

    [Header("Visuals")]
    public Image targetImage;           // the Image component on your open journal object
    public Sprite completedPageSprite;  // the sprite that shows the assembled page

    [Header("POV Panel")]
    public GameObject povPanel;         // the zoomed-in journal panel to open on second click

    [Header("Texts")]
    [TextArea] public string notReadyText = "The page is still torn.";
    [TextArea] public string swappedText  = "The pieces align…";

    void Start()
    {
        // hook up the Button click
        var btn = GetComponent<UnityEngine.UI.Button>();
        if (btn) btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        // 1) If scraps not complete yet: just say the torn line
        if (!GameManager.I.Get(scrapsFlag))
        {
            DialogueManager.I.Say(notReadyText);
            return;
        }

        // 2) If scraps are complete but we haven't swapped visuals yet: swap & set flag
        if (!GameManager.I.Get(visualFlag))
        {
            if (targetImage && completedPageSprite)
            {
                targetImage.sprite = completedPageSprite;
                var c = targetImage.color; c.a = 1f; targetImage.color = c;
            }
            GameManager.I.Set(visualFlag);
            DialogueManager.I.Say(swappedText);
            return; // stop here; next click will open the POV
        }

        // 3) Visual already swapped -> open the POV panel
        if (povPanel) povPanel.SetActive(true);

    }
}

