using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelectorManager : MonoBehaviour
{
    [SerializeField] Camera Camera;
    [SerializeField] MiddleManager MiddleManager;
    [SerializeField] GameManager GameManager;

    [SerializeField] GameObject TextMesh;
    private bool _initTextMeshState = false;

    [SerializeField] float incrementGameobject = 2.5f;

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
                    MiddleManager.changeColorMiddleCard(objectHit.GetComponent<Color>().CardColor);
                    Destroy(gameObject);
                    TextMesh.SetActive(_initTextMeshState);
                    GameManager.finishedTurn();
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
        MiddleManager = GameObject.FindGameObjectWithTag("MiddleCard").GetComponent<MiddleManager>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        TextMesh = GameObject.FindGameObjectWithTag("WithdrawCounter");

        _initTextMeshState = TextMesh.activeSelf;
        TextMesh.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.currentPlayer()) {
            calculateRayCast();
        }
    }
}
