﻿using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;

public class SocketInstance {
	public Socket sock;
	private List<Dictionary<string, float>> list;
	private static SocketInstance instance;

	public static SocketInstance GetInstance(){
		if (instance == null)
			instance = new SocketInstance ();
		return instance;
	}

	SocketInstance(){
		IPEndPoint ip = new IPEndPoint (IPAddress.Any, 8888);
		sock = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		sock.Bind (ip);
		list = new List<Dictionary<string, float>>();
		Debug.Log ("Listen 0.0.0.0:8888");

		Thread thread = new Thread (new ThreadStart (ReceiveSocket));
		thread.IsBackground = true;
		thread.Start ();
	}

	private void ReceiveSocket(){
		int recv;
		byte[] data = new byte[1024];
		IPEndPoint sender = new IPEndPoint (IPAddress.Any, 0);
		EndPoint Remote = (EndPoint)(sender);

		Dictionary<string, float> d;

		while (true) {
			recv = sock.ReceiveFrom (data, 1024, SocketFlags.None, ref Remote);
			DataParser(Encoding.ASCII.GetString(data,0,recv));
			d = Get();
			Debug.Log ("Recv: "+d["x"]+", "+d["y"]+", "+d["z"]+"["+list.Count+"]");
		}

	}

	private void DataParser(string recv){
		try{
			string[] spliteddata = new string[3];
			Dictionary<string, float> data = new Dictionary<String, float>();

			spliteddata = recv.Split (',');
			data.Add ("x", float.Parse(spliteddata [0]));
			data.Add ("y", float.Parse(spliteddata [1]));
			data.Add ("z", float.Parse(spliteddata [2]));

			list.Add (data);
		}catch (Exception e)
		{
			Debug.Log("data parser error: " + e );
		}
	}

	public Dictionary<string, float> Get(){
		Dictionary<string, float> d;
		if (list.Count > 0) {
			d = list [0];
			list.RemoveAt (0);
			return d;
		}
		return null;
	}
}