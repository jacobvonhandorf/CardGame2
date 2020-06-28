using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDeckBuilderButton : MonoBehaviour, YesNoHandler
{
    [SerializeField] private YesNoBox yesNoBoxPrefab;
    [SerializeField] private GameObject glassBackground;

    public void exitDeckBuilder()
    {
        if (DeckBuilderDeck.instance.unsavedChanges)
        {
            glassBackground.SetActive(true);
            YesNoBox yesNoBox = Instantiate(yesNoBoxPrefab);
            yesNoBox.setUp(this, "Unsaved changes", "There are unsaved changes to your deck. Are you sure you want to exit?");
        }
        else
        {
            SceneManager.LoadScene(ScenesEnum.MainMenu);
        }
    }

    public void onNoClicked()
    {
        glassBackground.SetActive(false);
    }

    public void onYesClicked()
    {
        SceneManager.LoadScene(ScenesEnum.MainMenu);
    }
}
