using DominionWars.Manager;
using Godot;

namespace DominionWars;

public partial class MainGame : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameManager.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GameManager.MainLoop();
		if (GameManager.IsStop)
		{
			GetTree().Quit();
		}
	}
}