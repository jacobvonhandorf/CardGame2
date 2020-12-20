using UnityEngine.Events;

public class IntEvent : UnityEvent<int> { }
public class StringEvent : UnityEvent<string> { }
public class CardEvent : UnityEvent<Card> { }
public class CardViewerEvent : UnityEvent<CardViewer> { }
public class CreatureEvent : UnityEvent<Creature> { }
public class SpellEvent : UnityEvent<SpellCard> { }
public class ScriptCreatureEvent : UnityEvent<IScriptCreature> { }
public class ScriptCardEvent : UnityEvent<IScriptCard> { }
