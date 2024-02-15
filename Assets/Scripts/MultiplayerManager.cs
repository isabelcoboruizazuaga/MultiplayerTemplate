using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    public const int MAX_PLAYER_AMOUNT = 4;
    private const string PLAYER_NAME = "NombreJugador";
    public static MultiplayerManager Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;
    public event EventHandler ConnectionApprovalCallBack;

    private NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //Se crea una nueva lista de datos de jugadores actualizable
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;

    }

    /**
     * Devuelve el nombre del jugador
     * */
    public string GetPlayerName()
    {
        return playerName;
    }


    /**
     * Setea el nombre del jugador
     * */
    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_NAME, playerName);
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    /**
     * Comienza a hostear el juego y carga la escena de Juego
     * */
    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    /**
     * Asigna el id del cliente al unirse este
     * */
    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
            skinIndex = 0,
            color = Color.white
        });
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }


    /**
     * Elimina el cliente de la lista de jugadores cuando este se desconecta
     * */
    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientId == clientId)
            {
                // Disconnected!
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }


    /**
     * Comprueba que quede espacio en la sala antes de unirse el nuevo jugador
     * */
    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "La sala está completa";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }


    /*
     * Desconexión del cliente desde el lado del cliente
     */
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    /*
     * Conexión del cliente desde el lado del cliente
     */
    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    /**
     * Conexión de cliente
     * */
    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    /**
     * Desconexión de cliente al fallar la conexión
     * */
    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string name, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.playerName = name;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }


    /**
     * Comprueba si un jugador está unido a la sala
     * */
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return (playerIndex < playerDataNetworkList.Count);
    }

    /**
     * Obtiene el índice de un jugador dado su id
     * */
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
                return i;
        }
        return -1;
    }


    /**
     * Obtiene los datos de un jugador dado su id
     * */
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
                return playerData;
        }
        return default;
    }

    /**
     * Obtiene el índice del jugador actual
     * */
    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }


    /**
     * Obtiene los datos de un jugador dado su índice
     * */
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    /*
     * Obtiene la skin de un jugador dado su índice
     */
    public int GetPlayerSkinIndexFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex].skinIndex;
    }


    /*
     * Cambia la skin de un jugador dado su índice
     */
    public void SetPlayerSkin(int skinIndex)
    {
        ChangePlayerSkinServerRpc(skinIndex);
    }

    /**
     * Cambia la skin de un jugador a la dada
     * */
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerSkinServerRpc(int skinIndex, ServerRpcParams serverRpcParams = default)
    {

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.skinIndex = skinIndex;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    /*
     * Cambia el color del jugador
     */
    public void SetPlayerColor(Color color)
    {
        SetPlayerColorServerRpc(color);
    }

    /*
     * Cambia el color de un jugador en el servidor
     */
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerColorServerRpc(Color color, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.color = color;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    /*
     *Comprueba que el jugador esté listo
     */
    public void IsPlayerReady(bool isReady)
    {
        IsPlayerReadyServerRpc(isReady);
    }

    /*
     *Comprueba que el jugador ésté listo desde el sevidor 
     */

    [ServerRpc(RequireOwnership = false)]
    private void IsPlayerReadyServerRpc(bool isReady, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.isPlayerReady = isReady;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    /*
     *Echa a un jugador del lobby
     */
    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }

}
