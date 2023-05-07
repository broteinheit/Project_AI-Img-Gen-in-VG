using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }

	[Export]
	public PackedScene HeartContainerScene { get; set; }
	
	private int _score;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<CanvasLayer>("RequestHUD").Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void GameOver()
	{
		GetNode<Player>("Player").Hide();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
		
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<Timer>("HeartTimer").Stop();
		
		GetNode<AudioStreamPlayer>("Music").Stop();
		
		GetNode<HeartDisplay>("HUD/HeartDisplay").Visible = false;
		GetNode<Hud>("HUD").ShowGameOver();
	}

	private void PlayerHit()
	{
		HeartDisplay hearts = GetNode<HeartDisplay>("HUD/HeartDisplay");

		hearts.PlayerHit();

		if (hearts.HP == 0)
		{
			GameOver();
		}
	}
	
	public void NewGame()
	{
		ReloadTextures();
		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		GetTree().CallGroup("heartContainers", Node.MethodName.QueueFree);
		_score = 0;
		
		var hud = GetNode<Hud>("HUD");
		hud.UpdateScore(_score);
		hud.ShowMessage("Get Ready!");
		var hearts = hud.GetNode<HeartDisplay>("HeartDisplay");
		hearts.HP = 3;
		hearts.UpdateHealthDisplay();
		hearts.Visible = true;


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
		GetNode<Timer>("HeartTimer").Start();
	}
	
	private void OnHudChangeTextures()
	{
		GetNode<CanvasLayer>("HUD").Hide();
		GetNode<CanvasLayer>("RequestHUD").Show();
	}
	
	private void OnRequestHudBackToMainMenu()
	{
		GetNode<CanvasLayer>("RequestHUD").Hide();
		GetNode<CanvasLayer>("HUD").Show();
	}

	private void OnHeartTimerTimeout() 
	{
		HeartContainer hc = HeartContainerScene.Instantiate<HeartContainer>();
		
		int x = GD.RandRange(128, GetWindow().Size.X - 128);
		int y = GD.RandRange(128, GetWindow().Size.Y - 128);
		hc.Position = new Vector2(x, y);
		hc.Scale = new Vector2(0.5f, 0.5f);
		AddChild(hc);
	}

	private void OnPlayerCollected()
	{
		var hearts = GetNode<HeartDisplay>("HUD/HeartDisplay");

		if (hearts.HP == 3) 
		{
			_score += 5;
			GetNode<Hud>("HUD").UpdateScore(_score);
		}
		else
		{
			hearts.AddHP();
		}
	}
	
	private void ReloadTextures()
	{
		//Player
		var playerSprite = GetNode<AnimatedSprite2D>("Player/AnimatedSprite2D");
		var playerPath = Array.Find(SpritePathList.Paths, p => p.SpriteName == SpritePathList.SpriteNameEnum.PLAYER);
		var playerImg = Image.LoadFromFile(playerPath.Path);
		
		if (playerImg.GetWidth() >= 128 && playerImg.GetHeight() >= 128)
		{
			playerImg.Resize(128, 128);
		}
		
		var playerTexture = ImageTexture.CreateFromImage(playerImg);
		playerSprite.SpriteFrames.Clear("walk");
		playerSprite.SpriteFrames.AddFrame("walk", playerTexture);
		
		//Mob
		Mob mob = MobScene.Instantiate<Mob>();
		var mobSprite = mob.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		var mobAnimationNames = mobSprite.SpriteFrames.GetAnimationNames();
		
		for (int i = 1; i <= mobAnimationNames.Length; i++)
		{
			var spriteEnum = Enum.Parse<SpritePathList.SpriteNameEnum>($"ENEMY{i}");
			
			var mobPath = Array.Find(SpritePathList.Paths, p => p.SpriteName == spriteEnum);
			var mobImg = Image.LoadFromFile(mobPath.Path);
			
			if (mobImg.GetWidth() >= 128 && mobImg.GetHeight() >= 128)
			{
				mobImg.Resize(128, 128);
			}
			
			var mobTexture = ImageTexture.CreateFromImage(mobImg);
			
			mobSprite.SpriteFrames.Clear(mobAnimationNames[i-1]);
			mobSprite.SpriteFrames.AddFrame(mobAnimationNames[i-1], mobTexture);
		}
		
	}
}

