using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFlagOnShow : MonoBehaviour
{
    public string flagName = "messageRead";
    bool done;

    void OnEnable()
    {
        if (done || GameManager.I == null) return;
        GameManager.I.Set(flagName);
        done = true; // only once
    }
}

