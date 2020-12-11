using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private TransformManager transformManager;

    private void Start()
    {
        transformManager = GetComponentInParent<TransformManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transformManager.ClearQueue();
        transformManager.Lock();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transformManager.UnLock();
    }
}
