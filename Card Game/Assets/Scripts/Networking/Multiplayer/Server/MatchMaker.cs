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
                MatchMakerObject mmo1 = matchMakerPool.First.Value;
                MatchMakerObject mmo2 =  matchMakerPool.First.Next.Value;
                matchMakerPool.Remove(mmo1);
                matchMakerPool.Remove(mmo2);
                PairUsers(mmo1, mmo2);
            }
            // for all users left increment their cycles waited
            foreach (MatchMakerObject mmo in matchMakerPool)
            {
                mmo.cyclesWaited++;
            }
            yield return 1f; // only do one cycle per second to save cpu
        }
    }

    private void PairUsers(MatchMakerObject mmo1, MatchMakerObject mmo2)
    {
        Server.Instance.PairUsers(mmo1, mmo2);
    }

    public void QueueMatchMakerObject(MatchMakerObject mmo)
    {
        matchMakerPool.AddLast(mmo);
    }

    public void RemoveMatchMakerObject(MatchMakerObject mmo)
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

    public static MatchMaker GetInstance()
    {
        if (instance != null)
            return instance;
        GameObject newInstance = new GameObject();
        instance = newInstance.AddComponent<MatchMaker>();
        return instance;
    }
}

// a way to uniquely identify users in mm pool
public class MatchMakerObject : IComparable<MatchMakerObject>
{
    public int connectionId;
    public int hostId;
    public int channelId;
    public int rating;
    public int cyclesWaited;

    public int CompareTo(MatchMakerObject other)
    {
        if (connectionId == other.connectionId)
            return 0;
        else
            return 1;
    }
}
