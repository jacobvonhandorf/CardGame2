using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHoveredOffset : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float offsetAmount;
    [SerializeField] private LocalTransformManager offsetTransform;
    [SerializeField] private TransformManager restingTransform;
    [SerializeField] private float moveSpeed = 9f;

    private Card card;

    private void Awake()
    {
        card = GetComponentInParent<Card>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (card.enabled)
        {
            Vector3 newPosition = Vector3.zero;
            newPosition.y = offsetAmount;
            offsetTransform.moveToImmediate(new LocalTransformStruct(offsetTransform.transform) { localPosition = newPosition, localScale = Vector3.one});
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (card.enabled)
        {
            offsetTransform.moveToImmediate(new LocalTransformStruct() { localPosition = Vector3.zero, localScale = Vector3.one });
        }
    }
}
