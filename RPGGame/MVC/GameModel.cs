namespace RPGGame.MVC;

using System.Collections.Generic;
using RPGGame.Logging;
using RPGGame.Items;

public class GameModel
{
    public Dungeon Dungeon { get; }
    public Dictionary<int, Player> Players { get; }
    public bool IsGameOver { get; set; }
    public string WelcomeMessage { get; set; }

    private readonly List<IView> _views;
    public readonly object StateLock = new object();

    public GameModel(Dungeon dungeon)
    {
        Dungeon = dungeon;
        Players = new Dictionary<int, Player>();
        _views = new List<IView>();
        IsGameOver = false;
    }
    
    public void ResolveCombat(Player player, Enemy enemy, Cell cellWithEnemy)
    {
        lock (StateLock)
        {
            var attack = player.CurrentAttackStrategy;

            int leftHandDamage = player.LeftHand != null 
                ? player.LeftHand.CalculateDamageWith(attack, player) + player.LeftHand.GetDamageModifier()
                : 0;

            int rightHandDamage = (player.RightHand != null && player.RightHand != player.LeftHand) 
                ? player.RightHand.CalculateDamageWith(attack, player) + player.RightHand.GetDamageModifier() 
                : 0;

            int totalPlayerDamage = leftHandDamage + rightHandDamage;
            int damageDealtToEnemy = Math.Max(0, totalPlayerDamage - enemy.Armor);
            enemy.Health -= damageDealtToEnemy;

            EventLogger.Instance.Log($"{player.PlayerName} attacked {enemy.Name} dealing {damageDealtToEnemy} damage.");

            if (enemy.Health <= 0)
            {
                EventLogger.Instance.Log($"{player.PlayerName} defeated the {enemy.Name}.");
                cellWithEnemy.Enemy = null;
                Dungeon.ActiveEnemies.Remove(enemy);
                player.DetachSoundObserver(enemy);
                enemy.Die(); 
                
                NotifyViews();
                return;
            }

            int leftHandDefense = player.LeftHand != null
                ? player.LeftHand.CalculateDefenseWith(attack, player)
                : attack.CalculateDefense((Item)null, player);
                
            int rightHandDefense = (player.RightHand != null && player.RightHand != player.LeftHand)
                ? player.RightHand.CalculateDefenseWith(attack, player)
                : 0;

            if (player.LeftHand == null && player.RightHand == null)
            {
                leftHandDefense = attack.CalculateDefense((Item)null, player);
            }

            int totalPlayerDefense = leftHandDefense + rightHandDefense;
            int damageDealtToPlayer = Math.Max(0, enemy.Attack - totalPlayerDefense);
            player.Health -= damageDealtToPlayer;

            EventLogger.Instance.Log($"Enemy {enemy.Name} attacked {player.PlayerName} dealing {damageDealtToPlayer} damage.");

            if (player.Health <= 0)
            {
                EventLogger.Instance.Log($"{player.PlayerName} has died.");
            }

            NotifyViews();
        }
    }

    public void TickEnemies()
    {
        lock (StateLock)
        {
            foreach (var enemy in Dungeon.ActiveEnemies.ToList())
            {
                if (enemy.Health <= 0) continue;

                int oldX = enemy.X;
                int oldY = enemy.Y;

                enemy.PerformMove(Dungeon);

                var playerToAttack = Players.Values.FirstOrDefault(p => p.X == enemy.X && p.Y == enemy.Y);

                if (playerToAttack != null)
                {
                    Dungeon[enemy.Y, enemy.X].Enemy = null;
                    enemy.X = oldX;
                    enemy.Y = oldY;
                    Dungeon[oldY, oldX].Enemy = enemy;

                    ResolveCombat(playerToAttack, enemy, Dungeon[oldY, oldX]);
                }
            }
            NotifyViews();
        }
    }

    public void AddPlayer(int id, Player player)
    {
        lock (StateLock)
        {
            Players[id] = player;
        }
        NotifyViews();
    }

    public void RemovePlayer(int id)
    {
        lock (StateLock)
        {
            Players.Remove(id);
        }
        NotifyViews();
    }

    public void AttachView(IView view)
    {
        lock (StateLock)
        {
            if (!_views.Contains(view))
            {
                _views.Add(view);
            }
        }
    }

    public void DetachView(IView view)
    {
        lock (StateLock)
        {
            _views.Remove(view);
        }
    }

    public void NotifyViews()
    {
        lock (StateLock)
        {
            foreach (var view in _views)
            {
                view.Render(this, -1);
            }
        }
    }
}