using rouge_sharp;

namespace sharp_rouge.entities;

internal class Wall : GameObject 
{
    public Wall(Point position, IScreenSurface hostingSurface) : base(new ColoredGlyph(Color.LightGray, Color.Black, '#'), position, hostingSurface)
    {
    }

    public override bool Touched(GameObject source, Map map)
    {
        // if (source == map.UserControlledObject)
        // {
        //     map.UserControlledObject.Money += _value;
        //     map.RemoveMapObject(this);
        //     return true;
        // }

        return false;
    }
}