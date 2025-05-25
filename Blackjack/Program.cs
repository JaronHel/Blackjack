using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Blackjack
{
    public class Program
    {
        static List<MenuOption> mainOptions = new List<MenuOption>();
        static List<MenuOption> gameOptions = new List<MenuOption>();

        static List<string> deck = new List<string>();

        public static List<string> dealerCards = new List<string>();
        public static List<string> playerCards = new List<string>();

        static string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        static string filePath = @"F:\Programming\C#\Blackjack\Blackjack\PlayerInfo.json";

        static bool betting;
        static bool gameRunning;
        public static bool isGameStart;
        static bool playerWon;
        static bool gameTie;

        public static int bet = 0;
        static double currentMoney = LoadPlayerInfo().Money;

        static ConsoleKeyInfo keyInfo;

        static Random rnd = new Random();

        static void Main()
        {
            Console.CursorVisible = false;
            MainMenu();
        }

        static void MainMenu()
        {
            deck.Clear();
            CreateDeck();

            mainOptions = new List<MenuOption>
            {
                new MenuOption("Start Game", () => StartNewGame()),
                new MenuOption("Exit", () => Environment.Exit(0))
            };

            int index = 0;

            do
            {
                WriteMenus.WriteMainMenu(mainOptions, mainOptions[index]);
                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < mainOptions.Count)
                    {
                        index++;
                        WriteMenus.WriteMainMenu(mainOptions, mainOptions[index]);
                    }
                }
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenus.WriteMainMenu(mainOptions, mainOptions[index]);
                    }
                }
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    mainOptions[index].Selected.Invoke();
                    index = 0;
                }
            }
            while (keyInfo.Key != ConsoleKey.X);
        }

        static void CreateDeck()
        {
            for (int i = 0; i < 6; i++)
            {
                deck.AddRange(ranks);
            }

            deck = deck.OrderBy(x => rnd.Next()).ToList();
        }

        static void StartNewGame()
        {
            bet = 0;
            dealerCards = [];
            playerCards = [];

            int index = 0;

            betting = true;
            gameRunning = true;
            isGameStart = true;
            playerWon = false;
            gameTie = false;

            Console.WriteLine($"Bank balance: {currentMoney}");
            Console.WriteLine($"Bet amount: {bet}");
            while (betting)
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (bet != 0)
                    {
                        bet -= 100;
                        Console.Clear();
                        Console.WriteLine($"Bank balance: {currentMoney}");
                        Console.WriteLine($"Bet amount: {bet}");
                    }
                }
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (bet + 100 <= currentMoney)
                    {
                        bet += 100;
                        Console.Clear();
                        Console.WriteLine($"Bank balance: {currentMoney}");
                        Console.WriteLine($"Bet amount: {bet}");
                    }
                }
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    betting = false;
                }
            }

            gameOptions = new List<MenuOption>
            {
                new MenuOption("Pull", () => OnPull()),
                new MenuOption("Stand", () => OnStand()),
            };

            for (int i = 0; i < 2; i++)
            {
                PullCard(dealerCards);
                PullCard(playerCards);
            }

            if (HasBlackjack())
            {
                ShowCards();
                Console.WriteLine();
                Console.WriteLine("Press enter to return to main menu...");
                Console.ReadLine();
                UpdatePlayerInfo();
                MainMenu();
                return;
            }

            while (gameRunning)
            {
                WriteMenus.WriteGameMenu(gameOptions, gameOptions[index]);
                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < gameOptions.Count)
                    {
                        index++;
                        WriteMenus.WriteGameMenu(gameOptions, gameOptions[index]);
                    }
                }
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenus.WriteGameMenu(gameOptions, gameOptions[index]);
                    }
                }
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    gameOptions[index].Selected.Invoke();
                    index = 0;
                }
            }
            Console.WriteLine("Press enter to return to main menu...");
            Console.ReadLine();
            UpdatePlayerInfo();
            MainMenu();
        }

        static void OnPull()
        {
            PullCard(playerCards);

            if (CalculateHandValue(playerCards) > 21)
            {
                Console.Clear();
                ShowCards();
                Console.WriteLine("You busted! Dealer wins.");
                BetFeedback();
                gameRunning = false;
            }
        }

        static void PullCard(List<string> currentPlayer)
        {
            int randomNumber = rnd.Next(deck.Count);

            currentPlayer.Add(deck[randomNumber]);
            deck.RemoveAt(randomNumber);
        }

        static void OnStand()
        {
            DealerPull();
            gameRunning = false;
        }

        static void DealerPull()
        {
            while (CalculateHandValue(dealerCards) < 17)
            {
                PullCard(dealerCards);
                Console.WriteLine($"Dealer pulls {dealerCards[dealerCards.Count - 1]}");
                Thread.Sleep(1500);
            }
            ShowCards();

            CheckWinner();
        }

        public static int CalculateHandValue(List<string> cards)
        {
            int totalValue = 0;
            int aceCount = 0;

            foreach (var card in cards)
            {
                if (card == "A")
                {
                    aceCount++;
                    totalValue += 11;
                }
                else if (card == "J" || card == "Q" || card == "K")
                {
                    totalValue += 10;
                }
                else
                {
                    totalValue += int.Parse(card);
                }
            }

            while (totalValue > 21 && aceCount > 0)
            {
                totalValue -= 10;
                aceCount--;
            }

            return totalValue;
        }

        static void CheckWinner()
        {
            int dealerCardValue = CalculateHandValue(dealerCards);
            int playerCardValue = CalculateHandValue(playerCards);

            if (dealerCardValue > 21)
            {
                Console.WriteLine("Dealer busted! You win.");
                playerWon = true;
                BetFeedback();
            }
            else if (dealerCardValue == playerCardValue)
            {
                Console.WriteLine("It's a tie! No winner.");
                gameTie = true;
                BetFeedback();
            }
            else if (dealerCardValue > playerCardValue)
            {
                Console.WriteLine("Dealer wins!");
                BetFeedback();
            }
            else
            {
                Console.WriteLine("You win!");
                playerWon = true;
                BetFeedback();
            }
        }

        static bool HasBlackjack()
        {
            bool dealerBJ = dealerCards.Count == 2 && dealerCards.Contains("A") &&
                        (dealerCards.Contains("10") || dealerCards.Contains("J") || dealerCards.Contains("Q") || dealerCards.Contains("K"));

            bool playerBJ = playerCards.Count == 2 && playerCards.Contains("A") &&
                            (playerCards.Contains("10") || playerCards.Contains("J") || playerCards.Contains("Q") || playerCards.Contains("K"));

            if (dealerBJ && playerBJ)
            {
                Console.WriteLine("Both have Blackjack! It's a tie!");
                gameTie = true;
                BetFeedback();
                return true;
            }
            else if (dealerBJ)
            {
                Console.WriteLine("Dealer has Blackjack! You lose.");
                BetFeedback();
                return true;
            }
            else if (playerBJ)
            {
                Console.WriteLine("You have Blackjack! You win!");
                currentMoney += bet * 1.5;
                Console.WriteLine();
                Console.WriteLine($"New bank balance: {currentMoney} (+{bet})");
                Console.WriteLine();
                return true;
            }
            return false;
        }

        public static void ShowCards()
        {
            Console.WriteLine($"Dealer Cards: {string.Join($", ", dealerCards)} (Value: {CalculateHandValue(dealerCards)})");
            Console.WriteLine($"Your Cards: {string.Join(", ", playerCards)} (Value: {CalculateHandValue(playerCards)})");
        }

        static void BetFeedback()
        {
            if (playerWon)
            {
                currentMoney += bet;
                Console.WriteLine();
                Console.WriteLine($"New bank balance: {currentMoney} (+{bet})");
                Console.WriteLine();
            }
            else if (gameTie)
            {
                Console.WriteLine();
                Console.WriteLine($"No money was lost or won!");
                Console.WriteLine();
            }
            else
            {
                currentMoney -= bet;
                Console.WriteLine();
                Console.WriteLine($"New bank balance: {currentMoney} (-{bet})");
                Console.WriteLine();
            }
        }

        public static PlayerInfo LoadPlayerInfo()
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string jsonString = sr.ReadToEnd();
                JObject obj = JObject.Parse(jsonString);
                double money = Math.Round(obj["Money"]!.Value<double>(), 2);

                PlayerInfo playerInfo = new PlayerInfo(money);

                return playerInfo;
            }
        }

        static void UpdatePlayerInfo()
        {
            var playerInfoJson = JsonConvert.SerializeObject(new PlayerInfo(currentMoney), Formatting.Indented);
            
            File.WriteAllText(filePath, playerInfoJson);
        }
    }
}