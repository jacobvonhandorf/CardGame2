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
    [SerializeField] private float backgroundWidth = 3f;
    [SerializeField] private float yMargin = .2f;
    [SerializeField] private float headerHeight = .2f;
    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private TextMeshPro headerText;
    [SerializeField] private OptionButton optionButtonPrefab;

    private OptionBoxHandler handler;
    private bool finished = false;

    #region Command
    public static void CreateAndQueue(List<string> options, string headerText, Player owner, OptionBoxHandler handler)
    {
        InformativeAnimationsQueue.instance.addAnimation(new OptionSelectCmd(options, headerText, owner, handler));
    }
    public static OptionSelectCmd CreateCommand(List<string> options, string headerText, Player owner, OptionBoxHandler handler)
    {
        return new OptionSelectCmd(options, headerText, owner, handler);
    }
    public class OptionSelectCmd : QueueableCommand
    {
        List<string> options;
        OptionBoxHandler handler;
        string headerText;
        OptionSelectBox optionBox;
        Player owner;

        public OptionSelectCmd(List<string> options, string headerText, Player owner, OptionBoxHandler handler)
        {
            this.options = options;
            this.handler = handler;
            this.headerText = headerText;
            this.owner = owner;
        }

        public override bool isFinished => optionBox != null && optionBox.finished || forceFinished;
        private bool forceFinished = false;

        public override void execute()
        {
            if (owner != NetInterface.Get().getLocalPlayer())
            {
                forceFinished = true;
                return;
            }
            optionBox = Instantiate(GameManager.Get().optionSelectBoxPrefab);
            optionBox.setUp(options, headerText, handler);
        }
    }
    #endregion

    public void setUp(List<string> options, string headerText, OptionBoxHandler handler)
    {
        this.handler = handler;
        if (GameManager.Get() != null)
            GameManager.Get().setPopUpGlassActive(true);
        // set background size based on number of options
        float backgroundHeight = yMargin * 2 + Math.Abs(offsetPerOption) * options.Count + headerHeight;
        float top = backgroundHeight / 2;

        // headerText
        float headerTextYPosition = top - headerHeight / 2 - yMargin;
        this.headerText.transform.position = new Vector3(xOffset, headerTextYPosition, 0);
        this.headerText.text = headerText;

        backgroundSprite.size = new Vector2(backgroundWidth, backgroundHeight);

        // create a button for each option
        optionButtonPrefab = GameManager.Get().getOptionButtonPrefab();
        int index = 0;
        foreach (string option in options)
        {
            float yPos = top - yMargin - headerHeight + offsetPerOption * index + offsetPerOption / 2;
            Vector3 position = new Vector3(xOffset, yPos, -1);
            OptionButton currentButton = Instantiate(optionButtonPrefab, position, Quaternion.identity, transform);
            currentButton.setUp(index, option, this);
            index++;
        }
    }

    public void submit(int index, string text)
    {
        handler.Invoke(index, text);
        GameManager.Get().setPopUpGlassActive(false);
        finished = true;
        Destroy(gameObject);
    }
}
