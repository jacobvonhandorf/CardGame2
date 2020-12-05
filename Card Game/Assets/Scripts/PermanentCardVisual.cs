using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PermanentCardVisual : CardVisual
{
    private const float scalingCoefficient = 2.4f;
    private const float entireCardScaleCoefficient = .165f;

    [SerializeField] private List<Transform> permanentOnlyIcons;
    [SerializeField] private TextMeshPro hasActedTextIndicator; // move to creature only
    [SerializeField] private Image friendOrFoeBorder;

    private CardToPermanentConverter cardToPermanentConverter;

    private void Awake()
    {
        cardToPermanentConverter = GetComponent<CardToPermanentConverter>();
    }

    public void ResizeToPermanent(Vector3 newPosition) => cardToPermanentConverter.DoConversion(newPosition);

    /*
    // Called when a card is played to a creature.
    // if the topology of a card is changed this method may need to be changed
    public void ResizeToPermanent(Tile creatureTile)
    {
        Vector3 newIconScale = new Vector3(scalingCoefficient, scalingCoefficient, 1);
        Vector3 newRootScale = new Vector3(entireCardScaleCoefficient, entireCardScaleCoefficient, 1);

        Vector3 newPosition = creatureTile.transform.position;
        //newPosition.z = 0;
        InformativeAnimationsQueue.Instance.addAnimation(new SwapToCreatureAnimation(this, newIconScale, newRootScale, newPosition));
    }
    private class SwapToCreatureAnimation : QueueableCommand
    {
        PermanentCardVisual cardVisual;
        Vector3 newIconScale;
        Vector3 newRootScale;
        Vector3 newPostion;

        public SwapToCreatureAnimation(PermanentCardVisual cardVisual, Vector3 newIconScale, Vector3 newRootScale, Vector3 newPostion)
        {
            this.cardVisual = cardVisual;
            this.newIconScale = newIconScale;
            this.newRootScale = newRootScale;
            this.newPostion = newPostion;
        }

        public override bool IsFinished => cardVisual.resizeToCreatureFinished;

        public override void Execute()
        {
            cardVisual.resizeToCreatureFinished = false;
            cardVisual.StartCoroutine(cardVisual.ResizeToCreature(cardVisual.transform, newRootScale, newIconScale, newPostion));
        }
    }

    [SerializeField] private float resizeSpeed = 1.3f;
    [SerializeField] private float pauseBetweenResize = 0.5f;
    [SerializeField] private float iconResizeSpeed = 10f;
    private bool resizeToCreatureFinished;
    IEnumerator ResizeToCreature(Transform cardRoot, Vector3 newRootScale, Vector3 newIconScale, Vector3 newPosition)
    {
        resizeToCreatureFinished = false;

        Vector3 positionStart = cardRoot.position;
        Vector3 positionEnd = newPosition;
        Vector3 scaleStart = cardRoot.localScale;
        Vector3 scaleEnd = newRootScale;
        // resize root
        float currentPercentage = 0;
        float timeForTotalAnimation = .3f;
        float timePassed = 0;
        while (currentPercentage < .98f)
        {
            timePassed += Time.deltaTime;
            currentPercentage = timePassed / timeForTotalAnimation;

            cardRoot.position = Vector3.Lerp(positionStart, positionEnd, currentPercentage);
            cardRoot.localScale = Vector3.Lerp(scaleStart, scaleEnd, currentPercentage);
            yield return null;
        }

        // pause
        yield return new WaitForSeconds(pauseBetweenResize);

        // resize icons
        while (Vector3.Distance(permanentOnlyIcons[0].localScale, newIconScale) > 0.02f)
        {
            foreach (Transform t in permanentOnlyIcons)
                t.localScale = Vector3.MoveTowards(t.localScale, newIconScale, iconResizeSpeed * Time.deltaTime);
            yield return null;
        }
        friendOrFoeBorder.gameObject.SetActive(true);
        resizeToCreatureFinished = true;
    }
    */

    // called when a creature leaves the field and needs to act like a card again
    // if the topology of a card is changed this method may need to be changed
    public void ResizeToCard()
    {
        friendOrFoeBorder.gameObject.SetActive(false);

        foreach (Transform icon in permanentOnlyIcons)
        {
            icon.localScale = Vector3.one;
        }

        Vector3 newRootScale = new Vector3(.5f, .5f, 1);
        transform.localScale = newRootScale;
    }

    public void SetIsAlly(bool isAlly) => friendOrFoeBorder.color = isAlly ? Color.blue : Color.red;
}
