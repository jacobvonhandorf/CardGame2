using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardIdGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<CardData> allIds = ResourceManager.Get().getAllCardData();
        string availableIds = "";
        for (int i = 0; i < allIds.Count; i++)
        {
            bool idFound = false;
            foreach (CardData data in allIds)
            {
                if (data.id == i)
                {
                    idFound = true;
                    break;
                }
            }
            if (!idFound)
                availableIds += i + " ";
        }
        availableIds += allIds.Count;
        Debug.Log(availableIds);
    }
}
