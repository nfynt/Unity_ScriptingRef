using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : Photon.PunBehaviour {

	public static string appVersion="1.0";
	public static int maxPlayers = 4;
	public static string roomName = "default";

	void Start()
	{
		PhotonNetwork.ConnectUsingSettings (appVersion);
	}

	public override void OnConnectedToPhoton ()
	{
		Debug.Log ("Connected to photon server");
	}

	public override void OnConnectedToMaster ()
	{
		Debug.Log ("Connected to master");

		PhotonNetwork.JoinRandomRoom ();
	}

	public override void OnConnectionFail (DisconnectCause cause)
	{
		Debug.Log ("Connection failed!");
	}

	public override void OnCreatedRoom ()
	{
		Debug.Log ("Room created with name: " + PhotonNetwork.room.Name);
	}

	public override void OnPhotonRandomJoinFailed (object[] codeAndMsg)
	{
		Debug.Log ("Failed to joing random room");

		RoomOptions roomOpt = new RoomOptions ();
		roomOpt.MaxPlayers = (byte)maxPlayers;
		roomOpt.IsVisible = true;	//makes the room available in lobby and  other to see
		roomOpt.IsOpen = true;		//if others could join this room

		PhotonNetwork.CreateRoom (roomName, roomOpt, TypedLobby.Default);
	}

	public override void OnJoinedRoom ()
	{
		Debug.Log ("Joined Room: " + PhotonNetwork.room.Name);
		PhotonNetwork.Instantiate ("NetworkPlayer", new Vector3 (Random.Range (0, 5), 1, Random.Range (0, 5)), Quaternion.identity, 0);
	}

}
