using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreenManager : MonoBehaviour
{
    public void onlineButtonPressed()
    {
        SceneManager.LoadScene(ScenesEnum.MMDeckSelect);
    }

    public void hotSeatButtonPressed()
    {
        SceneManager.LoadScene(ScenesEnum.GameSetup);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("Hot Seat Game Mode"));
    }

    public void deckbuilderButtonPressed()
    {
        SceneManager.LoadScene(ScenesEnum.DeckBuilder);
    }

    public void settingsButtonPressed()
    {
        throw new NotImplementedException();
    }
}
