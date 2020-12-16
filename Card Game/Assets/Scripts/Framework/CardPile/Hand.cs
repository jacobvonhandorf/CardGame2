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
    }

    protected override void OnCardAdded(Card c)
    {
        //c.returnGraphicsAndCollidersToScene(); // Cards know when to show themselves
        c.transform.localScale = Vector3.one * .5f;
        if (c is CreatureCard)
            (c as CreatureCard).SwapToCard();
        else if (c is StructureCard)
            (c as StructureCard).swapToCard();

        if (GameManager.gameMode != GameManager.GameMode.online)
        {
            /*
            if (GameManager.Get().activePlayer == handOwner)
                c.returnGraphicsAndCollidersToScene();
            else
                c.removeGraphicsAndCollidersFromScene();
                */
        }
        else if (GameManager.gameMode == GameManager.GameMode.online)
        {
            if (c.Owner != NetInterface.Get().localPlayer)
            {
                Debug.Log("Removing from scene");
                c.removeGraphicsAndCollidersFromScene();
            }
            else
            {
                Debug.Log("Adding to scene");
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
