using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionChecker : MonoBehaviour
{
    public static VersionChecker instance;

    private static bool alreadyRun = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void recieveMessage(Net_CheckVersion msg)
    {
        string serverVersion = msg.version;
        string clientVersion = PlayerPrefs.GetString(PlayerPrefEnum.clientVersion);

        if (clientVersion != serverVersion)
        {
            //CardIdChecker.runAsStatic();
            // then remove ui blocker
        }
    }
}
