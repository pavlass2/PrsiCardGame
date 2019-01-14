using System;

namespace PrsiCardGame
{
    /// <summary>
    /// Main menu
    /// </summary>
    public class Menu
    {
        private GamePlay gamePlay;

        public Menu()
        {
            gamePlay = new GamePlay();
        }

        /// <summary>
        /// Shows main game menu
        /// </summary>
        public void ShowMenu()
        {
            while (true)
            {
                //Show menu interface
                Console.WriteLine("********** - - - Prší - - - **********");
                Console.WriteLine("1. New game");
                Console.WriteLine("2. Tutorial");
                Console.WriteLine("3. Options");
                Console.WriteLine("4. Quit");

                switch (Console.ReadKey().KeyChar)
                {
                    case '1':
                        //Start new game
                        Console.WriteLine("\nStarting new game");
                        gamePlay.Play();
                        break;

                    case '2':
                        //Load game from xml file
                        Console.WriteLine("\nTutorial");
                        Tutorial();
                        break;

                    case '3':
                        //Set if aces are overrulable
                        Console.WriteLine("\nOptions menu:");
                        GameOption.SetOption();
                        break;

                    case '4':
                        //Make sure user really wants to exit
                        Console.WriteLine("\nPress 'y' to quit");
                        if (Console.ReadKey().KeyChar == 'y')
                        {
                            //user pressed y, exitting app
                            Environment.Exit(0);
                        }
                        else
                        {
                            //user pressed something else, do not quit app
                            Console.WriteLine();
                        }
                        break;

                    default:
                        //user pressed differnt key
                        Console.WriteLine("\nThis is not a valid option");
                        break;
                }
            }
        }

        private void Tutorial()
        {
            Console.WriteLine("You can toggle the overruling ace in options.");
            Console.WriteLine("Control:");
            Console.WriteLine("To choose cart, type number and press enter.");
            Console.WriteLine("In all other casses, pressing the key with desired number is sufficient.");
            Console.WriteLine("__________________");
            Console.WriteLine("Press any key to return to menu");
            Console.ReadKey();
        }
    }
}