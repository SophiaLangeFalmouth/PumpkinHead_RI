using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodePanel : MonoBehaviour
{
    public InputField codeInput;       // drag CodeInput here
    public string correctCode = "314";
    public string successFlag = "drawerUnlocked";
    public GameObject[] enableOnSuccess;
    public GameObject[] disableOnSuccess;

    public void Submit()
    {
        if (codeInput.text.Trim() == correctCode)
        {
            GameManager.I.Set(successFlag);
            DialogueManager.I.Say("You hear a soft click. The drawer unlocks.");

            // Activate or deactivate objects
            if (enableOnSuccess != null)
                foreach (var go in enableOnSuccess) if (go) go.SetActive(true);
            if (disableOnSuccess != null)
                foreach (var go in disableOnSuccess) if (go) go.SetActive(false);

            gameObject.SetActive(false);
        }
        else
        {
            DialogueManager.I.Say("That didn’t work.");
        }
    }
}

