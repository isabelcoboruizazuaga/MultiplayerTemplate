using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Declaraci�n de Variables
    [SerializeField] private GameObject gameModeMenu;
    [SerializeField] private GameObject multiplayerMenu;
    [SerializeField] private GameObject newLobbyMenu;

    private void Awake()
    {
        Instance = this;
    }


    // Configuraci�n inical de men�s 
    void Start()
    {
        // Activamos el men� inicial
        gameModeMenu.SetActive(true);


        // Desactivamos el resto de men�s
        multiplayerMenu.SetActive(false);
        newLobbyMenu.SetActive(false);

    }
}
