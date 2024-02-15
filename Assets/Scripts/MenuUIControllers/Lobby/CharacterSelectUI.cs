using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{

    [SerializeField] private Button salirBtn;
    [SerializeField] private Button readyBtn;
    [SerializeField] private Button skin1Btn;
    [SerializeField] private Button skin2Btn;
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text lobbyCode;

    // Botones de los colores
    [SerializeField] private Button noColorButton;
    [SerializeField] private Button whiteButton;
    [SerializeField] private Button blueButton;
    [SerializeField] private Button redButton;
    [SerializeField] private Button greenButton;
    [SerializeField] private Button purpleButton;

    private void Awake()
    {
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;


        lobbyName.text = LobbyManager.Instance.GetLobby().Name;
        lobbyCode.text = LobbyManager.Instance.GetLobby().LobbyCode;

        salirBtn.onClick.AddListener(() =>
        {
            LobbyManager.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MenuScene");
        });

        readyBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.SetPlayerReady();
           // MultiplayerManager.Instance.IsPlayerReady(true);
            readyBtn.interactable = false;
            var colors = readyBtn.colors;
            colors.disabledColor = new Color(0.5f, 1, 0, 1);
            Debug.Log("Player Ready: " + GameManager.Instance.IsLocalPlayerReady());
            Debug.Log("Game Ready: " + GameManager.Instance.gameReady.Value);

        });

        // Cambio de Skin
        skin1Btn.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.SetPlayerSkin(0);
        });

        skin2Btn.onClick.AddListener(() =>
        {
           MultiplayerManager.Instance.SetPlayerSkin(1);
        });



        // Cambio de Color
        noColorButton.onClick.AddListener(() =>
        {
            Debug.Log("a?");
            MultiplayerManager.Instance.SetPlayerColor(new Color(1, 1, 1, 0.5f));
        });

        whiteButton.onClick.AddListener(() =>
        {
           MultiplayerManager.Instance.SetPlayerColor(new Color(1, 1, 1, 1));
        });

        blueButton.onClick.AddListener(() =>
        {
           // MultiplayerManager.Instance.SetPlayerColor(new Color(0, 0, 1, 1));
        });

        redButton.onClick.AddListener(() =>
        {
            //MultiplayerManager.Instance.SetPlayerColor(new Color(1, 0, 0, 1));
        });

        greenButton.onClick.AddListener(() =>
        {
            //MultiplayerManager.Instance.SetPlayerColor(new Color(0, 1, 0, 1));
        });

        purpleButton.onClick.AddListener(() =>
        {
            //MultiplayerManager.Instance.SetPlayerColor(new Color(1, 0, 1, 1));
        });


    }
    public void ChangeColor()
    {
        Button button;
        Debug.Log("aaaaaaaaaa");
        button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        MultiplayerManager.Instance.SetPlayerColor((Vector4)button.targetGraphic.color);
    }

    private void GameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if (NetworkManager.Singleton.IsHost && GameManager.Instance.gameReady.Value)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);

        }
    }
}