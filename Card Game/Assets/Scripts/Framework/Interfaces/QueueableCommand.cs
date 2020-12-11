using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQueueableCommand
{
    void Execute();
    bool IsFinished { get; }
}

public class CompoundQueueableCommand : IQueueableCommand
{
    private Queue<IQueueableCommand> commandList;
    private IQueueableCommand currentCommand;

    public bool IsFinished => CheckFinished();

    public CompoundQueueableCommand(Queue<IQueueableCommand> commandList)
    {
        this.commandList = commandList;
    }

    private bool CheckFinished()
    {
        if (currentCommand.IsFinished)
        {
            if (commandList.Count > 0)
            {
                MoveToNextCommand();
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

    private void MoveToNextCommand()
    {
        currentCommand = commandList.Dequeue();
        currentCommand.Execute();
    }

    public void Execute() => MoveToNextCommand();

    #region Builder
    public class Builder
    {
        private Queue<IQueueableCommand> commands = new Queue<IQueueableCommand>();
        public Builder AddCommand(IQueueableCommand c)
        {
            commands.Enqueue(c);
            return this;
        }

        public CompoundQueueableCommand Build() => new CompoundQueueableCommand(commands);
        public void BuildAndQueue() => InformativeAnimationsQueue.Instance.AddAnimation(new CompoundQueueableCommand(commands));
    }
    #endregion
}