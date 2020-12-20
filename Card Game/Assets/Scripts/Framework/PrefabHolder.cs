using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// holds references to prefabs for static methods to use
public class PrefabHolder : MonoBehaviour
{
    #region Singleton
    public static PrefabHolder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (Resources.Load("Prefab Holder") as GameObject).GetComponent<PrefabHolder>();
            }
            return instance;
        }
    }
    private static PrefabHolder instance;
    #endregion
    #region Prefabs
    public XPickerBox xPickerPrefab;
    public CardPicker cardPicker;
    public OptionSelectBox optionBox;
    public AboveTileFilter aboveTileFilter;
    #endregion

    private void Awake()
    {
        instance = this;
    }
}
