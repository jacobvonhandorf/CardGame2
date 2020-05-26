using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionSelectBox : MonoBehaviour
{
    [SerializeField] private float xOffset = 1f;
    [SerializeField] private float yOffset = 1f;
    [SerializeField] private float offsetPerOption = .5f;
    //[SerializeField] private float backgroundHeight = .5f;
    [SerializeField] private float backgroundWidth = 3f;
    [SerializeField] private float yMargin = .2f;
    [SerializeField] private float headerHeight = .2f;
    //[SerializeField] private float backgroundOffsetPerButton = .2f;
    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private TextMeshPro headerText;
    [SerializeField] private OptionButton optionButtonPrefab;

    private OptionBoxHandler handler;

    public void setUp(List<string> options, OptionBoxHandler handler)
    {
        if (GameManager.Get() != null)
            GameManager.Get().setPopUpGlassActive(true);
        // set background size based on number of options
        float backgroundHeight = yMargin * 2 + Math.Abs(offsetPerOption) * options.Count + headerHeight;
        float top = backgroundHeight / 2;
        Debug.Log(backgroundHeight);
        Debug.Log(top);

        // position headerText at top of window
        float headerTextYPosition = top - headerHeight / 2 - yMargin;
        headerText.transform.position = new Vector3(xOffset, headerTextYPosition, 0);

        backgroundSprite.size = new Vector2(backgroundWidth, backgroundHeight);

        // create a button for each option

        if (optionButtonPrefab == null)
            optionButtonPrefab = GameManager.Get().getOptionButtonPrefab();
        int index = 0;
        foreach (string option in options)
        {
            //float yPos = top;
            float yPos = top - yMargin - headerHeight + offsetPerOption * index + offsetPerOption / 2;
            // Vector3 position = new Vector3(xOffset, yOffset + offsetPerOption * index, -1);
            Vector3 position = new Vector3(xOffset, yPos, -1);
            OptionButton currentButton = Instantiate(optionButtonPrefab, position, Quaternion.identity, transform);
            currentButton.setUp(index, option, handler, this);
            index++;
        }
    }

    public void setUp(List<string> options, OptionBoxHandler handler, string headerText)
    {
        this.headerText.text = headerText;
        setUp(options, handler);
    }

    public void close()
    {
        GameManager.Get().setPopUpGlassActive(false);
        Destroy(gameObject);
        EffectsManager.Get().signalEffectFinished();
    }

    // ----- code for testing ----- //
    /*
    private void Start()
    {
        List<string> options = new List<string>();
        options.Add("Option 1");
        options.Add("Option 2");
        options.Add("Option 3");

        setUp(options, new MyHandler());
    }

    private class MyHandler : OptionBoxHandler
    {
        public void receiveOptionBoxSelection(int selectedOptionIndex, string selectedOption)
        {
            Debug.Log("Index: " + selectedOptionIndex + ", Selected Option: " + selectedOption);
        }
    }
    */
}
