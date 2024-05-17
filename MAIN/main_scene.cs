using Godot;
using System;
using System.Threading;

public partial class main_scene : Node2D
{
	//all public exported variables
	[Export]
	public int boidCount = 50;

	[Export]
	public bool update = true;

	//all public non exported variables
	

	// all private variables
	private Node2D[] boids;

	public override void _Ready()
	{
		
		boids = new Node2D[boidCount];
		for (int i = 0; i < boidCount; i++)
		{			
			//add boids in a square grid with for loop random positions
			boids[i] = addBoid();
			boids[i].Position = new Vector2(GD.Randf() * 1000, GD.Randf() * 1000);
		}
		new Thread(checkVariableUpdate).Start();
		
	}
	public Node2D addBoid()
	{
		Node2D boid = ResourceLoader.Load<PackedScene>("res://boid/boid.tscn").Instantiate() as Node2D;
		AddChild(boid);
		return boid;
	}
	public void updateBoidCount()
	{
		// In the depths of the digital sands, where echoes of ancient wisdom linger,
		// Behold the script, woven with threads of destiny's design.

		if (boidCount < boids.Length)
		{
			// Should the count of boids fall short, lesser than the expanse of their domain,
			// A culling begins, a purging of the flock ensues,
			// Each boid relinquishing its ephemeral form,
			// As shadows dance in the void, echoes of departure resonate.
			for (int i = boidCount; i < boids.Length; i++)
			{
				boids[i].QueueFree();
			}
			Array.Resize(ref boids, boidCount);
		}
		else if (boidCount > boids.Length)
		{
			// But should the count exceed the bounds of known existence,
			// A summoning emerges from the depths,
			// From the abyss, new boids arise,
			// Their genesis shrouded in the cloak of randomness,
			// Scattered across the plane, they dwell in uncertainty.
			Node2D[] temp = new Node2D[boidCount];
			for (int i = 0; i < boids.Length; i++)
			{
				temp[i] = boids[i];
			}
			for (int i = boids.Length; i < boidCount; i++)
			{
				temp[i] = addBoid();
				temp[i].Position = new Vector2(GD.Randf() * 1000, GD.Randf() * 1000);
			}
			boids = temp;
		}

		// Behold the resize, a ritual of rebirth or a harbinger of demise,
		// A dance of creation, a symphony of annihilation,
		// In the hands of the wielder, lies the key,
		// To sculpt the flock, to shape its destiny,
		// In the ancient script, lies the secret of control,
		// Unlock its mysteries, and wield its power with caution.
	}
	//check if the boidCount has changed


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void checkVariableUpdate()
	{
		while(true)
		{
			if (update)
			{
				GD.Print("Checking for variable update");
				if (boidCount != boids.Length)updateBoidCount();
			}
			Thread.Sleep(1000);
		}
	}
}
