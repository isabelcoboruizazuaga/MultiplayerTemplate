using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewLobbyUI : MonoBehaviour
{

    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Button publicLobbyButton;
    [SerializeField] private Button privateLobbyButton;



    // Start is called before the first frame update
    void Start()
    {
        //Desactivamos los botones
        publicLobbyButton.interactable = false;
        privateLobbyButton.interactable = false;

        //Listener para el nombre de sala
        lobbyNameInputField.onValueChanged.AddListener(delegate
        {
            InputValueCheck();
        });

        //CLick listener para nuevas salas
        publicLobbyButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.NewLobby(lobbyNameInputField.text, false);
        });

        privateLobbyButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.NewLobby(lobbyNameInputField.text, true);
        });
    }


    public void InputValueCheck()
    {
        //Comprobación de que el nombre de sala no esté vacío para activar los botones
        if (lobbyNameInputField.text != null && lobbyNameInputField.text.Length > 0)
        {
            publicLobbyButton.interactable = true;
            privateLobbyButton.interactable = true;
        }
        else
        {
            publicLobbyButton.interactable = false;
            privateLobbyButton.interactable = false;
        }
    }

}
