using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ColorSelectorManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Camera Camera;
    [SerializeField] MiddleManager MiddleManager;
    [SerializeField] GameManager GameManager;

    [SerializeField] float incrementGameobject = 2.5f;

    public int _playerID = -1;

    private GameObject _aumentedGO = null;
    private void calculateRayCast() {
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            GameObject objectHit = hit.transform.gameObject;

            if (objectHit.layer == LayerMask.NameToLayer(Layers.ColorSelector.getString())) {
                if (_aumentedGO != objectHit) {
                    if (_aumentedGO != null) _aumentedGO.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                    objectHit.transform.localScale = new Vector3(incrementGameobject, incrementGameobject, incrementGameobject);
                    _aumentedGO = objectHit;
                }
            }else {
                if (_aumentedGO != null) {
                    _aumentedGO.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                    _aumentedGO = null;
                }
            }

            if (Input.GetMouseButtonDown(0)) {
                if (objectHit.layer == LayerMask.NameToLayer(Layers.ColorSelector.getString())) {
                    MiddleManager.GetComponent<PhotonView>().RPC("changeColorMiddleCard", RpcTarget.AllViaServer, objectHit.GetComponent<CardColorSelector>().CardColor);
                    //MiddleManager.changeColorMiddleCard(objectHit.GetComponent<CardColorSelector>().CardColor);
                    
                    MiddleManager.GetComponent<PhotonView>().RPC("destroyColorSelector", RpcTarget.AllViaServer);
                    
                    PhotonView.Find(_playerID).RPC("turnFinished", RpcTarget.AllViaServer, false, _playerID);
                    GameManager.GetComponent<PhotonView>().RPC("finishedTurn", RpcTarget.AllViaServer);
                    //GameManager.finishedTurn();
                }
            }
        }
    }

    public void assignPlayerID(short playerID) {
        _playerID = System.Convert.ToInt32(playerID);

        if(!PhotonView.Find(System.Convert.ToInt32(playerID)).IsMine) {
            gameObject.layer = (int)Layers.IgnoreRayCast;
        }

        transform.SetParent(MiddleManager.transform);
        transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
        transform.localEulerAngles = new Vector3(-125.0f, 0.0f, 0.0f);
    }

    // Start is called before the first frame update
    void Awake()
    {
        Camera = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
        MiddleManager = GameObject.FindGameObjectWithTag("MiddleCard").GetComponent<MiddleManager>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        //_initTextMeshState = TextMesh.activeSelf;
        //TextMesh.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerID != -1 && PhotonView.Find(_playerID).IsMine) {
            calculateRayCast();
        }
    }
}
