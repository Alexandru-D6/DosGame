using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField roomInputField;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject roomPanel;
    [SerializeField] TMP_Text roomName;

    [SerializeField] RoomItem roomItemPrefab;
    [SerializeField] Transform contentObject;
    List<RoomItem> roomItemsList = new List<RoomItem>();

    [SerializeField] float timeBetweenUpdates = 1.5f;
    float nextTimeUpdate;

    private List<PlayerItem> playerItemsList = new List<PlayerItem>();
    [SerializeField] PlayerItem playerItemPrefab;
    [SerializeField] Transform playerItemListing;

    [SerializeField] GameObject playButton;
    [SerializeField] int minPlayers = 1;

    public void Awake() {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
    }
    public void Start() {
        PhotonNetwork.JoinLobby();
    }

    public void OnClickCreate() {
        if (roomInputField.text.Length >= 3) {
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 4, BroadcastPropsChangeToAll = true});
        }
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> _roomList) {
        base.OnRoomListUpdate(_roomList);
        if (Time.time >= nextTimeUpdate) {
            UpdateRoomList(_roomList);
            nextTimeUpdate = Time.time + timeBetweenUpdates;
        }
    }

    private void UpdateRoomList(List<RoomInfo> _list) {
        foreach( RoomItem item in roomItemsList) {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in _list) {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoom(string _roomName) {
        PhotonNetwork.JoinRoom(_roomName);
    }

    public void OnClickLeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        base.OnLeftRoom();
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    private void UpdatePlayerList() {
        foreach(PlayerItem item in playerItemsList) {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom != null) {
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players) {
                PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemListing);
                newPlayerItem.SetPlayerInfo(player.Value);
                playerItemsList.Add(newPlayerItem);

                if (player.Value == PhotonNetwork.LocalPlayer) {
                    newPlayerItem.ApplyLocalChanges();
                }
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player _newPlayer) {
        base.OnPlayerEnteredRoom(_newPlayer);
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player _otherPlayer) {
        base.OnPlayerLeftRoom(_otherPlayer);
        UpdatePlayerList();
    }

    private void Update() {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= minPlayers) {
            playButton.SetActive(true);
        }else {
            playButton.SetActive(false);
        }
    }

    public void OnClickPlayButton() {
        PhotonNetwork.LoadLevel("Game");
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }
}
