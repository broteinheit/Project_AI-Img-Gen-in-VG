using Godot;
using System;

public partial class Player : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_up")) {
			velocity.Y -= 1;
		}

		if (Input.IsActionPressed("move_right")) {
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_down")) {
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_left")) {
			velocity.X -= 1;
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0) {
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		} else {
			animatedSprite2D.Stop();
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		if (velocity.X != 0)
		{
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipH = velocity.X < 0;
		}
	}
	
	private void _on_body_entered(Node2D body)
	{
		if (body is Mob)
		{
			EmitSignal(SignalName.Hit);
		}
	}

	private void _on_area_entered(Area2D body)
	{
		if (body is HeartContainer)
		{
			EmitSignal(SignalName.Collected);
			body.QueueFree();
		}
	}
	
	public void Start(Vector2 pos)
	{
		Position = pos;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
	
	[Export]
	public int Speed = 400;
	
	public Vector2 ScreenSize;

	[Signal]
	public delegate void HitEventHandler();

	[Signal]
	public delegate void CollectedEventHandler();
}

