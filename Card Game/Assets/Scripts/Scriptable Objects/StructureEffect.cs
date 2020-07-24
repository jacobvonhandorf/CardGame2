using UnityEngine;
using System.Collections;

public abstract class StructureEffect : MonoBehaviour
{
    protected Structure structure;
    protected StructureCard card;

    public void doEffect(object sender, StructureEventArgs e)
    {
        structure = e.structure;
        card = e.card as StructureCard;
    }
    protected abstract void activate();
}
