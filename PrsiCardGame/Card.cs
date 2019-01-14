using System;
using System.Drawing;

namespace PrsiCardGame
{
    /// <summary>
    /// All suits of cards
    /// </summary>
    public enum Suit { Heart, Diamond, Club, Spade }

    /// <summary>
    /// All names of cards
    /// </summary>
    public enum Name { Seven = 7, Eight = 8, Nine = 9, Ten = 10, DownGuy, Jack, Ace, King }

    public class Card
    {
        /// <summary>
        /// Suit of the card
        /// </summary>
        public Suit CardSuit { get; private set; }

        /// <summary>
        /// Name of the card
        /// </summary>
        public Name CardName { get; private set; }

        /// <summary>
        /// Consists of CardSuit symbol and CardName symbol
        /// </summary>
        private string cardString;

        /// <summary>
        /// Construct a card
        /// </summary>
        /// <param name="cardSuit">Suit of the card</param>
        /// <param name="cardName">Name of the card</param>
        public Card(Suit cardSuit, Name cardName)
        {
            CardSuit = cardSuit;
            CardName = cardName;


            //set cardString
            char firstPart = ' ';
            switch (CardSuit) //set suit part
            {
                case Suit.Club:
                    firstPart = (char)0x2663;
                    break;
                case Suit.Diamond:
                    firstPart = (char)0x2666;
                    break;
                case Suit.Heart:
                    firstPart = (char)0x2665;
                    break;
                case Suit.Spade:
                    firstPart = (char)0x2660;
                    break;
            }

            string secondPart = "";
            switch (CardName) //set name part
            {
                case Name.Ten:
                    secondPart = ((int)CardName).ToString();
                    break;
                case Name.Nine:
                case Name.Eight:
                case Name.Seven:
                    secondPart = " " + ((int)CardName).ToString();
                    break;
                case Name.Ace:
                    secondPart = " A";
                    break;
                case Name.DownGuy:
                    secondPart = " " + ((char)0x2193).ToString();
                    break;
                case Name.Jack:
                    secondPart = " " + ((char)0x2191).ToString();
                    break;
                case Name.King:
                    secondPart = " K";
                    break;
            }
            cardString = firstPart + secondPart;
        }

        /// <summary>
        /// Draw picture of the card into the console
        /// </summary>
        public void Draw()
        {
            Console.WriteLine(((char)0x250C).ToString() + (char)0x2500 + ((char)0x2500).ToString() + ((char)0x2500).ToString() + (char)0x2510);
            Console.WriteLine(((char)0x2502).ToString() + cardString + ((char)0x2502).ToString());
            Console.WriteLine(((char)0x2514).ToString() + (char)0x2500 + ((char)0x2500).ToString() + ((char)0x2500).ToString() + (char)0x2518);
        }        
    }
}