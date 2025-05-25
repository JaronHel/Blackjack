namespace Blackjack
{
    public class WriteMenus
    {
        static int width = Console.WindowWidth;

        public static void WriteMainMenu(List<MenuOption> options, MenuOption selectedOption)
        {
            Console.Clear();
            Show3dTitle();

            foreach (MenuOption option in options)
            {
                if (option == selectedOption)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ResetColor();
                }
                int x = (width - option.Name.Length) / 2;
                Console.SetCursorPosition(x, Console.CursorTop);
                Console.WriteLine(option.Name);
            }
        }

        public static void WriteGameMenu(List<MenuOption> options, MenuOption selectedOption)
        {
            Console.Clear();

            if (Program.isGameStart)
            {
                Console.WriteLine($"Dealer Cards: ?, {Program.dealerCards[1]}");
                Console.WriteLine($"Your Cards: {string.Join(", ", Program.playerCards)} (Value: {Program.CalculateHandValue(Program.playerCards)})");
            }
            else
            {
                Program.ShowCards();
            }
            Console.WriteLine();

            foreach (MenuOption option in options)
            {
                if (option == selectedOption)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write(" ");
                }
                Console.WriteLine(option.Name);
            }
        }

        static void Show3dTitle()
        {
            string title = @"
██████╗ ██╗      █████╗  ██████╗██╗  ██╗     ██╗ █████╗  ██████╗██╗  ██╗
██╔══██╗██║     ██╔══██╗██╔════╝██║ ██╔╝     ██║██╔══██╗██╔════╝██║ ██╔╝
██████╔╝██║     ███████║██║     █████╔╝      ██║███████║██║     █████╔╝ 
██╔══██╗██║     ██╔══██║██║     ██╔═██╗ ██   ██║██╔══██║██║     ██╔═██╗ 
██████╔╝███████╗██║  ██║╚██████╗██║  ██╗╚█████╔╝██║  ██║╚██████╗██║  ██╗
╚═════╝ ╚══════╝╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝ ╚════╝ ╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝
            ";

            var lines = title.Split('\n');
            int realTitleLength = lines.Max(line => line.Length);
            int x = Math.Max(0, (width - realTitleLength) / 2);

            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var line in lines)
            {
                Console.SetCursorPosition(x, Console.CursorTop);
                Console.WriteLine(line);
            }

            Console.ResetColor();
        }
    }
}
