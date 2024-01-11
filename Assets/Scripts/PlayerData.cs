using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;
    public int skinIndex;
    public ulong clientId;


    //Comprueba que el jugador guardado sea el mismo que el proporcionado
    public bool Equals(PlayerData other)
    {
        return
            playerName == other.playerName &&
            playerId == other.playerId &&
            skinIndex == other.skinIndex &&
            playerId == other.playerId;
    }

    //Serializa los valores para poder compartirlos
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref skinIndex);
    }
}