using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MatchMaker : MonoBehaviour
{
    private static MatchMaker instance;

    private Queue<MatchMakerObject> matchMakerPool = new Queue<MatchMakerObject>();

    private void Start()
    {
        StartCoroutine(mmCycle().GetEnumerator());
    }
    private IEnumerable mmCycle()
    {
        while (true)
        {
            // perform mm cycle
            for (int i = 0; i + 1 < matchMakerPool.Count; i += 2)
            {
                // for now just pair them against each other
                MatchMakerObject mmo1 =  matchMakerPool.Dequeue();
                MatchMakerObject mmo2 =  matchMakerPool.Dequeue();
                pairUsers(mmo1, mmo2);
            }
            // for all users left increment their cycles waited
            foreach (MatchMakerObject mmo in matchMakerPool)
            {
                mmo.cyclesWaited++;
            }
            yield return 1f; // only do one cycle per second to save cpu
        }
    }

    private void pairUsers(MatchMakerObject mmo1, MatchMakerObject mmo2)
    {
        Debug.LogError("Making match");
        Server.Instance.pairUsers(mmo1, mmo2);
    }

    public void queueMatchMakerObject(MatchMakerObject mmo)
    {
        matchMakerPool.Enqueue(mmo);
        Debug.LogError("User added to mm queue");
    }

    public static MatchMaker getInstance()
    {
        if (instance != null)
            return instance;
        GameObject newInstance = new GameObject();
        instance = newInstance.AddComponent<MatchMaker>();
        return instance;
    }
}

// a way to uniquely identify users in mm pool
// rating of person
// amount of time waited
public class MatchMakerObject
{
    public int connectionId;
    public int hostId;
    public int channelId;
    public int rating;
    public int cyclesWaited;
}
