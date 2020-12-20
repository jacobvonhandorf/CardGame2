using UnityEngine;
using UnityEngine.EventSystems;

public class Hand : CardPile, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float speedOnHover = 100;
    [SerializeField] private float moveUpDistance = 300;
    private Vector3 targetPosition;
    private Vector3 originalPosition;

    private void Start()
    {
        targetPosition = transform.position;
        originalPosition = transform.position;

        for (int i = 0; i < 5; i++)
        {
            //Card c = ResourceManager.Get().InstantiateCardById(CardIds.RingOfEternity);
            //AddCard(c);
        }
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, speedOnHover * Time.deltaTime);

        // this is a temporary fix that will need to changed
        foreach (Card c in cardList)
        {
            if (c.Owner != NetInterface.Get().localPlayer)
            {
                c.removeGraphicsAndCollidersFromScene();
            }
        }
    }

    protected override void OnCardAdded(Card c)
    {
        c.transform.localScale = Vector3.one * .5f;
        if (c is CreatureCard)
            (c as CreatureCard).SwapToCard();
        else if (c is StructureCard)
            (c as StructureCard).SwapToCard();

        if (GameManager.gameMode != GameManager.GameMode.online)
        {
        }
        else if (GameManager.gameMode == GameManager.GameMode.online)
        {
            if (c.Owner != NetInterface.Get().localPlayer)
            {
                c.removeGraphicsAndCollidersFromScene();
            }
            else
            {
                //Debug.Log("Adding to scene");
                c.returnGraphicsAndCollidersToScene();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetPosition = originalPosition + Vector3.up * moveUpDistance;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetPosition = originalPosition;
    }
}
