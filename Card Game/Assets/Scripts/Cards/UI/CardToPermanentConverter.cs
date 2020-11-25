﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardToPermanentConverter : MonoBehaviour
{
    [SerializeField] private TransformManager restingTransform;
    [SerializeField] private LocalTransformManager offsetTransform;
    [SerializeField] private GameObject FofBorder;
    [SerializeField] private float iconScaleWhenPermanent;
    [SerializeField] private float scaleWhenPermanent;

    private List<Transform> iconsToResize;
    private Card card;
    private Permanent permanent;
    private CardDragHandler dragHandler;

    private void Awake()
    {
        card = GetComponent<Card>();
        permanent = GetComponent<Permanent>();
        dragHandler = GetComponentInChildren<CardDragHandler>();
        iconsToResize = new List<Transform>(GetComponentsInChildren<Transform>()).FindAll(go => go.tag.Contains(TagsEnum.CardIcon));
    }

    public void doConversion(Vector3 newPosition)
    {
        card.enabled = false;
        permanent.enabled = true;
        dragHandler.enabled = false;
        StartCoroutine(resizeToCreature(newPosition));
    }

    [SerializeField] private float pauseBetweenResize;
    [SerializeField] private float iconResizeSpeed;
    [SerializeField] private float timeForShrinkAnimation;
    private bool resizeToCreatureFinished;

    IEnumerator resizeToCreature(Vector3 newPosition)
    {
        restingTransform.clearQueue();
        restingTransform.Lock();
        resizeToCreatureFinished = false;

        Vector3 positionStart = restingTransform.transform.localPosition;
        Vector3 positionEnd = newPosition;
        Debug.Log("Move from " + restingTransform.transform.localPosition);
        Vector3 scaleStart = restingTransform.transform.localScale;
        Vector3 scaleEnd = Vector3.one * scaleWhenPermanent;
        Vector3 offsetPosStart = offsetTransform.transform.localPosition;
        Vector3 offsetScaleStart = offsetTransform.transform.localScale;
        // resize root
        float currentPercentage = 0;
        float timePassed = 0;
        while (currentPercentage < .98f)
        {
            timePassed += Time.deltaTime;
            currentPercentage = timePassed / timeForShrinkAnimation;

            restingTransform.transform.localPosition = Vector3.Lerp(positionStart, positionEnd, currentPercentage);
            restingTransform.transform.localScale = Vector3.Lerp(scaleStart, scaleEnd, currentPercentage);

            offsetTransform.transform.localPosition = Vector3.Lerp(offsetPosStart, Vector3.zero, currentPercentage);
            offsetTransform.transform.localScale = Vector3.Lerp(offsetScaleStart, Vector3.one, currentPercentage);
            yield return null;
        }

        // pause
        yield return new WaitForSeconds(pauseBetweenResize);

        // resize icons
        Vector3 targetIconScale = Vector3.one * iconScaleWhenPermanent;
        while (Vector3.Distance(iconsToResize[0].transform.localScale, targetIconScale) > 0.02f)
        {
            foreach (Transform t in iconsToResize)
                t.localScale = Vector3.MoveTowards(t.localScale, targetIconScale, iconResizeSpeed * Time.deltaTime);
            yield return null;
        }
        FofBorder.SetActive(true);
        resizeToCreatureFinished = true;
        restingTransform.UnLock();
    }
}