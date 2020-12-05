using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using static Card;

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

    private Vector3 instantiationLocation = new Vector3(0, 0, 0); // instantiate cards off screen
    [SerializeField] private GameObject spellCardPrefab;
    [SerializeField] private GameObject structureCardPrefab;
    [SerializeField] private GameObject creatureCardPrefab;

    public Card BuildFromCardData(CardData cardData)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        Card card = null;
        switch (cardData)
        {
            case CreatureCardData creatureData:
                card = CreatureSetup(creatureData);
                break;
            case SpellCardData spellData:
                card = SpellSetup(spellData);
                break;
            case StructureCardData structureData:
                card = StructureSetup(structureData);
                break;
            default:
                throw new Exception("Unexpected cardData");
        }

        // do all generic setting of variables
        card.cardId = cardData.id;
        card.BaseManaCost = cardData.manaCost;
        card.ManaCost = cardData.manaCost;
        card.GoldCost = cardData.goldCost;
        card.BaseGoldCost = cardData.goldCost;
        card.CardName = cardData.cardName;
        card.gameObject.name = cardData.cardName;
        card.Art = cardData.art;
        card.EffectText = cardData.effectText;
        card.ElementalId = cardData.elementalIdentity;

        foreach (Keyword k in cardData.keywords)
            card.AddKeyword(k);
        foreach (Tag t in cardData.tags)
            card.Tags.Add(t);

        stopwatch.Stop();
        Debug.Log("Time to create card " + stopwatch.ElapsedMilliseconds + "ms");

        return card;
    }

    private Card CreatureSetup(CreatureCardData data)
    {
        CreatureCard card = Instantiate(creatureCardPrefab, instantiationLocation, Quaternion.identity).GetComponent<CreatureCard>();
        card.BaseManaCost = -1;
        Creature creature = card.Creature;
        creature.BaseHealth = data.health;
        creature.Health = data.health;
        creature.BaseAttack = data.attack;
        creature.AttackStat = data.attack;
        creature.BaseMovement = data.movement;
        creature.Movement = data.movement;
        creature.Range = data.range;
        creature.BaseRange = data.range;

        // Effects
        if (data.effects == null)
            return card;
        CreatureEffects effs = card.gameObject.AddComponent(data.effects.GetType()) as CreatureEffects;
        
        effs.card = card;
        effs.creature = creature;

        card.onInitilization = effs.onInitilization;
        // register activated Effect
        if (effs.activatedEffect != null)
            creature.activatedEffects.Add(effs.activatedEffect);
        // register triggers
        if (effs.onDeploy != null)
            creature.E_OnDeployed += effs.onDeploy;
        if (effs.onDeath != null)
            creature.E_Death += effs.onDeath;
        if (effs.onAttack != null)
            creature.E_OnAttack += effs.onAttack;
        if (effs.onDefend != null)
            creature.E_OnDefend += effs.onDefend;
        if (effs.onDamaged != null)
            creature.E_OnDamaged += effs.onDamaged;

        return card;
    }

    private Card StructureSetup(StructureCardData data)
    {
        StructureCard card = Instantiate(structureCardPrefab, instantiationLocation, Quaternion.identity).GetComponent<StructureCard>();
        card.BaseManaCost = -1;
        Structure structure = card.structure;
        structure.BaseHealth = data.health;
        structure.Health = data.health;

        StructureEffects effs = card.gameObject.AddComponent(data.effects.GetType()) as StructureEffects;
        effs.structure = structure;
        effs.card = card;
        if (effs == null)
            return card;
        card.onInitilization = effs.onInitilization;
        // register effects
        if (effs.activatedEffect != null)
            structure.activatedEffects.Add(effs.activatedEffect);
        if (effs.onDefend != null)
            structure.E_OnDefend += effs.onDefend;
        if (effs.onDeploy != null)
            structure.E_OnDeployed += effs.onDeploy;
        if (effs.onLeavesField != null)
            structure.E_OnLeavesField += effs.onLeavesField;
        if (effs.onMoveToCardPile != null)
            card.E_AddedToCardPile += effs.onMoveToCardPile;

        return card;
    }

    private Card SpellSetup(SpellCardData data)
    {
        SpellCard card = Instantiate(spellCardPrefab, instantiationLocation, Quaternion.identity).GetComponent<SpellCard>();
        card.BaseGoldCost = -1;
        card.effects = data.effects;

        SpellEffects effs = card.gameObject.AddComponent(data.effects.GetType()) as SpellEffects;
        effs.card = card;
        if (effs == null)
            return card;
        card.onInitilization = effs.onInitilization;
        // regist effects
        if (effs.onMoveToCardPile != null)
            card.E_AddedToCardPile += effs.onMoveToCardPile;

        return card;
    }
}
