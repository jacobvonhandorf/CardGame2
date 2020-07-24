using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager instance;
    private static Dictionary<int, CardData> cardDataMap;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.Log("More than one resource manager. Resource manager should be singleton");
            Destroy(gameObject);
            return;
        }
        // Get path for all cards
        if (cardDataMap == null)
            setupCardDataMap();
    }

    private void setupCardDataMap()
    {
        Debug.Log("Generating card data map");
        cardDataMap = new Dictionary<int, CardData>();
        CardData[] dataArray = Resources.LoadAll<CardData>("Card Data");
        foreach (CardData data in dataArray)
            cardDataMap.Add(data.id, data);
    }

    public static ResourceManager Get()
    {
        if (instance == null)
        {
            // if an instance doesn't exist then instantiate one
            GameObject resourceManager = new GameObject();
            resourceManager.name = "Resource Manager";
            instance = resourceManager.AddComponent<ResourceManager>();
        }
        return instance;
    }

    public Card instantiateCardById(int id) => CardBuilder.Instance.BuildFromCardData(cardDataMap[id]);

    public List<Card> getAllCardsVisibleInDeckBuilder()
    {
        List<Card> returnList = new List<Card>();
        foreach (CardData data in cardDataMap.Values)
        {
            if (data.visibleInDeckBuilder)
                returnList.Add(CardBuilder.Instance.BuildFromCardData(data));
        }
        return returnList;
    }

    public CardData getCardDataById(int id) => cardDataMap[id];
}
