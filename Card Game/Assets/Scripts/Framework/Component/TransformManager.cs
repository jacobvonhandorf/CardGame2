using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script that can be attached to GOs that move in normal ways
public class TransformManager : MonoBehaviour
{
    private bool locked = false;
    public bool removedFromScene = false;
    public float closeEnough = 1f;
    public float speed = 9f;
    private Queue<TransformStruct> transformQueue = new Queue<TransformStruct>();
    private Vector3 currentPosition;
    private Vector3 currentScale;
    private TransformStruct currentTransform;

    private void Update()
    {
        // first check if there is anything to do
        if (currentTransform == null && transformQueue.Count == 0 || removedFromScene || locked)
            return;

        // if close to transform or currentTransform is null then move to next transform
        if (currentTransform == null || Vector3.Distance(transform.localPosition, currentTransform.position) < closeEnough)
        {
            if (transformQueue.Count > 0)
                currentTransform = transformQueue.Dequeue();
            else
            {
                currentTransform = null;
                return;
            }
        }
        // move towards current transform
        if (currentTransform.useLocalPosition)
            transform.localPosition = Vector3.Lerp(transform.localPosition, currentTransform.position, speed * Time.deltaTime);
        else
            transform.position = Vector3.Lerp(transform.position, currentTransform.position, speed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(currentTransform.rotation), speed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, currentTransform.localScale, speed * Time.deltaTime);
    }
    // add to queue
    public void QueueMoveTo(TransformStruct t)
    {
        if (!locked && !removedFromScene)
            transformQueue.Enqueue(t);
    }
    // clear queue
    public void ClearQueue()
    {
        currentTransform = null;
        transformQueue.Clear();
    }
    // clear and add
    public void MoveToImmediate(TransformStruct t)
    {
        if (!locked && !removedFromScene)
        {
            ClearQueue();
            QueueMoveTo(t);
        }
    }
    public void MoveToInformativeAnimation(TransformStruct t)
    {
        InformativeAnimationsQueue.Instance.AddAnimation(new TransformCommand(this, t));
    }

    public void SetTransform(TransformStruct tStruct)
    {
        ClearQueue();
        transform.localPosition = tStruct.position;
        transform.localScale = tStruct.localScale;
        transform.localRotation = Quaternion.Euler(tStruct.rotation);

    }
    // lock (clear and don't accept new commands)
    // mostly used when graphics aren't on scene
    public void Lock()
    {
        locked = true;
    }
    public void UnLock()
    {
        locked = false;
    }

    // informative commands clear queue then add to it, then check for !contains in queue
    private class TransformCommand : IQueueableCommand
    {
        private TransformManager transformManager;
        private TransformStruct targetTransform;

        public TransformCommand(TransformManager transformManager, TransformStruct targetTransform)
        {
            this.transformManager = transformManager;
            this.targetTransform = targetTransform;
        }

        public bool IsFinished => IsFinishedCheck();

        private bool IsFinishedCheck()
        {
            // check for not being enabled here so queue doesn't get clogged
            return !transformManager.transformQueue.Contains(targetTransform) || !transformManager.enabled;
        }

        public void Execute()
        {
            transformManager.MoveToImmediate(targetTransform);
        }
    }
}

[Serializable]
public class TransformStruct
{
    public TransformStruct() { }
    public TransformStruct(Transform t)
    {
        position = t.position;
        localScale = t.localScale;
        rotation = t.rotation.eulerAngles;
    }

    public Vector3 position;
    public Vector3 localScale;
    public Vector3 rotation;
    public bool useLocalPosition = false;
}
