using UnityEngine;
using System;
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
			if (e.deltaPosition.x != 0 || e.deltaPosition.y != 0) {
				Byte[] x_bytes = BitConverter.GetBytes(e.deltaPosition.x),
					   y_bytes = BitConverter.GetBytes(e.deltaPosition.y);
				Byte[] bytes = new Byte[10];
				bytes[0] = 0x0A;
				bytes[1] = 0x00;
				bytes[2] = x_bytes[0];
				bytes[3] = x_bytes[1];
				bytes[4] = x_bytes[2];
				bytes[5] = x_bytes[3];
				bytes[6] = y_bytes[0];
				bytes[7] = y_bytes[1];
				bytes[8] = y_bytes[2];
				bytes[9] = y_bytes[3];
				Server.Send(bytes);
			}
		} else if (e.type == TouchType.Stop) {
			// todo..
		}		
	}
	
}
