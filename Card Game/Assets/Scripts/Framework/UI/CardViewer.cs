using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CardViewer : MonoBehaviour
{
    public ICanBeCardViewed CurrentlyDisplayed { get; private set; }
    public UnityEvent<CardViewer> E_OnClicked { get; private set; } = new CardViewerEvent();

    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private StatChangePropogator statChangePropogator;
    [SerializeField] private ToolTipBox toolTipPrefab;
    [SerializeField] private static Vector3 toolTipOffset;
    [SerializeField] private static Vector3 toolTipTOffsetPerBox;

    private void Awake()
    {
        GetComponentInChildren<OnMouseClickEvents>().OnMouseClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        E_OnClicked.Invoke(this);
    }

    public void SetCard(ICanBeCardViewed newCard)
    {
        CurrentlyDisplayed = newCard;
        statChangePropogator.Source = newCard.ReadableStats;
    }

    #region Tooltips
    private List<ToolTipBox> toolTips = new List<ToolTipBox>();
    public void ShowToolTips()
    {
        Debug.LogError("Not Implemented");
        //ShowToolTips(SourceCard.toolTipInfos);
    }
    public void ShowToolTips(List<ToolTipInfo> toolTipInfos)
    {
        //For some reason (probably Unity being bad) log statements in here break things, so don't do that
        int i = 0;
        foreach (ToolTipInfo info in toolTipInfos)
        {
            Vector3 position = toolTipOffset + (toolTipTOffsetPerBox * i);
            ToolTipBox box = Instantiate(toolTipPrefab, transform);
            box.transform.localPosition = position;
            box.setup(info.headerText, info.descriptionText);
            toolTips.Add(box);
            i++;
        }
    }
    public void ClearToolTips()
    {
        foreach (ToolTipBox box in toolTips)
        {
            Destroy(box.gameObject);
        }
        toolTips.Clear();
    }
    #endregion
}
