using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StructureStatsGetter : CardStatsGetter
{
    private const float scalingCoefficient = 2.4f;
    private const float entireCardScaleCoefficient = .165f;

    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private SpriteRenderer friendOrFoeBorder;

    public override void setCardViewer(CardViewer viewer)
    {
        viewer.gameObject.SetActive(true);

        // flip everything to active that needs to be active
        // and flip everything to inactive that should be inactive
        viewer.hpGameObject.SetActive(hpText.gameObject.activeInHierarchy);
        viewer.setGoldActive(goldText.gameObject.activeInHierarchy);
        viewer.setManaActive(manaText1.gameObject.activeInHierarchy);
        viewer.setManaLowerActive(manaText2.gameObject.activeInHierarchy);
        viewer.setFullBodyTextActive(true);
        viewer.setAttackActive(false);
        viewer.setMoveActive(false);
        viewer.setHalfBodyTextActive(false);

        // set all values that need to be set
        viewer.hpText.text = hpText.text;
        viewer.goldText.text = goldText.text;
        viewer.manaText1.text = manaText1.text;
        viewer.manaText2.text = manaText2.text;
        viewer.fullBodyText.text = effectText.text;
        viewer.nameText.text = nameText.text;
        viewer.typeText.text = typeText.text;

        // set all colors that need to be set. Will need to add more things here later probably
        viewer.hpText.color = hpText.color;
        viewer.goldText.color = goldText.color;
        viewer.manaText1.color = manaText1.color;
        viewer.manaText2.color = manaText2.color;

        //throw new System.NotImplementedException();
        viewer.background.sprite = background.sprite;
        viewer.setCardArt(cardArt.sprite);
    }

    public void swapToStructure(Tile structureTile)
    {
        List<Transform> iconsToResize = new List<Transform>();
        iconsToResize.Add(hpText.transform.parent);

        Vector3 newIconScale = new Vector3(scalingCoefficient, scalingCoefficient, 1);
        Vector3 newRootScale = new Vector3(entireCardScaleCoefficient, entireCardScaleCoefficient, 1);

        InformativeAnimationsQueue.instance.addAnimation(new SwapToCreatureAnimation(this, iconsToResize, newIconScale, newRootScale, structureTile.transform.position));
    }
    private class SwapToCreatureAnimation : QueueableCommand
    {
        StructureStatsGetter statsGetter;
        List<Transform> iconsToResize;
        Vector3 newIconScale;
        Vector3 newRootScale;
        Vector3 newPostion;

        public SwapToCreatureAnimation(StructureStatsGetter statsGetter, List<Transform> iconsToResize, Vector3 newIconScale, Vector3 newRootScale, Vector3 newPostion)
        {
            this.statsGetter = statsGetter;
            this.iconsToResize = iconsToResize;
            this.newIconScale = newIconScale;
            this.newRootScale = newRootScale;
            this.newPostion = newPostion;
        }

        public override bool isFinished => statsGetter.resizeToCreatureFinished;

        public override void execute()
        {
            statsGetter.resizeToCreatureFinished = false;
            statsGetter.StartCoroutine(statsGetter.resizeToCreature(statsGetter.transform, newRootScale, iconsToResize, newIconScale, newPostion));
        }
    }

    [SerializeField] private float resizeSpeed = .1f;
    [SerializeField] private float pauseBetweenResize = .5f;
    [SerializeField] private float iconResizeSpeed = 2f;
    private float timePaused = 0f;
    private bool resizeToCreatureFinished;
    IEnumerator resizeToCreature(Transform cardRoot, Vector3 newRootScale, List<Transform> iconsToEnlarge, Vector3 newIconScale, Vector3 newPosition)
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
        while (timePaused < pauseBetweenResize)
        {
            timePaused += Time.deltaTime;
        }
        timePaused = 0f;

        // resize icons
        while (Vector3.Distance(iconsToEnlarge[0].localScale, newIconScale) > 0.02f)
        {
            foreach (Transform t in iconsToEnlarge)
            {
                t.localScale = Vector3.MoveTowards(t.localScale, newIconScale, iconResizeSpeed * Time.deltaTime);
            }
            //cardRoot.localScale = Vector3.MoveTowards(cardRoot.localScale, newRootScale, iconResizeSpeed * Time.deltaTime);
            yield return null;
        }

        friendOrFoeBorder.gameObject.SetActive(true);
        resizeToCreatureFinished = true;
    }

    // called when a structure leaves the field and needs to act like a card again
    // if the topology of a card is changed this method may need to be changed
    public void swapToCard()
    {
        friendOrFoeBorder.gameObject.SetActive(false);

        List<Transform> iconsToResize = new List<Transform>();
        iconsToResize.Add(hpText.transform.parent);

        Vector3 newIconScale = new Vector3(1, 1, 1);
        foreach (Transform icon in iconsToResize)
        {
            icon.localScale = newIconScale;
        }

        Vector3 newRootScale = new Vector3(.5f, .5f, 1);
        transform.localScale = newRootScale;
    }

    // used to update the friend or foe border
    public void setAsAlly(bool isAlly)
    {
        if (isAlly)
            friendOrFoeBorder.color = Color.blue;
        else
            friendOrFoeBorder.color = Color.red;
    }
}
