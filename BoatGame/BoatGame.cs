using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoatGame
{
    public class BoatGame
    {
        private Map map = Map.Instance;

        public BoatGame()
        {
            var wolf = new Wolf(new Creature[] { });
            var sheep = new Sheep(new Creature[] { wolf });
            var cabbage = new Сabbage(new Creature[] { sheep });

            LeftSideCreatures = new List<Creature>() { wolf, sheep, cabbage };

            map.initializeMap(LeftSideCreatures);
        }
        public BoatGame(IEnumerable<Creature> creatures)
        {
            LeftSideCreatures = (List<Creature>)creatures;

            map.initializeMap(creatures);
        }

        public List<Creature> LeftSideCreatures { get; }
        public List<Creature> RightSideCreatures { get; } = new List<Creature>();

        public void UpClickHandler()
        {
            map.MoveArrowUp();
        }

        public void DownClickHandler()
        {
            map.MoveArrowDown();
        }

        public void EnterClickHandler()
        {
            map.ChangeCreatureSide();

            var selectedCreature = map.GetCreatureOfRow();
            var currentDirection = map.GetArrowDirection();
            if (LeftSideCreatures.Contains(selectedCreature) && currentDirection == eSide.Right)
            {
                LeftSideCreatures.Remove(selectedCreature);
                RightSideCreatures.Add(selectedCreature);
                GameStateCheck(currentDirection);
            }
            else if (RightSideCreatures.Contains(selectedCreature) && currentDirection == eSide.Left)
            {
                RightSideCreatures.Remove(selectedCreature);
                LeftSideCreatures.Add(selectedCreature);
                GameStateCheck(currentDirection);
            }
            else
            {
                GameStateCheck(currentDirection);
            }
        }

        private void GameStateCheck(eSide currentDirection)
        {
            if (RightSideCreatures.Count == 3)
            {
                Console.Clear();
                Console.WriteLine("YOU WIN");
                Console.ReadLine();
                Environment.Exit(0);
            }

            var isGameOver = false;
            if (LeftSideCreatures.Count == 2 && currentDirection == eSide.Right)
            {
                isGameOver = LeftSideCreatures.Any(f => LeftSideCreatures.Any(b => b.IsEnemy(f)));
            }
            else if (RightSideCreatures.Count == 2 && currentDirection == eSide.Left)
            {
                isGameOver = RightSideCreatures.Any(f => RightSideCreatures.Any(b => b.IsEnemy(f)));
            }

            if (isGameOver)
            {
                Console.Clear();
                Console.WriteLine("YOU LOSE");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        private class Map
        {
            private static readonly Map instance = new Map();
            private Map() { }
            public static Map Instance => instance;
            protected MapElement SelectedArrow { get; set; }

            private LinkedList<MapElement> Arrows = new LinkedList<MapElement>();
            private LinkedList<MapElement> Creatures = new LinkedList<MapElement>();
            private Dictionary<int, Creature> BaseCreatures = new Dictionary<int, Creature>();
            public Creature GetCreatureOfRow() => BaseCreatures[SelectedArrow.YCoordinate];
            public eSide GetArrowDirection() => SelectedArrow.Side;
            public void initializeMap(IEnumerable<Creature> creatures)
            {
                if (creatures is null) throw new NullReferenceException();
                if (!creatures.Any()) throw new Exception("Map Initialize without creaters not possible.");

                Arrows.Clear();
                Creatures.Clear();
                Console.Clear();

                Console.WriteLine("ENTER        ---> Select creature and change arrow direction");
                Console.WriteLine("Arrow UP     ---> Move arrow to Up");
                Console.WriteLine("Arrow DOWN   ---> Move arrow to Down");
                Console.WriteLine("=============================================");
                Console.WriteLine("                                             ");

                int maximumLength = creatures.Max(f => f.Name.Length), yCoordinate = 0;

                foreach (var creature in creatures)
                {
                    Console.WriteLine(creature.Name);
                    Arrows.AddLast(new MapElement(maximumLength + 4, yCoordinate + 5, "<---", eSide.Left));
                    Creatures.AddLast(new MapElement(maximumLength + 4 + "<---".Length + 4, yCoordinate + 5, creature.Name, eSide.Right));
                    BaseCreatures.Add(yCoordinate + 5, creature);
                    yCoordinate++;
                }

                yCoordinate = 0;

                foreach (var creature in creatures)
                {
                    Arrows.AddLast(new MapElement(maximumLength + 4, yCoordinate + 5, "--->", eSide.Right));
                    Creatures.AddLast(new MapElement(0, yCoordinate + 5, creature.Name, eSide.Left));
                    yCoordinate++;
                }

                var yMiddleCoordinate = (int)(yCoordinate % 2 == 0 ? yCoordinate / 2 : Math.Round((decimal)yCoordinate / 2, MidpointRounding.AwayFromZero));
                SelectedArrow = Arrows.FirstOrDefault(f => f.Picture == "<---" & f.YCoordinate == yMiddleCoordinate + 4);

                WriteAt(SelectedArrow.Picture, SelectedArrow.XCoordinate, SelectedArrow.YCoordinate);
            }

            public void MoveArrowUp()
            {
                if (SelectedArrow == null) throw new Exception("Map is not Initialized");
                ArrowRedraw(Arrows.Find(SelectedArrow).Previous?.Value);
            }

            public void MoveArrowDown()
            {
                if (SelectedArrow == null) throw new Exception("Map is not Initialized");
                ArrowRedraw(Arrows.Find(SelectedArrow).Next?.Value);
            }

            public void ChangeCreatureSide()
            {
                if (SelectedArrow == null) throw new Exception("Map is not Initialized");

                RowRedraw();
            }

            private void ArrowRedraw(MapElement nextElement)
            {
                if (nextElement == null || nextElement.Side != SelectedArrow.Side) return;

                ElementRedraw(SelectedArrow, nextElement);
                SelectedArrow = nextElement;
            }

            private void RowRedraw()
            {
                //change arrow side
                var Side = (eSide)((int)SelectedArrow.Side * -1);
                var reversedArrow = Arrows.FirstOrDefault(f => f.Side == Side && f.YCoordinate == SelectedArrow.YCoordinate);
                WriteAt(reversedArrow.Picture, reversedArrow.XCoordinate, reversedArrow.YCoordinate);

                //clear creature from current side
                MapElement currentCreature = default;
                //draw creature to another side
                MapElement inverseCreature = default;

                foreach (var creature in Creatures)
                {
                    if (creature.YCoordinate == SelectedArrow.YCoordinate)
                    {
                        if (creature.Side != Side)
                            currentCreature = creature;
                        else
                            inverseCreature = creature;
                    }

                    if (inverseCreature != null && currentCreature != null) break;
                }

                ElementRedraw(currentCreature, inverseCreature);
                SelectedArrow = reversedArrow;
            }

            private void ElementRedraw(MapElement clearElement, MapElement drawElement)
            {
                WriteAt(new string(clearElement.Picture.Select(f => (char)32).ToArray()), clearElement.XCoordinate, clearElement.YCoordinate); ;
                WriteAt(drawElement.Picture, drawElement.XCoordinate, drawElement.YCoordinate);
            }

            private static void WriteAt(string s, int x, int y)
            {
                try
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(s);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.Clear();
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

    public enum eSide : int
    {
        Left = 1,
        Right = -1
    }

    public class MapElement
    {
        public MapElement(int xCoordinate, int yCoordinate, string Picture, eSide side)
        {
            this.XCoordinate = xCoordinate;
            this.YCoordinate = yCoordinate;
            this.Picture = Picture;
            this.Side = side;
        }
        public eSide Side = eSide.Left;
        public int XCoordinate { get; }
        public int YCoordinate { get; }
        public string Picture { get; }
    }


}
