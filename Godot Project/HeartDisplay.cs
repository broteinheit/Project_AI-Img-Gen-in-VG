using Godot;
using System;

public partial class HeartDisplay : Node2D
{
	public int HP { get; set; }

	public void PlayerHit()
	{
		HP--;
		UpdateHealthDisplay();
	}


	public void AddHP() 
	{ 
		if (HP < 3) HP++;
		UpdateHealthDisplay();
	}


	public void UpdateHealthDisplay()
	{
		GetNode<Sprite2D>("Heart01").Visible = HP >= 1;
		GetNode<Sprite2D>("Heart02").Visible = HP >= 2;
		GetNode<Sprite2D>("Heart03").Visible = HP >= 3;
	}
}
