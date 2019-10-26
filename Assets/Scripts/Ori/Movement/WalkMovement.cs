using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseMovement))]
public class WalkMovement : MovementComponent
{
	//
	// Public members.
	//
	//! Walking speed.
	public float walkSpeed = 6.0f;
	//! Air control acceleration.
	public float accelerationTimeAirborne = .2f;
	//! Grounded acceleration.
	public float accelerationTimeGrounded = .1f;

	//
	// Private members.
	//
	//! Just for dumping velocity.
	float velocityXSmoothing;
	//! Movement from base.
	BaseMovement baseMovement;

	// Start is called before the first frame update
	protected override void Start()
    {
		base.Start();
		baseMovement = GetComponent<BaseMovement>();
		baseMovement.AddChildMovement(this);
	}

    // Update is called once per frame
    void Update()
    {
		baseMovement.velocity.x = Mathf.SmoothDamp(
			baseMovement.velocity.x,
			Input.GetAxis("A_H") * walkSpeed,
			ref velocityXSmoothing,
			(controller.GetCollisionState() & Controller2D.CollisionState.COLLIDE_BELOW) != 0
				? accelerationTimeGrounded
				: accelerationTimeAirborne);
	}
}
