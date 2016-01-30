using UnityEngine;
using System.Collections;

public class Targetting : MonoBehaviour {

    // Use this for initialization
    private RectTransform rectTransform;
    void Start () {
        rectTransform = GetComponent<RectTransform>();
    }

    public Transform target;
    public Vector3 offSet = new Vector3(0f, 0.5f, 0f);

	// Update is called once per frame
	void Update () {
        rectTransform.localPosition = target.position + offSet;
    }
}
