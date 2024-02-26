using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public FixedString64Bytes playerId;
    public FixedString64Bytes playerName;
    public int skinIndex;
    public Color color;
    public bool isPlayerReady;



    //Comprueba que el jugador guardado sea el mismo que el proporcionado
    public bool Equals(PlayerData other)
    {
        return
            playerId == other.playerId &&
            playerName == other.playerName &&
            skinIndex == other.skinIndex &&
            color == other.color &&
            isPlayerReady == other.isPlayerReady;
    }

    //Serializa los valores para poder compartirlos
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref skinIndex);
        serializer.SerializeValue(ref color);
        serializer.SerializeValue(ref isPlayerReady);
    }
}