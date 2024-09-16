using System;
using System.Diagnostics.CodeAnalysis;
using rouge_sharp;
using SadConsole.Input;

namespace sharp_rouge;

class RootScreen : ScreenObject
{
    private Map _map;

    public RootScreen()
    {
        _map = new Map(Game.Instance.ScreenCellsX, Game.Instance.ScreenCellsY - 5);

        Children.Add(_map.SurfaceObject);
    }

    public override void Render(TimeSpan delta)
    {
        base.Render(delta);

          _map.SurfaceObject.Surface.Print(1, 1, $"Health: {_map.UserControlledObject.Health}", Color.White);
          _map.SurfaceObject.Surface.Print(1, 2, $"Money: {_map.UserControlledObject.Money}", Color.Yellow);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        bool handled = false;

        if (keyboard.IsKeyPressed(Keys.Up))
        {
            _map.UserControlledObject.Move(_map.UserControlledObject.Position + Direction.Up, _map);
            handled = true;
        }
        else if (keyboard.IsKeyPressed(Keys.Down))
        {
            _map.UserControlledObject.Move(_map.UserControlledObject.Position + Direction.Down, _map);
            handled = true;
        }

        if (keyboard.IsKeyPressed(Keys.Left))
        {
            _map.UserControlledObject.Move(_map.UserControlledObject.Position + Direction.Left, _map);
            handled = true;
        }
        else if (keyboard.IsKeyPressed(Keys.Right))
        {
            _map.UserControlledObject.Move(_map.UserControlledObject.Position + Direction.Right, _map);
            handled = true;
        }

        if (keyboard.IsKeyPressed(Keys.Escape)) {
            // Todo: at some point this will pause/go to a menu, but for now close
            Environment.Exit(0);
        }

        return handled;
    }
}
