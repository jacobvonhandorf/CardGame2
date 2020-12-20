using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script that can be attached to GOs that move in normal ways
public class LocalTransformManager : MonoBehaviour
{
    private bool locked = false;
    public bool removedFromScene = false;
    public float closeEnough = 1f;
    public float speed = 9f;
    private Queue<LocalTransformStruct> transformQueue = new Queue<LocalTransformStruct>();
    private Vector3 currentPosition;
    private Vector3 currentScale;
    private LocalTransformStruct currentTransform;

    private void Update()
    {
        // first check if there is anything to do
        if (currentTransform == null && transformQueue.Count == 0 || removedFromScene || locked)
            return;

        // if close to transform or currentTransform is null then move to next transform
        if (currentTransform == null || Vector3.Distance(transform.localPosition, currentTransform.localPosition) < closeEnough && Vector3.Distance(transform.localPosition, currentTransform.localPosition) < closeEnough)
        {
            if (transformQueue.Count > 0)
                currentTransform = transformQueue.Dequeue();
            else
            {
                currentTransform = null;
                return;
            }
        }
        // move to current transform
        transform.localPosition = Vector3.Lerp(transform.localPosition, currentTransform.localPosition, speed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, currentTransform.localScale, speed * Time.deltaTime);
    }
    // add to queue
    public void queueMoveTo(LocalTransformStruct t)
    {
        if (!locked && !removedFromScene)
            transformQueue.Enqueue(t);
    }
    // clear queue
    public void clearQueue()
    {
        currentTransform = null;
        transformQueue.Clear();
    }
    // clear and add
    public void moveToImmediate(LocalTransformStruct t)
    {
        if (!locked && !removedFromScene)
        {
            clearQueue();
            queueMoveTo(t);
        }
    }
    public void moveToInformativeAnimation(LocalTransformStruct t)
    {
        InformativeAnimationsQueue.Instance.AddAnimation(new TransformCommand(this, t));
    }

    public void setTransform(LocalTransformStruct tStruct)
    {
        clearQueue();
        transform.localPosition = tStruct.localPosition;
        transform.localScale = tStruct.localScale;

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
        private LocalTransformManager transformManager;
        private LocalTransformStruct targetTransform;

        public TransformCommand(LocalTransformManager transformManager, LocalTransformStruct targetTransform)
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
            transformManager.moveToImmediate(targetTransform);
        }
    }
}

[Serializable]
public class LocalTransformStruct
{
    public LocalTransformStruct() { }
    public LocalTransformStruct(Transform t)
    {
        localPosition = t.localPosition;
        localScale = t.localScale;
    }

    public Vector3 localPosition;
    public Vector3 localScale;
}
