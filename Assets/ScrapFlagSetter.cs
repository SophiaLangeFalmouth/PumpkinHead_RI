using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapFlagSetter : MonoBehaviour
{
    public string flagName = "scrapsComplete";
    public bool announce = true;
    bool done;

    void Update()
    {
        if (done || GameManager.I == null) return;

        if (GameManager.I.Has("ScrapA") && GameManager.I.Has("ScrapB") && GameManager.I.Has("ScrapC"))
        {
            GameManager.I.Set(flagName);
            done = true;
            if (announce) DialogueManager.I.Say("The scraps fit together…");
        }
    }
}


