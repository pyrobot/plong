using UnityEngine;
using System;
using System.Collections;

public enum Modes {
	Master,
	Slave
}

public class Controller : MonoBehaviour {
	static Controller _this;
	
	public Paddle rightPaddle, leftPaddle;
	public Ball ball;
	
	public Modes mode;
	
	Paddle playerPaddle, remotePaddle;
	
	public static Modes Mode { get { return _this.mode; } }
	
	void Awake() {
		_this = this;	
	}
	
	void Start () {
		TouchMonitor.Register(onTouch);
		if (mode == Modes.Master) {
			playerPaddle = rightPaddle;
			remotePaddle = leftPaddle;
		} else {
			playerPaddle = leftPaddle;
			remotePaddle = rightPaddle;
		}
	}
	
	void onTouch( TouchEvent e ) {
		if (e.type == TouchType.Start || e.type == TouchType.Hold ) {
			playerPaddle.move(e.deltaPosition);
		} 		
	}	
	
	/*
	void moveRemote(float x, float y) {
		remotePaddle.move(new Vector3(x, y, 0));
	}
	
	public static void MoveRemote(float x, float y) {
		_this.moveRemote(x, y);
	}
	*/
	
	public static void SetRemotePosition(float x, float y) {
		_this.remotePaddle.setPos(new Vector2(-10, y));
	}
	
}
