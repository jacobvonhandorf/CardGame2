using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
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
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        cardDataMap = new Dictionary<int, CardData>();
        CardData[] dataArray = Resources.LoadAll<CardData>("Card Data");
        foreach (CardData data in dataArray)
            cardDataMap.Add(data.id, data);
        stopwatch.Stop();
        Debug.Log("Time to load card data " + stopwatch.ElapsedMilliseconds + "ms");
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

    public List<CardData> getAllCardDataVisibleInDeckBuilder()
    {
        List<CardData> returnList = new List<CardData>();
        foreach (CardData data in cardDataMap.Values)
        {
            if (data.visibleInDeckBuilder)
                returnList.Add(data);
        }
        return returnList;
    }

    public List<CardData> getAllCardData() => new List<CardData>(cardDataMap.Values);

    public CardData getCardDataById(int id) => cardDataMap[id];
}
