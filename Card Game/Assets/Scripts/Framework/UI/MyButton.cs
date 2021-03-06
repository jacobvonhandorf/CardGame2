﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MyButton : MonoBehaviour
{
    [SerializeField] private UnityEvent methodToCall;
    [SerializeField] private SpriteRenderer buttonBackground;
    [SerializeField] private Color up;
    [SerializeField] private Color hovered;
    [SerializeField] private Color down;
    [SerializeField] protected TextMeshPro textMesh;

    private void OnMouseUpAsButton()
    {
        if (methodToCall != null)
            methodToCall.Invoke();
    }

    private void OnMouseEnter()
    {
        buttonBackground.color = hovered;
    }
    private void OnMouseExit()
    {
        setTextToUp();
    }
    private void OnMouseDown()
    {
        buttonBackground.color = down;
    }
    private void OnMouseUp()
    {
        buttonBackground.color = up;
    }

    public void setTextToUp()
    {
        buttonBackground.color = up;
    }

    public void addAction(UnityAction action)
    {
        methodToCall.AddListener(action);
    }

    public void setText(string s)
    {
        textMesh.text = s;
    }

    public void enable()
    {
        setTextToUp();
        gameObject.SetActive(true);
    }

    public void disable()
    {
        setTextToUp();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        setTextToUp();
    }
}
