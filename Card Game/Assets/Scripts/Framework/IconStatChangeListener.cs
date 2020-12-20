using UnityEngine;
using TMPro;

public class IconStatChangeListener : StatChangeListener
{
    private TextMeshProUGUI textMesh;
    private int baseStat;
    private IntChangeListener currentStatListener;

    private void CurrentValueUpdated(int currentValue)
    {
        UpdateIcon();
    }

    // base stat updated
    protected override void OnValueUpdated(object value)
    {
        baseStat = (int)value;
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        // doing this here instead of awake because this can start inactive
        if (currentStatListener == null)
        {
            currentStatListener = GetComponentInChildren<IntChangeListener>();
            currentStatListener.ValueChanged.AddListener(CurrentValueUpdated);
            textMesh = GetComponentInChildren<TextMeshProUGUI>();
        }
        if (baseStat > currentStatListener.Value)
            textMesh.color = Color.red;
        else if (baseStat < currentStatListener.Value)
            textMesh.color = Color.green;
        else
            textMesh.color = Color.white;
    }
}
