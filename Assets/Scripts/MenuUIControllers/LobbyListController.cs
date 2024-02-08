using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    private Lobby lobby;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            LobbyManager.Instance.JoinWithId(lobby.Id);
        });
    }

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        Debug.Log(lobby.Id);
        lobbyNameText.text = lobby.Name;
    }
}
