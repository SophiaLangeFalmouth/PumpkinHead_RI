using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SfxEntry
{
    public string key;               // e.g. "pickup_key", "locked"
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager I;

    // 2D SFX AudioSource (no clip, no loop)
    public AudioSource source;

    // Key -> Clip library (fill in Inspector)
    public List<SfxEntry> library = new List<SfxEntry>();

    private Dictionary<string, SfxEntry> map;

    void Awake()
    {
        I = this;
        map = new Dictionary<string, SfxEntry>();
        if (library != null)
        {
            foreach (var e in library)
            {
                if (!string.IsNullOrEmpty(e.key) && e.clip && !map.ContainsKey(e.key))
                    map.Add(e.key, e);
            }
        }
    }

    public void Play(string key)
    {
        if (source == null || string.IsNullOrEmpty(key)) return;
        if (map != null && map.TryGetValue(key, out var e) && e.clip)
            source.PlayOneShot(e.clip, e.volume);
    }

    // Convenience mapping for inventory pickups
    public void PlayPickup(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return;
        switch (itemId)
        {
            case "Matches": Play("pickup_matches"); break;
            case "Key":     Play("pickup_key");     break;
            case "ScrapA":
            case "ScrapB":
            case "ScrapC":  Play("pickup_scrap");   break;
            default:        Play("pickup_generic"); break;
        }
    }
}
