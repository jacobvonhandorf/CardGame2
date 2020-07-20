using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;

    public void onlineButtonPressed()
    {
        SceneManager.LoadScene(ScenesEnum.MMDeckSelect);
    }

    public void hotSeatButtonPressed()
    {
        SceneManager.LoadScene(ScenesEnum.GameSetup);
    }

    public void deckbuilderButtonPressed()
    {
        SceneManager.LoadScene(ScenesEnum.DeckBuilder);
    }

    public void settingsButtonPressed()
    {
        settingsMenu.SetActive(true);
    }

    public void onQuitClicked()
    {
        Application.Quit();
    }
}
