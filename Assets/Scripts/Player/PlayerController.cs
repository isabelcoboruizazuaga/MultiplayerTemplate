using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerController : NetworkBehaviour
{

    public static event EventHandler OnAnyPlayerSpawed;

    public static PlayerController LocalInstance { get; private set; }

    [SerializeField] private float velocidad;
    [SerializeField] private float fuerzaSalto;
    [SerializeField] private List<GameObject> skinList;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerData localPlayerData;
    private GameObject localSkin;
    private int skinIndex = 1;

    private Camera camera;
    private bool right;



    private void Awake()
    {
       // right = true;
        DontDestroyOnLoad(transform.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerSetUp(OwnerClientId);
        rb = GetComponent<Rigidbody2D>();

        //   localPlayerData.playerName = MultiplayerManager.Instance.GetPlayerName();
        //Cargando variables
        /*rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        localSkin = skinList[skinIndex];

        this.transform.SetPositionAndRotation(new Vector3(-1f, 0f, 0), Quaternion.identity);*/

    }

    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
            camera= GameObject.FindObjectOfType<Camera>();
        }

        OnAnyPlayerSpawed?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
       //
    }

    // Rotación y movimiento
    void Update()
    {

        if (right)
            localSkin.transform.rotation = Quaternion.identity;
        else
            localSkin.transform.rotation = Quaternion.Euler(0, 180, 0);

        if (!IsOwner)
            return;

        if (camera == null)
        {
            camera = GameObject.FindObjectOfType<Camera>();
        }

        Move();


        camera.transform.SetPositionAndRotation(new Vector3(transform.position.x+2, transform.position.y+3, -10f), Quaternion.identity);

    }

    /*
     * Permite el movimiento del personaje usando los ejes horizontales
     */
    public void Move()
    {
        /* rb.velocity = (transform.right * velocidad * Input.GetAxis("Horizontal")) +
                    (transform.up * rb.velocity.y);

         this.transform.SetLocalPositionAndRotation(this.transform.position, Quaternion.identity);

         if (Input.GetButtonDown("Vertical") && (Mathf.Abs(rb.velocity.y) < 0.2f))
         {
             rb.AddForce(transform.up * fuerzaSalto);
         }

         anim.SetFloat("velocidadX", Mathf.Abs(rb.velocity.x));
         anim.SetFloat("velocidadY", rb.velocity.y);

         if (rb.velocity.x > 0.1f)
         {
             right = true;
         }
         else if (rb.velocity.x < -0.1f)
         {
             right = false;
         }*/

        rb.velocity = (transform.right * velocidad * Input.GetAxis("Horizontal")) +
                     (transform.up * rb.velocity.y);


        // Actualizamos la posición
        this.transform.SetLocalPositionAndRotation(this.transform.position, this.transform.rotation);


        // Rotamos el jugador en función de la pulsación de teclas.
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.GetComponentInChildren<SpriteRenderer>().flipX = false;
            //this.transform.rotation = Quaternion.identity;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.GetComponentInChildren<SpriteRenderer>().flipX = true;
           // this.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        // Configuración del salto.
        if (Input.GetKeyDown(KeyCode.Space) && (Mathf.Abs(rb.velocity.y) < 0.2f))
        {
            rb.AddForce(transform.up * fuerzaSalto);
        }

        // Actualizamos la animación
        anim.SetFloat("velocidadX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("velocidadY", rb.velocity.y);

        // Seguimiento de cámara
        //cam.transform.SetPositionAndRotation(new Vector3(this.transform.position.x + 2, this.transform.position.y + 2, -10), Quaternion.identity);

}

    /*
     *Selector de skin de personaje
     */
    public void SelectSkin(int skinIndex)
    {
        if (!IsOwner)
        {
            return;
        }
        // Asignamos los elementos a la nueva skin
        this.skinIndex = skinIndex;
        GameObject newSkin = skinList[skinIndex];
        newSkin.transform.rotation = localSkin.transform.rotation;

        //Desactivamos la anterior
        localSkin.SetActive(false);
        //Activamos el nuevo
        newSkin.SetActive(true);

        // Reasignamos nombres.
        localSkin = newSkin;

        // Actualizamos la animación
        anim = GetComponentInChildren<Animator>();
    }


    // Método que carga en el jugador la configuración del cliente.
    public void PlayerSetUp(ulong clientId)
    {
        localPlayerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(clientId);
        SetPlayerSkin(localPlayerData.skinIndex);
        SetPlayerColor(localPlayerData.color);
    }


    // Método para establecer la skin del jugador.
    public void SetPlayerSkin(int skin)
    {
        for (int i = 0; i < skinList.Count; i++)
        {
            if (i != skin)
            {
                skinList[i].SetActive(false);
            }
        }

        localSkin = skinList[skin];
        localSkin.SetActive(true);
        anim = localSkin.GetComponent<Animator>();
    }

    // Método para establecer el color de skin
    public void SetPlayerColor(Color color)
    {
        GetComponentInChildren<SpriteRenderer>().color = color;
    }

}
