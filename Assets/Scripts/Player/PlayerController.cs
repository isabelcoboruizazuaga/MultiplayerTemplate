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
        right = true;
        DontDestroyOnLoad(transform.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //   localPlayerData.playerName = MultiplayerManager.Instance.GetPlayerName();
        //Cargando variables
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        localSkin = skinList[skinIndex];

        this.transform.SetPositionAndRotation(new Vector3(-1f, 0f, 0), Quaternion.identity);

    }

    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
            camera= GameObject.FindObjectOfType<Camera>();
        }

        OnAnyPlayerSpawed?.Invoke(this, EventArgs.Empty);
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
        rb.velocity = (transform.right * velocidad * Input.GetAxis("Horizontal")) +
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
        }
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
}
