namespace RPGGame.MVC.Controllers;

using System;
using RPGGame.Network.DTOs;

public class InputController : IController
{
    private readonly int _localPlayerId;
    private readonly Action<CommandDTO> _onCommandGenerated;
    private readonly Action _onViewLogsRequested;

    public InputController(int localPlayerId, Action<CommandDTO> onCommandGenerated, Action onViewLogsRequested)
    {
        _localPlayerId = localPlayerId;
        _onCommandGenerated = onCommandGenerated;
        _onViewLogsRequested = onViewLogsRequested;
    }

    public void ProcessInput()
    {
        if (Console.KeyAvailable)
        {
            var keyInfo = Console.ReadKey(true);
            string cmdType = null;

            switch (keyInfo.Key)
            {
                case ConsoleKey.W: cmdType = "MOVE_UP"; break;
                case ConsoleKey.S: cmdType = "MOVE_DOWN"; break;
                case ConsoleKey.A: cmdType = "MOVE_LEFT"; break;
                case ConsoleKey.D: cmdType = "MOVE_RIGHT"; break;
                case ConsoleKey.E: cmdType = "PICKUP"; break;
                case ConsoleKey.Q: cmdType = "EQUIP"; break;
                case ConsoleKey.T: cmdType = "DROP"; break;
                case ConsoleKey.L: cmdType = "FREE_LEFT"; break;
                case ConsoleKey.R: cmdType = "FREE_RIGHT"; break;
                case ConsoleKey.Z: cmdType = "CYCLE_STRATEGY"; break;
                case ConsoleKey.UpArrow: cmdType = "INV_UP"; break;
                case ConsoleKey.DownArrow: cmdType = "INV_DOWN"; break;
                case ConsoleKey.J: 
                    _onViewLogsRequested(); 
                    break;
            }

            if (cmdType != null)
            {
                _onCommandGenerated(new CommandDTO 
                { 
                    PlayerId = _localPlayerId, 
                    CommandType = cmdType 
                });
            }
        }
    }
}