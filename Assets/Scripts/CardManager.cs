using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CardManager : MonoBehaviour {

    [SerializeField] float MaxAngle;
    [SerializeField] float MaxRotation;

    [SerializeField] GameObject Cube;
    [SerializeField] GameObject Front;

    [SerializeField] public CardType CardType;
    [SerializeField] public CardColor CardColor;

    private bool _isRised = false;

    public void setAngle(float angle) {
        float sign = 1.0f;
        if (angle < 0.0f) sign = -1.0f;

        this.transform.localEulerAngles = new Vector3(0,0,sign * Mathf.Min(Mathf.Abs(angle), MaxAngle));
        //Cube.transform.localEulerAngles = new Vector3(0,-1.0f * sign * Mathf.Min((Mathf.Abs(angle) * MaxRotation)/MaxAngle, MaxRotation), 0);

        angle = sign * Mathf.Min(Mathf.Abs(angle), MaxAngle);
    }

    public void setZ(float z) {
        transform.localPosition = new Vector3(0.0f, -5.0f, z);
    }

    public void riseCard() {
        Cube.transform.localPosition = new Vector3(0.0f, 5.5f, Cube.transform.localPosition.z);
        _isRised = true;
    }

    public void sitCard() {
        Cube.transform.localPosition = new Vector3(0.0f, 5.0f, Cube.transform.localPosition.z);
        _isRised = false;
    }

    public bool isRised() { return _isRised; }

    public void changeCard(CardType value, CardColor color) {

        CardType = value;
        CardColor = color;

        string card = value.getString() + "_" + color.getString();

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists("./Assets/Resource/PNGCards/" + card + ".png")) {
            fileData = File.ReadAllBytes("./Assets/Resource/PNGCards/" + card + ".png");

            tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            tex.LoadImage(fileData);
            Front.GetComponent<MeshRenderer>().material.mainTexture = tex;
        }
    }

    public void initCard(Pair<CardType, CardColor> card) {
        changeCard(card.first, card.second);
        transform.localPosition = new Vector3(0, -5, 0);
        setAngle(0.0f);
    }
}
