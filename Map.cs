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

    private List<Rectangle> _rooms = [];


    public Map(int mapWidth, int mapHeight, int rooms)
    {
        _mapSurface = new ScreenSurface(mapWidth, mapHeight)
        {
            UseMouse = false
        };

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                var nextPos = new Point(x, y);
                var wall = new Wall(nextPos, _mapSurface);
                _mapObjects.Add(wall);
            }
        }

        // Dig out rooms
        var generatedRooms = 0;
        while (generatedRooms < rooms)
        {
            var Width = Game.Instance.Random.Next(4, 10);
            var Height = Game.Instance.Random.Next(6, 10);
            var startingPos = new Point(Game.Instance.Random.Next(0, _mapSurface.Surface.Width),
                                        Game.Instance.Random.Next(0, _mapSurface.Surface.Height));

            var startingRect = new Rectangle(startingPos.X, startingPos.Y, Width, Height);

            if (startingPos.X + Width > _mapSurface.Surface.Width ||
                    startingPos.Y + Height > _mapSurface.Surface.Height ||
                    _rooms.Any(r => r.Intersects(startingRect)))
            {
                continue;
            }

            for (int x = startingPos.X; x < startingPos.X + Width; x++)
            {
                for (int y = startingPos.Y; y < startingPos.Y + Height; y++)
                {
                    var foundObject = _mapObjects.Find(o => o.Position.X == x && o.Position.Y == y);

                    if (foundObject != null) RemoveMapObject(foundObject);
                }
            }

            _rooms.Add(startingRect);
            generatedRooms++;
        }

        // Now connect the rooms
        for (var r = 0; r < _rooms.Count; r++)
        {
            if (_rooms.Count == 1) continue;

            var rect1 = _rooms[r];
            var rect2 = _rooms[r];

            if (r == _rooms.Count - 1)
            {
                rect1 = _rooms[r - 1];
                rect2 = _rooms[r];
            }
            else
            {
                rect2 = _rooms[r + 1];
            }

            var startingPointX = rect1.Center.X < rect2.Center.X ? rect1.Center.X : rect2.Center.X;

            for (var x = startingPointX; x < startingPointX + Math.Abs(rect1.Center.X - rect2.Center.X); x++)
            {
                var foundObject = _mapObjects.Find(o => o.Position.X == x && o.Position.Y == rect1.Center.Y);
                RemoveMapObject(foundObject);
            }

            var startingY = rect1.Center.Y < rect2.Center.Y ? rect1.Center.Y : rect2.Center.Y;
            for (var y = startingY; y < startingY + Math.Abs(rect1.Center.Y - rect2.Center.Y); y++)
            {
                var foundObject = _mapObjects.Find(o => o.Position.Y == y && o.Position.X == rect2.Center.X);
                RemoveMapObject(foundObject);
            }
        }

        UserControlledObject = new Player(new ColoredGlyph(Color.White, Color.Black, 2), _rooms[0].Center, _mapSurface);

        CreateTreasure();
        // CreateMonster(room);

        UserControlledObject.Health = 10;
    }

    private void CreateTreasure()
    {
        for (int x = 0; x < _rooms.Count; x++)
        {
            Point randomPosition = new(Game.Instance.Random.Next(_rooms[x].X, _rooms[x].X + _rooms[x].Width),
                                        Game.Instance.Random.Next(_rooms[x].Y, _rooms[x].Y + _rooms[x].Height));

            bool foundObject = _mapObjects.Any(obj => obj.Position == randomPosition);
            if (foundObject) continue;

            var treasure = new Treasure(randomPosition, _mapSurface, 5);
            _mapObjects.Add(treasure);
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

    public void RemoveMapObject(GameObject? mapObject)
    {
        if (mapObject == null) return;

        if (_mapObjects.Contains(mapObject))
        {
            _mapObjects.Remove(mapObject);
            mapObject.RestoreMap(this);
        }
    }
}
