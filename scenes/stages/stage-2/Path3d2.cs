using Godot;
using System;

public partial class Path3d2 : Path3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GD.Print("Path3d2", Curve.GetPointPosition(58));
        GD.Print("Path3d2global", ToGlobal(Curve.GetPointPosition(58)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
