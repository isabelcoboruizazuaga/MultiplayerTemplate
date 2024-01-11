using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Declaración de Variables
    [SerializeField] private GameObject gameModeMenu;
    [SerializeField] private GameObject multiplayerMenu;
    [SerializeField] private GameObject newLobbyMenu;

    private void Awake()
    {
        Instance = this;
    }


    // Configuración inical de menús 
    void Start()
    {
        // Activamos el menú inicial
        gameModeMenu.SetActive(true);


        // Desactivamos el resto de menús
        multiplayerMenu.SetActive(false);
        newLobbyMenu.SetActive(false);

    }
}
