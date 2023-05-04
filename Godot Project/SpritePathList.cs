using Godot;
using System;

public partial class SpritePathList : Node
{
	public struct SpritePath 
	{
		public SpritePath(string name, string path) 
		{
			DisplayName = name;
			Path = path;
		}
		
		public string DisplayName;
		public string Path;
	}
	
	public static SpritePath[] Paths = new SpritePath[]{
		new SpritePath("Player", "user://sprites/player.png"),
		new SpritePath("Enemy #1", "user://sprites/enemy01.png"),
		new SpritePath("Enemy #2", "user://sprites/enemy02.png"),
		new SpritePath("Enemy #3", "user://sprites/enemy03.png"),
		new SpritePath("Enemy #4", "user://sprites/enemy04.png"),
	};
}
