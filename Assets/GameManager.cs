using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Inventory UI (fixed 5 slots)")]
    public Button[] slotButtons;   // Slot01..Slot05 (scene objects with Button)
    public Image[]  slotIcons;     // Each slot's child "Icon" (Image)

    [Header("Item Icons")]
    public Sprite matchesIcon;
    public Sprite keyIcon;
    public Sprite scrapAIcon;
    public Sprite scrapBIcon;
    public Sprite scrapCIcon;

    // ----- State -----
    readonly HashSet<string> flags = new HashSet<string>();
    string[] slotIds;                    // item id stored per slot
    int activeIndex = -1;
    string activeItemId = null;

    [Header("Auto-wire (optional)")]
    public Transform inventoryBar; // drag the InventoryBar parent here in Inspector

   void Awake()
    {
        I = this;

        // Auto-wire if arrays empty or first is null
        if (inventoryBar && 
           (slotButtons == null || slotButtons.Length == 0 || slotButtons[0] == null))
        {
            var btns  = new List<Button>();
            var icons = new List<Image>();
            foreach (Transform child in inventoryBar)
            {
                var b    = child.GetComponent<Button>();
                var icon = child.Find("Icon")?.GetComponent<Image>();
                if (b && icon) { btns.Add(b); icons.Add(icon); }
            }
            slotButtons = btns.ToArray();
            slotIcons   = icons.ToArray();
        }

        slotIds = new string[slotIcons.Length];

        for (int i = 0; i < slotIcons.Length; i++)
        {
            ClearSlot(i);
            int idx = i;
            if (i < slotButtons.Length && slotButtons[i] != null)
                slotButtons[i].onClick.AddListener(() => SelectIndex(idx));
        }
    }


    // ===== Flags =====
    public bool Get(string flag) => flags.Contains(flag);
    public void Set(string flag) { flags.Add(flag); }

    // ===== Inventory =====
    public bool Has(string id)
    {
        for (int i = 0; i < slotIds.Length; i++)
            if (slotIds[i] == id) return true;
        return false;
    }

    public string ActiveItem() => activeItemId;

    public void Give(string id, Sprite overrideIcon = null)
    {
        if (Has(id))
        {
            DialogueManager.I.Say($"You already have {id}.");
            return;
        }

        int idx = FindFirstEmptySlot();
        if (idx == -1)
        {
            DialogueManager.I.Say("Your bag is full.");
            return;
        }

        slotIds[idx] = id;

        var icon = overrideIcon ?? GetIconFor(id);
        Debug.Log($"[Give] id={id}, idx={idx}, slotIconNull={slotIcons[idx]==null}, mappedIconNull={icon==null}");
        
        if (slotIcons[idx] != null)
        {
            slotIcons[idx].sprite = icon;
            slotIcons[idx].preserveAspect = true;
            var c = slotIcons[idx].color; c.a = 1f; slotIcons[idx].color = c;
            Debug.Log($"[Give] set sprite={(slotIcons[idx].sprite ? slotIcons[idx].sprite.name : "NULL")} alpha={slotIcons[idx].color.a}");
        }


        SelectIndex(idx);
        DialogueManager.I.Say($"Picked up {id}.");
    }

    public void ConsumeActive()
    {
        if (activeIndex < 0) return;

        // remove the item in that slot
        slotIds[activeIndex] = null;
        if (slotIcons[activeIndex] != null)
        {
            slotIcons[activeIndex].sprite = null;
            var c = slotIcons[activeIndex].color; c.a = 0f; slotIcons[activeIndex].color = c;
        }
        if (activeIndex < slotButtons.Length && slotButtons[activeIndex] != null)
            slotButtons[activeIndex].GetComponent<Image>().color = new Color(0,0,0,0.5f);

        CompactSlotsLeft();

        activeItemId = null;
        activeIndex  = -1;
    }

    public void Consume(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        for (int i = 0; i < slotIds.Length; i++)
        {
            if (slotIds[i] == id)
            {
                slotIds[i] = null;
                if (slotIcons[i] != null)
                {
                    slotIcons[i].sprite = null;
                    var c = slotIcons[i].color; c.a = 0f; slotIcons[i].color = c;
                }
                if (i < slotButtons.Length && slotButtons[i] != null)
                    slotButtons[i].GetComponent<Image>().color = new Color(0,0,0,0.5f);
                CompactSlotsLeft();
                break;
            }
        }
    }


    // ===== Helpers =====
    int FindFirstEmptySlot()
    {
        for (int i = 0; i < slotIcons.Length; i++)
            if (slotIcons[i] == null || slotIcons[i].sprite == null) return i;
        return -1;
    }

    void SelectIndex(int idx)
    {
        if (idx < 0 || idx >= slotIcons.Length) return;
        if (slotIcons[idx] == null || slotIcons[idx].sprite == null) return;

        // clear previous highlight
        //if (activeIndex >= 0 && activeIndex < slotButtons.Length && slotButtons[activeIndex] != null)
        //   slotButtons[activeIndex].GetComponent<Image>().color = new Color(0,0,0,0.5f);

        activeIndex  = idx;
        activeItemId = slotIds[idx];

        // if (slotButtons[idx] != null)
        //     slotButtons[idx].GetComponent<Image>().color = new Color(0.85f,1f,0.85f,0.5f);
    }

    void CompactSlotsLeft()
    {
        int write = 0;
        for (int read = 0; read < slotIcons.Length; read++)
        {
            if (slotIcons[read] != null && slotIcons[read].sprite != null)
            {
                if (read != write)
                {
                    // move sprite
                    slotIcons[write].sprite = slotIcons[read].sprite;
                    slotIcons[write].color  = new Color(1,1,1,1);
                    slotIcons[read].sprite  = null;
                    slotIcons[read].color   = new Color(1,1,1,0);

                    // move id
                    slotIds[write] = slotIds[read];
                    slotIds[read]  = null;
                }
                write++;
            }
        }

        // clear anything past 'write'
        for (int i = write; i < slotIcons.Length; i++)
        {
            if (slotIcons[i] != null)
            {
                slotIcons[i].sprite = null;
                slotIcons[i].color  = new Color(1,1,1,0);
            }
            slotIds[i] = null;
        }
    }

    Sprite GetIconFor(string id)
    {
        switch (id)
        {
            case "Matches": return matchesIcon;
            case "Key":     return keyIcon;
            case "ScrapA":  return scrapAIcon;
            case "ScrapB":  return scrapBIcon;
            case "ScrapC":  return scrapCIcon;
            default:        return null;
        }
    }

    void ClearSlot(int i)
    {
        if (i < 0 || i >= slotIcons.Length) return;
        slotIds[i] = null;

        if (slotIcons[i] != null)
        {
            slotIcons[i].sprite = null;
            var c = slotIcons[i].color; 
            c.a = 0f; 
            slotIcons[i].color = c;
        }

        if (i < slotButtons.Length && slotButtons[i] != null)
            slotButtons[i].GetComponent<Image>().color = new Color(0,0,0,0.5f);
    }

}




