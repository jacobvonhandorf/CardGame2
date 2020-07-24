using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class CardBuilder : MonoBehaviour
{
    public static CardBuilder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (Instantiate(Resources.Load("Card Builder")) as GameObject).GetComponent<CardBuilder>();
            }
            return instance;
        }
    }
    private static CardBuilder instance;

    private Vector3 instantiationLocation = new Vector3(999, 999, 999); // instantiate cards off screen
    [SerializeField] private GameObject spellCardPrefab;
    [SerializeField] private GameObject structureCardPrefab;
    [SerializeField] private GameObject creatureCardPrefab;

    public Card BuildFromCardData(CardData cardData)
    {
        Card card = null;
        switch (cardData)
        {
            case CreatureCardData creatureData:
                card = creatureSetup(creatureData);
                break;
            case SpellCardData spellData:
                card = Instantiate(spellCardPrefab, instantiationLocation, Quaternion.identity).GetComponent<Card>();
                break;
            case StructureCardData structureData:
                card = structureSetup(structureData);
                break;
            default:
                throw new Exception("Unexpected cardData");
        }

        // do all generic setting of variables
        card.setBaseManaCost(cardData.manaCost);
        card.setManaCost(cardData.manaCost);
        card.setBaseGoldCost(cardData.goldCost);
        card.setGoldCost(cardData.goldCost);
        card.cardName = cardData.cardName;
        card.cardStatsScript.nameText.text = cardData.cardName;
        card.cardStatsScript.setSprite(cardData.art);
        card.cardStatsScript.effectText.text = cardData.effectText;
        card.cardStatsScript.setElementIdentity(cardData.elementalIdentity);
        foreach (Keyword k in cardData.keywords)
            card.addKeyword(k);

        return card;
    }

    private Card creatureSetup(CreatureCardData creatureData)
    {
        CreatureCard card = Instantiate(creatureCardPrefab, instantiationLocation, Quaternion.identity).GetComponent<CreatureCard>();
        BlankCreature creature = card.creature as BlankCreature;
        creature.baseHealth = creatureData.health;
        creature.setHealth(creatureData.health);
        creature.baseAttack = creatureData.attack;
        creature.setAttack(creatureData.attack);
        creature.baseMovement = creatureData.movement;
        creature.setMovement(creatureData.movement);
        creature.range = creatureData.range;
        creature.baseRange = creatureData.range;
        creature.creatureCardId = creatureData.id;
        foreach (UnityEngine.Object eff in creatureData.activatedEffects)
        {
            Type effType = eff.GetType();
            CreatureActivatedEffect effectInstance = creature.gameObject.AddComponent(effType) as CreatureActivatedEffect;
            creature.addEffect(effectInstance);
        }
        creatureData.onInitilization.onInitialization(card);

        return card;
    }
    private Card structureSetup(StructureCardData data)
    {
        StructureCard card = Instantiate(structureCardPrefab, instantiationLocation, Quaternion.identity).GetComponent<StructureCard>();
        Structure structure = card.structure;
        structure.setBaseHealth(data.health);
        structure.setHealth(data.health);

        return card;
    }
}
