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
        source.NumCardsChanged.AddListener(SetPositions);
    }

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            //source.AddCard(ResourceManager.Get().InstantiateCardById(CardIds.RingOfEternity));
        }
    }

    public void SetPositions()
    {
        if (source.CardList.Count * desiredDistanceBetweenCards > rect.rect.width * 2)
            SetPositions(source.CardList, rect.rect.width * 2 / source.CardList.Count);
        else
            SetPositions(source.CardList, desiredDistanceBetweenCards);
    }

    private void SetPositions(IReadOnlyList<Card> cards, float distanceBetweenCards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            TransformStruct tStruct = new TransformStruct(source.transform);
            multiplier = (((float)cards.Count) / -2) + .5f + i;
            offset = multiplier * distanceBetweenCards;
            tStruct.position = Vector3.right * offset;
            tStruct.rotation = new Vector3(0, 0, multiplier * -maxRotation);
            tStruct.useLocalPosition = true;

            cards[i].TransformManager.MoveToImmediate(tStruct);
            cards[i].transform.SetSiblingIndex(i);
            cards[i].transform.localPosition = Vector3.zero;
        }
    }
}
