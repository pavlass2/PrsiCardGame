using System;
using System.Collections.Generic;
using System.Linq;

namespace PrsiCardGame
{
    public class Player
    {
        /// <summary>
        /// Player's nick
        /// </summary>
        public string Nick { get; private set; }

        /// <summary>
        /// Cards in player's hand
        /// </summary>
        public List<Card> Hand { get; set; }

        /// <summary>
        /// Send data containing user input here
        /// </summary>
        private GamePlay gamePlay;

        /// <summary>
        /// Sets if overrulig aces is allowed
        /// </summary>
        private bool overrulableAce;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gamePlay">To give data to containing user input</param>
        /// <param name="nick">Nick of the player</param>
        public Player(GamePlay gamePlay, string nick)
        {
            this.gamePlay = gamePlay;
            Nick = nick;
            Hand = new List<Card>();
            overrulableAce = GameOption.LoadOption();
        }

        ///////////////////////////////////////////////////////////////////////////
        ///Public methods called from gamePlay taking care in various scenarious///
        ///////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// General situation, card from opponent has no special effect
        /// </summary>
        /// <param name="card"></param>
        public void PlayNormal(Card card)
        {
            //playable are: every svrsek, same suit and same name
            Play(Hand.FindAll(c => (c.CardName == Name.Jack) || (c.CardName == card.CardName) || (c.CardSuit == card.CardSuit)));            
        }

        /// <summary>
        /// Force user to play Seven of hearts
        /// Used for getting opponent back to the game
        /// </summary>
        public void PlaySevenOfHearts()
        {
            Console.WriteLine("Virtual dealer: Your opponent has no cards.");
            List<Card> playableCards = Hand.FindAll(c => (c.CardName == Name.Seven) && (c.CardSuit == Suit.Heart));
            DrawUI(playableCards);
            int handIndex = ManageUserInput(playableCards, false); //Call for user input but reject 0
            gamePlay.PlaySeven(Hand[handIndex]); ////Send data to gamePlay
            Hand.RemoveAt(handIndex); //Remove from hand
        }

        /// <summary>
        /// Procces ace card from opponent
        /// </summary>
        public void ActiveAce()
        {
            //if user has no ace to overrule or overruling ace is now allowd
            if ((Hand.FirstOrDefault(c => c.CardName == Name.Ace) == null) || overrulableAce == false)
            {
                Console.WriteLine("Virtual dealer: " + Nick + " is frozen in this turn.");
                Console.WriteLine("Pres any key to continue.");
                Console.ReadKey();
                gamePlay.DoNothing();
            }
            else //let the user choose ace from his hand
            {
                Console.WriteLine("You can overrule your opponent's ace!");
                List<Card> playableCards = Hand.FindAll(c => c.CardName == Name.Ace);
                DrawUI(playableCards, true);
                int handIndex = ManageUserInput(playableCards);

                if (handIndex < 0) //if player does not want to use his ace
                {
                    gamePlay.DoNothing();
                }
                else
                {
                    gamePlay.PlayCard(Hand[handIndex]); //Send data to gamePlay
                    Hand.RemoveAt(handIndex); //Remove from hand
                }
            }
        }

        /// <summary>
        /// Procces Seven card from opponent
        /// </summary>
        /// <param name="cardAmount">Amount of cards to draw</param>
        public void ActiveSeven(int cardAmount)
        {
            Console.WriteLine("Virtual dealer: Draw " + cardAmount + " cards.");
            List<Card> playableCards = Hand.FindAll(c => c.CardName == Name.Seven);
            DrawUI(playableCards);
            int handIndex = ManageUserInput(playableCards);
            if (handIndex < 0) //user chose draw
            {
                for (int i = 0; i < cardAmount; i += 1)
                {
                    Hand.Add(gamePlay.Draw());
                }
                gamePlay.SetSevenAmountToZero(); //Send data to gamePlay
            }
            else
            {
                gamePlay.PlaySeven(Hand[handIndex]); //Send data to gamePlay
                Hand.RemoveAt(handIndex); //Remove from hand
            }
        }

        /// <summary>
        /// Procces Jack card from opponent
        /// </summary>
        /// <param name="suit">Suit into which Jack changed</param>
        public void ActiveJack(Suit suit)
        {
            Console.WriteLine("Virtual dealer: Svrsek changed suit to " + suit + ".");
            Play(Hand.FindAll(c => (c.CardName == Name.Jack) || (c.CardSuit == suit)));
        }

        

        ///////////////////////////////////////////////////////
        ///Private methods providing logic to public methods///
        ///////////////////////////////////////////////////////

        /// <summary>
        /// Serves the user with information, checks his input and sends it to gameplay
        /// </summary>
        /// <param name="playableCards">Cards in hand that can be legally played</param>
        private void Play(List<Card> playableCards)
        {
            DrawUI(playableCards);
            int handIndex = ManageUserInput(playableCards);

            if (handIndex < 0) //negative index means draw
            {
                Hand.Add(gamePlay.Draw());
            }
            else //procces cards
            {
                if (Hand[handIndex].CardName == Name.Jack) //procces Jack
                {
                    Suit suit = Suit.Club; //set any default value
                    Console.WriteLine("Choose suit");
                    Console.WriteLine("1. " + (char)0x2663); //club
                    Console.WriteLine("2. " + (char)0x2666); //diamond
                    Console.WriteLine("3. " + (char)0x2665); //heart
                    Console.WriteLine("4. " + (char)0x2660); //spade

                    bool ok = false;
                    while (ok == false)
                    {
                        switch (Console.ReadKey().KeyChar) //let user set the suit
                        {
                            case '1':
                                suit = Suit.Club;
                                ok = true;
                                break;
                            case '2':
                                suit = Suit.Diamond;
                                ok = true;
                                break;
                            case '3':
                                suit = Suit.Heart;
                                ok = true;
                                break;
                            case '4':
                                suit = Suit.Spade;
                                ok = true;
                                break;
                            default:
                                Console.WriteLine("You have to choose one of the suits");
                                break;
                        }
                    }
                    gamePlay.PlayJack(Hand[handIndex], suit); ////Send data to gamePlay
                    Hand.RemoveAt(handIndex); //Remove from hand
                }
                else if (Hand[handIndex].CardName == Name.Seven) //Procces Seven
                {
                    gamePlay.PlaySeven(Hand[handIndex]); ////Send data to gamePlay
                    Hand.RemoveAt(handIndex); //Remove from hand
                }
                else //Procces other cards
                {
                    gamePlay.PlayCard(Hand[handIndex]); ////Send data to gamePlay
                    Hand.RemoveAt(handIndex); //Remove from hand
                }
            }
        }

        /// <summary>
        /// Draws UI and cards in hand
        /// </summary>
        /// <param name="cards">Cards in hand in order to draw their pictures</param>
        /// <param name="ace">Sets if 0 means do nothing</param>
        private void DrawUI(List<Card> cards, bool ace = false)
        {
            Console.WriteLine("________________________________");
            Console.WriteLine(Nick + ", your hand:");
            
            //iterate through all cards in hand and draw their pictures
            for (int i = 0; i < Hand.Count; i++)
            {
                Console.WriteLine(i + 1 + ".");
                Hand[i].Draw();
            }

            Console.WriteLine("0.");
            if (ace == false)
            {
                Console.WriteLine("Draw");
            }
            else
            {
                Console.WriteLine("Do nothing");
            }
        }

        /// <summary>
        /// Takes care about input from user
        /// </summary>
        /// <param name="cards">Cards in hand that are legal to play</param>
        /// <param name="allowDraw">Determins whether is allowed to draw</param>
        /// <returns>Index of the card in hand choosed by user input</returns>
        private int ManageUserInput(List<Card> cards, bool allowDraw = true)
        {
            int playersChoice;
            bool playOk = false;
            do //do while input from user is no correct
            {
                //if user inputs key not related with any card
                if (!(int.TryParse(Console.ReadLine(), out playersChoice)) || (playersChoice > Hand.Count) || (playersChoice < 0))
                {
                    Console.WriteLine("Virtual dealer: You cannot do that.");
                }
                else
                {
                    if (playersChoice > 0) //if user chooses card correctly
                    {
                        //if the chosed card can be legally played
                        if (cards.FirstOrDefault(c => c == Hand[playersChoice - 1]) != null)
                        {
                            playOk = true;
                        }
                        else //playing card would be against the rules
                        {
                            Console.WriteLine("Virtual dealer: That would be against the rules, try it again.");
                        }
                    }
                    else if (playersChoice == 0 && allowDraw) //if user chooses to draw a card and it is allowed
                    {
                        playOk = true;
                    }
                    else //only in case user is forced to play seven of hearts
                    {
                        Console.WriteLine("Virtual dealer: You cannot draw a card at this moment.");
                    }
                }
            } while (playOk == false);
            return playersChoice - 1;
        }
    }
}