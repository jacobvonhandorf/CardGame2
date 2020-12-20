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
            UIEvents.EnableUIBlocker.Invoke();
        }
    }

    public void onSurrenderClicked()
    {
        GameManager.Instance.Surrender();
    }

    public void onExitGameClicked()
    {
        NetInterface.Get().SendSurrenderMessage();
        Application.Quit();
    }
}
