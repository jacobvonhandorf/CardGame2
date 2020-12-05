using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardViewer : MonoBehaviour
{
    public Card SourceCard { get; private set; }
    ICanBeCardViewed currentlyDisplayed;

    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private StatChangePropogator statChangePropogator;
    [SerializeField] private ToolTipBox toolTipPrefab;

    private void OnDestroy()
    {
        //if (SourceCard != null)
        //    SourceCard.removeFromCardViewer(this);
    }

    public virtual void SetCard(int cardId)
    {
        //if (SourceCard != null)
        //    SourceCard.removeFromCardViewer(this);
        CardData data = ResourceManager.Get().getCardDataById(cardId);
        SetCard(data);
    }
    public void SetCard(ICanBeCardViewed newCard)
    {
        //newCard.OnRemovedFromViewer(this);
        if (currentlyDisplayed != null)
            currentlyDisplayed = newCard;
        statChangePropogator.Source = newCard.ReadableStats;

    }

    #region Tooltips
    private List<ToolTipBox> toolTips = new List<ToolTipBox>();
    private static Vector3 toolTipOffset = new Vector3(3.7f, 2.1f, 0);
    private static Vector3 toolTipTOffsetPerBox = new Vector3(0, -1.74f, 0);
    public void ShowToolTips()
    {
        ShowToolTips(SourceCard.toolTipInfos);
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
