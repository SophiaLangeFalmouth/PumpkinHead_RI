using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorNormal;     // your PNG (Texture type: Cursor or Default)
    public Vector2 hotspot = new Vector2(0, 0); // pixel offset of the tip

    void Start()
    {
        Set(cursorNormal, hotspot);
    }

    public void Set(Texture2D tex, Vector2 hs)
    {
        Cursor.SetCursor(tex, hs, CursorMode.Auto);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}

