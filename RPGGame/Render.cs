using RPGGame.Items;

namespace RPGGame;

public class Render(Dungeon dungeon, Player player, GameEngine engine)
{
    private void RenderBoard()
    {
        for (int i = 0; i < dungeon.Rows; i++)
        {
            Console.SetCursorPosition(0, 5 + i);
            for (int j = 0; j < dungeon.Columns; j++)
            {
                if (player.X == j && player.Y == i)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write('¶');
                }
                else if (dungeon[i, j].ItemsOnGround.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(dungeon[i, j].ItemsOnGround.Peek().Symbol);
                }
                else
                {
                    Console.ForegroundColor = dungeon[i, j].GetColor();
                    Console.Write(dungeon[i, j].GetSymbol());
                }
            }
        }
    }

    private void RenderPlayerBackpack()
    {
        int startX = dungeon.Columns + 5;
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

            if (currentY - 5 >= dungeon.Rows)
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
        int promptY = dungeon.Rows + 1 + 4;
        Console.SetCursorPosition(0, promptY);

        var itemsUnderPlayer = dungeon[player.Y, player.X].ItemsOnGround;

        if (itemsUnderPlayer.Count > 0)
        {
            Item topItem = itemsUnderPlayer.Peek();

            Console.ForegroundColor = ConsoleColor.Yellow;

            string promptText = $"Press {engine.KeyPickUp} to pick up: {topItem.ToString()} - \"{topItem.Description}\"";

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
        int startY = dungeon.Rows + 6; 
    
        Console.SetCursorPosition(0, startY);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("=== CONTROLS ===".PadRight(80));
        Console.ResetColor();

        Console.SetCursorPosition(0, startY + 1);
        string row1 = $"[{engine.KeyMoveUp}/{engine.KeyMoveLeft}/{engine.KeyMoveDown}/{engine.KeyMoveRight}] Move\t";
        if (dungeon.HasPickups) row1 += $"[{engine.KeyPickUp}] Pick Up\t";
        if (dungeon.HasInventoryItems) row1 += $"[↑/↓] Select\t[{engine.KeyDrop}] Drop";
        Console.Write(row1.PadRight(80));
    
        Console.SetCursorPosition(0, startY + 2);
        string row2 = "";
        if (dungeon.HasInventoryItems)
        {
            row2 = $"[{engine.KeyEquip}] Equip\t[{engine.KeyFreeLeftHand}] Free L-Hand\t[{engine.KeyFreeRightHand}] Free R-Hand\t[{engine.KeyChangeStrategy}] Change Atk";
        }
        Console.Write(row2.PadRight(80));

        Console.SetCursorPosition(0, startY + 3);
        if (engine != null && !string.IsNullOrEmpty(engine.LastMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(engine.LastMessage.PadRight(100));
            Console.ResetColor();
        }
        else
        {
            Console.Write("".PadRight(100));
        }
    }

    private void RenderEquippedItems()
    {
        Console.SetCursorPosition(0, 1);
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("=== EQUIPPED WEAPONS ===".PadRight(50));
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
        int startX = dungeon.Columns + 5;

        Console.SetCursorPosition(startX, 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("=== WALLET ===".PadRight(30));
        Console.ResetColor();

        Console.SetCursorPosition(startX, 2);
        Console.Write($"Coins: {player.Coins}".PadRight(30));

        Console.SetCursorPosition(startX, 3);
        Console.Write($"Gold:  {player.Gold}".PadRight(30));
    }

    private void RenderPlayerAttributes()
    {
        int startX = dungeon.Columns + 35;

        Console.SetCursorPosition(startX, 1);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("=== ATTRIBUTES ===".PadRight(35));
        Console.ResetColor();

        Console.SetCursorPosition(startX, 2);
        Console.Write($"STR: {player.BaseStrength,-5} AGI: {player.BaseAgility}".PadRight(35));

        Console.SetCursorPosition(startX, 3);
        Console.Write($"WIS: {player.BaseWisdom,-5} LUK: {player.BaseLuck}".PadRight(35));

        Console.SetCursorPosition(startX, 4);
        Console.Write($"AGG: {player.Aggression,-5} HEL: {player.Health}".PadRight(35));
        
        Console.SetCursorPosition(startX, 5);
        Console.ForegroundColor = ConsoleColor.Cyan;
        string attackName = player.CurrentAttackStrategy.GetType().Name.Replace("Attack", "").ToUpper();
        Console.Write($"ATK MODE: {attackName}".PadRight(35));
        Console.ResetColor();
    }

    public void RenderUI()
    {
        RenderBoard();
        RenderActionPrompt();
        RenderPlayerBackpack();
        RenderEquippedItems();
        RenderPlayerStats();
        RenderPlayerAttributes();
        RenderControls();
    }
    public void RenderGameOverScreen()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        
        string[] gameOverArt = 
        {
            " __   __  _______  __   __    ______   ___   _______  ______  ",
            "|  | |  ||       ||  | |  |  |      | |   | |       ||      | ",
            "|  |_|  ||   _   ||  | |  |  |  _    ||   | |    ___||  _    |",
            "|       ||  | |  ||  |_|  |  | | |   ||   | |   |___ | | |   |",
            "|_     _||  |_|  ||       |  | |_|   ||   | |    ___|| |_|   |",
            "  |   |  |       ||       |  |       ||   | |   |___ |       |",
            "  |___|  |_______||_______|  |______| |___| |_______||______| "
        };

        int startY = 10;
        foreach (string line in gameOverArt)
        {
            Console.SetCursorPosition((120 - line.Length) / 2, startY++);
            Console.WriteLine(line);
        }

        Console.ResetColor();

        string statsMessage = $"You died, but you managed to collect {player.Coins} Coins and {player.Gold} Gold.";
        Console.SetCursorPosition((120 - statsMessage.Length) / 2, startY + 2);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(statsMessage);
        
        string exitMessage = "Press any key to exit the game...";
        Console.SetCursorPosition((120 - exitMessage.Length) / 2, startY + 4);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(exitMessage);
        
        Console.ResetColor();
    }
}