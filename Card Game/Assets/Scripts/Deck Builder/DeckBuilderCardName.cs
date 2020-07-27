using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderCardName : MonoBehaviour
{
    private const float X_OFFSET = 3.0f;
    private const float MIN_Y = -2.5f;

    public int cardId;

    private void OnMouseOver()
    {
        CardViewer viewer = DeckBuilderDeck.instance.hoveredCardViewer;
        viewer.setCard(cardId);
        Vector3 newPosition = transform.position;
        newPosition.x = X_OFFSET;
        if (newPosition.y < MIN_Y)
            newPosition.y = MIN_Y;
        viewer.transform.position = newPosition;
    }

    private void OnMouseExit()
    {
        if (DeckBuilderDeck.instance.hoveredCardViewer.sourceCardId == cardId)
            DeckBuilderDeck.instance.hoveredCardViewer.gameObject.SetActive(false);
    }

    private void OnMouseUpAsButton()
    {
        DeckBuilderDeck.instance.removeCard(cardId);
    }

    private void OnDestroy()
    {
        if (DeckBuilderDeck.instance != null && DeckBuilderDeck.instance.hoveredCardViewer != null)
            if (DeckBuilderDeck.instance.hoveredCardViewer.sourceCardId == cardId)
                DeckBuilderDeck.instance.hoveredCardViewer.gameObject.SetActive(false);
    }
}
