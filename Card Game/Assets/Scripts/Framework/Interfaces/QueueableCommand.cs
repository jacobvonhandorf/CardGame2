using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QueueableCommand
{
    public abstract void execute();
    public abstract bool isFinished { get; }
}

public class CompoundQueueableCommand : QueueableCommand
{
    private List<QueueableCommand> commands;

    public CompoundQueueableCommand(List<QueueableCommand> commands)
    {
        this.commands = commands;
    }

    public override bool isFinished => getIsFinished();

    public override void execute()
    {
        foreach (QueueableCommand cmd in commands)
        {
            cmd.execute();
        }
    }
    private bool getIsFinished()
    {
        foreach (QueueableCommand cmd in commands)
        {
            if (!cmd.isFinished)
                return false;
        }
        return true;
    }
}
