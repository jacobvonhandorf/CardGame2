using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// close button for CardPileViewer
public class CardPileViewerClose : MonoBehaviour
{
    public CardPileViewer cardPileViwer;

    private void OnMouseDown()
    {
        cardPileViwer.Close();
    }
}
