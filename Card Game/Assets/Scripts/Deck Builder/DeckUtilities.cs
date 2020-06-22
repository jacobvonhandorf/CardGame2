using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DeckUtilities : MonoBehaviour
{
    public static Deck getDeckFromFileName(string deckFileName)
    {
        Dictionary<int, int> idToAmountMap = getDeckMapFromFileName(deckFileName);
        GameObject deckGo = new GameObject();
        deckGo.name = "Deck";
        deckGo.AddComponent<Deck>();
        Deck deck = deckGo.GetComponent<Deck>();

        ResourceManager rs = ResourceManager.Get();
        foreach (KeyValuePair<int, int> kvp in idToAmountMap)
        {
            int id = kvp.Key;
            int amount = kvp.Value;

            for (int i = 0; i < amount; i++)
            {
                rs.instantiateCardById(id).moveToCardPile(deck, false);
            }
        }
        return deck;
    }

    private static Dictionary<int, int> getDeckMapFromFileName(string deckFileName)
    {
        // get file
        if (!File.Exists(Application.persistentDataPath + "/decks/" + deckFileName))
            throw new System.Exception("Deck does not exist: " + Application.persistentDataPath + "/decks/" + deckFileName);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.OpenRead(Application.persistentDataPath + "/decks/" + deckFileName);
        Dictionary<int, int> idToAmountMap = (Dictionary<int, int>)bf.Deserialize(file);
        file.Close();

        return idToAmountMap;
    }

    public static List<Card> getCardListFromFileName(string deckFileName)
    {
        // get file
        if (!File.Exists(Application.persistentDataPath + "/decks/" + deckFileName))
            throw new System.Exception("Deck does not exist: " + Application.persistentDataPath + "/decks/" + deckFileName);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.OpenRead(Application.persistentDataPath + "/decks/" + deckFileName);
        Dictionary<int, int> idToAmountMap = (Dictionary<int, int>)bf.Deserialize(file);
        file.Close();

        ResourceManager rs = ResourceManager.Get();
        List<Card> returnList = new List<Card>();
        foreach (KeyValuePair<int, int> kvp in idToAmountMap)
        {
            int id = kvp.Key;
            int amount = kvp.Value;

            for (int i = 0; i < amount; i++)
            {
                returnList.Add(rs.instantiateCardById(id));
            }
        }

        return returnList;
    }


    public static List<string> getAllDeckNames()
    {
        createFolderIfNotExists();
        List<string> returnList = new List<string>();

        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/decks/");
        FileInfo[] files = dir.GetFiles("*.dek", SearchOption.TopDirectoryOnly);
        foreach (FileInfo file in files)
        {
            returnList.Add(file.Name.Replace(".dek", ""));
        }

        return returnList;
    }

    private static void createFolderIfNotExists()
    {
        if (!File.Exists(Application.persistentDataPath + "/decks"))
            Directory.CreateDirectory(Application.persistentDataPath + "/decks");
    }

}
