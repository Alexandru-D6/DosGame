using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ColorSelectorManager : MonoBehaviourPunCallbacks
{
    [SerializeField] new Camera camera;
    [SerializeField] MiddleManager middleManager;
    [SerializeField] GameManager gameManager;

    [SerializeField] float incrementGameobject = 2.5f;

    public int playerID = -1;

    private GameObject _aumentedGO = null;
    private void calculateRayCast() {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

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
                    middleManager.GetComponent<PhotonView>().RPC("changeColorMiddleCard", RpcTarget.AllViaServer, objectHit.GetComponent<CardColorSelector>().CardColor);
                    //MiddleManager.changeColorMiddleCard(objectHit.GetComponent<CardColorSelector>().CardColor);
                    
                    middleManager.GetComponent<PhotonView>().RPC("destroyColorSelector", RpcTarget.AllViaServer);
                    
                    PhotonView.Find(playerID).RPC("turnFinished", RpcTarget.AllViaServer, false, playerID);
                    gameManager.GetComponent<PhotonView>().RPC("finishedTurn", RpcTarget.AllViaServer);
                    //GameManager.finishedTurn();
                }
            }
        }
    }

    public void assignPlayerID(short _playerID) {
        playerID = System.Convert.ToInt32(_playerID);

        if(!PhotonView.Find(System.Convert.ToInt32(_playerID)).IsMine) {
            gameObject.layer = (int)Layers.IgnoreRayCast;
        }

        transform.SetParent(middleManager.transform);
        transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
        transform.localEulerAngles = new Vector3(-125.0f, 0.0f, 0.0f);
    }

    // Start is called before the first frame update
    void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
        middleManager = GameObject.FindGameObjectWithTag("MiddleCard").GetComponent<MiddleManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        //_initTextMeshState = TextMesh.activeSelf;
        //TextMesh.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerID != -1 && PhotonView.Find(playerID).IsMine) {
            calculateRayCast();
        }
    }
}
