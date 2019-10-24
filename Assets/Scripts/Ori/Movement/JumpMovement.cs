using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseMovement))]
public class JumpMovement : MovementComponent
{
	//
	// Public members.
	//
	//! Height of the jump.
	public float lowJumpHeight = 4.0f;
	//! Time till jump apex.
	public float lowJumpApexTime = 0.4f;
	//! Height of the jump.
	public float highJumpHeight = 6.0f;
	//! Possible number of jumps.
	public uint jumpsNumber = 1;

	//
	// Private members.
	//
	//! Movement from base.
	BaseMovement baseMovement;
	//! Velocity for jump.
	float jumpVelocity;
	//! Jumps counter.
	[HideInInspector]
	public uint jumps = 0;
	//! Low jump gravity.
	float lowJumpGravity;
	//! High jump gravity.
	float highJumpGravity;
	//! Time till jump apex.
	float highJumpApexTime;
	//! Is player currently jumping.
	bool jumping = false;

	//
	// Public interface.
	//
	//! Makes jump.
	public void Jump(float xVelocity = 0.0f)
	{
		jumping = true;
		baseMovement.gravity = highJumpGravity;
		baseMovement.velocity.y = jumpVelocity;
		baseMovement.velocity.x += xVelocity;
	}

	//
	// Private methods.
	//
	//! Resets all gravity parameters.
	void ResetGravityParams()
	{
		lowJumpGravity = -2 * lowJumpHeight / (lowJumpApexTime * lowJumpApexTime);
		jumpVelocity = -lowJumpGravity * lowJumpApexTime;
		highJumpApexTime = 2 * highJumpHeight / jumpVelocity;
		highJumpGravity = -jumpVelocity / highJumpApexTime;

		baseMovement.gravity = lowJumpGravity;
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
		Controller2D.CollisionState state = controller.GetCollisionState();

		if (jumping)
		{
			if (state != 0)
			{
				jumping = false;
			}

			if (!jumping || baseMovement.velocity.y < 0 || !Input.GetButton("B_J"))
			{
				baseMovement.gravity = lowJumpGravity;
			}
		}

		if ((state & Controller2D.CollisionState.COLLIDE_BELOW) != 0)
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
