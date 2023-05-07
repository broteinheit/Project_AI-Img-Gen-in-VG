using Godot;
using System;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Text.Json;

public partial class RequestHud : CanvasLayer
{
	[Signal]
	public delegate void BackToMainMenuEventHandler();
	
	private readonly System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

	private bool _RemoveBG = false;

	private readonly int RGB_DISTANCE = 14;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnBackButtonPressed()
	{
		GetNode<PromptHud>("PromptHUD").ClearContent();
		GetNode<TextureRect>("ImageDisplay").Texture = null;
		GetNode<Button>("RemoveBGButton").Visible = false;
		GetNode<Button>("FlipButton").Visible = false;
		EmitSignal(SignalName.BackToMainMenu);
	}
	
	private void OnSendRequestButtonPressed()
	{
		GetNode<Button>("SendRequestButton").Disabled = true;
		GetNode<Button>("SaveButton").Disabled = true;
		GetNode<Button>("BackButton").Disabled = true;
		_RemoveBG = false;
		CheckRemoveBGButtonEnabled();
		
		new Thread(new ThreadStart(async () => {
			//make HTTP Request to Image Generation Server
			Dictionary<string, string> values = new Dictionary<string, string>
			{
				{ "prompt", GetNode<LineEdit>("PromptHUD/PromptInput").Text },
				{ "steps", "25" }
			};
			var body = JsonContent.Create(values, values.GetType());
			var response = await client.PostAsync("http://localhost:9999/sd", body);
  			var responseString = await response.Content.ReadAsStringAsync();
			
			//Decode response as JSON
			Dictionary<string, string[]> imagesJson =
				JsonSerializer.Deserialize<Dictionary<string, string[]>>(responseString);
			
			//load image from base64 string
			byte[] imageBytes = Convert.FromBase64String(imagesJson["images"][0]);
			Image img = new Image();
			img.LoadPngFromBuffer(imageBytes);
			img.Resize(256, 256);
			GetNode<TextureRect>("ImageDisplay").Texture = ImageTexture.CreateFromImage(img);
			
			GetNode<Button>("SendRequestButton").Disabled = false;
			GetNode<Button>("SaveButton").Disabled = false;
			GetNode<Button>("BackButton").Disabled = false;

			GetNode<Button>("RemoveBGButton").Visible = true;
			GetNode<Button>("FlipButton").Visible = true;
			_RemoveBG = false;
			CheckRemoveBGButtonEnabled();
		})).Start();
	}

	private void OnFlipButtonPressed() 
	{
		var image = GetNode<TextureRect>("ImageDisplay").Texture.GetImage();

		if (image != null) 
		{
			image.FlipX();
			GetNode<TextureRect>("ImageDisplay").Texture = ImageTexture.CreateFromImage(image);
		}
	}
	
	private void OnSaveButtonPressed()
	{
		ItemList itemList = GetNode<ItemList>("SpriteList/ScrollContainer/SpriteItemList");
		
		if (itemList.IsAnythingSelected()) 
		{
			int selectedIdx = itemList.GetSelectedItems()[0]; //index 0 since only 1 item can be selected
			string displayName = itemList.GetItemText(selectedIdx);
			
			SpritePathList.SpritePath spritePath = 
				Array.Find(SpritePathList.Paths, p => p.DisplayName == displayName);
			
			GetNode<TextureRect>("ImageDisplay").Texture.GetImage().SavePng(spritePath.Path);
			GetNode<SpriteList>("SpriteList").LoadListItems();
		}
		else
		{
			//show message
		}
	}

	private void OnResetButtonPressed()
	{
		ItemList itemList = GetNode<ItemList>("SpriteList/ScrollContainer/SpriteItemList");
		
		if (itemList.IsAnythingSelected()) 
		{
			int selectedIdx = itemList.GetSelectedItems()[0]; //index 0 since only 1 item can be selected
			string displayName = itemList.GetItemText(selectedIdx);
			
			SpritePathList.SpritePath spritePath = 
				Array.Find(SpritePathList.Paths, p => p.DisplayName == displayName);
			
			Image.LoadFromFile(spritePath.ResPath).SavePng(spritePath.Path);

			GetNode<SpriteList>("SpriteList").LoadListItems();
		}
		else
		{
			//show message
		}
	}

	private void OnRemoveBGButtonPressed()
	{
		_RemoveBG = !_RemoveBG;
		CheckRemoveBGButtonEnabled();
	}

	private void OnImageDisplayGuiInput(InputEvent @event) 
	{
		if (@event is InputEventMouseButton) 
		{
			InputEventMouseButton mb = @event as InputEventMouseButton;
			if (!mb.Pressed && _RemoveBG)
			{
				Image img = GetNode<TextureRect>("ImageDisplay").Texture.GetImage();
				img.Convert(Image.Format.Rgba8);

				if (img.GetSize().X == 256 && img.GetSize().Y == 256)
				{
					Color c = img.GetPixel(((int)mb.Position.X), ((int)mb.Position.Y));
					GetNode<TextureRect>("ImageDisplay").Texture = ImageTexture.CreateFromImage(RemoveColorFromImage(img, c));
				}
				_RemoveBG = false;
				CheckRemoveBGButtonEnabled();
			}
		}
	}

	private Image RemoveColorFromImage(Image image, Color color) 
	{
		for (int x=0; x < image.GetWidth(); x++) 
		{
			for (int y=0; y < image.GetHeight(); y++)
			{
				Color pixel = image.GetPixel(x, y);
				if ((pixel.R8 >= color.R8 - RGB_DISTANCE && pixel.R8 <= color.R8 + RGB_DISTANCE)
					&& (pixel.G8 >= color.G8 - RGB_DISTANCE && pixel.G8 <= color.G8 + RGB_DISTANCE)
					&& (pixel.B8 >= color.B8 - RGB_DISTANCE && pixel.B8 <= color.B8 + RGB_DISTANCE)) 
				{
					image.SetPixel(x, y, new Color(Colors.White, 0)); //white with alpha 0 => transparent
				}
			}
		}
		image.FixAlphaEdges();
		return image;
	}

	private void CheckRemoveBGButtonEnabled()
	{
		if (_RemoveBG) GetNode<Button>("RemoveBGButton").Disabled = true;
		else GetNode<Button>("RemoveBGButton").Disabled = false;
	}
}

