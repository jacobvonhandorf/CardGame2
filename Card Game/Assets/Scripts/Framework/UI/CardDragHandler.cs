using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private TransformManager restingTransform;
    [SerializeField] private LocalTransformManager transformManager;
    [SerializeField] private Vector3 offset;

    private Card card;
    private Image[] allImages;
    private TextMeshProUGUI[] allText;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        allImages = transform.parent.GetComponentsInChildren<Image>();
        allText = transform.parent.GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!enabled)
            return;
        offset = Input.mousePosition - transformManager.transform.position;
        UIEvents.BeginCardDrag(card);
    }

    TransformStruct ts = new TransformStruct() { localScale = Vector3.one };
    Vector3 tempPos = new Vector3();
    Color tempColor;
    public void OnDrag(PointerEventData eventData)
    {
        if (!enabled)
            return;
        tempPos.Set(Input.mousePosition.x - offset.x, Input.mousePosition.y - offset.y, 0);
        transformManager.Lock();
        transformManager.transform.position = tempPos;
        if (getTileMouseIsOver() != null)
            setAlpha(.5f);
        else
            setAlpha(1f);
    }

    private void setAlpha(float alpha)
    {
        foreach (Image i in allImages)
        {
            tempColor = i.color;
            tempColor.a = alpha;
            i.color = tempColor;
        }
        foreach (TextMeshProUGUI text in allText)
        {
            tempColor = text.color;
            tempColor.a = alpha;
            text.color = tempColor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!enabled)
            return;
        transformManager.queueMoveTo(new LocalTransformStruct() { localPosition = Vector3.zero, localScale = Vector3.one });
        setAlpha(1f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!enabled)
            return;
        transformManager.clearQueue();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!enabled)
            return;
        transformManager.UnLock();
    }

    private Tile getTileMouseIsOver()
    {
        Vector2 origin = Input.mousePosition;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero, 0.1f, LayerMask.GetMask("Tile"));
        if (hit.collider != null)
        {
            Tile objectHit = hit.transform.gameObject.GetComponent<Tile>();
            return objectHit;
        }
        return null;
    }
}
