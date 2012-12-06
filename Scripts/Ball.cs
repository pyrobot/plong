using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	
	Vector3 velocity;
	Transform thisTransform;
	Rigidbody thisRigidbody;
	float speed = 1;
	
	void Start () {
		thisTransform = transform;
		thisRigidbody = rigidbody;
		setVelocity(5, 1);	
	}
	
	void setVelocity(float x, float y) {
		velocity = new Vector3(x,y,0);
		thisRigidbody.velocity = velocity;
	}
	
	void OnCollisionEnter(Collision col) {
		Collider objHit = col.collider;
		
		if (objHit.tag == "reflectY") {
			setVelocity(velocity.x, -velocity.y);	
		} else if (objHit.tag == "reflectX" || objHit.tag == "kill") {
			setVelocity(-velocity.x, velocity.y);	
		}
	}
	
	void Update () {
		
	}
}
