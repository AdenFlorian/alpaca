using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
	[Range(0, 100)]
	public float MaxInteractionDistance;
	public Transform RayOrigin;
	public Player Player;

	SpacePlane HighlightedSpacePlane;

	void Awake()
	{
	}

	void Start()
	{
        if (Player.NetObjGene.IsLocalPlayer == false)
        {
            this.enabled = false;
        }
	}

	void Update()
	{
		Ray ray = new Ray(RayOrigin.position, RayOrigin.forward * MaxInteractionDistance);

		//Debug.DrawRay(ray.origin, ray.direction * MaxInteractionDistance, Color.red, 0.5f);

		RaycastHit hitInfo;
		var hitSomething = Physics.Raycast(ray, out hitInfo, MaxInteractionDistance);
		if (hitSomething)
		{
			OnHitSomething(hitInfo);
		}

		if (Input.GetKeyDown(KeyCode.F) && HighlightedSpacePlane != null)
		{
			Player.GetInPlane(HighlightedSpacePlane);
		}
	}

	void OnHitSomething(RaycastHit hitInfo)
	{
        //MyLogger.LogInfo("Hit something...");
		var spacePlane = hitInfo.collider.GetComponentInParent<SpacePlane>();
		if (spacePlane != null)
		{
			OnHitSpacePlane(spacePlane);
		}
	}

	void OnHitSpacePlane(SpacePlane spacePlane)
	{
		spacePlane.OnPointedAt();
		HighlightedSpacePlane = spacePlane;
		//MyLogger.LogInfo("Hit spaceplane!");
		spacePlane._disableInput = false;
	}
}
