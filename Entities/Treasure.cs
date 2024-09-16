
namespace rouge_sharp.entities;

internal class Treasure : GameObject
{
    private int _value = 1;

    public Treasure(Point position, IScreenSurface hostingSurface, int value) : base(new ColoredGlyph(Color.Yellow, Color.Black, 'C'), position, hostingSurface)
    {
        _value = value;
    }

    public override bool Touched(GameObject source, Map map)
    {
        if (source == map.UserControlledObject)
        {
            map.UserControlledObject.Money += _value;
            map.RemoveMapObject(this);
            return true;
        }

        return false;
    }
}