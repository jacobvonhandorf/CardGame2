using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetTestingClient : MonoBehaviour
{
    public void doGameSetup()
    {
        NetInterface.Get().SyncStartingDeck();
    }
}
