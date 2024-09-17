
using sharp_rouge.entities;

namespace rouge_sharp.entities;

internal class Room
{
    public int Width {get; private set;}
    public int Height {get ; private set;}
    public Point startingPos { get; private set; }

    private IScreenSurface _hostingSurface;
    private List<GameObject> _mapObjects;

    private List<Point> _wallPoints = [];

    public Room(IScreenSurface hostingSurface, List<GameObject> mapObjects)
    {
        _hostingSurface = hostingSurface;
        _mapObjects = mapObjects;

        GenerateRoom();
    }

    // public bool IsInRoom(Point point) 
    // {
    // TODO might be nice to quickly call something to know if in room (shrug)
    // }

    private void GenerateRoom()
    {
        Width = Game.Instance.Random.Next(8, 15);
        Height = Game.Instance.Random.Next(10, 15);

        // TODO: Would be nice limit looping if possible (just did it to start getting ideas on the page)
        // Todo: Also should modify code so it can handle generating multiple rooms

        for (int i = 0; i < 1000; i++) 
        {
            startingPos = new (Game.Instance.Random.Next(0, _hostingSurface.Surface.Width),
                                        Game.Instance.Random.Next(0, _hostingSurface.Surface.Height));

            if (startingPos.X + Width > _hostingSurface.Surface.Width ||
                startingPos.Y + Height > _hostingSurface.Surface.Height) {
                    continue;
                }

            if (startingPos.X > _hostingSurface.Surface.Area.Center.X || 
                startingPos.X + Width < _hostingSurface.Surface.Area.Center.X ||
                startingPos.Y > _hostingSurface.Surface.Area.Center.Y ||
                startingPos.Y + Height < _hostingSurface.Surface.Area.Center.Y) {
                    continue;
                }

            // Found starting point, now to make sure our shape fits and isn't hitting anything
            for (var x = startingPos.X; x < startingPos.X + Width; x++) {
                for (var y = startingPos.Y; y < startingPos.Y + Height; y++) {
                    var nextPos = new Point(x, y);
                    if (_mapObjects.Any(obj => obj.Position == nextPos)) {
                        continue;
                    }
                }
            }

            // Generate walls now after validation complete
            for (var x = startingPos.X; x < startingPos.X + Width; x++) {
                for (var y = startingPos.Y; y < startingPos.Y + Height; y++) {
                    var nextPos = new Point(x, y);
                    
                    if (nextPos.X == startingPos.X || nextPos.X == startingPos.X + Width - 1 ||
                        nextPos.Y == startingPos.Y || nextPos.Y == startingPos.Y + Height -1)
                    {
                        var wall = new Wall(nextPos, _hostingSurface);
                        _mapObjects.Add(wall);
                        _wallPoints.Add(nextPos);
                    }
                }
            }
            break;
        }
    }

}