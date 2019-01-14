using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PrsiCardGame
{
    public class GamePlay
    {
        ////////////
        ///Fields///
        ////////////

        /// <summary>
        /// Store generatad cards here
        /// </summary>
        private List<Card> deck;        

        /// <summary>
        /// Main deck from which players will be drawing cards
        /// </summary>
        private Stack<Card> shuffledDeck;

        /// <summary>
        /// Card that was just played
        /// </summary>
        private Card justPlayed;

        /// <summary>
        /// All cards that were already played
        /// </summary>
        private Stack<Card> usedCards;

        /// <summary>
        /// Amount of cards to be drawn after seven was played
        /// </summary>
        private byte sevenAmount;

        /// <summary>
        /// Suit that was last Jack changed to
        /// </summary>
        private Suit suitToChange;

        /// <summary>
        /// Last action that was take by any player
        /// [0] player that took the action
        /// [1] action that was taken
        /// </summary>
        private string[] recentAction;

        /// <summary>
        /// Random used to shuffle the deck
        /// </summary>
        private Random random;

        /// <summary>
        /// Player that won the game
        /// </summary>
        private Player winner;


        /////////////////
        ///Constructor///
        /////////////////

        /// <summary>
        /// Constructor
        /// </summary>
        public GamePlay()
        {
            deck = new List<Card>();
            shuffledDeck = new Stack<Card>();
            usedCards = new Stack<Card>();
            random = new Random();
            sevenAmount = 0;
            recentAction = new string[2] { "Game started", "" };
                       
            //Generate cards
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Name name in Enum.GetValues(typeof(Name)))
                {
                    deck.Add(new Card(suit, name));
                }
            }            
        }


        /////////////
        ///Methods///
        /////////////

        /// <summary>
        /// Main method taking care about play itself
        /// </summary>
        public void Play()
        {
            //preparing game 

            int randomIndex = 0;
            while (deck.Count > 0)
            {
                randomIndex = random.Next(0, deck.Count); //randomize index                    
                shuffledDeck.Push(deck[randomIndex]); //add card from original deck on randomizded index to shuffledDeck
                deck.RemoveAt(randomIndex); //remove the card from the original deck
            }
            suitToChange = shuffledDeck.Peek().CardSuit;

            //Prepare player one
            Console.Write("Virtual dealer: Player one, how should I call you? ");
            Player[] players = new Player[2];
            players[0] = new Player(this, Console.ReadLine());

            //Prepare player two
            Console.Write("\nVirtual dealer: ...and now your name, player two. ");
            players[1] = new Player(this, Console.ReadLine());           
            
            //Deal cards to players
            for (int i = 0; i < 4; i += 1)
            {
                players[0].Hand.Add(shuffledDeck.Pop());
                players[1].Hand.Add(shuffledDeck.Pop());
            }

            usedCards.Push(shuffledDeck.Pop());

            //Let the game begin
            winner = null;            
            int turn = 0;
            while (true)
            {
                turn += 1;

                //Player one
                DrawBasicUI(players[0], players[1], turn);
                PlayerManagement(players[0]);                

                //Chech player's hand for winning condition
                if (CheckWinningConditions(players[0]))
                {
                    //If other player has seven of hearts and he can legally play it, let the game continue
                    if ((players[1].Hand.FirstOrDefault(c => (c.CardName == Name.Seven) && (c.CardSuit == Suit.Heart)) != null) && ((usedCards.Peek().CardName == Name.Seven) || (usedCards.Peek().CardSuit == Suit.Heart) || ((justPlayed.CardName == Name.Jack) && (suitToChange == Suit.Heart))))
                    {
                        winner = players[0];
                    }
                    else
                    {
                        winner = players[0];
                        break;
                    }
                }
                recentAction[0] = players[0].Nick; //this player took most recent action

                
                //Player two
                DrawBasicUI(players[1], players[0], turn);                
                PlayerManagement(players[1]);               

                //Chech player's hand for winning condition
                if (CheckWinningConditions(players[1]))
                {
                    //If other player has seven of hearts and he can legally play it, let the game continue                       
                    if ((players[0].Hand.FirstOrDefault(c => (c.CardName == Name.Seven) && (c.CardSuit == Suit.Heart)) != null) && ((usedCards.Peek().CardName == Name.Seven) || (usedCards.Peek().CardSuit == Suit.Heart) || ((justPlayed.CardName == Name.Jack) && (suitToChange == Suit.Heart))))
                    {
                        winner = players[1];
                    }
                    else
                    {
                        winner = players[1];
                        break;
                    }
                }
                recentAction[0] = players[1].Nick; //this player took most recent action
            }

            //Show winner
            recentAction[0] = winner.Nick;
            recentAction[1] = "won the game";
            Console.Clear();
            DrawBasicUI(players[0], players[1], turn);
            Console.WriteLine(winner.Nick + ", congratulations! You are the winner.");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.WriteLine();
        }


        /////////////////////////////////////////////////////////////
        ///Private methods providing helpful logic for Play method///
        /////////////////////////////////////////////////////////////

        /// <summary>
        /// Takes care about logic necessary to procces the turn for player 
        /// </summary>
        /// <param name="player">Player to take turn</param>
        private void PlayerManagement(Player player)
        {
            Console.WriteLine("Upcard:");
            usedCards.Peek().Draw();

            if (winner != null) //if winner is known but game is still going, at this this point it means that winner can be put back in game
            {
                player.PlaySevenOfHearts();
                winner = null;
            }
            else if (justPlayed != null) //if justPlayed is not null, it means last card special powers are on
            {
                if (justPlayed.CardName == Name.Ace) //ace special powers
                {
                    player.ActiveAce();
                }
                else if (justPlayed.CardName == Name.Seven) //seven special powers
                {
                    player.ActiveSeven(sevenAmount);
                }
                else if (justPlayed.CardName == Name.Jack) //jack special powers
                {
                    //if svrsek's suit and suitToChange are same, treat them as normal card 
                    if (justPlayed.CardSuit == suitToChange)
                    {
                        player.PlayNormal(justPlayed);
                    }
                    else
                    {
                        player.ActiveJack(suitToChange); //else treat them as jack and suit
                    }
                }
                else //justPlayedCard is card without special powers
                {
                    player.PlayNormal(usedCards.Peek());
                }
            }
            //svrsek must be taken care of even if it is no longer new card
            else if (usedCards.Peek().CardName == Name.Jack)
            {
                //if svrsek's suit and suitToChange are same, treat them as normal card 
                if (usedCards.Peek().CardSuit == suitToChange)
                {
                    player.PlayNormal(usedCards.Peek());
                }
                else
                {
                    player.ActiveJack(suitToChange); //else treat them as jack and suit
                }
            }
            else
            {
                player.PlayNormal(usedCards.Peek()); //last card is card without special powers
            }
        }       

        /// <summary>
        /// Draws UI for the gameplay
        /// </summary>
        /// <param name="playerOne"></param>
        /// <param name="playerTwo"></param>
        /// <param name="turn"></param>
        private void DrawBasicUI(Player playerOne, Player playerTwo, int turn)
        {
            System.Console.BackgroundColor = ConsoleColor.DarkGreen;
            System.Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("********** - - - Prší - - - **********");
            Console.WriteLine("|||| " + playerOne.Nick + " vs. " + playerTwo.Nick + " |||| Turn: " + turn);
            Console.WriteLine(playerOne.Nick + " is on turn");
            Console.WriteLine("Last action: {0} {1}", recentAction);
        }

        /// <summary>
        /// Checks if a player's hand is empty, therefore he wins
        /// </summary>
        /// <param name="player">Player to check winning conditions against</param>
        /// <returns></returns>
        private bool CheckWinningConditions(Player player)
        {
            if (player.Hand.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        ////////////////////////////////////////////////////////////////////////
        //These methods take care about processing calls and data from players//
        ////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Takes card from player and procces it as played
        /// </summary>
        /// <param name="card">Card to play</param>
        public void PlayCard(Card card)
        {
            justPlayed = card;
            usedCards.Push(card);
            recentAction[1] = "played " + card.CardName;
        }

        /// <summary>
        /// Takes Jack from player and procces it as played
        /// </summary>
        /// <param name="card">Jack to play</param>
        /// <param name="changeTo">Suit to change Jack into</param>
        public void PlayJack(Card card, Suit changeTo)
        {
            justPlayed = card;
            usedCards.Push(card);
            suitToChange = changeTo;
            recentAction[1] = "played " + card.CardName + " and changed the suit to " + changeTo;
        }

        /// <summary>
        /// Takes Seven from player and procces it as played
        /// </summary>
        /// <param name="card">Seven to play</param>
        public void PlaySeven(Card card)
        {
            justPlayed = card;
            sevenAmount += 2;
            usedCards.Push(card);
            recentAction[1] = "played " + card.CardName;
        }

        /// <summary>
        /// Resets the sevenAmount counter
        /// Player calls this after he is forced to draws multiple cards by Seven
        /// </summary>
        public void SetSevenAmountToZero()
        {
            sevenAmount = 0;
        }

        /// <summary>
        /// Lets player draw a card from the deck
        /// </summary>
        /// <returns>Top card from the deck</returns>
        public Card Draw()
        {
            justPlayed = null;
            
            //In case there is no card in a deck
            if (shuffledDeck.Count == 0)
            {                
                Card card = usedCards.Pop(); //take the last card from usedCards
                usedCards.Reverse(); //reverse the rest
                shuffledDeck = usedCards; //put them in deck
                usedCards = new Stack<Card>(); //empty usedCards
                usedCards.Push(card); //put the last card back into usedCards
                Console.WriteLine("Virtual dealer: reversing deck."); //Notify players
                Thread.Sleep(1500);
            }

            //In case, shuffle deck is still empty, user is probably trying to break the game, exit
            if (shuffledDeck.Count == 0)
            {
                Console.WriteLine("All cards in possesion of players. Please comeback after you learn how to play this game.");
                Console.WriteLine("Press any key to quit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            recentAction[1] = "drawed a card";
            return shuffledDeck.Pop();
        }

        /// <summary>
        /// Sets justPlayed to null
        /// Player calls this when he is frozen by ace
        /// </summary>
        public void DoNothing()
        {
            justPlayed = null;
        }
    }
}