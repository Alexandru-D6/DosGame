using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{

    [SerializeField] MiddleManager MiddleManager;
    [SerializeField] Material[] materials;
    [SerializeField] float blockDuration;
    // Start is called before the first frame update
    private void changeMaterial() {
        int index = MiddleManager.getMiddleColor().getIndex();
        foreach (var mat in transform.GetComponentsInChildren<MeshRenderer>()) {
            mat.material = materials[index];
        }
    }
    // Start is called before the first frame update
    void Start() {
        MiddleManager = GameObject.FindGameObjectWithTag("MiddleCard").GetComponent<MiddleManager>();
        changeMaterial();
        StartCoroutine(Countdown());
    }

    private void AnimateRotation() {
        transform.Rotate(Vector3.up, 360*Time.deltaTime);
    }

    private IEnumerator Countdown() {
        yield return new WaitForSeconds(blockDuration);

        transform.SetParent(null);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AnimateRotation();
    }
}
