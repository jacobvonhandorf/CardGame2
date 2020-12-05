using UnityEngine;
using TMPro;

public class IconStatChangeListener : StatChangeListener
{
    [SerializeField] private bool activeToggleable;
    private TextMeshProUGUI textMesh;
    private int baseStat;
    private int currentStat;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        GetComponentInChildren<IntChangeListener>().ValueChanged.AddListener(CurrentValueUpdated);
    }

    private void CurrentValueUpdated(int currentValue)
    {
        currentStat = currentValue;
        UpdateIcon();
    }

    // base stat updated
    protected override void OnValueUpdated(object value)
    {
        baseStat = (int)value;
        UpdateIcon();
    }

    protected override void ValueMissing()
    {
        gameObject.SetActive(false);
    }

    private void UpdateIcon()
    {
        if (textMesh == null)
            textMesh = GetComponentInChildren<TextMeshProUGUI>();

        if (activeToggleable)
        {
            //Debug.Log("Setting active to " + (baseStat < 0));
            gameObject.SetActive(baseStat >= 0);
        }

        if (baseStat > currentStat)
            textMesh.color = Color.red;
        else if (baseStat < currentStat)
            textMesh.color = Color.green;
        else
            textMesh.color = Color.white;
    }
}
