using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class BaseMovement : MovementComponent
{
	//
	// Public members.
	//
	//! Speed of charecter walking.
	public float walkSpeed = 5.0f;
	//! Gravity acceleration.
	public float gravity = 10;
	//! Air control acceleration.
	public float accelerationTimeAirborne = .2f;
	//! Grounded acceleration.
	public float accelerationTimeGrounded = .1f;
	//! Max speed from gravitational force when charecter sliding from wall.
	public float maxSlidingSpeed = 3.0f;
	//! Current velocity.
	[HideInInspector]
	public Vector2 velocity;

	//
	// Private members.
	//
	//! Just for dumping velocity.
	float velocityXSmoothing;

	// Start is called before the first frame update
	protected override void Start()
    {
		base.Start();
	}

    // Update is called once per frame
    void Update()
	{
		velocity.x =Mathf.SmoothDamp(
			velocity.x, 
			Input.GetAxis("A_H") * walkSpeed, 
			ref velocityXSmoothing,
			(controller.GetCollisionState() & Controller2D.CollisionState.COLLIDE_BELOW) != 0 
				? accelerationTimeGrounded
				: accelerationTimeAirborne);

		velocity.y += gravity * Time.deltaTime;

		controller.Move(velocity * Time.deltaTime);

		Controller2D.CollisionState state = controller.GetCollisionState();

		if ((state & (Controller2D.CollisionState.COLLIDE_BELOW | Controller2D.CollisionState.COLLIDE_ABOVE)) != 0) {
			velocity.y = 0.0f;
		}

		if ((state & Controller2D.CollisionState.COLLIDE_BELOW) == 0
			&& (state & (Controller2D.CollisionState.COLLIDE_LEFT | Controller2D.CollisionState.COLLIDE_RIGHT)) != 0)
		{
			if (velocity.y < maxSlidingSpeed) {
				velocity.y = -maxSlidingSpeed;
			}
		}

	}
}
