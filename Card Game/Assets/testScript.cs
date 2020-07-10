using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour
{
    private SpellCard spell;

    private void Start()
    {
        StructureCard manaWell = (StructureCard)ResourceManager.Get().instantiateCardById(ManaWell.CARD_ID);
        manaWell.structure.gameObject.SetActive(true);
        spell = (SpellCard)ResourceManager.Get().instantiateCardById(PowerBlast.CARD_ID);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            GameEvents.TriggerSpellCastEvents(this, new GameEvents.SpellCastEventArgs() { spell = spell });
    }
}

