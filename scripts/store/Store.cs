using Godot;

/**
 TODO: Cada caminho deve ter um array de paths, um modelo do mapa e um "trilho"
 da camera
 */

public partial class Store : Node
{
    public static Store Instance { get; private set; }

    public PathManager PathManager { get; private set; } = new PathManager();

    public override void _Ready()
    {
        Instance = this;
    }
}
