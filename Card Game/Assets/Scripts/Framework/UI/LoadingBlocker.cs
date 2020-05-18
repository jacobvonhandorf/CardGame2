using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBlocker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitAndDisable());
    }

    IEnumerator waitAndDisable()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
        GameManager.Get().activePlayer.hand.resetCardPositions();
        GameManager.Get().nonActivePlayer.hand.resetCardPositions();
    }
}
