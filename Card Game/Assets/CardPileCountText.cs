using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPileCountText : MonoBehaviour
{
    [SerializeField] private CardPile sourcePile;
    private TextMeshPro textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        sourcePile.numCardsChanged.AddListener(UpdateText);
    }

    private void OnDisable()
    {
        sourcePile.numCardsChanged.RemoveListener(UpdateText);
    }

    public void UpdateText()
    {
        textMesh.text = sourcePile.CardList.Count.ToString();
    }
}
