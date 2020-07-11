using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformativeAnimationsQueue : MonoBehaviour
{
    // singleton
    public static InformativeAnimationsQueue instance;
    private void Start()
    {
        instance = this;
    }

    private Queue<QueueableCommand> animationQueue = new Queue<QueueableCommand>();
    private QueueableCommand currentAnimation;
    void Update()
    {
        processCommands();
    }
    private void processCommands()
    {
        if (currentAnimation != null && !currentAnimation.isFinished) // command is in progress
            return;
        if (currentAnimation != null && currentAnimation.isFinished)
            currentAnimation = null;
        if (animationQueue.Count == 0) // command is finished but there is no new command
            return;
        Debug.Log("Executing new animation");
        currentAnimation = animationQueue.Dequeue();
        currentAnimation.execute();
    }

    public void addAnimation(QueueableCommand cmd)
    {
        animationQueue.Enqueue(cmd);
    }
}
