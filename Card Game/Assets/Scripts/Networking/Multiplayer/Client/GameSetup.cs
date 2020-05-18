using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class GameSetup : MonoBehaviour
{
    private const int NUM_PILES_TO_REGISTER = 6;

    private static GameSetup instance;

    private bool netIsPlayer1HasBeenSet = false; // stage 0
    private bool allCardsSpawned = false; // stage 1
    private bool allCardPilesRegistered = false; // stage 2
    private bool allCardsPlacedInDeck = false; // stage 3

    private bool allGameSetupComplete = false;

    private int numPilesRegistered = 0;

    private List<CardPile> pilesToRegister = new List<CardPile>();

    public static GameSetup Get()
    {
        if (instance != null)
            return instance;
        instance = new GameSetup();
        return instance;
    }

    internal void registerCardPile(CardPile cardPile)
    {
        pilesToRegister.Add(cardPile);
    }

    public void player1HasBeenSet()
    {
        netIsPlayer1HasBeenSet = true;
    }

    private void flushPilesToRegister()
    {
        if (!netIsPlayer1HasBeenSet) // don't set up card piles if we don't know who is Player1
            return;
        foreach (CardPile cp in pilesToRegister)
        {
            NetInterface.Get().registerCardPile(cp);
            numPilesRegistered++;
        }
        if (numPilesRegistered == NUM_PILES_TO_REGISTER)
            allCardPilesRegistered = true;
        else if (numPilesRegistered > NUM_PILES_TO_REGISTER)
            throw new Exception("Too many piles registered");
        pilesToRegister.Clear();
    }

    // check if game setup is completed
    private void Update()
    {
        if (allGameSetupComplete)
        {
            NetInterface.Get().notifyGameSetupComplete();
            Destroy(gameObject);
            return;
        }

        if (!allCardPilesRegistered)
            flushPilesToRegister();
            
    }

    private void Start()
    {
        if (instance != null)
            throw new Exception("More than one GameSetup detected");
        instance = this;
    }
}
*/