using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewerForPiles : CardViewer
{
    private void OnMouseEnter()
    {
        if (GameManager.Get() != null)
            GameManager.Get().getCardViewer().setCard(sourceCard);
    }

}
