using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class MovementComponent : MonoBehaviour
{
	//! Controller.
	protected Controller2D controller;
	//! Children movement components.
	List<MovementComponent> children = new List<MovementComponent>();

	//
	// Public interface.
	// 
	//! Adds component to movement children.
	public void AddChildMovement(MovementComponent component)
	{
		children.Add(component);
	}
	//! Removes component from movement children.
	public void RemoveChildMovement(MovementComponent component)
	{
		children.Remove(component);
	}

	// Start is called before the first frame update
	public void Start()
    {
		controller = GetComponent<Controller2D>();
	}

	private void OnDestroy()
	{
		for (int i = 0; i < children.Count; ++i) {
			Destroy(children[i]);
		}
	}
}
