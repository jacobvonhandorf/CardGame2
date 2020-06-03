using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionChecker : MonoBehaviour
{
    public static VersionChecker instance;

    // Start is called before the first frame update
    void Start()
    {
        // CardIdChecker.runAsStatic();
        return;
        instance = this;
        // send message to server asking for current version. Also put up a ui blocker.
        Net_CheckVersion msg = new Net_CheckVersion();
        Client.Instance.SendServer(msg);
    }

    public void recieveMessage(Net_CheckVersion msg)
    {
        string serverVersion = msg.version;
        string clientVersion = PlayerPrefs.GetString(PlayerPrefEnum.clientVersion);

        if (clientVersion != serverVersion)
        {
            CardIdChecker.runAsStatic();
            // then remove ui blocker
        }
    }
}
