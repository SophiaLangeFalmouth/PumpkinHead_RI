using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAmbienceOnShow : MonoBehaviour
{
    [Tooltip("Optional gate so we only switch after the journal is truly done.")]
    public string requiredFlag = "journalVisualComplete";
    public bool onlyOnce = true;

    bool done;

    void OnEnable()
    {
        if (onlyOnce && done) return;

        // If you want to be extra safe that the journal is completed:
        if (!string.IsNullOrEmpty(requiredFlag) && GameManager.I && !GameManager.I.Get(requiredFlag))
            return;

        var ambience = FindObjectOfType<AmbienceManager>();
        if (ambience) ambience.SwitchToEndAmbience();

        done = true;
    }
}

