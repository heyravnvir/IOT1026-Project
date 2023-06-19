namespace MinotaurLabyrinth
{
    /// <summary>
    /// this room is filled with poison in air so the player can only move 5 steps before he dies.
    /// </summary>
    public class Toxic : Room
    {
        const int MAX_STEPS = 5;
        static Toxic()
        {
            RoomFactory.Instance.Register(RoomType.Toxic, () => new Toxic());
        }

        public override RoomType Type { get; } = RoomType.Toxic;

        public override bool IsActive { get; protected set; } = true;

        public override void Activate(Hero hero, Map map)
        {
            if (IsActive)
            {
                ConsoleHelper.WriteLine("You walk into the Toxic room, and you have to play a dice game.If you win, nothing happen, If you lost, you would get punishment!", ConsoleColor.Red);
                ConsoleHelper.WriteLine("Choose large, small, after that will dice 3 dices, and calculate the result.", ConsoleColor.Gray);
                bool smallFlag = GetUserSelectOption();

                if (PlayGame(smallFlag))
                {
                    HandleWinGame();
                }
                else
                {
                    HandleLostGame(hero);
                }

                IsActive = false;
            }
        }

        public override DisplayDetails Display()
        {
            return IsActive ? new DisplayDetails($"[{Type.ToString()[0]}]", ConsoleColor.Red)
                            : base.Display();
        }

        public override bool DisplaySense(Hero hero, int heroDistance)
        {
            if (!IsActive)
            {
                if (base.DisplaySense(hero, heroDistance))
                {
                    return true;
                }
                if (heroDistance == 0)
                {
                    ConsoleHelper.WriteLine("You will experiece a terrible game in this room.", ConsoleColor.DarkGray);
                    return true;
                }
            }
            else if (heroDistance == 1 || heroDistance == 2)
            {
                ConsoleHelper.WriteLine(heroDistance == 1 ? "You feel a toxic. There is a toxic in a nearby room!" : "Your intuition tells you that something scary is nearby", ConsoleColor.DarkGray);
                return true;
            }
            return false;
        }

        //need to set virtual and protected, then the Test can override this funtion return value.
        protected virtual int Dice3Dices()
        {
            int number = 0;
            for (int i = 0; i < 3; ++i)
            {
                number += RandomNumberGenerator.Next(1, 6);
            }
            ConsoleHelper.WriteLine($"You dice: {number} when you are playing the dice game!", ConsoleColor.Yellow);
            return number;
            
        }

        protected virtual bool GetUserSelectOption()
        {
            return Console.ReadLine() switch
            {
                "small" => true,
                "large" => false,
                _ => false
            };
        }

        protected virtual void HandleWinGame()
        {
            ConsoleHelper.WriteLine("Congrat you win the dice game, nothing will happen.", ConsoleColor.Yellow);
        }

        protected virtual void HandleLostGame(Hero hero)
        {
            ConsoleHelper.WriteLine($"Be careful, you can play at most {MAX_STEPS} steps from now.", ConsoleColor.Red);
            hero.IsPoisoned = true;
            hero.StillCanPlaySteps = MAX_STEPS;

            //set the callback for the game to call when game ended.
            //because this room already finish, but the game keep going.
            //I want this feature for that can affect the game flow, not only the room close then nothing happen.
            hero.Callback = HandleCauseDeth;
        }

        private bool PlayGame(bool selectSmall)
        {
            bool winFlag = false;
            int point = Dice3Dices();
            if (selectSmall)
            {
                winFlag = point <= 10;
            }
            else
            {
                winFlag = point >= 11;
            }
            return winFlag;
        }

        //handle call back when the game ended.
        public static void HandleCauseDeth(Hero hero)
        {
            hero.Kill($"You have gotton a toxic and died because You cannot exit this labyrinth before {MAX_STEPS} steps.");
        }
    }
}
