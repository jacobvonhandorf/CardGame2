using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MatchMaker : MonoBehaviour
{
    private static MatchMaker instance;

    private LinkedList<MatchMakerObject> matchMakerPool = new LinkedList<MatchMakerObject>();

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
                MatchMakerObject mmo1 = matchMakerPool.First.Value;
                MatchMakerObject mmo2 =  matchMakerPool.First.Next.Value;
                matchMakerPool.Remove(mmo1);
                matchMakerPool.Remove(mmo2);
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
        matchMakerPool.AddLast(mmo);
        Debug.LogError("User added to mm queue");
    }

    public void removeMatchMakerObject(MatchMakerObject mmo)
    {
        foreach (MatchMakerObject current in matchMakerPool)
        {
            if (current.connectionId == mmo.connectionId)
            {
                matchMakerPool.Remove(current);
                return;
            }
        }
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
public class MatchMakerObject : IComparable<MatchMakerObject>
{
    public int connectionId;
    public int hostId;
    public int channelId;
    public int rating;
    public int cyclesWaited;

    public int CompareTo(MatchMakerObject other)
    {
        Debug.Log("Connection id 1 = " + connectionId + "\nConnection id 2 = " + other.connectionId);
        if (connectionId == other.connectionId)
            return 0;
        else
            return 1;
    }
}
