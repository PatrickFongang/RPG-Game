using RPGGame.Items;

namespace RPGGame;

public class Render(Board board, Player player)
{
    private void RenderBoard()
    {
        Console.SetCursorPosition(0, 5);
        for (int i = 0; i < board.Rows; i++)
        {
            for (int j = 0; j < board.Columns; j++)
            {
                if (player.X == j && player.Y == i)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write('P');
                }
                else if (board[i, j].ItemsOnGround.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(board[i, j].ItemsOnGround.Peek().Symbol);
                }
                else if (board[i, j].Terrain == TerrainType.Wall)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write('█');
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write('▓');
                }
            }
            Console.WriteLine();
        }
    }

    private void RenderPlayerBackpack()
    {
        int startX = board.Columns + 5; 
        int columnWidth = 30; 
    
        int currentX = startX;
        int currentY = 5;

        void PrintLine(string text, bool isHighlighted = false)
        {
            Console.SetCursorPosition(currentX, currentY);
        
            if (isHighlighted)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.Write(text.PadRight(columnWidth));
            Console.ResetColor();

            currentY++; 

            if (currentY - 5 >= board.Rows)
            {
                currentY = 6; 
                currentX += columnWidth + 2; 
            }
        }
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        PrintLine("=== BACKPACK ===");
        Console.ResetColor();

        var items = player.Backpack.Items; 

        if (items.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            PrintLine("Empty...");
            Console.ResetColor();
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                bool isSelected = (i == player.Backpack.SelectedItemIndex);
                string itemText = items[i].ToString();
            
                PrintLine(itemText, isSelected);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            PrintLine(" ");
        }
    }
    private void RenderActionPrompt()
    {
        int promptY = board.Rows + 1 + 4;
        Console.SetCursorPosition(0, promptY);

        var itemsUnderPlayer = board[player.Y, player.X].ItemsOnGround;

        if (itemsUnderPlayer.Count > 0)
        {
            Item topItem = itemsUnderPlayer.Peek();

            Console.ForegroundColor = ConsoleColor.Yellow;
        
            string promptText = $"Press E to pick up: {topItem.ToString()} - \"{topItem.Description}\"";
        
            Console.Write(promptText.PadRight(100)); 
        
            Console.ResetColor();
        }
        else
        {
            Console.Write(new string(' ', 200));
        }
    }
    private void RenderControls()
    {
        int startY = board.Rows + 6; 
    
        Console.SetCursorPosition(0, startY);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("=== CONTROLS ===".PadRight(80));
        Console.ResetColor();

        Console.SetCursorPosition(0, startY + 1);
        Console.WriteLine("[W/A/S/D] Move       [E] Pick Up Item      [T] Drop/Throw Item".PadRight(80));
    
        Console.SetCursorPosition(0, startY + 2);
        Console.WriteLine("[↑ / ↓]   Select     [Q] Equip Selected".PadRight(80));
    
        Console.SetCursorPosition(0, startY + 3);
        Console.WriteLine("[L] Free Left Hand   [R] Free Right Hand".PadRight(80));
    }
    private void RenderEquippedItems()
    {
        Console.SetCursorPosition(0, 1);
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("=== EQUIPPED WEAPONS ===".PadRight(50));
        Console.ResetColor();

        Console.SetCursorPosition(0, 2);
        string leftHandText = player.LeftHand != null ? player.LeftHand.ToString() : "Empty";
        Console.Write($"Left Hand:  {leftHandText}".PadRight(50));

        Console.SetCursorPosition(0, 3);
        string rightHandText = player.RightHand != null ? player.RightHand.ToString() : "Empty";
        Console.Write($"Right Hand: {rightHandText}".PadRight(50));
    }
    private void RenderPlayerStats()
    {
        int startX = board.Columns + 5; 

        Console.SetCursorPosition(startX, 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("=== WALLET ===".PadRight(30));
        Console.ResetColor();

        Console.SetCursorPosition(startX, 2);
        Console.Write($"Coins: {player.Coins}".PadRight(30));

        Console.SetCursorPosition(startX, 3);
        Console.Write($"Gold:  {player.Gold}".PadRight(30));
    }
    public void RenderPlayerAttributes()
    {
        int startX = board.Columns + 35; 

        Console.SetCursorPosition(startX, 1);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("=== ATTRIBUTES ===".PadRight(35));
        Console.ResetColor();

        Console.SetCursorPosition(startX, 2);
        Console.Write($"STR: {player.Strength, -5} AGI: {player.Agility}".PadRight(35));

        Console.SetCursorPosition(startX, 3);
        Console.Write($"WIS: {player.Wisdom, -5} LUK: {player.Luck}".PadRight(35));

        Console.SetCursorPosition(startX, 4);
        Console.Write($"AGG: {player.Aggression, -5}".PadRight(35));
    }
    public void RenderUI()
    {
        RenderBoard();
        RenderPlayerBackpack();
        RenderActionPrompt();
        RenderEquippedItems();
        RenderPlayerStats();
        RenderPlayerAttributes();
        RenderControls();
    }
}