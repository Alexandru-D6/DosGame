using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using System;
using System.Text;

[System.Serializable]
public class RoundInfo {
    public bool isHisTurn;
    public bool hasToDraw;
    public bool isBlocked;
    public bool automaticPlay;
    public short playerID;
    public short playerRotation;

    public RoundInfo(bool _isHisTurn, bool _hasToDraw, bool _isBlocked, bool _automaticPlay, short _playerID, short _playerRotation) {
        isHisTurn = _isHisTurn;
        hasToDraw = _hasToDraw;
        isBlocked = _isBlocked;
        automaticPlay = _automaticPlay;
        playerID = _playerID;
        playerRotation = _playerRotation;
    }

    public RoundInfo() {
        isHisTurn = false;
        hasToDraw = false;
        isBlocked = false;
        automaticPlay = false;
        playerID = -1;
        playerRotation = -1;
    }

    private const int sizeRoundInfo = 6 * 4;
    public static readonly byte[] memRoundInfo = new byte[sizeRoundInfo];

    //Modify the Start Method to implement a new Serialization
    public static short SerializeRoundInfo(StreamBuffer outStream, object customobject) {
        RoundInfo vo = (RoundInfo)customobject;

        int index = 0;
        lock (memRoundInfo) {
            byte[] bytes = memRoundInfo;
            Protocol.Serialize(Convert.ToInt16(vo.isHisTurn), bytes, ref index);
            Protocol.Serialize(Convert.ToInt16(vo.hasToDraw), bytes, ref index);
            Protocol.Serialize(Convert.ToInt16(vo.isBlocked), bytes, ref index);
            Protocol.Serialize(Convert.ToInt16(vo.automaticPlay), bytes, ref index);
            Protocol.Serialize(vo.playerID, bytes, ref index);
            Protocol.Serialize(vo.playerRotation, bytes, ref index);
            //Debug.Log("1---> " + vo.playerID);
            //Debug.Log("Bytes: " + string.Join(" ", bytes));
            outStream.Write(bytes, 0, sizeRoundInfo);
            
        }

        return sizeRoundInfo;
    }

    public static object DeserializeRoundInfo(StreamBuffer inStream, short length) {
        RoundInfo vo = new RoundInfo();
        if (length != sizeRoundInfo) {
            return vo;
        }

        lock (memRoundInfo) {
            inStream.Read(memRoundInfo, 0, sizeRoundInfo);
            int index = 0;
            short temp;

            //Debug.Log("Bytes1: " + string.Join(" ", memRoundInfo));

            Protocol.Deserialize(out temp, memRoundInfo, ref index);
            vo.isHisTurn = Convert.ToBoolean(temp);
            Protocol.Deserialize(out temp, memRoundInfo, ref index);
            vo.hasToDraw = Convert.ToBoolean(temp);
            Protocol.Deserialize(out temp, memRoundInfo, ref index);
            vo.isBlocked = Convert.ToBoolean(temp);
            Protocol.Deserialize(out temp, memRoundInfo, ref index);
            vo.automaticPlay = Convert.ToBoolean(temp);
            Protocol.Deserialize(out vo.playerID, memRoundInfo, ref index);
            Protocol.Deserialize(out vo.playerRotation, memRoundInfo, ref index);
            //Debug.Log("2--->" + vo.playerID);
        }

        return vo;
    }
}
