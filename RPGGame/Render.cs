using RPGGame.Items;
using RPGGame.Logging;

namespace RPGGame;

public class Render(Dungeon dungeon, Player player, GameEngine engine)
{
    private void RenderEquippedItems()
    {
        int width = 45;
        Console.SetCursorPosition(0, 1);
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("=== EQUIPPED WEAPONS ===".PadRight(width));
        Console.ResetColor();

        Console.SetCursorPosition(0, 2);
        string leftHandText = player.LeftHand != null ? player.LeftHand.ToString() : "Empty";
        string line1 = $"Left Hand:  {leftHandText}";
        if (line1.Length > width) line1 = line1.Substring(0, width - 3) + "...";
        Console.Write(line1.PadRight(width));

        Console.SetCursorPosition(0, 3);
        string rightHandText = player.RightHand != null ? player.RightHand.ToString() : "Empty";
        string line2 = $"Right Hand: {rightHandText}";
        if (line2.Length > width) line2 = line2.Substring(0, width - 3) + "...";
        Console.Write(line2.PadRight(width));
        
        Console.SetCursorPosition(0, 4); Console.Write(new string(' ', width));
        Console.SetCursorPosition(0, 5); Console.Write(new string(' ', width));
    }

    private void RenderPlayerStats()
    {
        int startX = 48;
        int width = 27;

        Console.SetCursorPosition(startX, 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("=== WALLET ===".PadRight(width));
        Console.ResetColor();

        int y = 2;
        foreach (var currency in player.Wallet)
        {
            if (y > 5) break; 
            
            Console.SetCursorPosition(startX, y++);
            string text = $"{currency.Key}: {currency.Value}";
            if (text.Length > width) text = text.Substring(0, width - 3) + "...";
            Console.Write(text.PadRight(width));
        }

        for (; y <= 5; y++)
        {
            Console.SetCursorPosition(startX, y);
            Console.Write(new string(' ', width));
        }
    }

    private void RenderPlayerAttributes()
    {
        int startX = 78;
        int width = 42;

        Console.SetCursorPosition(startX, 1);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("=== ATTRIBUTES ===".PadRight(width));
        Console.ResetColor();

        Console.SetCursorPosition(startX, 2);
        Console.Write($"STR: {player.BaseStrength,-5} AGI: {player.BaseAgility}".PadRight(width));

        Console.SetCursorPosition(startX, 3);
        Console.Write($"WIS: {player.Wisdom,-5} LUK: {player.Luck}".PadRight(width));

        Console.SetCursorPosition(startX, 4);
        Console.Write($"AGG: {player.Aggression,-5} HEL: {player.Health}".PadRight(width));
        
        Console.SetCursorPosition(startX, 5);
        Console.ForegroundColor = ConsoleColor.Cyan;
        string attackName = player.CurrentAttackStrategy.GetType().Name.Replace("Attack", "").ToUpper();
        Console.Write($"ATK MODE: {attackName}".PadRight(width));
        Console.ResetColor();
    }

    private void RenderPlayerBackpack()
    {
        int startX = 42;
        int startY = 6; 
        int columnWidth = 31;

        Console.SetCursorPosition(startX, startY);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("=== BACKPACK ===".PadRight(columnWidth));
        Console.ResetColor();

        var items = player.Backpack.Items;
        int currentY = startY + 1;

        if (items.Count == 0)
        {
            Console.SetCursorPosition(startX, currentY++);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Empty...".PadRight(columnWidth));
            Console.ResetColor();
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                Console.SetCursorPosition(startX, currentY++);
                bool isSelected = (i == player.Backpack.SelectedItemIndex);
                if (isSelected) { Console.BackgroundColor = ConsoleColor.White; Console.ForegroundColor = ConsoleColor.Black; }

                string itemText = items[i].ToString();
                if (itemText.Length > columnWidth) 
                {
                    itemText = itemText.Substring(0, columnWidth - 3) + "...";
                }
                
                Console.Write(itemText.PadRight(columnWidth));
                Console.ResetColor();
            }
        }

        for (int i = currentY; i < startY + dungeon.Rows; i++)
        {
            Console.SetCursorPosition(startX, i);
            Console.Write(new string(' ', columnWidth));
        }
    }

    private void RenderBoard()
    {
        for (int i = 0; i < dungeon.Rows; i++)
        {
            Console.SetCursorPosition(0, 6 + i);
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
    private void RenderLatestLogs()
    {
        int startX = 75;
        int startY = 7; 
        int width = 43; 
        int maxLines = dungeon.Rows - 1; 

        var allWrappedLines = new List<string>();
        var rawLogs = EventLogger.Instance.GetLogs();

        foreach (var log in rawLogs)
        {
            allWrappedLines.AddRange(WrapText(log, width));
        }

        var linesToShow = allWrappedLines.TakeLast(maxLines).ToList();

        Console.SetCursorPosition(startX, startY - 1);
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write("=== EVENT LOG ===".PadRight(width));
        Console.ResetColor();

        for (int i = 0; i < maxLines; i++)
        {
            Console.SetCursorPosition(startX, startY + i);
            if (i < linesToShow.Count)
                Console.Write(linesToShow[i].PadRight(width));
            else
                Console.Write(new string(' ', width));
        }
    }

    private List<string> WrapText(string text, int width)
    {
        List<string> lines = new List<string>();
        string[] words = text.Split(' ');
        string currentLine = "";

        foreach (var word in words)
        {
            if ((currentLine + word).Length > width)
            {
                if (!string.IsNullOrEmpty(currentLine))
                    lines.Add(currentLine.TrimEnd());
                currentLine = word + " ";
            }
            else
            {
                currentLine += word + " ";
            }
        }

        if (!string.IsNullOrEmpty(currentLine))
            lines.Add(currentLine.TrimEnd());

        return lines;
    }

    private void RenderActionPrompt()
    {
        int promptY = 6 + dungeon.Rows; 
        Console.SetCursorPosition(0, promptY);

        var itemsUnderPlayer = dungeon[player.Y, player.X].ItemsOnGround;
        if (itemsUnderPlayer.Count > 0)
        {
            Item topItem = itemsUnderPlayer.Peek();
            Console.ForegroundColor = ConsoleColor.Yellow;
            string promptText = $"Press {engine.KeyPickUp} to pick up: {topItem.ToString()} - \"{topItem.Description}\"";
            if (promptText.Length > 119) promptText = promptText.Substring(0, 119);
            Console.Write(promptText.PadRight(120));
            Console.ResetColor();
        }
        else
        {
            Console.Write(new string(' ', 120));
        }
    }

    private void RenderControls()
    {
        int startY = 6 + dungeon.Rows + 1; 
    
        for(int i = 0; i < 3; i++) 
        {
            Console.SetCursorPosition(0, startY + i);
            Console.Write(new string(' ', 120));
        }

        Console.SetCursorPosition(0, startY);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("=== CONTROLS ===".PadRight(120));
        Console.ResetColor();

        Console.SetCursorPosition(0, startY + 1);
        string row1 = $"[{engine.KeyMoveUp}/{engine.KeyMoveLeft}/{engine.KeyMoveDown}/{engine.KeyMoveRight}] Move   ";
        if (dungeon.HasPickups) row1 += $"[{engine.KeyPickUp}] Pick Up   ";
        if (dungeon.HasInventoryItems) row1 += $"[UP/DOWN] Select   [{engine.KeyDrop}] Drop   ";
        row1 += $"[{engine.KeyViewLogs}] View Logs";
        Console.Write(row1.PadRight(120));
    
        Console.SetCursorPosition(0, startY + 2);
        string row2 = "";
        if (dungeon.HasInventoryItems)
            row2 = $"[{engine.KeyEquip}] Equip   [{engine.KeyFreeLeftHand}] Free L-Hand   [{engine.KeyFreeRightHand}] Free R-Hand   [{engine.KeyChangeStrategy}] Change Atk";
        Console.Write(row2.PadRight(120));
    }

    public void RenderFullLogsScreen()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("=== FULL EVENT LOG ===");
        Console.ResetColor();
        Console.WriteLine();
        var logs = EventLogger.Instance.GetLogs().ToList();
        if (logs.Count == 0) Console.WriteLine("No events recorded yet.");
        else foreach (var log in logs) Console.WriteLine(log);
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Press any key to return to the game...");
        Console.ResetColor();
        Console.ReadKey(true);
        Console.Clear();
    }

    public void RenderUI()
    {
        Console.CursorVisible = false;
        RenderEquippedItems();
        RenderPlayerStats();
        RenderPlayerAttributes();
        RenderBoard();
        RenderPlayerBackpack();
        RenderLatestLogs();
        RenderActionPrompt();
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

        string walletContents = string.Join(" and ", player.Wallet.Select(kvp => $"{kvp.Value} {kvp.Key}"));
        if (string.IsNullOrEmpty(walletContents)) walletContents = "nothing";

        string statsMessage = $"You died, but you managed to collect {walletContents}.";
        Console.SetCursorPosition((120 - statsMessage.Length) / 2, startY + 2);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(statsMessage);

        string logMessage = $"Your adventure was recorded in: {Config.ConfigManager.Instance.Config.LogDirectory}";
        Console.SetCursorPosition((120 - logMessage.Length) / 2, startY + 4);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(logMessage);
        
        string exitMessage = "Press any key to exit the game...";
        Console.SetCursorPosition((120 - exitMessage.Length) / 2, startY + 6);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(exitMessage);
        
        Console.ResetColor();
    }
}