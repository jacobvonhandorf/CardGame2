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

    public void setToCard(Card card, DeckBuilderDeck deck)
    {
        cardNameText.text = card.getCardName();
        amount = 0;
        incrementAmount();

        if (card.getElementIdentity() == Card.ElementIdentity.Earth)
            setBackgroundColor(earthColor);
        if (card.getElementIdentity() == Card.ElementIdentity.Fire)
            setBackgroundColor(fireColor);
        if (card.getElementIdentity() == Card.ElementIdentity.Water)
            setBackgroundColor(waterColor);
        if (card.getElementIdentity() == Card.ElementIdentity.Wind)
            setBackgroundColor(windColor);

        minusButton.card = card;
        minusButton.deck = deck;
        plusButton.card = card;
        plusButton.deck = deck;

        cardNameBox.sourceCard = card;

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
