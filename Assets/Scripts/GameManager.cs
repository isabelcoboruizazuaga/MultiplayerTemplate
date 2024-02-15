using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    // Declaraci�n de Variables
    [SerializeField] private GameObject gameModeMenu;
    [SerializeField] private GameObject multiplayerMenu;
    [SerializeField] private GameObject newLobbyMenu;


    public event EventHandler OnLocalPlayerReadyChanged;
    public event EventHandler OnGameRadyChanged;


    // Para comenzar cuando todos los jugadores est�n preparados.
    private bool isLocalPlayerReady;
    private Dictionary<ulong, bool> playerReadyDictionary;
    public NetworkVariable<bool> gameReady;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        isLocalPlayerReady = false;

        playerReadyDictionary = new Dictionary<ulong, bool>();
        gameReady = new NetworkVariable<bool>(false);
    }


    // Configuraci�n inical de men�s 
    void Start()
    {

        OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
        OnGameRadyChanged += GameManager_OnGameReadyChanged;

        // Activamos el men� inicial
        gameModeMenu.SetActive(true);


        // Desactivamos el resto de men�s
        multiplayerMenu.SetActive(false);
        newLobbyMenu.SetActive(false);

    }


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += GameManager_OnLoadEventCompleted;
        }

    }



    private void GameManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "GameScene")
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                //Transform playerTransform = Instantiate(playerPrefab);
                //playerTransform.GetComponent<PlayerController>().PlayerSetUp(clientId);
                //playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

            }
        }

    }




    private void GameManager_OnGameReadyChanged(object sender, EventArgs e)
    {
        if (gameReady.Value)
        {
            GameManagerLoadSeceneServerRpc();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void GameManagerLoadSeceneServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }


    private void GameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        Debug.Log("Detectamos evento OnLocalPlayerReadyChanged");
        SetPlayerReadyServerRpc();
    }


    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public void SetPlayerReady()
    {
        isLocalPlayerReady = true;
        SetPlayerReadyServerRpc();
        OnLocalPlayerReadyChanged(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {

        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {

            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }

        }

        if (allClientsReady)
        {
            gameReady.Value = true;
            OnGameRadyChanged?.Invoke(this, EventArgs.Empty);
        }


    }




}
