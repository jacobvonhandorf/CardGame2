using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardInDeckBuilder : MonoBehaviour
{
    [SerializeField] private TextMeshPro amountText;
    [SerializeField] private TextMeshPro cardNameText;
    [SerializeField] private Color fireColor;
    [SerializeField] private Color windColor;
    [SerializeField] private Color earthColor;
    [SerializeField] private Color waterColor;
    [SerializeField] private Color nuetralColor;
    [SerializeField] private DeckBuilderMinus minusButton;
    [SerializeField] private DeckBuilderPlus plusButton;
    [SerializeField] private DeckBuilderCardName cardNameBox;
    private int amount = 0;
    private List<SpriteRenderer> sprites;

    private void Awake()
    {
        sprites = new List<SpriteRenderer>();
        foreach (SpriteRenderer sr in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if (sr.gameObject.name == "Background")
                sprites.Add(sr);
        }
    }

    public void setToCard(CardData card, DeckBuilderDeck deck)
    {
        cardNameText.text = card.cardName;
        amount = 0;
        incrementAmount();

        if (card.elementalIdentity == ElementIdentity.Earth)
            setBackgroundColor(earthColor);
        else if (card.elementalIdentity == ElementIdentity.Fire)
            setBackgroundColor(fireColor);
        else if (card.elementalIdentity == ElementIdentity.Water)
            setBackgroundColor(waterColor);
        else if (card.elementalIdentity == ElementIdentity.Wind)
            setBackgroundColor(windColor);
        else if (card.elementalIdentity == ElementIdentity.Nuetral)
            setBackgroundColor(nuetralColor);
        else
            Debug.LogError(card.elementalIdentity);

        minusButton.cardId = card.id;
        minusButton.deck = deck;
        plusButton.cardId = card.id;
        plusButton.deck = deck;

        cardNameBox.cardId = card.id;

        Debug.Log("Setting up card in deck builder");
    }

    public void incrementAmount()
    {
        amount++;
        amountText.text = "" + amount;
    }

    public void decrementAmount()
    {
        amount--;
        amountText.text = "" + amount;
        if (amount == 0)
            Destroy(gameObject);
    }

    private void setBackgroundColor(Color newColor)
    {
        foreach (SpriteRenderer sr in sprites)
            sr.color = newColor;
    }
}
