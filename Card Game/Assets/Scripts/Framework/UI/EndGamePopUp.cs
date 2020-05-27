using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePopUp : MonoBehaviour
{
    [SerializeField] TextMeshPro text;

    public void setup(string messageText)
    {
        text.text = messageText;
    }

    public void onExitButtonClicked()
    {
        NetInterface.Reset();
        SceneManager.LoadScene(ScenesEnum.MMDeckSelect);
    }
}
