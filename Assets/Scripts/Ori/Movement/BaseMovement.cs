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
		velocity.x = Input.GetAxis("A_H") * walkSpeed;
		velocity.y += gravity * Time.deltaTime;

		controller.Move(velocity * Time.deltaTime);

		if ((controller.GetCollisionState() & (Controller2D.CollisionState.COLLIDE_BELOW | Controller2D.CollisionState.COLLIDE_ABOVE)) != 0) {
			velocity.y = 0.0f;
		}
	}
}
