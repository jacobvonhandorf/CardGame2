using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QueueableCommand
{
    public abstract void Execute();
    public abstract bool IsFinished { get; }
}

public class CompoundQueueableCommand : QueueableCommand
{
    private Queue<QueueableCommand> commandList;
    private QueueableCommand currentCommand;

    public override bool IsFinished => checkFinished();

    public CompoundQueueableCommand(Queue<QueueableCommand> commandList)
    {
        this.commandList = commandList;
    }

    private bool checkFinished()
    {
        if (currentCommand.IsFinished)
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
        currentCommand.Execute();
    }

    public override void Execute()
    {
        moveToNextCommand();
    }

    #region Builder
    public class Builder
    {
        private Queue<QueueableCommand> commands = new Queue<QueueableCommand>();
        public Builder addCommand(QueueableCommand c)
        {
            commands.Enqueue(c);
            return this;
        }

        public CompoundQueueableCommand Build()
        {
            return new CompoundQueueableCommand(commands);
        }
        public void BuildAndQueue()
        {
            InformativeAnimationsQueue.Instance.addAnimation(new CompoundQueueableCommand(commands));
        }
    }
    #endregion
}