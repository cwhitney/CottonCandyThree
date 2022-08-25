using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Threading;
using UnityEngine;

using System.IO.Ports;
using System;

namespace sharkbox {

	public struct PortInfo {
		public PortInfo(string name, string description = "") {
			Name = name;
			Description = description;
		}
		public string Name { get; }
		public string Description { get; }
	};

	public class SerialManager {

		public delegate void SerialConnectAction(object sender, EventArgs e);
		public event SerialConnectAction OnSerialConnected;

		public delegate void serialMessageDelegate(string msg);
		public serialMessageDelegate OnSerialMessage;

		//public delegate void DeviceConnectAction(object sender, EventArgs e);
		//public event DeviceConnectAction OnDeviceConnected;

		//public delegate void DeviceDisconnectAction();
		//public event DeviceDisconnectAction OnDeviceDisconnected;

		private const int DEFAULT_BAUD_RATE = 115200;
		private SerialPort serial = new SerialPort();

		private Thread deviceThread, mSerialEnqueueThread, mSerialDequeThread;
		private bool threadIsActive = true;
		private List<PortInfo> portList, prevPortList;
		ConcurrentQueue<string> mReceiveBuffer = new ConcurrentQueue<string>();
		ConcurrentQueue<string> mSendQueue = new ConcurrentQueue<string>();

		public int LinesAvailable {
			get { return mReceiveBuffer.Count; }
		}

		public SerialManager() {
			portList = new List<PortInfo>();

			//	deviceThread = new Thread(new ThreadStart(ThreadPollDevices));
			//	deviceThread.Start();

			mSerialEnqueueThread = new Thread(new ThreadStart(ThreadReadSerial));
			mSerialEnqueueThread.IsBackground = true;
		}

		public void Stop() {
			Debug.Log("SerialManager Close");
			threadIsActive = false;
			//	deviceThread.Join();
			mSerialEnqueueThread.Join();

			serial.Close();
		}

		public List<PortInfo> GetPorts() {
			portList.Clear();

			string[] ports = SerialPort.GetPortNames();
			foreach (string s in ports) {
				portList.Add(new PortInfo(s));
			}

			return portList;
		}

		public List<PortInfo> GetPortsDetail() {
			string dataPath = Application.dataPath + "/bin/GetSerialInfo";

			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.EnableRaisingEvents = false;
			proc.StartInfo.FileName = dataPath;
			proc.StartInfo.CreateNoWindow = true;
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.RedirectStandardOutput = true;
			proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
			proc.Start();
			string output = proc.StandardOutput.ReadToEnd();
			proc.WaitForExit();

			char[] charSeparators = new char[] { '\n' };
			string[] ports = output.Trim().Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries);

			portList.Clear();
			foreach (string p in ports) {
				string[] portDesc = p.Trim().Split(',');

				Debug.Log("port descriptions and shit???");

				portList.Add(new PortInfo(portDesc[0], portDesc[1]));
			}

			return portList;
		}

		public void Connect(string port, int baudRate = DEFAULT_BAUD_RATE) {

			Debug.Log("Connect called");

			if (serial.IsOpen) {
				serial.Close();
			}
			if (mSerialEnqueueThread.IsAlive) {
				mSerialEnqueueThread.Join();
			}

			serial.PortName = port;
			serial.BaudRate = baudRate;
			serial.ReadTimeout = 10;
			serial.WriteTimeout = 50;
			serial.Parity = Parity.None;
			serial.Open();

			mSerialEnqueueThread.Start();
			OnSerialConnected?.Invoke(this, EventArgs.Empty);
		}

		private void ThreadReadSerial() {
			while (threadIsActive) {

				// SEND
				while (mSendQueue.Count > 0) {
					bool success = mSendQueue.TryDequeue(out string cmd);
					if (success) {
						serial.WriteLine(cmd);
					}
				}

				// RECEIVE
				try {
					string str = serial.ReadLine();
					if (str != "") {
						mReceiveBuffer.Enqueue(str);
					}
				} catch (Exception e) {
					//	Debug.LogException(e);
				}

				Thread.Sleep(500);
			}
		}

		public string ReadLine() {
			if (mReceiveBuffer.Count > 0) {
				bool s = mReceiveBuffer.TryDequeue(out string localStr);
				if (s) return localStr;
			}

			return string.Empty;
		}

		public void WriteLine(string line) {
			mSendQueue.Enqueue(line);
		}

		private void ThreadPollDevices() {
			while (threadIsActive) {
				// poll devices, save and compare to previous

				// if new devices are found, invoke events

				Thread.Sleep(1000);
			}
		}
	}

}