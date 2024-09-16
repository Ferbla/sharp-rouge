
namespace rouge_sharp.entities;

internal class Player : GameObject
{
    public int Money { get; set; }


    public Player(ColoredGlyph appearance, Point positioin, IScreenSurface hostingSurface) : base(appearance, positioin, hostingSurface)
    {
    }
}
