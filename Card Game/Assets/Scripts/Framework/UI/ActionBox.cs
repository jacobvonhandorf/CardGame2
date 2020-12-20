using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionBox : MonoBehaviour
{
    public static ActionBox instance;

    [SerializeField] private Vector2 positionOffset;
    [SerializeField] private GameObject effectsButton;
    [SerializeField] private GameObject attackButton;
    private Permanent source;

    public void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void Show(Permanent source)
    {
        this.source = source;
        SetPosition(source);
        effectsButton.SetActive(source.ActivatedEffects.Count > 0);
        attackButton.SetActive(source is Creature);
        gameObject.SetActive(true);
    }
    private void SetPosition(Permanent source)
    {
        Vector2 newPos = source.SourceCard.TransformManager.transform.position;
        newPos += positionOffset;
        transform.position = newPos;
    }

    #region Button Functions
    public void Attack()
    {
        if (!(source as Creature).ActionAvailable)
        {
            Toaster.Instance.DoToast("You have already acted with this creature");
            return;
        }
        AttackControl.Setup(source as Creature);
        gameObject.SetActive(false);
    }
    public void Effect()
    {
        if (source.ActivatedEffects.Count == 1)
            source.ActivatedEffects[0].Effect.Invoke();
        else
        {
            List<string> options = source.ActivatedEffects.Select(eff => eff.Name).ToList();
            OptionSelectBox.CreateAndQueue(options, "Select which effect to activate", NetInterface.Get().localPlayer, delegate (int index, string option) 
            {
                source.ActivatedEffects[index].Effect.Invoke();
            });
        }
        gameObject.SetActive(false);
    }
    public void Cancel()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
