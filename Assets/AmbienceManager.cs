using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceManager : MonoBehaviour
{
    public AudioSource ambienceSource;       // main gameplay ambience
    public AudioClip mainAmbience;
    public AudioClip endScreenAmbience;

    void Start()
    {
        // start playing the main ambience
        ambienceSource.clip = mainAmbience;
        ambienceSource.loop = true;
        ambienceSource.Play();
    }

    public void SwitchToEndAmbience()
    {
        ambienceSource.loop = false; // stop looping old sound
        ambienceSource.clip = endScreenAmbience;
        ambienceSource.loop = true;
        ambienceSource.Play();
    }
}

