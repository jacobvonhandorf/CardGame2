﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBlocker : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(waitAndDisable());
    }

    IEnumerator waitAndDisable()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
}
