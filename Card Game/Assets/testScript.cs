using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class testScript : MonoBehaviour, CanReceivePickedCards, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("On pointer click");
    }

    public void receiveCardList(List<Card> cardList)
    {
        Debug.Log("Submitted");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
            Debug.Log("Mouse button 2 pressed");
    }

    private void OnMouseDown()
    {
        Debug.Log("Mouse down");
    }
}
