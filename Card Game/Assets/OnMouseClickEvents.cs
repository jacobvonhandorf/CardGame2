using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnMouseClickEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnMouseClick;
    private bool mouseDownOnThis = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        mouseDownOnThis = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (mouseDownOnThis && EventSystem.current.IsPointerOverGameObject())
        {
            OnMouseClick.Invoke();
        }
    }

    private void Update()
    {
        mouseDownOnThis = mouseDownOnThis && !(Input.GetMouseButtonUp(0) && EventSystem.current.IsPointerOverGameObject());
    }
}
