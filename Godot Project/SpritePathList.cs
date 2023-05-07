using Godot;
using System;

public partial class SpritePathList : Node
{
	public enum SpriteNameEnum {
		PLAYER,
		ENEMY1,
		ENEMY2,
		ENEMY3,
		ENEMY4,
	}
	
	public struct SpritePath 
	{
		public SpritePath(string name, string path, SpriteNameEnum spriteName, string resPath) 
		{
			DisplayName = name;
			Path = path;
			SpriteName = spriteName; 
			ResPath = resPath;
		}
		
		public string DisplayName;
		public string Path;
		public SpriteNameEnum SpriteName;
		public string ResPath;
	}
	
	public static int MobTypeCount = 4;
	
	public static SpritePath[] Paths = new SpritePath[]{
		new SpritePath("Player", "user://sprites/player.png", SpriteNameEnum.PLAYER, "res://sprites/player/playerGrey_walk1.png"),
		new SpritePath("Enemy #1", "user://sprites/enemy01.png", SpriteNameEnum.ENEMY1, "res://sprites/enemy/enemyWalking_1.png"),
		new SpritePath("Enemy #2", "user://sprites/enemy02.png", SpriteNameEnum.ENEMY2, "res://sprites/enemy/enemyWalking_1.png"),
		new SpritePath("Enemy #3", "user://sprites/enemy03.png", SpriteNameEnum.ENEMY3, "res://sprites/enemy/enemyWalking_1.png"),
		new SpritePath("Enemy #4", "user://sprites/enemy04.png", SpriteNameEnum.ENEMY4, "res://sprites/enemy/enemyWalking_1.png"),
	};
}
