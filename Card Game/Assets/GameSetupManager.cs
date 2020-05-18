using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
/*
public class GameSetupManager : MonoBehaviour
{
    [SerializeField] TMP_Dropdown p1Dropdown;
    [SerializeField] TMP_Dropdown p2Dropdown;

    void Start()
    {
        List<string> allDeckNames = DeckUtilities.getAllDeckNames();
        p1Dropdown.AddOptions(allDeckNames);
        p2Dropdown.AddOptions(allDeckNames);
    }


    private const string gameMode = "Hot Seat Game Mode";
    public void submit()
    {
        Debug.Log(p1Dropdown.options[p1Dropdown.value].text);
        GameManager.p1DeckName = p1Dropdown.options[p1Dropdown.value].text;
        GameManager.p2DeckName = p1Dropdown.options[p2Dropdown.value].text;

        // show loading screen
        SceneManager.LoadScene(gameMode);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameMode));
    }
}
*/