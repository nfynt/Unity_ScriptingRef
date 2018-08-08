using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

namespace Shubham.Networking
{
    public class NetworkManager : Photon.PunBehaviour
    {
        public delegate void OnConnectedToServer(bool success);
        public delegate void OnRoomJoined(bool success);
        public delegate void OnNewPlayerJoined(PhotonPlayer player);

        public static event OnConnectedToServer ConnectedToPhoton;
        public static event OnRoomJoined RoomJoined;
        public static event OnNewPlayerJoined NewPlayerJoined;

        
        /// <summary>
        /// List populated after connecting to photon server, listing out all the available and public rooms
        /// </summary>
        public List<string> roomList = new List<string>();
        public List<int> roomPlayerCnt = new List<int>();

        public List<string> participantsList = new List<string>();

        #region getter_setter methods

        public bool IsHost
        {
            get { return isHost; }
        }

        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }

        public string RoomName
        {
            get { return roomName; }
        }

        public int RoomCount
        {
            get { return roomList.Count; }
        }

        public int ParticipantsCount
        {
            get { return participantsList.Count; }
        }

        #endregion

        /// <summary>
        /// To maintain the server versions
        /// </summary>
        private string gameVersion = "v1.0";
        /// <summary>
        /// The room name used to connect or join game server
        /// </summary>
        private string roomName;
        /// <summary>
        /// Flag value true is the user is host of current session
        /// </summary>
        private bool isHost = false;
        /// <summary>
        /// Player name to identify him in room
        /// </summary>
        private string playerName = "default";

        private void Awake()
        {
            PhotonNetwork.automaticallySyncScene = true;

            if (LobbyController.Instance.netManager == null)
                LobbyController.Instance.netManager = this;
        }

        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings(gameVersion);
            if (PlayerPrefs.HasKey("PlayerName"))
            {
                PlayerName = PlayerPrefs.GetString("PlayerName");
            }
            else
            {
                PlayerName = "Test_" + Random.Range(100, 999).ToString();
                PlayerPrefs.SetString("PlayerName", playerName);
            }
        }

        /// <summary>
        /// Call this to Host room if isHost or else join as attendee
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="isHost"></param>
        public void StartOrJoinRoom(string roomName, bool isHost=false)
        {
            this.roomName = roomName;
            this.isHost = isHost;
            if (isHost)
            {
                RoomOptions opts = new RoomOptions();
                opts.IsVisible = true;
                opts.MaxPlayers = 10;
                opts.IsOpen = true;
                PhotonNetwork.CreateRoom(roomName, opts, TypedLobby.Default);
            }
            else
            {
                PhotonNetwork.JoinRoom(roomName);
            }
}

        public void RefreshRoomList()
        {
            RoomInfo[] info = PhotonNetwork.GetRoomList();
            roomList.Clear();
            roomPlayerCnt.Clear();
            foreach(RoomInfo ri in info)
            {
                roomList.Add(ri.Name);
                roomPlayerCnt.Add(ri.PlayerCount);
            }
        }

        public void RefreshParticipantsList(string roomName)
        {
            PhotonPlayer[] playerInfo = PhotonNetwork.playerList;

            participantsList.Clear();

            foreach (PhotonPlayer pp in playerInfo)
            {
                participantsList.Add(pp.NickName);
            }
        }
        
        public void KickPlayerOut(PhotonPlayer player)
        {
            PhotonNetwork.CloseConnection(player);
        }

        public PhotonPlayer GetPlayerFromIndex(int participantIndex)
        {
            PhotonPlayer[] playerInfo = PhotonNetwork.playerList;
            return playerInfo[participantIndex];
        }

        #region pun_overrides

        public override void OnConnectedToPhoton()
        {
            Debug.Log("Connected to Photon");
            RefreshRoomList();
            PhotonNetwork.playerName = this.playerName;
            if (ConnectedToPhoton != null)
                ConnectedToPhoton.Invoke(true);
        }

        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            Debug.Log("Failed to connect to Photon: "+cause.ToString());
            if (ConnectedToPhoton != null)
                ConnectedToPhoton.Invoke(false);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined room successfully");
            if (RoomJoined != null)
                RoomJoined.Invoke(true);
        }

        public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
        {
            Debug.Log("Failed to join room. " + codeAndMsg[1].ToString());
            if (RoomJoined != null)
                RoomJoined.Invoke(false);
        }

        public override void OnDisconnectedFromPhoton()
        {
            Debug.Log("Disconnected from Photon");
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            Debug.Log("New player joined: " + newPlayer.NickName);
            if (NewPlayerJoined != null)
                NewPlayerJoined.Invoke(newPlayer);
        }

        #endregion

    }//end of Network manager class

}//end of namespace