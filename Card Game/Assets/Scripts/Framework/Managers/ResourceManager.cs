using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager instance;
    private static Dictionary<int, string> idToPathPairs;

    private void Start()
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
        if (idToPathPairs == null)
            setupNameToPathPairs();
    }

    private void setupNameToPathPairs()
    {
        if (!File.Exists(Application.persistentDataPath + "/cardData.dat"))
            CardIdChecker.runAsStatic();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.OpenRead(Application.persistentDataPath + "/cardData.dat");
        idToPathPairs = (Dictionary<int, string>)bf.Deserialize(file);
        file.Close();
    }

    public static ResourceManager Get()
    {
        if (instance == null)
        {
            // if an instance doesn't exist then instantiate one
            // this method of instantiation is not desired because it is inefficient. Place a resource manager in scene instead.
            GameObject resourceManager = new GameObject();
            resourceManager.name = "Resource Manager";
            ResourceManager newInstance = resourceManager.AddComponent<ResourceManager>();
            return newInstance;
        }
        else
            return instance;
    }

    // if you call this then make sure you disable either the card or structure script
    public Structure getStructureFromResources(Player owner, string structureName)
    {
        GameObject structureGameObject = Resources.Load("TokenPrefabs/Structure/" + structureName + " Variant") as GameObject;
        structureGameObject = Instantiate(structureGameObject);

        Structure structure = structureGameObject.transform.Find("Graphics Root/Structure Script").GetComponent<Structure>();
        StructureCard sourceCard = structureGameObject.GetComponentInChildren<StructureCard>();
        StructureStatsGetter statsScript = structureGameObject.GetComponentInChildren<StructureStatsGetter>();

        structure.sourceCard = sourceCard;
        structure.owner = owner;
        structure.controller = owner;
        structure.setStatsScript(statsScript);
        sourceCard.structure = structure;

        return structure;
    }

    public Card instantiateCardById(int id)
    {
        if (idToPathPairs == null)
            setupNameToPathPairs();
        string pathToCard;
        try
        {
            pathToCard = idToPathPairs[id];
        } catch (KeyNotFoundException e)
        {
            Debug.LogError("Id not found + " + id);
            throw e;
        }
        GameObject gameObject = Resources.Load(pathToCard) as GameObject;
        GameObject instantiatedGameObject = Instantiate(gameObject);
        Card card = instantiatedGameObject.GetComponentInChildren<Card>();
        if (card == null)
            throw new System.Exception("Error loading card from resources");

        // if online game is in progress then sync the new card
        //if (GameManager.Get().gameInProgress && GameManager.gameMode == GameManager.GameMode.online && syncAcrossNetwork)
        //    NetInterface.Get().syncNewCardToOpponent(card);
        return card;
    }

}
