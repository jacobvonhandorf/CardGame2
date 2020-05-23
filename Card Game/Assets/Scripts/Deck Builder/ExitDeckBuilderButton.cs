using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDeckBuilderButton : MonoBehaviour
{
    public void exitDeckBuilder()
    {
        SceneManager.LoadScene(ScenesEnum.MainMenu);
    }
}
