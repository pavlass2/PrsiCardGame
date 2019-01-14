using System;
using System.IO;

namespace PrsiCardGame
{
    //Sets if ace is overrulable
    public static class GameOption
    {
        /// <summary>
        /// Ask user to set the ace overrule 
        /// </summary>
        public static void SetOption()
        {
            Console.WriteLine("Overrule ace : [y/n]");
            switch (Console.ReadKey().KeyChar)
            {
                case 'y':
                    SaveOption(true);
                    Console.WriteLine("\nAces now can be overruled by another ace");
                    break;
                case 'n':
                    SaveOption(false);
                    Console.WriteLine("\nAces now cannot be overruled by another ace");
                    break;
                default:
                    Console.WriteLine("\nYou need to choose either yes [y] or no [n]");
                    SetOption();
                    break;
            }
        }

        /// <summary>
        /// Saves the option
        /// </summary>
        /// <param name="overrulable">Is it possible to overrule ace?</param>
        private static void SaveOption(bool overrulable)
        {
            //Saving files right into program folder may not be correct but for this simple program I find it sufficient
            using (StreamWriter writer = new StreamWriter(@"option.txt"))
            {
                writer.Write(overrulable.ToString());
                writer.Flush();
            }
        }

        /// <summary>
        /// Loads option information
        /// </summary>
        /// <returns>Bool which sets if it is possible to overrule ace by anouther ace</returns>
        public static bool LoadOption()
        {
            if (File.Exists(@"option.txt"))
            {
                string textBool;
                using (StreamReader reader = new StreamReader(@"option.txt"))
                {
                     textBool = reader.ReadLine();
                }

                if (textBool == "True")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}