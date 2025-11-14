using UnityEngine;

public class StartPanel : MonoBehaviour
{
    public GameObject panel;

    public void HidePanel()
    {
        panel.SetActive(false);
    }
}

