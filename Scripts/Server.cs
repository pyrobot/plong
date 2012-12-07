using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class Server : MonoBehaviour {
	
	static Server _this;
	
	void Awake () {
		_this = this;	
	}
	
	void Start () {
		int port;
		if (Controller.Mode == Modes.Master) {
			port = 1337;
		} else {
			port = 1338;	
		}
		BeginConnect("guildbros.com", port);
	}
	
	Socket thisSocket = null;
	
	public static void Send(Byte[] bytes) {
		if (_this.thisSocket != null) {
			_this.thisSocket.BeginSend(bytes, 0, bytes.Length, 0, SendCallback, 0);
		}
	}
	
	public static void SendCallback(IAsyncResult ar) {
		// Debug.Log ("sent");
	}
	
	public static void ConnectCallback(IAsyncResult ar)
	{		
	    _this.thisSocket = (Socket) ar.AsyncState;
	    _this.thisSocket.EndConnect(ar);
		Debug.Log ("connected");
	}
	
	public static void BeginConnect(string host, int port)
	{
	    IPAddress[] IPs = Dns.GetHostAddresses(host);
	
	    Socket s = new Socket(AddressFamily.InterNetwork,
	        SocketType.Stream,
	        ProtocolType.Tcp);
	    s.BeginConnect(IPs[0], port, 
	        new AsyncCallback(ConnectCallback), s);
	}
	
	void FixedUpdate() {
		if (_this.thisSocket != null) {
			if (_this.thisSocket.Available > 0) {
				Byte[] buf = new Byte[_this.thisSocket.Available];
				_this.thisSocket.Receive(buf, buf.Length, 0);
				if (buf[0] == 0x0A) {
					float xPos = BitConverter.ToSingle(new Byte[] { buf[2], buf[3], buf[4], buf[5] }, 0),
						   yPos = BitConverter.ToSingle(new Byte[] { buf[6], buf[7], buf[8], buf[9] }, 0);
					
					if (buf[1] == 0x01) {
						Controller.SetRemotePosition(xPos, yPos);
					} else if (buf[1] == 0x03) {
						Ball.SetRemotePosition(xPos, yPos);	
					}
					
				}				
			}
		}
	}
	
	public static void SendPosition(byte msg, float x, float y) {
		_this.sendPosition(msg, x, y);
	}
	
	void sendPosition(byte msg, float x, float y) {
		Byte[] x_bytes = BitConverter.GetBytes(x),
			   y_bytes = BitConverter.GetBytes(y);
		Byte[] bytes = new Byte[10];
		bytes[0] = 0x0A;
		bytes[1] = msg;
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
	
}