using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class DeckBuilderDeck : MonoBehaviour
{
    private const int maxNumberOfCardsInDeck = 60;
    private const int minNumerOfCardsInDeck = 40;
    private const int maxCopiesOfCard = 3;
    [SerializeField] private CardInDeckBuilder cardInDeckBuilderPrefab;
    [SerializeField] private Transform scollTransform;
    [SerializeField] private SpriteScroller scroller;
    [SerializeField] private TextMeshPro cardCountText;
    [SerializeField] private TMP_InputField deckNameField;
    [SerializeField] private YesNoBox yesNoBoxPrefab;
    public GameObject glassBackground;


    private int cardCount = 0;
    private List<CardAmountPair> cardList = new List<CardAmountPair>();

    private void Start()
    {
        deckNameField.characterLimit = 22;
    }

    public void addCard(Card newCard)
    {
        bool cardAlreadyInList = false;
        foreach (CardAmountPair pair in cardList)
        {
            if (pair.card.getCardId() == newCard.getCardId())
            {
                cardAlreadyInList = true;
                if (pair.amount == maxCopiesOfCard) // can't put more than 3 cards in deck
                    return;
                pair.cardInDeckBuilder.incrementAmount();
                pair.amount++;
                resetCardBarPositions();

                break;
            }
        }

        if (!cardAlreadyInList)
        {
            CardInDeckBuilder newCardInDeckBuilder = Instantiate(cardInDeckBuilderPrefab, scollTransform);
            newCardInDeckBuilder.setToCard(newCard, this);
            cardList.Add(new CardAmountPair(newCard, 1, newCardInDeckBuilder));
            resetCardBarPositions();
        }
        cardCount++;
        cardCountText.text = "Card Count: " + cardCount;
    }

    public void removeCard(Card newCard)
    {
        foreach (CardAmountPair pair in cardList)
        {
            if (pair.card == newCard)
            {
                pair.amount--;
                pair.cardInDeckBuilder.decrementAmount();
                cardCount--;
                cardCountText.text = "Card Count: " + cardCount;
                if (pair.amount == 0)
                {
                    cardList.Remove(pair);
                    resetCardBarPositions();
                }
                break;
            }
        }
    }

    public void save(string deckName)
    {
        if (deckName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            YesNoBox yesNoBox = Instantiate(yesNoBoxPrefab);
            yesNoBox.setUp(null, "Invalid Deck Name", "The deck name " + deckName + " contains illegal characters. Please choose a different name.");
            yesNoBox.setNoButtonActive(false);
            yesNoBox.setYesButtonText("Okay");
            return;
        }

        // put together string that will be the deck file
        string filePath = Application.persistentDataPath + "/decks/" + deckName + ".dek";

        // create deck folder if needed
        string directoryPath = Application.persistentDataPath + "/decks";
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(directoryPath);

        Dictionary<int, int> idToAmountMap = new Dictionary<int, int>();
        foreach (CardAmountPair pair in cardList)
        {

            int id = pair.card.getCardId();
            int amount = pair.amount;
            idToAmountMap.Add(id, amount);
        }
        // save the map as a file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Open(filePath, FileMode.Create);
        bf.Serialize(fs, idToAmountMap);
    }

    public void save()
    {
        // check if deck is more than 40 cards
        if (cardCount < minNumerOfCardsInDeck)
        {
            Toaster.Get().doToast("Deck must contain at least " + minNumerOfCardsInDeck + " cards");
            return;
        }
        else if (cardCount > maxNumberOfCardsInDeck)
        {
            Toaster.Get().doToast("Deck must be under " + maxNumberOfCardsInDeck + " cards");
            return;
        }

        string deckName = deckNameField.text.Trim();

        // check to see if the deck already exists
        string filePath = Application.persistentDataPath + "/decks/" + deckName + ".dek";
        if (File.Exists(filePath))
        {
            glassBackground.SetActive(true);
            // if it does display a yes/no box
            YesNoBox yesNoBox = Instantiate(yesNoBoxPrefab);
            SaveConfirmHandler handler = new SaveConfirmHandler();
            handler.deckBuilderDeck = this;
            handler.deckName = deckName;
            yesNoBox.setUp(handler, "Overwrite Confimation", "A deck with the name " + deckName + " already exists. Would you like to overwrite the saved deck?");
            
        }
        else
        {
            // otherwise just save
            save(deckName);
            Toaster.Get().doToast("Deck Saved");
        }
    }

    private class SaveConfirmHandler : YesNoHandler
    {
        public string deckName;
        public DeckBuilderDeck deckBuilderDeck;

        public void onNoClicked()
        {
            deckBuilderDeck.glassBackground.SetActive(false);
        }

        public void onYesClicked()
        {
            deckBuilderDeck.glassBackground.SetActive(false);
            deckBuilderDeck.save(deckName);
            Toaster.Get().doToast("Deck Saved");
        }
    }

    private class CardAmountPair : IComparable<CardAmountPair>
    {
        public Card card;
        public int amount;
        public CardInDeckBuilder cardInDeckBuilder;

        public CardAmountPair(Card newCard, int v, CardInDeckBuilder newCardInDeckBuilder)
        {
            card = newCard;
            amount = v;
            cardInDeckBuilder = newCardInDeckBuilder;
        }

        public int CompareTo(CardAmountPair other)
        {
            return new CardComparatorByCostFirst().Compare(card, other.card);
        }
    }

    public float cardBarYOffset;
    public float cardBarXOffset;
    public float cardBarCoeff;
    public float scrollerOffset;
    public float scrollerCoefficient;
    private void resetCardBarPositions()
    {
        Debug.Log("resetting bar positions");
        cardList.Sort();
        // should probably sort here first
        int index = 0;
        Transform t = scroller.contentTransform;
        foreach (CardAmountPair pair in cardList)
        {
            CardInDeckBuilder cardInDeckBuilder = pair.cardInDeckBuilder;
            Vector3 newPosition = new Vector3(t.position.x + cardBarXOffset, t.position.y + cardBarYOffset + (index * cardBarCoeff), -1);
            cardInDeckBuilder.transform.position = newPosition;
            index++;
        }
        float scrollerNewMaxY = scrollerOffset + scrollerCoefficient * index;
        if (scrollerNewMaxY < 0f)
            scrollerNewMaxY = 0f;
        scroller.maxY = scrollerNewMaxY;
    }

    public void load(string deckName)
    {
        // clear current list before loading new one
        clear();

        if (File.Exists(Application.persistentDataPath + "/decks/" + deckName + ".dek"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.OpenRead(Application.persistentDataPath + "/decks/" + deckName + ".dek");
            Dictionary<int, int> deckMap = (Dictionary<int, int>)bf.Deserialize(file);
            file.Close();
            foreach (int id in deckMap.Keys)
            {
                Card newCard = ResourceManager.Get().instantiateCardById(id);
                for (int i = 0; i < deckMap[id]; i++)
                {
                    addCard(newCard);
                }
                Destroy(newCard.getRootTransform().gameObject);
            }
            // set deck name text
            Debug.Log("Setting deck name text to: " + deckName);
            deckNameField.text = deckName;
        }
        else
        {
            throw new Exception("Deck not found: " + deckName);
        }
    }

    // remove all cards from deck view
    public void clear()
    {
        List<CardAmountPair> tempList = new List<CardAmountPair>();
        foreach (CardAmountPair c in cardList)
        {
            tempList.Add(c);
        }
        foreach (CardAmountPair cap in tempList)
        {
            while (cap.amount > 0)
                removeCard(cap.card);
        }
    }

    public void delete()
    {
        string deckName = deckNameField.text;
        string filePath = Application.persistentDataPath + "/decks/" + deckName + ".dek";
        if (File.Exists(filePath))
        {
            // show confirm dialouge
            YesNoBox yesNoBox = Instantiate(yesNoBoxPrefab);
            DeleteConfirmHandler handler = new DeleteConfirmHandler();
            handler.deckName = deckName;
            handler.deckBuilder = this;
            yesNoBox.setUp(handler, "Delete Confirmation", "Are you sure you want to delete " + deckName + "?");
            glassBackground.SetActive(true);
        }
        else
        {
            // do nothing (maybe change to error dialouge later)
            Debug.LogError("Deck not found " + filePath);
        }
    }
    private class DeleteConfirmHandler : YesNoHandler
    {
        public string deckName;
        public DeckBuilderDeck deckBuilder;

        public void onNoClicked()
        {
            deckBuilder.glassBackground.SetActive(false);
        }

        public void onYesClicked()
        {
            deckBuilder.glassBackground.SetActive(false);
            // do another check to make sure the file exists then delete it
            string filePath = Application.persistentDataPath + "/decks/" + deckName + ".dek";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            deckBuilder.clear();
        }
    }


    public static DeckBuilderDeck getDeckByName(string deckName)
    {
        if (File.Exists(Application.persistentDataPath + "/deck/" + deckName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.OpenRead(Application.persistentDataPath + "/decks/" + deckName);
            string deckString = (string)bf.Deserialize(file);
            file.Close();

            string[] lines = deckString.Split('\n');
            foreach (string line in lines)
            {
                //ResourceManager.Get().
            }
        }

        throw new Exception("Deck " + deckName + " not found");
    }
}
