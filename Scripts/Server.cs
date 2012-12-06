using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class Server : MonoBehaviour {
	
	void Start () {
		BeginConnect("guildbros.com", 1337);
	}
	
	public static void BeginConnect(string host, int port)
	{
		// set up a new bsd socket for a streaming TCP/IP connection
		Socket sock = new 
			Socket(				
				AddressFamily.InterNetwork,
		 		SocketType.Stream, 
		 		ProtocolType.Tcp
				);  	
		
		// set up ascii encoding then use it to encode an HTTP GET request
		System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
    	byte[] bytes = encoding.GetBytes("GET / HTTP/1.1\n\n");
		
		// connect and then send (synchronously)
		sock.Connect(host, port);
		sock.Send(bytes);
		
		// console output
		Debug.Log ("Done");
	}
}