using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewerForPiles : CardViewer
{
    private void OnMouseEnter()
    {
        GameManager.Get().getCardViewer().setCard(sourceCard);
    }

}
