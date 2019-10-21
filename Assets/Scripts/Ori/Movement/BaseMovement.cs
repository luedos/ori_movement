using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class BaseMovement : MonoBehaviour
{
	//
	// Public members.
	//
	//! Height of the jump.
	public float jumpHeight = 4.0f;
	//! Time till jump apex.
	public float jumpApexTime = 0.4f;
	//! Speed of charecter walking.
	public float walkSpeed = 5.0f;

	//
	// Private members.
	//
	//! Gravity acceleration.
	float gravity;
	//! Velocity for jump.
	float jumpVelocity;
	//! Controller.
	Controller2D controller;
	//! Current velocity.
	Vector2 velocity;	

    // Start is called before the first frame update
    void Start()
    {
		controller = GetComponent<Controller2D>();

		ResetGravityParams();
	}

	//! Resets all gravity parameters.
	void ResetGravityParams()
	{
		gravity = -2 * jumpHeight / (jumpApexTime * jumpApexTime);
		jumpVelocity = -gravity * jumpApexTime;
	}

    // Update is called once per frame
    void FixedUpdate()
	{
		ResetGravityParams();

		if ((controller.GetCollisionState() & (Controller2D.CollisionState.COLLIDE_BELOW | Controller2D.CollisionState.COLLIDE_ABOVE)) != 0) {
			velocity.y = 0.0f;
		}

		if ((controller.GetCollisionState() & Controller2D.CollisionState.COLLIDE_BELOW) != 0 && Input.GetButtonDown("B_J")) {
			velocity.y = jumpVelocity;
		}

		velocity.x = Input.GetAxis("A_H") * walkSpeed;
		velocity.y += gravity * Time.fixedDeltaTime;

		controller.Move(velocity * Time.fixedDeltaTime);		
    }
}
