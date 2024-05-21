using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class main_scene : Node2D
{
	//all public exported variables
	[Export]
	public bool update = true;


	[Export]
	public int boidCount = 150;


	[Export]
	public float minSpeed = 3;

	[Export]
	public float maxSpeed = 4;



	[Export]
	public float protectedArea = 13;

	[Export]
	public float avoidFactor = 0.1f;

	[Export]
	public bool includeProtectedArea = true;


	[Export]
	public float visualArea = 40;

	[Export]
	public float matchingFactor = 0.05f;
	
	[Export]
	public float centeringFactor = 0.000001f;


	[Export]
	public float turnFactor = 0.07f;

	[Export]
	public int turnMargin = 50;

	[Export]
	public bool continuesWalls = false;


	[Export]
	public float Bias = 0.00004f;

	//all public non exported variables

	// all private variables
	private Boid[] boids;

	public override void _Ready()
	{
		
		boids = new Boid[boidCount];
		for (int i = 0; i < boidCount; i++)
		{			
			//add boids in a square grid with for loop random Positions
			boids[i] = addBoid();
		}
		new Thread(VariableUpdateChecker).Start();
		
	}
	public Boid addBoid()
	{
		Boid boid = ResourceLoader.Load<PackedScene>("res://boid/Boid.tscn").Instantiate() as Boid;
		CallDeferred("add_child", boid);
		boid.Position = new Vector2(GD.Randf() * 1000, GD.Randf() * 1000);
		boid.velocity = new Vector2((GD.Randf() * 2) - 1, (GD.Randf() * 2 ) - 1).Normalized() * minSpeed;

		return boid;
	}
	public void updateBoidCount()
	{


		if (boidCount < boids.Length)
		{
			for (int i = boidCount; i < boids.Length; i++)
			{
				boids[i].QueueFree();
			}
			Array.Resize(ref boids, boidCount);
		}
		else if (boidCount > boids.Length)
		{

			Boid[] temp = new Boid[boidCount];
			for (int i = 0; i < boids.Length; i++)
			{
				temp[i] = boids[i];
			}
			for (int i = boids.Length; i < boidCount; i++)
			{
				temp[i] = addBoid();
			}
			boids = temp;
		}

	}

	private void VariableUpdateChecker()
	{
		while(true)
		{
			if (!update) continue;
			
			if (boidCount != boids.Length) updateBoidCount();
			

			Thread.Sleep(1000);
		}
	}

	public override void _Process(double delta)
	{
		int windowWidth = (int)GetViewport().GetVisibleRect().Size.X;
		int windowHeight = (int)GetViewport().GetVisibleRect().Size.Y;

		int firstHalf = boids.Length / 2;
		int secondHalf = boids.Length - firstHalf;

		for (int i = 0; i < boids.Length; i++)
		{
			Boid boid = boids[i];
			
			Vector2 averageVelocity = new Vector2();
			Vector2 averagePosition = new Vector2();
			
			int neighborCount = 0;

			for (int j = 0; j < boids.Length; j++)
			{
				Boid otherBoid = boids[j];
				Vector2 closeDistance = new Vector2();

				if (i != j)
				{
					if (boid.Position.DistanceTo(otherBoid.Position) <= protectedArea)
					{
						closeDistance += boid.Position - otherBoid.Position;
					}

					boid.velocity += closeDistance * avoidFactor;

					if(boid.Position.DistanceTo(otherBoid.Position) <= visualArea)
					{
						if(includeProtectedArea)
						{
							averageVelocity += otherBoid.velocity;
							averagePosition += otherBoid.Position;
							neighborCount++;
						}
						else if(!includeProtectedArea && boid.Position.DistanceTo(otherBoid.Position) > protectedArea)
						{
							averageVelocity += otherBoid.velocity;
							averagePosition += otherBoid.Position;
							neighborCount++;
						}
					}
				}

				if (neighborCount > 0)
				{
					averageVelocity /= neighborCount;
					boid.velocity += (averageVelocity - boid.velocity) * matchingFactor;

					averagePosition /= neighborCount;
					boid.velocity += (averagePosition - boid.velocity) * centeringFactor;
				}
			}
			
			if (boid.Position.X < 0)
				boid.Position = new Vector2(windowWidth, boid.Position.Y);

			if (boid.Position.X > windowWidth)
				boid.Position = new Vector2(0, boid.Position.Y);

			if (boid.Position.Y < 0)
				boid.Position = new Vector2(boid.Position.X, windowHeight);

			if (boid.Position.Y > windowHeight)
				boid.Position = new Vector2(boid.Position.X, 0);

			if (!continuesWalls)
			{

				if (boid.Position.X < turnMargin)
					boid.velocity.X += turnFactor;

				if (boid.Position.X > windowWidth - turnMargin)
					boid.velocity.X -= turnFactor;

				if (boid.Position.Y > windowHeight - turnMargin)
					boid.velocity.Y -= turnFactor;

				if (boid.Position.Y < turnMargin)
					boid.velocity.Y += turnFactor;
			}

			if(i < firstHalf)
				boid.velocity.X = (1 - Bias) * boid.velocity.X + (Bias * 1 );
			
			else
				boid.velocity.X = (1 - Bias) * boid.velocity.X + (Bias * -1 );

			if (boid.velocity.Length() > maxSpeed)
				boid.velocity = boid.velocity.Normalized() * maxSpeed;

			else if (boid.velocity.Length() < minSpeed)
				boid.velocity = boid.velocity.Normalized() * minSpeed;
			
			boid.Position += boid.velocity;
		}
	}
}
