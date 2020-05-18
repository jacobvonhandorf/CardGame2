using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MMDeckSelectScene : MonoBehaviour
{
    public static MMDeckSelectScene Instance;

    [SerializeField] private TMP_Dropdown deckSelectDropdown;
    [SerializeField] private GameObject uiBlocker;
    [SerializeField] private Button enterMMPoolButton;

    public void submitAndEnterMMQueue()
    {
        deckSelectDropdown.interactable = false;
        enterMMPoolButton.interactable = false;
        uiBlocker.gameObject.SetActive(true);

        string deckName = deckSelectDropdown.options[deckSelectDropdown.value].text;
        NetInterface.Get().selectedDeckName = deckName;
        Client.Instance.enterMMPool();
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    internal void startGame()
    {
        // load game scene
        GameManager.gameMode = GameManager.GameMode.online;
        SceneManager.LoadScene(ScenesEnum.HotSeatGameMode);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(ScenesEnum.OnlineGameScene));
    }
}
