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
		EmitSignal(SignalName.BackToMainMenu);
	}
	
	private void OnSendRequestButtonPressed()
	{
		GetNode<Button>("SendRequestButton").Disabled = true;
		GetNode<Button>("SaveButton").Disabled = true;
		GetNode<Button>("BackButton").Disabled = true;
		
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
			GetNode<TextureRect>("ImageDisplay").Texture = ImageTexture.CreateFromImage(img);
			
			GetNode<Button>("SendRequestButton").Disabled = false;
			GetNode<Button>("SaveButton").Disabled = false;
			GetNode<Button>("BackButton").Disabled = false;
		})).Start();
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
}

