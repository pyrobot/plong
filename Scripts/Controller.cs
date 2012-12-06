using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
		
	public Paddle rightPaddle, leftPaddle;
	public Ball ball;
	
	Paddle playerPaddle;
	
	void Start () {
		TouchMonitor.Register(onTouch);
		playerPaddle = rightPaddle;
	}
	
	void onTouch( TouchEvent e ) {
		if (e.type == TouchType.Start || e.type == TouchType.Hold ) {
			rightPaddle.move(e.deltaPosition);			
		} else if (e.type == TouchType.Stop) {
			// todo..
		}		
	}
	
}
