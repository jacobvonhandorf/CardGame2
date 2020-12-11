using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;
using System.Linq;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager instance;
    private static Dictionary<int, CardData> cardDataMap;

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
            SetupCardDataMap();
    }

    private void SetupCardDataMap()
    {
        Debug.Log("Generating card data map");
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        cardDataMap = new Dictionary<int, CardData>();
        CardData[] dataArray = Resources.LoadAll<CardData>("Card Data");
        foreach (CardData data in dataArray)
        {
            //Debug.Log(data.name + " " + data.id);
            cardDataMap.Add(data.id, data);
        }
        stopwatch.Stop();
        Debug.Log("Time to load card data " + stopwatch.ElapsedMilliseconds + "ms");
    }


    public Card InstantiateCardById(int id) => CardBuilder.Instance.BuildFromCardData(cardDataMap[id]);
    public Card InstantiateCardById(CardIds id) => InstantiateCardById((int)id);

    public List<CardData> GetAllCardDataVisibleInDeckBuilder() => cardDataMap.Values.Where(d => d.visibleInDeckBuilder).ToList();
    public List<CardData> GetAllCardData() => new List<CardData>(cardDataMap.Values);
    public CardData GetCardDataById(int id) => cardDataMap[id];
    public CardData GetCardDataById(CardIds id) => GetCardDataById((int)id);
}
