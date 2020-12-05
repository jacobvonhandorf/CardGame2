using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameEscMenu : MonoBehaviour
{
    [SerializeField] GameObject content;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            content.SetActive(!content.activeSelf);
            GameManager.Get().SetPopUpGlassActive(content.activeSelf);
        }
    }

    public void onSurrenderClicked()
    {
        GameManager.Get().surrender();
    }

    public void onExitGameClicked()
    {
        NetInterface.Get().SendSurrenderMessage();
        Application.Quit();
    }
}
