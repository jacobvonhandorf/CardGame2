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
    private Queue<QueueableCommand> commandList;
    private QueueableCommand currentCommand;

    public override bool isFinished => checkFinished();

    public CompoundQueueableCommand(Queue<QueueableCommand> commandList)
    {
        this.commandList = commandList;
    }

    private bool checkFinished()
    {
        if (currentCommand.isFinished)
        {
            if (commandList.Count > 0)
            {
                moveToNextCommand();
                return false;
            }
            else
                return true;
        }
        else
        {
            return false;
        }
    }

    private void moveToNextCommand()
    {
        currentCommand = commandList.Dequeue();
        currentCommand.execute();
    }

    public override void execute()
    {
        throw new System.NotImplementedException();
    }

    #region Static
    public class Builder
    {
        private Queue<QueueableCommand> commands;
        public Builder addCommand(QueueableCommand c)
        {
            commands.Enqueue(c);
            return this;
        }

        public CompoundQueueableCommand Build()
        {
            return new CompoundQueueableCommand(commands);
        }
    }
    #endregion
}