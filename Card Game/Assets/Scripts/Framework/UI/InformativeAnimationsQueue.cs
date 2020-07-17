using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformativeAnimationsQueue : MonoBehaviour
{
    // singleton
    public static InformativeAnimationsQueue instance;
    private void Awake()
    {
        instance = this;
    }

    private object playingAnimationsLock = new object();
    private Queue<QueueableCommand> animationQueue = new Queue<QueueableCommand>();
    private QueueableCommand currentAnimation;
    void Update()
    {
        processCommands();
    }
    private void processCommands()
    {
        if (currentAnimation != null && !currentAnimation.isFinished) // command is in progress
        {
            return;
        }
        if (currentAnimation != null && currentAnimation.isFinished) // command finishes
        {
            currentAnimation = null;
            NetInterface.Get().getLocalPlayer().removeLock(playingAnimationsLock);
        }
        if (animationQueue.Count == 0) // command is finished but there is no new command
            return;
        currentAnimation = animationQueue.Dequeue();
        NetInterface.Get().getLocalPlayer().addLock(playingAnimationsLock);
        currentAnimation.execute();
    }

    public void addAnimation(QueueableCommand cmd)
    {
        animationQueue.Enqueue(cmd);
    }
}
