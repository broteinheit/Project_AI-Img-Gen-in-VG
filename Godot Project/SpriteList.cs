using Godot;
using System;

public partial class SpriteList : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LoadListItems();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private bool LoadListItems() 
	{
		ItemList spriteItemList = GetNode<ItemList>("ScrollContainer/SpriteItemList");
		
		spriteItemList.Clear();
		
		foreach(SpritePathList.SpritePath path in SpritePathList.Paths)
		{
			Image img = Image.LoadFromFile(path.Path);
			//img.Resize(128, 128);
			ImageTexture imgTexture = ImageTexture.CreateFromImage(img);
			
			spriteItemList.AddItem(path.DisplayName, imgTexture);
		}
		
		return true;
	}
}
