using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckBuilderButtons : MonoBehaviour
{
    [SerializeField] private GameObject glassBackground;
    [SerializeField] private YesNoBox yesNoBoxPrefab;
    [SerializeField] private GameObject filtersPopUp;

    public void OnExitClick()
    {
        if (DeckBuilderDeck.instance.unsavedChanges)
        {
            glassBackground.SetActive(true);
            YesNoBox yesNoBox = Instantiate(yesNoBoxPrefab);
            yesNoBox.setUp(switchToMainMenu, setGlassBackgroundActiveFalse, "Unsaved changes", "There are unsaved changes to your deck. Are you sure you want to exit?");
        }
        else
        {
            SceneManager.LoadScene(ScenesEnum.MainMenu);
        }
    }
    public void OnFiltersClicked()
    {
        filtersPopUp.SetActive(true);
        glassBackground.SetActive(true);
    }
    public void OnDeleteClicked()
    {
        DeckBuilderDeck.instance.delete();
    }
    public void OnSaveClicked()
    {
        DeckBuilderDeck.instance.save();
    }
    public void OnSaveAsClicked()
    {
        DeckBuilderDeck.instance.saveAs();
    }

    private void setGlassBackgroundActiveFalse() { glassBackground.SetActive(false); }
    private void switchToMainMenu() { SceneManager.LoadScene(ScenesEnum.MainMenu); }
}
