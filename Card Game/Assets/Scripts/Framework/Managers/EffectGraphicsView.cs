﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGraphicsView : MonoBehaviour
{
    private const float DISPLAY_TIME = 2f;

    [SerializeField] private CardViewer cardViewer;
    [SerializeField] private GameObject cvGameObject;
    [SerializeField] private SpriteRenderer remainingTimeGraphic;

    private float remainingTimeDefaultWidth;

    private Queue<EffectGraphic> effectGraphics = new Queue<EffectGraphic>();

    #region Singleton
    private static EffectGraphicsView instance;
    private void Awake()
    {
        instance = this;
        remainingTimeDefaultWidth = remainingTimeGraphic.size.x;
    }
    public static EffectGraphicsView Get()
    {
        return instance;
    }
    #endregion

    public void AddToQueue(EffectGraphic newGraphic)
    {
        InformativeAnimationsQueue.Instance.AddAnimation(new AnimationCommand(newGraphic, this));
    }
    private class AnimationCommand : IQueueableCommand
    {
        private EffectGraphic graphic;
        private EffectGraphicsView view;
        public AnimationCommand(EffectGraphic graphic, EffectGraphicsView view)
        {
            this.graphic = graphic;
            this.view = view;
        }

        public bool IsFinished => finished;
        public bool finished = false;

        public void Execute()
        {
            Debug.Log("Showing effect graphic");
            view.StartCoroutine(view.showGraphicCoroutine(graphic, this));
        }
    }
    #region ShowingGraphics
    private float timePassed;
    private float timePassedCoeff = 1;
    private bool animationFinished = true;
    private Vector2 cachedV2 = new Vector2();
    private float newX;
    IEnumerator showGraphicCoroutine(EffectGraphic graphic, AnimationCommand cmd)
    {
        timePassed = 0;
        showGraphic(graphic);
        while (timePassed < DISPLAY_TIME)
        {
            newX = remainingTimeDefaultWidth - remainingTimeDefaultWidth * timePassed / DISPLAY_TIME;
            cachedV2.Set(newX, remainingTimeGraphic.size.y);
            remainingTimeGraphic.size = cachedV2;
            timePassed += Time.deltaTime * timePassedCoeff;
            yield return null;
        }
        cvGameObject.SetActive(false);
        cmd.finished = true;
    }
    private void showGraphic(EffectGraphic graphic)
    {
        switch (graphic)
        {
            case CardEffectGraphic g:
                showCardEffectGraphic(g);
                break;
            case TextEffectGraphic g:
                showTextEffectGraphic(g);
                break;
            default:
                throw new Exception("Unexpected graphic type");
        }
    }
    private void showTextEffectGraphic(TextEffectGraphic textEffectGraphic)
    {
        throw new NotImplementedException();
    }
    private void showCardEffectGraphic(CardEffectGraphic cardEffectGraphic)
    {
        //cardEffectGraphic.card.RegisterToCardViewer(cardViewer);
        cvGameObject.SetActive(true);
    }
    #endregion

    private void OnMouseEnter()
    {
        timePassedCoeff = 0;
    }
    private void OnMouseExit()
    {
        timePassedCoeff = 1;
    }
}

#region EffectGraphics
public abstract class EffectGraphic
{
    public static EffectGraphic NewEffectGraphic(Card c) { return new CardEffectGraphic(c); }
    public static EffectGraphic NewEffectGraphic(string header, string description) { return new TextEffectGraphic(header, description); }

}
public class CardEffectGraphic : EffectGraphic
{
    public Card card;
    public CardEffectGraphic(Card c)
    {
        card = c;
    }
}
public class TextEffectGraphic : EffectGraphic
{
    public string header;
    public string description;
    public TextEffectGraphic(string header, string description)
    {
        this.header = header;
        this.description = description;
    }
}
#endregion
