using System.Diagnostics.CodeAnalysis;
using rouge_sharp.entities;
using sharp_rouge.entities;

namespace rouge_sharp;

internal class Map 
{
    private List<GameObject> _mapObjects;
    private ScreenSurface _mapSurface;

    public IReadOnlyList<GameObject> GameObjects => _mapObjects.AsReadOnly();
    public ScreenSurface SurfaceObject => _mapSurface;

    public Player UserControlledObject { get; set; }


    public Map(int mapWidth, int mapHeight)
    {
        _mapObjects = [];

        _mapSurface = new ScreenSurface(mapWidth, mapHeight)
        {
            UseMouse = false
        };

        // GenerateRoom();
        var room = new Room(_mapSurface, _mapObjects);

        // FillBackground();

        UserControlledObject = new Player(new ColoredGlyph(Color.White, Color.Black, 2), _mapSurface.Surface.Area.Center, _mapSurface);

        CreateTreasure(room);
        // CreateMonster(room);

        UserControlledObject.Health = 10;
    }

    

    // private void FillBackground() {
    //     // Color[] colors = new[] { Color.LightGreen, Color.Coral, Color.CornflowerBlue, Color.DarkGreen };
    //     // float[] colorStops = new[] { 0f, 0.35f, 0.75f, 1f };

    //     // Algorithms.GradientFill(_mapSurface.FontSize,
    //     //                         _mapSurface.Surface.Area.Center,
    //     //                         _mapSurface.Surface.Width / 3,
    //     //                         45,
    //     //                         _mapSurface.Surface.Area,
    //     //                         new Gradient(colors, colorStops),
    //     //                         (x, y, color) => _mapSurface.Surface[x, y].Background = color);
    // }

    private void CreateTreasure(Room room)
    {
        for (int i = 0; i < 1000; i++) 
        {
            Point randomPosition = new (Game.Instance.Random.Next(room.startingPos.X, room.startingPos.X + room.Width),
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

            if (otherGameObject.Position == position && otherGameObjectType == typeof(Wall)) {
                gameObject = otherGameObject;
                return true;
            }
        }

        gameObject = null;
        return false;
    }

    public void RemoveMapObject(GameObject mapObject)
    {
        if (_mapObjects.Contains(mapObject))
        {
            _mapObjects.Remove(mapObject);
            mapObject.RestoreMap(this);
        }
    }
}
