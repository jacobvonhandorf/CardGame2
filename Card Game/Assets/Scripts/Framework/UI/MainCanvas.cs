using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public static Canvas Instance { get; private set; }

    private void Awake()
    {
        Instance = GetComponent<Canvas>();
    }
}
