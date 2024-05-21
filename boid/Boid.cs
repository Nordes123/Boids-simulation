using Godot;
using System;
	public partial class Boid : Node2D
	{
		// Public exported variables
		public Vector2 velocity = new Vector2(0, 0);

		// Public non-exported variables

		// Private variables
		private Sprite2D sprite;
		public override void _Ready()
		{	
			sprite = GetNode<Sprite2D>("Boid-sprite");
			//get the sprite node

		}

		public override void _Process(double delta)
		{
			sprite.Rotation = velocity.Angle();
		}
	}
