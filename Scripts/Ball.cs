using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	
	static Ball _this;
	
	Vector3 velocity;
	Transform thisTransform;
	Rigidbody thisRigidbody;
	float speed = 1;
	
	void Awake() {
		_this = this;	
	}
	
	void Start () {
		thisTransform = transform;
		thisRigidbody = rigidbody;
		if (Controller.Mode == Modes.Master) {
			setVelocity(5, 1);	
		}
	}
	
	void setVelocity(float x, float y) {
		if (Controller.Mode == Modes.Slave) return;
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
	
	public static void SetRemotePosition(float x, float y) {
		_this.thisTransform.localPosition = new Vector3(x, y, 0);	
	}
	
	void FixedUpdate() {
		if (Controller.Mode == Modes.Master) {
			Server.SendPosition(0x02, thisTransform.position.x, thisTransform.position.y);
		}
	}
}
