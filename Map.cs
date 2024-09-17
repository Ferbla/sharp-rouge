using System.Diagnostics.CodeAnalysis;
using rouge_sharp.entities;
using sharp_rouge.entities;

namespace rouge_sharp;

internal class Map
{
    private List<GameObject> _mapObjects = [];
    private ScreenSurface _mapSurface;

    public IReadOnlyList<GameObject> GameObjects => _mapObjects.AsReadOnly();
    public ScreenSurface SurfaceObject => _mapSurface;

    public Player UserControlledObject { get; set; }


    public Map(int mapWidth, int mapHeight)
    {
        _mapSurface = new ScreenSurface(mapWidth, mapHeight)
        {
            UseMouse = false
        };

        var room = new Room(_mapSurface, _mapObjects, true);
        var room2 = new Room(_mapSurface, _mapObjects, false);

        ConnectRooms(room, room2);

        UserControlledObject = new Player(new ColoredGlyph(Color.White, Color.Black, 2), _mapSurface.Surface.Area.Center, _mapSurface);

        CreateTreasure(room);
        // CreateMonster(room);

        UserControlledObject.Health = 10;
    }

    private void ConnectRooms(Room room1, Room room2)
    {
        var rect1 = new Rectangle(room1.startingPos.X,
                                  room1.startingPos.Y,
                                  room1.Width,
                                  room1.Height);


        var rect2 = new Rectangle(room2.startingPos.X,
                                  room2.startingPos.Y,
                                  room2.Width,
                                  room2.Height);

        for (int x = Math.Min(rect1.Center.X, rect2.Center.X); x <= Math.Max(rect1.Center.X, rect2.Center.X); x++)
        {

            _mapObjects.Add(new Wall(new Point(x, rect1.Center.Y + 1), _mapSurface));
            RemoveMapObject(_mapObjects.Find(o => o.Position.X == x && o.Position.Y == rect1.Center.Y));
            _mapObjects.Add(new Wall(new Point(x, rect1.Center.Y - 1), _mapSurface));
        }

        for (int y = Math.Min(rect1.Center.Y, rect2.Center.Y); y <= Math.Max(rect1.Center.Y, rect2.Center.Y); y++)
        {
            _mapObjects.Add(new Wall(new Point(rect2.Center.X + 1, y), _mapSurface));
            // RemoveMapObject(_mapObjects.Find(x => x.Position.X == r))
            _mapObjects.Add(new Wall(new Point(rect2.Center.Y - 1, y), _mapSurface));

        }
    }

    private void CreateTreasure(Room room)
    {
        for (int i = 0; i < 1000; i++)
        {
            Point randomPosition = new(Game.Instance.Random.Next(room.startingPos.X, room.startingPos.X + room.Width),
                                        Game.Instance.Random.Next(room.startingPos.Y, room.startingPos.Y + room.Height));

            bool foundObject = _mapObjects.Any(obj => obj.Position == randomPosition);
            if (foundObject) continue;

            var treasure = new Treasure(randomPosition, _mapSurface, 5);
            _mapObjects.Add(treasure);
            break;
        }
    }

    // private void CreateMonster(Room room)
    // {
    //     // Try 1000 times to get an empty map position
    //     for (int i = 0; i < 1000; i++)
    //     {
    //         // Get a random position
    //         Point randomPosition = new Point(Game.Instance.Random.Next(0, _mapSurface.Surface.Width),
    //                                             Game.Instance.Random.Next(0, _mapSurface.Surface.Height));

    //         // Check if any object is already positioned there, repeat the loop if found
    //         bool foundObject = _mapObjects.Any(obj => obj.Position == randomPosition);
    //         if (foundObject || !room.IsInRoom(randomPosition)) continue;

    //         // If the code reaches here, we've got a good position, create the game object.
    //         var monster = new Monster(new ColoredGlyph(Color.Red, Color.Black, 'M'), randomPosition, _mapSurface);
    //         _mapObjects.Add(monster);
    //         break;
    //     }
    // }

    public bool TryGetMapObject(Point position, [NotNullWhen(true)] out GameObject? gameObject)
    {
        foreach (var otherGameObject in _mapObjects)
        {
            var otherGameObjectType = otherGameObject.GetType();

            if (otherGameObject.Position == position && otherGameObjectType == typeof(Treasure))
            {
                gameObject = otherGameObject;
                return true;
            }

            if (otherGameObject.Position == position && otherGameObjectType == typeof(Monster))
            {
                gameObject = otherGameObject;

                UserControlledObject.Health -= 1;
                return true;
            }

            if (otherGameObject.Position == position && otherGameObjectType == typeof(Wall))
            {
                gameObject = otherGameObject;
                return true;
            }
        }

        gameObject = null;
        return false;
    }

    public void RemoveMapObject(GameObject mapObject)
    {
        if (mapObject == null) return;

        if (_mapObjects.Contains(mapObject))
        {
            _mapObjects.Remove(mapObject);
            mapObject.RestoreMap(this);
        }
    }
}
