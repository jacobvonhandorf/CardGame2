using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testScript : MonoBehaviour, CanReceivePickedCards
{
    [SerializeField] CardPicker cardPicker;

    public void receiveCardList(List<Card> cardList)
    {
        Debug.Log("Submitted");
    }

    // Start is called before the first frame update
    void Start()
    {

        // make list of cards
        List<Card> cardList = new List<Card>();
        for (int i = 0; i < 10; i++)
        {
            Card c = ResourceManager.Get().instantiateCardById(76);
            c.removeGraphicsAndCollidersFromScene();
            cardList.Add(c);
        }

        cardPicker.setUp(cardList, this, 2, 4, "Testing :)");
    }
}
