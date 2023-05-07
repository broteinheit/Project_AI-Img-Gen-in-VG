using Godot;
using System;

public partial class HeartContainer : Area2D
{
	public override void _Ready()
	{
		var heartPath = Array.Find(SpritePathList.Paths, p => p.SpriteName == SpritePathList.SpriteNameEnum.HEART);
		var heartImg = Image.LoadFromFile(heartPath.Path);
		
		if (heartImg.GetWidth() >= 128 && heartImg.GetHeight() >= 128)
		{
			heartImg.Resize(64, 64);
		}

		GetNode<Sprite2D>("HeartContainerSprite").Texture = ImageTexture.CreateFromImage(heartImg);
	}
}
