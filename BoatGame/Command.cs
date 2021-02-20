using System;
using System.Collections.Generic;
using System.Linq;
using BoatGame;

namespace BoatGame
{
    public abstract class Command
    {
        protected BoatGame boatGame;
        public Command(BoatGame boatGame)
        {
            this.boatGame = boatGame;
        }
       public  ConsoleKey Key;
        public abstract void Execute();
    }

    public class UpCommand : Command
    {
        public UpCommand(BoatGame boatGame) : base(boatGame)
        {
            Key = ConsoleKey.UpArrow;  
        }

        public override void Execute()
        {
            boatGame.UpClickHandler();
        }
    }

    public class DownCommand : Command
    {
        public DownCommand(BoatGame boatGame) : base(boatGame)
        {
            Key = ConsoleKey.DownArrow;
        }

        public override void Execute()
        {
            boatGame.DownClickHandler();
        }
    }

    public class EnterCommand : Command
    {
        public EnterCommand(BoatGame boatGame) : base(boatGame)
        {
            Key = ConsoleKey.Enter;
        }

        public override void Execute()
        {
            boatGame.EnterClickHandler();
        }
    }

    class Invoker
    {
        private Dictionary<ConsoleKey, Command> commands = new Dictionary<ConsoleKey, Command>();

        public void SetCommand(Command command)
        {
            commands.Add(command.Key, command);
        }

        public void ExecuteCommand(ConsoleKey key)
        {
          commands.FirstOrDefault(f=>f.Key == key).Value?.Execute();
        }
    }
}
