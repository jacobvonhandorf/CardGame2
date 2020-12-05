using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformativeAnimationsQueue : MonoBehaviour
{
    // singleton
    public static InformativeAnimationsQueue Instance {
        get
        {
            if (instance == null)
                instance = new GameObject().AddComponent<InformativeAnimationsQueue>();
            return instance;
        }
    }
    private static InformativeAnimationsQueue instance;
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
        if (currentAnimation != null && !currentAnimation.IsFinished) // command is in progress
        {
            return;
        }
        if (currentAnimation != null && currentAnimation.IsFinished) // command finishes
        {
            currentAnimation = null;
            if (NetInterface.Get().localPlayer != null)
                NetInterface.Get().localPlayer.removeLock(playingAnimationsLock);
        }
        if (animationQueue.Count == 0) // command is finished but there is no new command
            return;
        currentAnimation = animationQueue.Dequeue();
        if (NetInterface.Get().localPlayer != null)
            NetInterface.Get().localPlayer.addLock(playingAnimationsLock);
        currentAnimation.Execute();
    }

    public void addAnimation(QueueableCommand cmd)
    {
        animationQueue.Enqueue(cmd);
    }
}
