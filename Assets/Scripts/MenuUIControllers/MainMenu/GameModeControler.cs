using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameModeControler : MonoBehaviour
{
    // Declaraci�n de variables
    [SerializeField] private Button singlePlayerBtn;
    [SerializeField] private Button exitBtn;




    // Start is called before the first frame update
    void Start()
    {
        //Cargar escena de juego de un jugador
        singlePlayerBtn.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));

        //Cerrar la aplicaci�n
        exitBtn.onClick.AddListener(Application.Quit);

        //El modo multijugador se controla desde unity activando el siguiente men�

    }
}
