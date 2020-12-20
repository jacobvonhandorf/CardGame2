using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DropHandler : MonoBehaviour, IDropHandler
{
    public UnityEvent DropAction;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop event");
        DropAction?.Invoke();
    }
}
