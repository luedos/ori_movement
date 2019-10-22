using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JumpMovement))]
public class WallJumpMovement : MovementComponent
{

	//
	// Private members.
	//
	//! Jump movement component.
	JumpMovement jumpMovement;

	//
	// Public members.
	//
	public float jumpVelocity = 10;

    // Start is called before the first frame update
    protected override void Start()
    {
		base.Start();
		jumpMovement = GetComponent<JumpMovement>();
		jumpMovement.AddChildMovement(this);
	}

    // Update is called once per frame
    void Update()
    {
        if ((controller.GetCollisionState() & (Controller2D.CollisionState.COLLIDE_LEFT | Controller2D.CollisionState.COLLIDE_RIGHT)) != 0
			&& jumpMovement.jumpsNumber > 0
			&& Input.GetButtonDown("B_J"))
		{
			jumpMovement.jumps = jumpMovement.jumpsNumber - 1;
			jumpMovement.Jump((controller.GetCollisionState() & Controller2D.CollisionState.COLLIDE_LEFT) != 0 ? jumpVelocity : -jumpVelocity);
		}
    }
}
