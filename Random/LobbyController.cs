using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shubham.Networking
{
    public class LobbyController : Singleton<LobbyController>
    {
        public GameObject homePanel;
        [Space(10)]
        [Header("Host Panel Component")]
        public GameObject hostPanel;
        public Button createBtn;
        [Space(10)]
        [Header("Join Panel Component")]
        public GameObject joinPanel;
        public Button joinBtn;
        public GameObject roomListContainer;
        [Space(10)]
        [Header("Game Panel Component")]
        public GameObject gamePanel;
        public Button gameStartBtn;
        public GameObject playerListContainer;
        [HideInInspector]
        public NetworkManager netManager;


        private void Start()
        {
            SwitchToHomePanel();
        }

        private void OnDisable()
        {
            NetworkManager.RoomJoined -= ShowUsersList;
            NetworkManager.NewPlayerJoined -= RefreshPlayersList;
        }

        public void CreateRoom(InputField roomName)
        {
            NetworkManager.RoomJoined += ShowUsersList;
            netManager.StartOrJoinRoom(roomName.text, true);
        }

        public void JoinRoom(int index)
        {
            NetworkManager.RoomJoined += ShowUsersList;
            netManager.StartOrJoinRoom(netManager.roomList[index], false);
        }

        public void ShowUsersList(bool success)
        {
            NetworkManager.RoomJoined -= ShowUsersList;
            if (success)
            {
               SwitchToGamePanel();
            }
            else
            {
                ShowNotification("Failed to join room");
            }
        }

        public void ShowNotification(string msg)
        {
            Debug.LogError(msg);
        }

        public void ToggleButtonInteractable(Button btn)
        {
            btn.interactable = !btn.interactable;
        }

        public void SwitchToHomePanel()
        {
            hostPanel.SetActive(false);
            joinPanel.SetActive(false);
            homePanel.SetActive(true);
            gamePanel.SetActive(false);
        }

        public void SwitchToHostPanel()
        {
            hostPanel.SetActive(true);
            joinPanel.SetActive(false);
            homePanel.SetActive(false);

        }

        public void SwitchToJoinPanel()
        {
            hostPanel.SetActive(false);
            joinPanel.SetActive(true);
            homePanel.SetActive(false);
            PopulateRoomList();
        }
        
        public void StartGame()
        {
            NetworkManager.NewPlayerJoined -= RefreshPlayersList;
            //PhotonNetwork.LoadLevel("scene name");
        }

        public void RefreshPlayersList(PhotonPlayer pp)
        {
            PopulateParticipantsList();
        }

        void SwitchToGamePanel()
        {
            hostPanel.SetActive(false);
            joinPanel.SetActive(false);
            gamePanel.SetActive(true);

            gameStartBtn.onClick.RemoveAllListeners();
            gameStartBtn.onClick.AddListener(StartGame);

            if (netManager.IsHost)
                gameStartBtn.interactable = true;
            else
                gameStartBtn.interactable = false;

            PopulateParticipantsList();

            NetworkManager.NewPlayerJoined += RefreshPlayersList;
        }

        void PopulateParticipantsList()
        {
            netManager.RefreshParticipantsList(netManager.RoomName);

            int cnt = netManager.ParticipantsCount;

            if (cnt == 0)
            {
                playerListContainer.transform.GetChild(0).GetChild(0).GetComponent<Button>().interactable = false;
                playerListContainer.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "NA";
                return;
            }

            for (int i = playerListContainer.transform.childCount - 1; i > 0; i--)
                Destroy(playerListContainer.transform.GetChild(i));

            for (int i = 0; i < cnt; i++)
            {
                if (i == 0)
                {
                    if (netManager.IsHost)
                    {
                        playerListContainer.transform.GetChild(0).GetChild(0).GetComponent<Button>().interactable = false;
                        //playerListContainer.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                        //playerListContainer.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
                        //{
                        //    PhotonPlayer pp = netManager.GetPlayerFromIndex(i);
                        //    netManager.KickPlayerOut(pp);
                        //});
                        playerListContainer.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = netManager.participantsList[i]+" (Host)";
                    }
                    else
                    {
                        playerListContainer.transform.GetChild(0).GetChild(0).GetComponent<Button>().interactable = false;
                        playerListContainer.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = netManager.participantsList[i] + " (Host)";
                    }
                }
                else
                {
                    GameObject btn = Instantiate(playerListContainer.transform.GetChild(0).gameObject, playerListContainer.transform) as GameObject;
                    
                    if (netManager.IsHost)
                    {
                        btn.transform.GetChild(0).GetComponent<Button>().interactable = true;
                        btn.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                        btn.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
                        {
                            PhotonPlayer pp = netManager.GetPlayerFromIndex(i);
                            netManager.KickPlayerOut(pp);
                        });
                        btn.transform.GetChild(1).GetComponent<Text>().text = netManager.participantsList[i];
                    }
                    else
                    {
                        btn.transform.GetChild(0).GetComponent<Button>().interactable = false;
                        btn.transform.GetChild(1).GetComponent<Text>().text = netManager.participantsList[i];
                    }
                }
            }//end for loop
        }//end populate participants list func

        void PopulateRoomList()
        {
            netManager.RefreshRoomList();

            int roomCnt = netManager.RoomCount;
            if (roomCnt == 0)
            {
                roomListContainer.transform.GetChild(0).GetComponent<Button>().interactable = false;
                roomListContainer.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "NA";
                return;
            }

            for (int i = roomListContainer.transform.childCount-1; i >0; i--)
                Destroy(roomListContainer.transform.GetChild(i));

            for(int i=0;i<roomCnt;i++)
            {
                if(i==0)
                {
                    int ind = i;
                    roomListContainer.transform.GetChild(0).GetComponent<Button>().interactable = true;
                    roomListContainer.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                    roomListContainer.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { JoinRoom(ind); });
                    roomListContainer.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = netManager.roomList[i]+"("+netManager.roomPlayerCnt[i].ToString()+")";

                }
                else
                {
                    int ind = i;
                    GameObject btn = Instantiate(roomListContainer.transform.GetChild(0).gameObject,roomListContainer.transform) as GameObject;
                    btn.GetComponent<Button>().interactable = true;
                    btn.GetComponent<Button>().onClick.RemoveAllListeners();
                    btn.GetComponent<Button>().onClick.AddListener(delegate { JoinRoom(ind); });
                    btn.transform.GetChild(0).GetComponent<Text>().text = netManager.roomList[i] + "(" + netManager.roomPlayerCnt[i].ToString() + ")";

                }
            }
        }//end Populate room list func
    }
}
