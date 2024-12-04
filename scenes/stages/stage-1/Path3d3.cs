using Godot;
using System;

public partial class Path3d3 : Path3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// GD.Print("Path3d2", Curve.GetPointPosition(6));
        // GD.Print("Path3d2global", ToGlobal(Curve.GetPointPosition(6)));
        // Referência para a outra curva Path3D4
        Path3D path4 = GetNode<Path3D>("../Path3D4");

        // Índice do ponto da curva que você deseja usar
        int pointIndex = 6;

        // Obtém a posição do ponto no espaço local de Path3D3
        Vector3 pointPositionLocal = Curve.GetPointPosition(pointIndex);

        // Converte a posição do ponto para o espaço global
        Vector3 pointPositionGlobal = ToGlobal(pointPositionLocal);

        // Converte a posição global para o espaço local de Path3D4
        Vector3 pointPositionInPath4Local = path4.ToLocal(pointPositionGlobal);

        // Log para debug
        // GD.Print("Posição global do ponto no Path3D3: ", pointPositionGlobal);
        // GD.Print("Posição no espaço local do Path3D4: ", pointPositionInPath4Local);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
