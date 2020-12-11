using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HandPositionSetter : MonoBehaviour
{
    [SerializeField] private float desiredDistanceBetweenCards;
    [SerializeField] private float maxRotation;
    private RectTransform rect;
    private Hand source;

    private Vector3 temp; // used to prevent memory allocation when setting tranform
    private float multiplier;
    private float offset;
    //private TransformStruct tStruct = new TransformStruct();

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        source = GetComponent<Hand>();
    }

    private void Start()
    {
        /*
        source.AddCard(ResourceManager.Get().InstantiateCardById(CardIds.Altar));
        source.AddCard(ResourceManager.Get().InstantiateCardById(CardIds.Mercenary));
        source.AddCard(ResourceManager.Get().InstantiateCardById(CardIds.RingOfEternity));
        */
        Card c1 = ResourceManager.Get().InstantiateCardById(CardIds.Altar);
        Card c2 = ResourceManager.Get().InstantiateCardById(CardIds.Mercenary);
        Card c3 = ResourceManager.Get().InstantiateCardById(CardIds.RingOfEternity);
        Card c4 = ResourceManager.Get().InstantiateCardById(CardIds.SpellRecycling);
        Card c5 = ResourceManager.Get().InstantiateCardById(CardIds.RingOfEternity);
        //SetPositions(new List<Card>() { c1, c2, c3 }, 100);
        source.AddCards(new List<Card>() { c1, c2, c3, c4, c5 });
        c3.transform.localScale = Vector3.one * .5f;
        c4.transform.localScale = Vector3.one * .5f;
        c5.transform.localScale = Vector3.one * .5f;
        //for (int i = 0; i < 8; i++)
          //  source.AddCard(ResourceManager.Get().InstantiateCardById(CardIds.Tower));
    }

    public void SetPositions()
    {
        if (source.CardList.Count * desiredDistanceBetweenCards > rect.rect.width)
            SetPositions(source.CardList, rect.rect.width / source.CardList.Count);
        else
            SetPositions(source.CardList, desiredDistanceBetweenCards);
    }

    private void SetPositions(IReadOnlyList<Card> cards, float distanceBetweenCards)
    {
        Debug.Log("Distance btween cards " + distanceBetweenCards);
        for (int i = 0; i < cards.Count; i++)
        {
            TransformStruct tStruct = new TransformStruct(source.transform);
            multiplier = (((float)cards.Count) / -2) + .5f + i;
            offset = multiplier * distanceBetweenCards;
            tStruct.position = Vector3.right * offset + source.transform.position;
            tStruct.rotation = new Vector3(0, 0, multiplier * -maxRotation);
            cards[i].TransformManager.MoveToImmediate(tStruct);
            cards[i].transform.SetSiblingIndex(i);
        }
    }
}
