
public class GameMessage
{
    public string Result { get; set; }
}
public class Game : IObservable<GameMessage>
{
    private List<IObserver<GameMessage>> Players;
    private Dictionary<int , int> _items = new Dictionary<int , int>
    {
        { 0, 3 },
        { 1, 5 },
        { 2, 7 },
    };

    public Game ()
    {
        Players = new List<IObserver<GameMessage>>();
    }

    public IDisposable Subscribe (IObserver<GameMessage> player)
    {
        Players.Add(player);
        return new UnSubscribe(Players , player);
    }
    private void ChangeCanTake (Player player)
    {
        Players.ForEach(s =>
        {
            if (s != player)
            {
                ((Player)s).CanTake = player.CanTake;
            }
        });
        player.CanTake = !player.CanTake;

    }
    private void GameOver ()
    {

        foreach (var item in Players)
        {
            item.OnCompleted();
        }
    }
    public void TakeAndNotify (Player player , int row , int count)
    {
        if (player.CanTake)
        {
            var rowitemcount = _items [ row ];
            if (rowitemcount > 0 && count > 0)
            {
                if (rowitemcount >= count)
                {
                    _items [ row ] = rowitemcount - count;
                    player.OnNext(new GameMessage { Result = $"hi {player.Name} You Take:{count} from {row} row, this row still leave {_items [ row ]}。" });
                }
                else
                {
                    //当前行拿取数量大于剩余数量，则全部拿走
                    player.OnNext(new GameMessage { Result = $"hi {player.Name} You Take:{rowitemcount} from {row} row,no more items!" });
                    _items [ row ] = 0;
                }
                ChangeCanTake(player);
            }
            else
            {
                if (rowitemcount == 0)
                {
                    player.OnNext(new GameMessage { Result = $"hi {player.Name} row: {row} no more items , please retry! " });
                    return;
                }
                if (count <= 0)
                {
                    player.OnNext(new GameMessage { Result = $"hi {player.Name} don't cheat , please retry!" });
                    return;
                }
            }



            var sum = _items.AsQueryable().Count(s => s.Value > 0);

            if (sum == 0)
            {
                Players.ForEach(s =>
                {
                    if ((Player)s != player)
                    {
                        s.OnNext(new GameMessage { Result = $"HI {((Player)s).Name} YOU ARE WiN!" });
                    }
                });

                player.OnNext(new GameMessage { Result = $"HI {player.Name} YOU ARE LOST!" });

                GameOver();
            }
        }
        else
        {
            player.OnNext(new GameMessage { Result = $"hi {player.Name} It's not your turn!" });
        }
    }

    public class UnSubscribe : IDisposable
    {
        List<IObserver<GameMessage>> _Players;
        IObserver<GameMessage> _player;
        public UnSubscribe (List<IObserver<GameMessage>> Players , IObserver<GameMessage> player)
        {
            _Players = Players;
            _player = player;
        }
        public void Dispose ()
        {
            if (_Players.Contains(_player))
            {
                _Players.Remove(_player);
            }
        }
    }
}
public class Player : IObserver<GameMessage>
{
    private Game.UnSubscribe exitor;
    private Game _game;
    public bool CanTake { get; set; } = true;
    public Player (string name)
    {
        Name = name;
    }

    public void Exit ()
    {
        exitor.Dispose();
        Console.WriteLine($"{Name} exit game!");
    }
    public void JoinGame (Game game)
    {
        if (_game == null)
        {
            exitor = (Game.UnSubscribe)game.Subscribe(this);
            _game = game;

            Console.WriteLine($"{Name} join game!");
        }
    }

    public void TakeItems (int row , int count)
    {
        if (_game != null)
        {
            _game.TakeAndNotify(this , row , count);
        }
    }

    public string Name { get; set; }

    public void OnCompleted ()
    {
        _game = null;
        Console.WriteLine($"Hi {Name}, Game Over!");
    }

    public void OnError (Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext (GameMessage GameMessage)
    {
        Console.WriteLine(GameMessage.Result);
    }
}