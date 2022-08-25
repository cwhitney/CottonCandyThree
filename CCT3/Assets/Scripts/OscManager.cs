using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscManager : MonoBehaviour
{
	public string host = "127.0.0.1";
	public int port = 8000;

	public OSC mOsc;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	public void Open() {
		mOsc.outIP = host;
		mOsc.outPort = port;
		mOsc.Open();
	}

	public void Close() {
		mOsc.Close();
	}

	// returns if the values have changed at all
	public bool updateClientInfo(string hostIP, int hostPort) {
		if(hostIP != host || hostPort != port) {
			host = hostIP;
			port = hostPort;

			//mOsc.Close();
			//Open();
			return true;
		}
		return false;
	}

	public void SendStart() {
		OscMessage message = new OscMessage();
		message.address = "/start";
		message.values.Add(1);
		mOsc.Send(message);
	}

	public void SendStop() {
		OscMessage message = new OscMessage();
		message.address = "/stop";
		message.values.Add(1);
		mOsc.Send(message);
	}

	void SendWandDirSpeed( float speed ) {

	}
}
