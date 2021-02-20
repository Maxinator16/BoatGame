using System;

namespace BoatGame
{
    /*
        ДЗ: Реализовать консольную игру на основе логической задачи:

        Крестьянину нужно перевезти через реку волка, козу и капусту. 
        Но лодка такова, что в ней может поместиться только крестьянин, а с ним или один волк, или одна коза, или одна капуста. 
        Но если оставить волка с козой, то волк съест козу, а если оставить козу с капустой, то коза съест капусту. 
        Как перевез свой груз крестьянин?

        1) везем козу 
        2) везем волка
        3) возвращаем козу
        4) везем капусту
        5) везем козу

    */


    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            var boatGame = new BoatGame();

            Invoker invoker = new Invoker();
            invoker.SetCommand(new UpCommand(boatGame));
            invoker.SetCommand(new DownCommand(boatGame));
            invoker.SetCommand(new EnterCommand(boatGame));


            while (true)
            {
                var key = Console.ReadKey(true);
                invoker.ExecuteCommand(key.Key);
            }
        }
    }
}
