using Godot;
using System.Collections.Generic;

public partial class Game : Node3D
{
    private Path3D _path3D;

    public override void _Ready()
    {
        _path3D = GetNode<Path3D>("Path3D2");


        Store.Instance.PathManager.SetPaths(new Dictionary<string, Path3D>
        {
            { "start", _path3D }
        });
        Store.Instance.PathManager.SetCurrentPath("start");
    }
}
