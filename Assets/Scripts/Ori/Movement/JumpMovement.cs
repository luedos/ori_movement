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
	//! Swiches off or on wall jumps;
	public bool wallJump = true;
	//! Wall jump x velocity.
	public float wallJumpVelocity = 10;

	//
	// Private members.
	//
	//! Movement from base.
	BaseMovement baseMovement;
	//! Velocity for jump.
	float jumpVelocity;
	//! Jumps counter.
	uint jumpsCounter = 0;
	//! Low jump gravity.
	float lowJumpGravity;
	//! High jump gravity.
	float highJumpGravity;
	//! Is player currently jumping.
	bool jumping = false;

	//
	// Public interface.
	//
	public bool IsJumping()
	{
		return jumping;
	}
	//! Makes jump.
	public void Jump(float xVelocity = 0.0f)
	{
		if (jumpsCounter == 0) {
			return;
		}
		jumping = true;
		baseMovement.gravity = highJumpGravity;
		baseMovement.velocity.y = jumpVelocity;
		baseMovement.velocity.x += xVelocity;

		--jumpsCounter;
	}

	//
	// Private methods.
	//
	//! Resets all gravity parameters.
	void ResetGravityParams()
	{
		lowJumpGravity = -2 * lowJumpHeight / (lowJumpApexTime * lowJumpApexTime);
		jumpVelocity = -lowJumpGravity * lowJumpApexTime;
		highJumpGravity = -0.5f * (jumpVelocity * jumpVelocity) / highJumpHeight;

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

		bool grounded = (state & Controller2D.CollisionState.COLLIDE_BELOW) != 0;
		bool wallGrounded = wallJump && (state & (Controller2D.CollisionState.COLLIDE_LEFT | Controller2D.CollisionState.COLLIDE_RIGHT)) != 0;

		if (jumping)
		{
			if (grounded || baseMovement.velocity.y <= 0)
			{
				jumping = false;
			}

			if (!jumping || !Input.GetButton("B_J"))
			{
				baseMovement.gravity = lowJumpGravity;
			}
		}

		if (grounded || wallGrounded)
		{
			jumpsCounter = jumpsNumber;
		}

		if (jumpsCounter > 0 && Input.GetButtonDown("B_J"))
		{
			float xVelocity = 0;

			if (wallGrounded) {
				xVelocity = (state & Controller2D.CollisionState.COLLIDE_LEFT) != 0 ? wallJumpVelocity : -wallJumpVelocity;
			}

			Jump(xVelocity);
		}
	}
}
