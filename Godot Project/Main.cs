using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }
	
	private int _score;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void GameOver()
	{
		GetNode<AudioStreamPlayer>("DeathSound").Play();
		
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		
		GetNode<AudioStreamPlayer>("Music").Stop();
		
		GetNode<Hud>("HUD").ShowGameOver();
	}
	
	public void NewGame()
	{
		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		_score = 0;
		
		var hud = GetNode<Hud>("HUD");
		hud.UpdateScore(_score);
		hud.ShowMessage("Get Ready!");

		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();
		GetNode<AudioStreamPlayer>("Music").Play();
	}
	
	private void OnMobTimerTimeout()
	{
		Mob mob = MobScene.Instantiate<Mob>();
		
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();
		
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;
		
		mob.Position = mobSpawnLocation.Position;
		
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;
		
		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);
		
		AddChild(mob);
	}


	private void OnScoreTimerTimeout()
	{
		_score++;
		
		if (_score > 10) 
		{
			var mobTimer = GetNode<Timer>("MobTimer");
			mobTimer.Stop();
			mobTimer.Start(0.5 * (1/_score));
		}
		Console.WriteLine(GetNode<Timer>("MobTimer").WaitTime);
		Console.WriteLine("score: " + _score.ToString());
		
		GetNode<Hud>("HUD").UpdateScore(_score);
	}


	private void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}
}
