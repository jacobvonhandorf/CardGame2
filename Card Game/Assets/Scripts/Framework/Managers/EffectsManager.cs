using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// effects that require input from user must use the effects manager
public class EffectsManager : MonoBehaviour
{
    private static EffectsManager manager;
    [SerializeField] private List<EffectActuator> effectsQueue = new List<EffectActuator>();
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
            Debug.LogError("More than one effects manager");
    }

    public void addEffect(EffectActuator newEffect, Player effectOwner)
    {
        if (effectOwner != NetInterface.Get().getLocalPlayer())
            return;
        effectsQueue.Add(newEffect);
    }

    public void addEffect(EffectActuator newEffect, string informationText, Player effectOwner)
    {
        newEffect.informationText = informationText;
        addEffect(newEffect, effectOwner);
    }

    public void addEffectToStartOfQueue(EffectActuator newEffect, string informationText, Player effectOwner)
    {
        if (effectOwner != NetInterface.Get().getLocalPlayer())
            return;
        newEffect.informationText = informationText;
        effectsQueue.Insert(0, newEffect);
    }

    private int effectCount = 0;
    private object effectsManagerLock = new object();
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
                throw new Exception("Unimplemented");
                //GameManager.Get().activePlayer.locked = false;
                //GameManager.Get().nonActivePlayer.locked = false;
            }
            else
            {
                // TODO fix this for netplay
                GameManager.Get().activePlayer.removeLock(effectsManagerLock);
            }
            // reset bool
            effectJustFinished = false;
            return;
        }

        // do not trigger the next effect if one is already taking place or there is no next effect
        if (effectInProcess || effectsQueue.Count == 0)
            return;

        EffectActuator effectToActivate = effectsQueue[0];
        effectsQueue.Remove(effectToActivate);
        effectInProcess = true;
        effectToActivate.activate();
        GameManager.Get().activePlayer.addLock(effectsManagerLock);
        //GameManager.Get().activePlayer.locked = true;
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
