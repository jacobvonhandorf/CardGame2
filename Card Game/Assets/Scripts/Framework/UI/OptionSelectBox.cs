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
    [SerializeField] private float backgroundHeight = .5f;
    [SerializeField] private float backgroundWidth = 3f;
    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private TextMeshPro headerText;

    private OptionBoxHandler handler;

    public void setUp(List<string> options, OptionBoxHandler handler)
    {
        GameManager.Get().setPopUpGlassActive(true);
        // set background size based on number of options
        backgroundSprite.size = new Vector2(backgroundWidth, backgroundHeight * options.Count + .5f);

        // create a button for each option
        OptionButton button = GameManager.Get().getOptionButtonPrefab();
        int index = 0;
        foreach (string option in options)
        {
            Vector3 position = new Vector3(xOffset, yOffset + offsetPerOption * index, -1);
            OptionButton currentButton = Instantiate(button, position, Quaternion.identity, transform);
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
