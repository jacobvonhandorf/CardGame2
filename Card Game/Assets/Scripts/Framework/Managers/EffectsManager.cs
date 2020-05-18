using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// effects that require input from user must use the effects manager
public class EffectsManager : MonoBehaviour
{
    private static EffectsManager manager;
    private List<EffectActuator> effectsQueue = new List<EffectActuator>();
    private bool effectInProcess;
    private bool effectJustFinished = false;

    [SerializeField] private MyButton cancelButton;
    [SerializeField] private TextMeshPro effectText;
    [SerializeField] private TextMeshPro activePlayerText;

    public static EffectsManager Get()
    {
        return manager;
    }

    private void Start()
    {
        if (manager == null)
        {
            manager = this;
            effectsQueue = new List<EffectActuator>();
        }
        else
            Debug.Log("More than one effects manager");
    }

    public void addEffect(EffectActuator newEffect)
    {
        Debug.Log("Effect added to queue");
        effectsQueue.Add(newEffect);
    }

    public void addEffect(EffectActuator newEffect, string informationText)
    {
        newEffect.informationText = informationText;
        addEffect(newEffect);
    }

    void Update()
    {
        // do not allow effects until game setup is complete
        if (!NetInterface.Get().gameSetupComplete)
            return;
        if (effectJustFinished) // need to do this here to prevent race condition
        {
            // unlock each player
            if (GameManager.gameMode == GameManager.GameMode.hotseat)
            {
                GameManager.Get().activePlayer.locked = false;
                GameManager.Get().nonActivePlayer.locked = false;
            }
            else
            {
                // TODO fix this for netplay
                GameManager.Get().activePlayer.locked = false;

            }
            // reset bool
            effectJustFinished = false;
            return;
        } 
        if (effectInProcess || effectsQueue.Count == 0)
            return;
        Debug.Log("Effects manager activating next effect");
        EffectActuator effectToActivate = effectsQueue[0];
        effectsQueue.Remove(effectToActivate);
        effectInProcess = true;
        effectToActivate.activate();
        GameManager.Get().activePlayer.locked = true;
        if (effectToActivate.informationText != null)
            flipEffectTextOn(effectToActivate.informationText);
    }

    public void signalEffectFinished()
    {
        effectJustFinished = true;
        effectInProcess = false;
        cancelButton.disable();
        flipEffectTextOff();
    }

    private void flipEffectTextOn(string text)
    {
        activePlayerText.gameObject.SetActive(false);
        effectText.gameObject.SetActive(true);
        effectText.text = text;
    }

    private void flipEffectTextOff()
    {
        activePlayerText.gameObject.SetActive(true);
        effectText.gameObject.SetActive(false);
    }
}
