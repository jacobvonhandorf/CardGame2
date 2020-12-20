using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelectBox : MonoBehaviour
{
    [SerializeField] private OptionButton optionButtonPrefab;
    [SerializeField] private TextMeshProUGUI headerText;

    private OptionBoxHandler handler;
    private bool finished = false;

    #region Command
    public static void CreateAndQueue(List<string> options, string headerText, IScriptPlayer owner, OptionBoxHandler handler) => InformativeAnimationsQueue.Instance.AddAnimation(new OptionSelectCmd(options, headerText, owner, handler));
    public static OptionSelectCmd CreateCommand(List<string> options, string headerText, IScriptPlayer owner, OptionBoxHandler handler) => new OptionSelectCmd(options, headerText, owner, handler);

    public class OptionSelectCmd : IQueueableCommand
    {
        List<string> options;
        OptionBoxHandler handler;
        string headerText;
        OptionSelectBox optionBox;
        IScriptPlayer owner;

        public OptionSelectCmd(List<string> options, string headerText, IScriptPlayer owner, OptionBoxHandler handler)
        {
            this.options = options;
            this.handler = handler;
            this.headerText = headerText;
            this.owner = owner;
        }

        public bool IsFinished => optionBox != null && optionBox.finished || forceFinished;
        private bool forceFinished = false;

        public void Execute()
        {
            if (GameManager.gameMode == GameManager.GameMode.online && owner != NetInterface.Get().localPlayer)
            {
                forceFinished = true;
                return;
            }
            optionBox = Instantiate(PrefabHolder.Instance.optionBox, MainCanvas.Instance.transform);
            optionBox.SetUp(options, headerText, handler);
        }
    }
    #endregion

    public void SetUp(List<string> options, string headerText, OptionBoxHandler handler)
    {
        UIEvents.EnableUIBlocker.Invoke();
        this.handler = handler;
        this.headerText.text = headerText;

        // create a button for each option
        int index = 0;
        foreach (string option in options)
        {
            Instantiate(optionButtonPrefab, transform).SetUp(index, option, this);
            index++;
        }
    }

    public void Submit(int index, string text)
    {
        handler.Invoke(index, text);
        UIEvents.DisableUIBlocker.Invoke();
        finished = true;
        Destroy(gameObject);
    }
}
