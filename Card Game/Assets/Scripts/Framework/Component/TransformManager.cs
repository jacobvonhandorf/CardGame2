using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script that can be attached to GOs that move in normal ways
public class TransformManager : MonoBehaviour
{
    private bool locked = false;
    public float speed = 9f;
    private Queue<TransformStruct> transformQueue = new Queue<TransformStruct>();
    private Vector3 currentPosition;
    private Vector3 currentScale;
    private TransformStruct currentTransform;
    private void Update()
    {
        // first check if there is anything to do
        if (currentTransform == null && transformQueue.Count == 0)
            return;

        // if close to transform or currentTransform is null then move to next transform
        if (currentTransform == null || Vector3.Distance(transform.position, currentTransform.position) < 0.02f && Vector3.Distance(transform.position, currentTransform.position) < 0.02f)
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
        transform.position = Vector3.Lerp(transform.position, currentTransform.position, speed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, currentTransform.localScale, speed * Time.deltaTime);
    }
    // add to queue
    public void queueMoveTo(TransformStruct t)
    {
        if (!locked)
            transformQueue.Enqueue(t);
    }
    // clear queue
    public void clearQueue()
    {
        currentTransform = null;
        transformQueue.Clear();
    }
    // clear and add
    public void moveToImmediate(TransformStruct t)
    {
        if (!locked)
        {
            clearQueue();
            queueMoveTo(t);
        }
    }
    public void moveToInformativeAnimation(TransformStruct t)
    {
        InformativeAnimationsQueue.instance.addAnimation(new TransformCommand(this, t));
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
    private class TransformCommand : QueueableCommand
    {
        private TransformManager transformManager;
        private TransformStruct targetTransform;

        public TransformCommand(TransformManager transformManager, TransformStruct targetTransform)
        {
            this.transformManager = transformManager;
            this.targetTransform = targetTransform;
        }

        public override bool isFinished => isFinishedCheck();

        private bool isFinishedCheck()
        {
            Debug.Log("Doing is finished check");
            Debug.Log("Count " + transformManager.transformQueue.Count);
            Debug.Log(transformManager.transformQueue.Contains(targetTransform));
            //Debug.Log("Peek " + transformManager.transformQueue.Peek());
            return !transformManager.transformQueue.Contains(targetTransform);
        }

        public override void execute()
        {
            transformManager.moveToImmediate(targetTransform);
        }
    }
}

public class TransformStruct
{
    public TransformStruct() { }
    public TransformStruct(Transform t)
    {
        position = t.position;
        localScale = t.localScale;
    }

    public Vector3 position;
    public Vector3 localScale;
}
