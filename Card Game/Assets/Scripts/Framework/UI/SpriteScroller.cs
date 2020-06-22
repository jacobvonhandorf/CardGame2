using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScroller : MonoBehaviour
{
    [SerializeField] public Transform contentTransform;
    [SerializeField] private Transform verticalScrollBar;
    [SerializeField] private Transform rootTransform;
    [SerializeField] private bool lockHorizontal;
    [SerializeField] private bool lockVertical;
    [SerializeField] private float scrollSpeed = .5f;
    [SerializeField] private float verticalScrollBarRatio = -2f;
    [SerializeField] private BoxCollider2D myCollider;
    private Vector3 offset;
    private Vector3 restingPosition;
    private bool isBeingDragged;
    public float maxX = 1;
    public float maxY = 0;
    public float minX = -1;
    public float minY = -4;

    void Start()
    {
        restingPosition = contentTransform.position;
        updateContentPosition(new Vector3(-9999, -9999, 0)); // set scroll position to as high and as left as possible by default
        // updateMaxAndMin();
    }

    private void OnMouseDown()
    {
        // updateMaxAndMin();
        isBeingDragged = true;
        offset = contentTransform.position -
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
    }

    private void OnMouseDrag()
    {
        Vector3 newPosition;
        if (!lockHorizontal && !lockVertical)
            newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        else if (lockHorizontal)
            newPosition = new Vector3(contentTransform.position.x, Input.mousePosition.y, 0);
        else if (lockVertical)
            newPosition = new Vector3(Input.mousePosition.x, restingPosition.y, 0);
        else
            newPosition = new Vector3(contentTransform.position.x, contentTransform.position.y, 0);

        Vector3 positionAfterScreenToWorld = Camera.main.ScreenToWorldPoint(newPosition) + offset;
        Vector3 finalPosition = positionAfterScreenToWorld;

        if (lockHorizontal)
        {
            finalPosition.x = contentTransform.position.x;
        }

        // contentTransform.position = Camera.main.ScreenToWorldPoint(newPosition) + offset;
        // contentTransform.position = finalPosition;
        updateContentPosition(finalPosition);

        isBeingDragged = true;

    }

    private void OnMouseUp()
    {
        restingPosition = transform.position;
        isBeingDragged = false;
    }

    private void Update()
    {
        // save CPU
        if (Input.mouseScrollDelta.y != 0)
        {
            if (!lockVertical)
            {
                // cast ray and if it hits spritescroller then do the scroll
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMaskEnum.ScrollView);
                if (hit.collider == myCollider)
                {
                    Vector3 newPosition = contentTransform.position;
                    newPosition.y = contentTransform.position.y + (Input.mouseScrollDelta.y * -scrollSpeed);
                    updateContentPosition(newPosition);
                }
            }
        }

    }

    public void updateContentPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        contentTransform.position = position;

        if (!verticalScrollBar.gameObject.activeInHierarchy)
            return;
        Vector3 verticalScrollBarPosition = verticalScrollBar.position;
        verticalScrollBarPosition.y = ((position.y / (maxY - minY) * -verticalScrollBarRatio) - 2) * rootTransform.localScale.y;
        verticalScrollBar.position = verticalScrollBarPosition;
    }

    private void updateMaxAndMin()
    {
        throw new NotImplementedException();
        float localMaxX = 0;
        float localMaxY = 0;
        float localMinX = 0;
        float localMinY = 0;
        
        foreach(Transform child in transform.GetComponentsInChildren<Transform>())
        {
            child.position = new Vector3(10, 10, 10);
            Debug.Log("asdf");
            Vector3 position = transform.localPosition;
            Debug.Log(child.gameObject.name + " - " + position);
            if (position.x > localMaxX)
                localMaxX = position.x;
            if (position.x < localMinX)
                localMinX = position.x;
            if (position.y > localMaxY)
                localMaxY = position.y;
            if (position.y < localMinY)
                localMinY = position.y;
        }

        maxX = localMaxX;
        maxY = localMaxY;
        minX = localMinX;
        minY = localMinY;

    }

    public void addContent(Transform newContent)
    {
        throw new NotImplementedException();
    }

}
