using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseMovement))]
[RequireComponent(typeof(Controller2D))]
public class JumpMovement : MovementComponent
{
	//
	// Public members.
	//
	//! Height of the jump.
	public float jumpHeight = 4.0f;
	//! Time till jump apex.
	public float jumpApexTime = 0.4f;
	//! Possible number of jumps.
	public uint jumpsNumber = 1;

	//
	// Private members.
	//
	//! Movement from base.
	BaseMovement baseMovement;
	//! Velocity for jump.
	float jumpVelocity;
	//! Jumps count.
	uint jumps = 0;
	
	//
	// Public interface.
	//
	//! Makes jump.
	void Jump(float xVelocity = 0.0f)
	{
		baseMovement.velocity.y = jumpVelocity;
		baseMovement.velocity.x += xVelocity;
	}

	//
	// Private methods.
	//
	//! Resets all gravity parameters.
	void ResetGravityParams()
	{
		baseMovement.gravity = -2 * jumpHeight / (jumpApexTime * jumpApexTime);
		jumpVelocity = -baseMovement.gravity * jumpApexTime;
	}
	// Start is called before the first frame update
	protected override void Start()
    {
		base.Start();
		baseMovement = GetComponent<BaseMovement>();
		baseMovement.AddChildMovement(this);
		ResetGravityParams();
	}
    // Update is called once per frame
    void Update()
    {
		if ((controller.GetCollisionState() & Controller2D.CollisionState.COLLIDE_BELOW) != 0)
		{
			jumps = jumpsNumber;
		}

		if (jumps > 0 && Input.GetButtonDown("B_J"))
		{
			--jumps;
			Jump();
		}
	}
}
