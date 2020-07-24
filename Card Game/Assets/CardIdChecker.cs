using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class CardIdChecker : MonoBehaviour
{
    private const string cardsPath = "All Cards";
    public const string cardDataFileName = "cardData.dat";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting...");
        long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // check for duplicate and unused IDs
        if (checkForDuplicateIDs())
            throw new Exception("Duplicate card IDs found. Fix before continuing");

        // build map
        generateAndSaveCardMap();

        long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Debug.Log("Finished generating and saving map in " + (endTime - startTime) + "ms");

    }

    public static void runAsStatic()
    {
        long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        //if (checkForDuplicateIDs())
        //    throw new Exception("Duplicate card IDs found. Fix before continuing");
        generateAndSaveCardMap();
        long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Debug.Log("Finished generating and saving map in " + (endTime - startTime) + "ms");
    }

    // returns true if there are duplicate IDs
    private static bool checkForDuplicateIDs()
    {
        bool duplicateFound = false;

        // have a list of IDs
        List<int> usedIds = new List<int>();

        // loop through each card and add to usedIds
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>(cardsPath);
        foreach (GameObject prefab in loadedPrefabs)
        {
            GameObject newGameObject = Instantiate(prefab);
            Card newCard = newGameObject.GetComponentInChildren<Card>();
            if (newCard == null)
            {
                Debug.LogError(newGameObject + " had no Card component");
                continue;
            }

            if (usedIds.Contains(newCard.cardId))
            {
                duplicateFound = true;
                Debug.LogError(newCard.getCardName() + " has duplicate ID " + newCard.cardId);
            }
            else
            {
                usedIds.Add(newCard.cardId);
            }

            // Destroy card when finished with it
            Destroy(newGameObject);
        }

        // print out unused IDs
        for (int i = 0; i < usedIds.Count; i++)
        {
            if (!usedIds.Contains(i))
                Debug.Log("Unused ID: " + i);
        }

        return duplicateFound;
    }

    private static void generateAndSaveCardMap()
    {
        GameObject[] allCards = Resources.LoadAll<GameObject>(cardsPath);
        Vector3 tempPosition = new Vector3(999, 999, 999);
        foreach (GameObject go in allCards)
        {
            GameObject instantiatedGo = Instantiate(go, tempPosition, Quaternion.identity);
            //cardIdMap.Add(instantiatedGo.GetComponent<Card>().cardId, )
            Destroy(instantiatedGo);
        }

        Dictionary<int, string> cardIdMap = new Dictionary<int, string>(); // ID, path to card
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/" + cardsPath);
        FileInfo[] files = dir.GetFiles("*.prefab", SearchOption.AllDirectories);

        Debug.Log("Number of cards detected " + files.Length);
        foreach (FileInfo f in files)
        {
            //Debug.Log(f.FullName);
            string pathToRemove = Application.dataPath.Replace("/", "\\");
            string pathInResources = f.FullName.Replace(pathToRemove, "");
            pathInResources = pathInResources.Replace("\\", "/");
            pathInResources = pathInResources.Replace("/Resources/", "");
            pathInResources = pathInResources.Replace(".prefab", "");
            //Debug.Log(pathInResources);
            GameObject card = Resources.Load(pathInResources) as GameObject;
            GameObject objectToDestroy =  Instantiate(card, new Vector3(999, 999), Quaternion.identity);
            cardIdMap.Add(objectToDestroy.GetComponent<Card>().cardId, pathInResources);
            Destroy(objectToDestroy);
        }

        // save map
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Open(Application.persistentDataPath + "/" + cardDataFileName, FileMode.Create);
        bf.Serialize(fs, cardIdMap);
        fs.Close();
    }

}
