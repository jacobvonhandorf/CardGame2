using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CardType
{
    Spell,
    Creature,
    Structure,
    Other
}
public enum Tag
{
    Human, Fairy, Arcane,
    Income, Tactic,
    Gem
}
public enum ElementIdentity
{
    Fire = 0, Water = 1, Wind = 2, Earth = 3, Nuetral = 4 // numbered so cards can be sorted by element
}
public abstract class Card : MonoBehaviour, IHasCardTags, ICanBeCardViewed, IScriptCard
{
    // basic fields
    public List<Tag> Tags { get; } = new List<Tag>();
    private bool hidden = true; // if true then all players can see the card
    public CardPile CardPile { get; private set; }
    public Player Owner { get; set; }
    public EmptyHandler onInitilization;
    public int CardId { get; set; }

    // stats
    public StatsContainer Stats { get; } = new StatsContainer();
    public int GoldCost { get { return (int)Stats.StatList[StatType.GoldCost]; } set { Stats.SetValue(StatType.GoldCost, value); } }
    public int BaseGoldCost { get { return (int)Stats.StatList[StatType.BaseGoldCost]; } set { Stats.SetValue(StatType.BaseGoldCost, value); } }
    public int ManaCost { get { return (int)Stats.StatList[StatType.ManaCost]; } set { Stats.SetValue(StatType.ManaCost, value); } }
    public int BaseManaCost { get { return (int)Stats.StatList[StatType.BaseManaCost]; } set { Stats.SetValue(StatType.BaseManaCost, value); } }
    public string CardName { get { return (string)Stats.StatList[StatType.Name]; } set { Stats.SetValue(StatType.Name, value); } }
    public string TypeText { set { Stats.SetValue(StatType.TypeText, value); } }
    public string EffectText { get { return (string)Stats.StatList[StatType.EffectText]; } set { Stats.SetValue(StatType.EffectText, value); } }
    public Sprite Art { get { return (Sprite)Stats.StatList[StatType.Art]; } set { Stats.SetValue(StatType.Art, value); } }
    public ElementIdentity ElementalId { get { return Stats.GetValue<ElementIdentity>(StatType.ElementalId); } set { Stats.SetValue(StatType.ElementalId, value); } }
    public int TotalCost { get { return GoldCost + ManaCost; } }
    public IHaveReadableStats ReadableStats => Stats;
    public abstract CardType CardType { get; }
    public abstract List<Tile> LegalTargetTiles { get; }

    public CardVisual CardVisuals { get { return cardVisuals; } }
    [SerializeField] private CardVisual cardVisuals;

    public List<ToolTipInfo> toolTipInfos = new List<ToolTipInfo>();
    public TransformManager TransformManager { get; private set; }
    [SerializeField] private StatChangePropogator statChangePropogator;
    public Card AsCard => this;

    #region Events
    public event EventHandler<AddedToCardPileArgs> E_AddedToCardPile;
    public class AddedToCardPileArgs : EventArgs
    {
        public AddedToCardPileArgs(CardPile previousCardPile, CardPile newCardPile, Card source)
        {
            this.previousCardPile = previousCardPile;
            this.newCardPile = newCardPile;
            this.source = source;
        }
        public CardPile previousCardPile { get; set; }
        public CardPile newCardPile { get; set; }
        public Card source { get; set; }
    }
    public delegate void AddedToCardPileHandler(object sender, AddedToCardPileArgs e);
    public void TriggerAddedToCardPileEffects(object sender, AddedToCardPileArgs args)
    {
        if (E_AddedToCardPile != null)
            E_AddedToCardPile.Invoke(sender, args);
    }
    #endregion

    protected virtual void Awake()
    {
        effectGraphic = EffectGraphic.NewEffectGraphic(this);
        TransformManager = GetComponentInChildren<TransformManager>();
        statChangePropogator.Source = Stats;
        Stats.AddType(StatType.BaseGoldCost);
        Stats.AddType(StatType.GoldCost);
        Stats.AddType(StatType.BaseManaCost);
        Stats.AddType(StatType.ManaCost);
        Stats.SetValue(StatType.CardType, CardType);
    }

    public bool IsType(CardType type) => CardType == type;

    public abstract void Initialize();

    // move card to a pile and remove it from the old one
    public void MoveToCardPile(CardPile newPile, Card source)
    {
        ActualMove(newPile, source);
        if (GameManager.gameMode == GameManager.GameMode.online)
            NetInterface.Get().SyncMoveCardToPile(this, newPile, source);
    }

    public void SyncCardMovement(CardPile newPile, Card source)
    {
        ActualMove(newPile, source);
    }

    private void ActualMove(CardPile newPile, Card source)
    {
        if (CardPile != null)
        {
            if (newPile == CardPile) // if we're already in the new pile then do nothing
                return;
            CardPile.RemoveCard(this);
        }
        CardPile previousPile = CardPile;
        CardPile = newPile;

        newPile.AddCard(this);
        TriggerAddedToCardPileEffects(this, new AddedToCardPileArgs(previousPile, newPile, source));
    }

    private EffectGraphic effectGraphic; // initialized in awake
    public void ShowInEffectsView()
    {
        EffectGraphicsView.Get().AddToQueue(effectGraphic);
    }

    #region MovingGraphicsMethods
    private const float smoothing = 9f; // speed at which cards snap into their place
    // methods for removing Card from scene by moving them way off screen
    // and for returning them to the scene afterwards
    [HideInInspector] public Vector3 positionOnScene;
    private bool onScene = true;
    public void removeGraphicsAndCollidersFromScene()
    {
        //if (!onScene) not sure if this is needed. Commenting it out fixed card not going to grave when killed over network
        //    return;
        
        positionOnScene = transform.position;
        transform.position = new Vector3(99999f, 99999f, 99999f);
        TransformManager.Lock();
        TransformManager.enabled = false;
        //interuptMove = true;
        onScene = false;
        
    }
    public void returnGraphicsAndCollidersToScene()
    {
        if (GameManager.gameMode == GameManager.GameMode.online)
        {
            // if card is in hand and the owner is not the local player then don't do anything
            if (CardPile is Hand && Owner != NetInterface.Get().localPlayer)
            {
                Debug.Log("Escaping return to scene");
                return;
            }
        }

        TransformManager.enabled = true;
        TransformManager.UnLock();
        transform.position = positionOnScene;
        onScene = true;
    }
    public bool isOnScene()
    {
        return onScene;
    }

    // these methods and coroutines change the cards position smoothly
    public void moveTo(Vector3 position)
    {
        TransformStruct ts = new TransformStruct(TransformManager.transform);
        ts.position = position;
        TransformManager.MoveToImmediate(ts);
    }
    #endregion

    public bool OwnerCanPayCosts()
    {
        if (GoldCost > Owner.Gold)
            return false;
        if (ManaCost > Owner.Mana)
            return false;
        return true;
    }
    private void PayCosts(Player player)
    {
        player.Gold -= Mathf.Clamp(GoldCost, 0, 999999);
        player.Mana -= Mathf.Clamp(ManaCost, 0, 999999);
    }

    #region GettersAndSetters
    public virtual void ResetToBaseStats()
    {
        GoldCost = BaseGoldCost;
        ManaCost = BaseManaCost;
    }
    public virtual void resetToBaseStatsWithoutSyncing()
    {
        GoldCost = BaseGoldCost;
        ManaCost = BaseManaCost;
    }
    #endregion
    #region Keyword
    private List<Keyword> keywordList = new List<Keyword>();
    public void AddKeyword(Keyword keyword)
    {
        keywordList.Add(keyword);
        toolTipInfos.Add(KeywordData.getData(keyword).info);
    }
    public void RemoveKeyword(Keyword keyword)
    {
        keywordList.Remove(keyword);
        toolTipInfos.Remove(KeywordData.getData(keyword).info);
    }
    public bool HasKeyword(Keyword keyword) => keywordList.Contains(keyword);
    public ReadOnlyCollection<Keyword> KeywordList => keywordList.AsReadOnly();
    [SerializeField] private float hoverTimeForToolTips = .5f;
    private float timePassed = 0;
    #endregion

    #region OverrideMethods
    public abstract void Play(Tile t);
    public abstract bool CanBePlayed();
    #endregion
}
