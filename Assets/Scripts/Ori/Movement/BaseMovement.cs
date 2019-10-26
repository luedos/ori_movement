using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class BaseMovement : MovementComponent
{
	//
	// Public members.
	//
	//! Gravity acceleration.
	public float gravity = 10;
	//! Max speed from gravitational force when charecter sliding from wall.
	public float maxSlidingSpeed = 3.0f;
	//! Current velocity.
	[HideInInspector]
	public Vector2 velocity;

	// Start is called before the first frame update
	protected override void Start()
    {
		base.Start();
	}

    // Update is called once per frame
    void Update()
	{
		velocity.y += gravity * Time.deltaTime;

		controller.Move(velocity * Time.deltaTime);

		Controller2D.CollisionState state = controller.GetCollisionState();

		bool grounded = (state & Controller2D.CollisionState.COLLIDE_BELOW) != 0;

		if ((state & (Controller2D.CollisionState.COLLIDE_BELOW | Controller2D.CollisionState.COLLIDE_ABOVE)) != 0) {
			velocity.y = 0.0f;
		}
		
		if ((state & (Controller2D.CollisionState.COLLIDE_LEFT | Controller2D.CollisionState.COLLIDE_RIGHT)) != 0
			&& velocity.y < -maxSlidingSpeed)
		{
			velocity.y = -maxSlidingSpeed;
		}
	}
}
