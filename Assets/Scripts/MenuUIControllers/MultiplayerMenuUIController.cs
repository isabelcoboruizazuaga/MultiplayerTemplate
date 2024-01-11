using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenuUIController : MonoBehaviour
{

    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private Button quickJoinBtn;
    [SerializeField] private Button newLobbyBtn;
    [SerializeField] private Button codeJoinBtn;

    [SerializeField] private GameObject newLobbyUI;




    // Start is called before the first frame update
    void Start()
    {
        //No se podrá unir a una partida hasta que el nombre de jugador esté
        quickJoinBtn.interactable = false;
        newLobbyBtn.interactable = false;
        codeJoinBtn.interactable = false;

        //Listener del nombre del jugador
        playerNameInputField.onValueChanged.AddListener(delegate
        {
            InputValueCheck();
           // MultiplayerManager.Instance.SetPlayerName(playerNameInputField.text);
        });

        //Listener del código de partida
        codeInputField.onValueChanged.AddListener(delegate
        {
            InputValueCheck();
        });


        //Botón de quick join, comienza el cliente
        quickJoinBtn.onClick.AddListener(() =>
        {
            // Start Client
            //NetworkManager.Singleton.StartClient();


            //LobbyManager.Instance.QuickJoin();
        });

        //Crea un lobby nuevo
       // newLobbyBtn.onClick.AddListener(() =>
        //{
            //Start Host
            //NetworkManager.Singleton.StartHost();
            //NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);

            //newLobbyUI.gameObject.SetActive(true);
        //});


        //Unirse con código
        codeJoinBtn.onClick.AddListener(() =>
        {
            //LobbyManager.Instance.JoinWithCode(codeInputField.text);
        });



    }

    /**
     * Función que comprueba los valores de los input
     * */
    public void InputValueCheck()
    {
        //Comprueba que el nombre del jugador esté relleno
        if (playerNameInputField.text != null && playerNameInputField.text.Length > 0)
        {
            quickJoinBtn.interactable = true;
            newLobbyBtn.interactable = true;
           
            //Solo se activa la opción de unirse con código si está relleno el código
            if (codeInputField.text != null && codeInputField.text.Length > 0)
            {
                codeJoinBtn.interactable = true;
            }
            else
            {
                codeJoinBtn.interactable = false;
            }
        }
        else
        {
            quickJoinBtn.interactable = false;
            newLobbyBtn.interactable = false;
            codeJoinBtn.interactable = false;
        }

    }
}
