using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks {
	bool isConnecting;

	[Tooltip("The maximum number of players per room")]
	[SerializeField]
	private byte maxPlayersPerRoom = 2;

	void Awake() {
		// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	private void Start() {
		Connect();
	}

	void Connect() {
		isConnecting = true;

		// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
		if (PhotonNetwork.IsConnected) {
			Debug.Log("Joining Room...");
			// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
			PhotonNetwork.JoinRandomRoom();
		} else {
			Debug.Log("Connecting...");

			// #Critical, we must first and foremost connect to Photon Online Server.
			PhotonNetwork.GameVersion = "1.0";
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	public override void OnConnectedToMaster() {
		// we don't want to do anything if we are not attempting to join a room. 
		// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
		// we don't want to do anything.
		if (isConnecting) {
			// LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
			Debug.Log("PUN Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

			// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
			PhotonNetwork.JoinRandomRoom();
		}
	}

	public override void OnJoinRandomFailed(short returnCode, string message) {
		Debug.Log("PUN Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

		// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
		PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
	}

	public override void OnDisconnected(DisconnectCause cause) {
		Debug.LogError("PUN Launcher:Disconnected");
		isConnecting = false;
	}

	public override void OnJoinedRoom() {
		Debug.Log("PUN Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

		// #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
		if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
			Debug.Log("We load the 'Room for Player 1' ");

			// Load the Room Game scene. 
			PhotonNetwork.LoadLevel("GameScene");

		}
	}
}
