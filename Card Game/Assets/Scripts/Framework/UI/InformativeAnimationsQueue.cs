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

    private readonly object playingAnimationsLock = new object();
    private Queue<IQueueableCommand> animationQueue = new Queue<IQueueableCommand>();
    private IQueueableCommand currentAnimation;
    void Update()
    {
        ProcessCommands();
    }
    private void ProcessCommands()
    {
        if (currentAnimation != null && !currentAnimation.IsFinished) // command is in progress
        {
            return;
        }
        if (currentAnimation != null && currentAnimation.IsFinished) // command finishes
        {
            currentAnimation = null;
            if (NetInterface.Get().localPlayer != null)
                NetInterface.Get().localPlayer.RemoveLock(playingAnimationsLock);
        }
        if (animationQueue.Count == 0) // command is finished but there is no new command
            return;
        currentAnimation = animationQueue.Dequeue();
        if (NetInterface.Get().localPlayer != null)
            NetInterface.Get().localPlayer.AddLock(playingAnimationsLock);
        currentAnimation.Execute();
    }

    public void AddAnimation(IQueueableCommand cmd)
    {
        animationQueue.Enqueue(cmd);
    }
}
