using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{


    [SerializeField] private int playerIndex;
    [SerializeField] private TextMeshPro playerNameTmpTxt;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private List<GameObject> skinList;


    private void Start()
    {

        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerManager_OnPlayerDataNetworkListChanged;
        readyGameObject.SetActive(false);

        UpdatePlayer();
    }

    private void MultiplayerManager_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }



    private void UpdatePlayer()
    {
        if (MultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            playerNameTmpTxt.text = playerData.playerName.ToString();



            // Actualizaci�n del Skin
            foreach (GameObject skin in skinList)
            {
                skin.SetActive(false);
            }

            skinList[playerData.skinIndex].SetActive(true);

            if (playerData.color != null)
            {
                SpriteRenderer[] sprites= GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sprite in sprites)
                {
                    sprite.color= playerData.color;
                }
                //GetComponentInChildren<SpriteRenderer>().color = playerData.color;
            }

            // Actualizaci�n de Ready
            if (playerData.isPlayerReady)
            {
                readyGameObject.SetActive(true);
            }


        }
        else
        {
            Hide();
        }
    }



    private void Show()
    {
        gameObject.SetActive(true);
    }


    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
