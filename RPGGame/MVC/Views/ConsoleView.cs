namespace RPGGame.MVC.Views;

using System;
using System.Linq;
using RPGGame.Network.DTOs;

public class ConsoleView
{
    public void Render(GameStateDTO state, int localPlayerId)
    {
        if (!state.Players.TryGetValue(localPlayerId, out var player)) return;

        Console.CursorVisible = false;
        RenderEquippedItems(player);
        RenderPlayerStats(player);
        RenderPlayerAttributes(player);
        RenderBoard(state, localPlayerId);
        RenderPlayerBackpack(player, state.Rows);
        RenderLatestLogs(state);
        RenderActionPrompt(state, player);
        RenderControls(state);
    }
    public void RenderWelcomeScreen(string message)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== WELCOME TO THE DUNGEON ===");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine(message);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Press any key to begin your adventure...");
        Console.ResetColor();
        Console.ReadKey(true);
        Console.Clear();
    }
    private void RenderEquippedItems(PlayerDTO player)
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

    private void RenderPlayerStats(PlayerDTO player)
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

    private void RenderPlayerAttributes(PlayerDTO player)
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
        Console.Write($"ATK MODE: {player.AttackStrategyName}".PadRight(width));
        Console.ResetColor();
    }

    private void RenderPlayerBackpack(PlayerDTO player, int dungeonRows)
    {
        int startX = 42;
        int startY = 6; 
        int columnWidth = 31;

        Console.SetCursorPosition(startX, startY);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("=== BACKPACK ===".PadRight(columnWidth));
        Console.ResetColor();

        var items = player.Backpack;
        int currentY = startY + 1;

        if (items == null || items.Count == 0)
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
                bool isSelected = (i == player.SelectedItemIndex);
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

        for (int i = currentY; i < startY + dungeonRows; i++)
        {
            Console.SetCursorPosition(startX, i);
            Console.Write(new string(' ', columnWidth));
        }
    }

    private void RenderBoard(GameStateDTO state, int localPlayerId)
    {
        for (int i = 0; i < state.Rows; i++)
        {
            Console.SetCursorPosition(0, 6 + i);
            for (int j = 0; j < state.Columns; j++)
            {
                var playerOnCell = state.Players.Values.FirstOrDefault(p => p.X == j && p.Y == i);
                
                if (playerOnCell != null)
                {
                    if (playerOnCell.Id == localPlayerId)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write('¶');
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(playerOnCell.Id);
                    }
                }
                else
                {
                    var enemyOnCell = state.Enemies.FirstOrDefault(e => e.X == j && e.Y == i);
                    if (enemyOnCell != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(enemyOnCell.Symbol);
                    }
                    else
                    {
                        var cell = state.Board[i][j];
                        Console.ForegroundColor = (ConsoleColor)cell.ForegroundColor;
                        Console.Write(cell.Symbol);
                    }
                }
            }
        }
    }

    private void RenderLatestLogs(GameStateDTO state)
    {
        int startX = 75;
        int startY = 7; 
        int width = 43; 
        int maxLines = state.Rows - 1; 

        var allWrappedLines = new List<string>();
        if (state.LatestLogs != null)
        {
            foreach (var log in state.LatestLogs)
            {
                allWrappedLines.AddRange(WrapText(log, width));
            }
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

    private void RenderActionPrompt(GameStateDTO state, PlayerDTO player)
    {
        int promptY = 6 + state.Rows; 
        Console.SetCursorPosition(0, promptY);

        var cell = state.Board[player.Y][player.X];
        
        if (cell.TopItemOnGround != null)
        {
            ItemDTO topItem = cell.TopItemOnGround;
            Console.ForegroundColor = ConsoleColor.Yellow;
            string promptText = $"Press E to pick up: {topItem.ToString()} - \"{topItem.Description}\"";
            if (promptText.Length > 119) promptText = promptText.Substring(0, 119);
            Console.Write(promptText.PadRight(120));
            Console.ResetColor();
        }
        else
        {
            Console.Write(new string(' ', 120));
        }
    }

    private void RenderControls(GameStateDTO state)
    {
        int startY = 6 + state.Rows + 1; 
    
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
        string row1 = $"[W/A/S/D] Move   ";
        if (state.HasPickups) row1 += $"[E] Pick Up   ";
        if (state.HasInventoryItems) row1 += $"[UP/DOWN] Select   [T] Drop   ";
        row1 += $"[J] View Logs";
        Console.Write(row1.PadRight(120));
    
        Console.SetCursorPosition(0, startY + 2);
        string row2 = "";
        if (state.HasInventoryItems)
            row2 = $"[Q] Equip   [L] Free L-Hand   [R] Free R-Hand   [Z] Change Atk";
        Console.Write(row2.PadRight(120));
    }
    public void RenderFullLogsScreen(GameStateDTO state)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("=== FULL EVENT LOG ===");
        Console.ResetColor();
        Console.WriteLine();
        
        if (state.LatestLogs == null || state.LatestLogs.Count == 0) 
            Console.WriteLine("No events recorded yet.");
        else 
            foreach (var log in state.LatestLogs) Console.WriteLine(log);
            
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Press any key to return to the game...");
        Console.ResetColor();
        Console.ReadKey(true);
        Console.Clear();
    }

    public void RenderGameOverScreen(GameStateDTO state, int localPlayerId)
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

        if (state.Players.TryGetValue(localPlayerId, out var player))
        {
            string walletContents = string.Join(" and ", player.Wallet.Select(kvp => $"{kvp.Value} {kvp.Key}"));
            if (string.IsNullOrEmpty(walletContents)) walletContents = "nothing";

            string statsMessage = $"You died, but you managed to collect {walletContents}.";
            Console.SetCursorPosition((120 - statsMessage.Length) / 2, startY + 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(statsMessage);
        }

        string logMessage = "Your adventure was recorded in the Logs directory";
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