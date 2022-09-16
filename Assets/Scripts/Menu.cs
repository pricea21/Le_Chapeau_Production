using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{
	[Header("Screens")]
	public GameObject mainScreen;
	public GameObject lobbyScreen;

	[Header("Main Screen")]
	public Button createRoomButton;
	public Button joinRoomButton;

	[Header("Lobby Screen")]
	public TextMeshProUGUI playerListText;
	public Button startGameButton;

	void Start()
	{
		//disable buttons at the start as we are not connected to the server yet
		createRoomButton.interactable = false;
		joinRoomButton.interactable = false;
	}

	public override void OnConnectedToMaster()
	{
		createRoomButton.interactable = true;
		joinRoomButton.interactable = true;
	}

	void SetScreen (GameObject Screen)
	{
		//deactivate all screens
		mainScreen.SetActive(false);
		lobbyScreen.SetActive(false);

		//enable requested screen
		Screen.SetActive(true);
	}

	public void OnCreateRoomButton (TMP_InputField roomNameInput)
	{
		NetworkManager.instance.CreateRoom(roomNameInput.text);
	}

	public void OnJoinRoomButton (TMP_InputField roomNameInput)
	{
		NetworkManager.instance.JoinRoom(roomNameInput.text);
	}

	public void OnPlayerNameUpdate (TMP_InputField playerNameInput)
	{
		PhotonNetwork.NickName = playerNameInput.text;
	}

	public override void OnJoinedRoom ()
	{
		SetScreen(lobbyScreen);

		photonView.RPC("UpdateLobbyUI", RpcTarget.All);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		UpdateLobbyUI();
	}

	[PunRPC]
	public void UpdateLobbyUI()
	{
		playerListText.text = "";

		//display all the players currently in the lobby
		foreach(Player player in PhotonNetwork.PlayerList)
		{
			playerListText.text += player.NickName + "\n";
		}

		//only the host can start the game
		if(PhotonNetwork.IsMasterClient)
			startGameButton.interactable = true;
		else
			startGameButton.interactable = false;
	}

	public void OnLeaveLobbyButton ()
	{
		PhotonNetwork.LeaveRoom();
		SetScreen(mainScreen);
	}

	public void OnStartGameButton()
	{
		NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
	}
}
