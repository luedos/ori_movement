using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
	//
	// Private members.
	//
	struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
	struct CollisionData
	{
		//! Resets collision data.
		public void Reset()
		{
			state = 0;
			lastTerrainAngle = terrainAngle;
			terrainAngle = 0.0f;
		}
		//! State of collision detections.
		public CollisionState state;
		//! Angle of terrain controller colliding with.
		public float terrainAngle;
		//! Angle of terrain controller collided on last move.
		public float lastTerrainAngle;
	}
	//! Origins for collision checking.
	RaycastOrigins relOrigins;
	//! Collider.
	BoxCollider2D collider;
	//! Vertical spacing of of raytraicing.
	float verRaySpacing;
	//! Horizontal spacing of of raytraicing.
	float horRaySpacing;
	//! Depth into box collision for raycasting.
	float skinWidth = 0.015f;
	//! Number of vertical raycasts.
	uint verRayNum = 8;
	//! Number of horizontal raycasts.
	uint horRayNum = 16;
	//! Data about collision after last move.
	CollisionData collisions;

	//
	// Public members.
	//
	[System.Flags]
	public enum CollisionState
	{
		COLLIDE_BELOW	= 0x01,
		COLLIDE_ABOVE	= 0x02,
		COLLIDE_LEFT	= 0x04,
		COLLIDE_RIGHT	= 0x08,
		CLIMB_SLOPE		= 0x10,
		DESCEND_SLOPE	= 0x20
	}
	//! Layers to colide with.
	public LayerMask collisionLayers;
	//! Angle of terrain controller can climb on.
	public float maxClimbAngle = 70f;
	//! Angle of terrain controller can decend on;
	public float maxDecendAngle = 75f;
	//! Max velosity calculation step.
	public float maxVelocityStep = 0.10f;
	//! Depth into box collision for raycasting.
	float SkinWidth
	{
		set
		{
			skinWidth = value;
			UpdateRaycastData();
		}
		get
		{
			return skinWidth;
		}
	}
	//! Number of vertical raycasts.
	uint VerRayNum
	{
		set
		{
			verRayNum = value;
			UpdateRaycastData();
		}
		get
		{
			return verRayNum;
		}
	}
	//! Number of horizontal raycasts.
	uint HorRayNum
	{
		set
		{
			horRayNum = value;
			UpdateRaycastData();
		}
		get
		{
			return horRayNum;
		}
	}

	//
	// Private methods.
	//
	//! Updates every data related to collision testing.
	void UpdateRaycastData()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(-2 * skinWidth);

		relOrigins.bottomLeft = new Vector2(-bounds.extents.x, -bounds.extents.y);
		relOrigins.bottomRight = new Vector2(bounds.extents.x, -bounds.extents.y);
		relOrigins.topLeft = new Vector2(-bounds.extents.x, bounds.extents.y);
		relOrigins.topRight = new Vector2(bounds.extents.x, bounds.extents.y);

		if (horRayNum < 2) {
			horRayNum = 2;
		}

		if (verRayNum < 2) {
			verRayNum = 2;
		}
		
		horRaySpacing = bounds.size.x / (horRayNum - 1);
		verRaySpacing = bounds.size.y / (verRayNum - 1);
	}
	//! Vertical movement.
	void VerticalCollisions(ref Vector2 velocity, Vector3 position)
	{
		if (velocity.y == 0.0f) {
			return;
		}

		float rayLength = Mathf.Abs(velocity.y) + skinWidth;
		float direction = Mathf.Sign(velocity.y);

		Vector3 origin = direction > 0 ? relOrigins.topLeft : relOrigins.bottomLeft;
		origin += position;
		origin.x += velocity.x;

		for (uint i = 0; i < horRayNum; ++i)
		{
			Debug.DrawRay(origin, Vector2.up * velocity.y, Color.red);
			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * direction, rayLength, collisionLayers);
			if (hit)
			{
				rayLength = hit.distance;
				velocity.y = (rayLength - skinWidth) * direction;

				if ((collisions.state & CollisionState.CLIMB_SLOPE) != 0)
				{
					velocity.x = velocity.y / Mathf.Tan(collisions.terrainAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}

				collisions.state |= direction > 0
					? CollisionState.COLLIDE_ABOVE
					: CollisionState.COLLIDE_BELOW;
			}
			origin.x += horRaySpacing;
		}

		if ((collisions.state & CollisionState.CLIMB_SLOPE) != 0)
		{
			direction = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			origin = (direction < 0.0f ? relOrigins.bottomLeft : relOrigins.bottomRight);
			origin.y += velocity.y;
			origin += position;

			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, rayLength, collisionLayers);

			if (hit)
			{
				float slope_angle = Vector2.Angle(hit.normal, Vector2.up);
				if (slope_angle != collisions.terrainAngle)
				{
					velocity.x = (hit.distance - skinWidth) * direction;
					collisions.terrainAngle = slope_angle;
				}
			}
		}
	}
	//! Horisontal movement.
	void HorizontalCollisions(ref Vector2 velocity, Vector3 position)
	{
		if (velocity.x == 0.0f) {
			return;
		}

		Vector2 initial_velocity = velocity;
		DecendSlope(ref velocity, position);

		float rayLength = Mathf.Abs(velocity.x) + skinWidth;
		float direction = Mathf.Sign(velocity.x);
		Vector3 origin = direction > 0 ? relOrigins.bottomRight : relOrigins.bottomLeft;
		origin += position;

		for (uint i = 0; i < verRayNum; ++i)
		{
			Debug.DrawRay(origin, Vector2.right * velocity.x, Color.red);
			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, rayLength, collisionLayers);
			if (hit)
			{
				float terrainAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (i == 0 && terrainAngle <= maxClimbAngle) 
				{
					if ((collisions.state & CollisionState.DESCEND_SLOPE) != 0)
					{
						collisions.state &= ~CollisionState.DESCEND_SLOPE;
						velocity = initial_velocity;
					}

					float distance_to_slope = 0;
					if (terrainAngle != collisions.lastTerrainAngle)
					{
						distance_to_slope = (hit.distance - skinWidth) * direction;
						velocity.x -= distance_to_slope;
					}
					ClimbSlope(ref velocity, terrainAngle);
					velocity.x += distance_to_slope;
				}

				if ((collisions.state & CollisionState.CLIMB_SLOPE) == 0 || terrainAngle > maxClimbAngle)
				{
					rayLength = hit.distance;
					velocity.x = (rayLength - skinWidth) * direction;

					if ((collisions.state & CollisionState.CLIMB_SLOPE) != 0) {
						velocity.y = Mathf.Tan(collisions.terrainAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}

					collisions.state |= direction > 0
						? CollisionState.COLLIDE_RIGHT
						: CollisionState.COLLIDE_LEFT;
				}
			}
			origin.y += verRaySpacing;
		}
	}
	//! Calculates velocity if controller moving on slope.
	void ClimbSlope(ref Vector2 velocity, float angle)
	{
		float yDelta = Mathf.Abs(velocity.x) * Mathf.Sin(angle * Mathf.Deg2Rad);

		if (velocity.y <= yDelta)
		{
			velocity.y = yDelta;
			velocity.x *= Mathf.Cos(angle * Mathf.Deg2Rad);
			collisions.state |= CollisionState.COLLIDE_BELOW | CollisionState.CLIMB_SLOPE;
			collisions.terrainAngle = angle;
		}
	}
	//! Calculates velocity if controller decending on slope.
	void DecendSlope(ref Vector2 velocity, Vector3 position)
	{
		if (velocity.y >= 0.0f) {
			return;
		}

		float direction = Mathf.Sign(velocity.x);

		Vector3 rayOrigin = direction > 0 ? relOrigins.bottomLeft : relOrigins.bottomRight;
		rayOrigin += position;

		do
		{
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionLayers);
			
			if (!hit) {
				break;
			}

			float slope_angle = Vector2.Angle(hit.normal, Vector2.up);

			if (slope_angle == 0 || slope_angle > maxDecendAngle) {
				break;
			}

			if (Mathf.Sign(hit.normal.x) != direction) {
				break;
			}

			if (hit.distance - skinWidth > Mathf.Tan(slope_angle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
				break;
			}

			float y_delta = Mathf.Abs(velocity.x) * Mathf.Sin(slope_angle * Mathf.Deg2Rad);
			velocity.x *= Mathf.Cos(slope_angle * Mathf.Deg2Rad);
			velocity.y -= y_delta;

			collisions.state |= CollisionState.DESCEND_SLOPE | CollisionState.COLLIDE_BELOW;
			collisions.terrainAngle = slope_angle;
		} while (false);
	}

	//
	// Public interface.
	//
	// Start is called before the first frame update
	void Start()
	{
		collider = GetComponent<BoxCollider2D>();
		UpdateRaycastData();
	}
	//! Moves character in sertain diraction.
	public void Move(Vector2 velocity)
	{
		collisions.Reset();
		Vector3 startPos = transform.position;
		Vector3 stepPos = startPos;
		{
			Vector2 stepVelocity;

			Vector2 direction = velocity.normalized;
			float distance = velocity.magnitude;

			while (distance != 0)
			{
				if (distance > maxVelocityStep)
				{
					stepVelocity = direction * maxVelocityStep;
					distance -= maxVelocityStep;
				}
				else
				{
					stepVelocity = direction * distance;
					distance = 0.0f;
				}

				HorizontalCollisions(ref stepVelocity, stepPos);

				VerticalCollisions(ref stepVelocity, stepPos);

				stepPos.x += stepVelocity.x;
				stepPos.y += stepVelocity.y;
			}
		}
		
		transform.Translate(stepPos - startPos);
	}
	//! Returns state of collisions.
	public CollisionState GetCollisionState()
	{
		return collisions.state;
	}
	//! Returns angle of terrain controller colides with.
	public float GetTerrainAngle()
	{
		return collisions.terrainAngle;
	}
}
