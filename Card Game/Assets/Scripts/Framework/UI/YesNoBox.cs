using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class YesNoBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI noButtonText;
    [SerializeField] private TextMeshProUGUI yesButtonText;
    [SerializeField] private GameObject yesButtonGO;
    [SerializeField] private GameObject noButtonGO;
    private YesNoHandler handler;
    public delegate void ButtonClickHandler();
    private ButtonClickHandler yesHandler;
    private ButtonClickHandler noHandler;

    public void setUp(ButtonClickHandler yesHandler, ButtonClickHandler noHandler, string headerText, string descriptionText)
    {
        this.yesHandler = yesHandler;
        this.noHandler = noHandler;
        this.headerText.text = headerText;
        this.descriptionText.text = descriptionText;
    }

    public void setUp(YesNoHandler handler, string headerText, string descriptionText)
    {
        this.handler = handler;
        this.headerText.text = headerText;
        this.descriptionText.text = descriptionText;
    }

    public void yesClicked()
    {
        yesHandler?.Invoke();
        if (handler != null)
            handler.onYesClicked();
        Destroy(gameObject);
    }

    public void onNoClicked()
    {
        noHandler?.Invoke();
        if (handler != null)
            handler.onNoClicked();
        Destroy(gameObject);
    }

    public void setYesButtonText(string newText)
    {
        yesButtonText.text = newText;
    }
    public void setNoButtonText(string newText)
    {
        noButtonText.text = newText;
    }
    public void setYesButtonActive(bool value)
    {
        yesButtonGO.SetActive(value);
    }
    public void setNoButtonActive(bool value)
    {
        noButtonGO.SetActive(value);
    }
}

public interface YesNoHandler
{
    void onYesClicked();
    void onNoClicked();
}
