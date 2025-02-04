using Godot;
using System.Collections.Generic;

#nullable enable

/**
 * TODO: Cada caminho deve ter um nome único, ou seja, precisa ser um dicionário
 que não tenha chave duplicada

 * TODO: Um path precisa ter um starting point padrão

 * TODO: Um path precisa ter um "local" que o player chega a partir de onde ele veio
 preciso pensar melhor em como definir isso, se vai ser no path, no mapa e etc. Talvez o
 melhor modo de fazer isso é, para cada "porta" temos a referencia de um path e uma posiçào nesse path

 * TODO: Se eu tenho um path com alguma ramificação, como eu defino que o player vai para uma ou outra
 pensando no modelo de "porta" referenciar um path e um ponto no path? Pensei que poderia cada "mapa"
 ter um grupo de paths num array, ai eu poderia especificar o mapa, o path e o ponto no path
 */

public class PathManager
{
    private Dictionary<string, Path3D> paths = new Dictionary<string, Path3D>();

    private string? current = null;

    public void SetPaths(Dictionary<string, Path3D> newPaths)
    {
        paths = newPaths;
    }

    public void SetCurrentPath(string pathName)
    {
        if (paths.ContainsKey(pathName))
        {
            current = pathName;
        }
        else
        {
            GD.PushError("Path not defined");
        }
    }

    public Path3D? FindPath(string name)
    {
        return paths.ContainsKey(name) ? paths[name] : null;
    }

    public void ClearPaths()
    {
        paths.Clear();
    }
}
