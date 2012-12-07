using UnityEngine;
using System.Collections;

public class Paddle : MonoBehaviour {
	
	float maxMovement     = 10
		, currentMovement = 0
		, bounds 		  = 5
		, speed 		  = 4
		;
	
	Vector3 lastPosition;
	
	Transform thisTransform;
	
	void Start () {
		thisTransform = transform;	
	}
	
	public void move( Vector2 pos ) {
		float movement = Mathf.Clamp(pos.y, -maxMovement, maxMovement);
		currentMovement = movement * Time.deltaTime * speed;
	}
	
	public void setPos( Vector2 pos ) {
		thisTransform.localPosition = new Vector3(thisTransform.localPosition.x, pos.y, 0);
	}
	
	void FixedUpdate () {
		if (currentMovement != 0) {
			thisTransform.localPosition = new Vector3(thisTransform.localPosition.x, Mathf.Clamp(thisTransform.localPosition.y + currentMovement, -bounds, bounds), thisTransform.localPosition.z);
			currentMovement = Mathf.Lerp(currentMovement, 0, Time.deltaTime * 10);
			Server.SendPosition (0x00, thisTransform.localPosition.x, thisTransform.localPosition.y);
		}
	}
}
