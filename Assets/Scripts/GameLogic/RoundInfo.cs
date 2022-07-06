using ExitGames.Client.Photon;
using System;

[System.Serializable]
public class RoundInfo {
    public bool isHisTurn;
    public bool hasToDraw;
    public bool isBlocked;
    public bool automaticPlay;
    public int playerID;

    public RoundInfo(bool iHT, bool hTD, bool iB, bool aP, int p) {
        isHisTurn = iHT;
        hasToDraw = hTD;
        isBlocked = iB;
        automaticPlay = aP;
        playerID = p;
    }

    public RoundInfo() {
        isHisTurn = false;
        hasToDraw = false;
        isBlocked = false;
        automaticPlay = false;
        playerID = -1;
    }

    private const int SizeRoundInfo = 5 * 4;
    public static readonly byte[] memRoundInfo = new byte[SizeRoundInfo];

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
            outStream.Write(bytes, 0, SizeRoundInfo);
        }

        return SizeRoundInfo;
    }

    public static object DeserializeRoundInfo(StreamBuffer inStream, short length) {
        RoundInfo vo = new RoundInfo();
        if (length != SizeRoundInfo) {
            return vo;
        }

        lock (memRoundInfo) {
            inStream.Read(memRoundInfo, 0, SizeRoundInfo);
            int index = 0;
            int temp;
            Protocol.Deserialize(out temp, memRoundInfo, ref index);
            vo.isHisTurn = Convert.ToBoolean(temp);
            Protocol.Deserialize(out temp, memRoundInfo, ref index);
            vo.hasToDraw = Convert.ToBoolean(temp);
            Protocol.Deserialize(out temp, memRoundInfo, ref index);
            vo.isBlocked = Convert.ToBoolean(temp);
            Protocol.Deserialize(out temp, memRoundInfo, ref index);
            vo.automaticPlay = Convert.ToBoolean(temp);
            Protocol.Deserialize(out vo.playerID, memRoundInfo, ref index);
        }

        return vo;
    }
}
