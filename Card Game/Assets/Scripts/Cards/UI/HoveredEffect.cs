using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoveredEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UnityEvent HoverEffect;
    [SerializeField] private float secondsToHover;

    private bool isHovered = false;
    private float timeHovered;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        StartCoroutine(HoverCoroutine());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    private IEnumerator HoverCoroutine()
    {
        timeHovered = 0f;
        while (timeHovered < secondsToHover)
        {
            if (!isHovered)
                yield break;
            timeHovered += Time.deltaTime;
            yield return null;
        }
        HoverEffect.Invoke();
    }
}
