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

    public void setUp(YesNoHandler handler, string headerText, string descriptionText)
    {
        this.handler = handler;
        this.headerText.text = headerText;
        this.descriptionText.text = descriptionText;
    }

    public void yesClicked()
    {
        if (handler != null)
            handler.onYesClicked();
        Destroy(gameObject);
    }

    public void onNoClicked()
    {
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
