using System.Collections.Generic;
using System.Linq;

namespace BoatGame
{
    public abstract class Creature
    {
        public Creature(string name)
        {
            Name = name;
        }
        public string Name { get; }

        public bool IsEnemy(Creature enemy) => enemies.Any(f => f.Name == enemy.Name);
        protected IEnumerable<Creature> enemies;
    }

    public class Сabbage : Creature
    {
        public Сabbage(IEnumerable<Creature> _enemies) : base("Сabbage")
        {
            enemies = _enemies;
        }
    }

    public class Wolf : Creature
    {
        public Wolf(IEnumerable<Creature> _enemies) : base("Wolf")
        {
            enemies = _enemies;
        }
    }

    public class Sheep : Creature
    {
        public Sheep(IEnumerable<Creature> _enemies) : base("Sheep")
        {
            enemies = _enemies;
        }
    }

  
}
