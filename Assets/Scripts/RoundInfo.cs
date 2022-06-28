using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundInfo {
    public bool isHisTurn;
    public bool hasToDraw;
    public bool isBlocked;

    public RoundInfo(bool iHT, bool hTD, bool iB) {
        isHisTurn = iHT;
        hasToDraw = hTD;
        isBlocked = iB;
    }
}
