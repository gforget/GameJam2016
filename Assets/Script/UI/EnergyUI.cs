using UnityEngine;
using System.Collections;

public class EnergyUI : MonoBehaviour {

    public float posXEmpty = -390.4f;
    public GameObject Player;

    private float xPosition = 0f;
    private RectTransform rectTransform;
    private Energy energyPlayer;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        energyPlayer = Player.GetComponent<Energy>();
        energyPlayer.OnDeath += Death;
    }

    // Update is called once per frame
    private float energyPercent;
	void Update () {
        energyPercent = (energyPlayer.currentEnergy / energyPlayer.totalEnergy);
        xPosition = posXEmpty * (1-energyPercent);
        rectTransform.localPosition = new Vector3(xPosition, 0f, 0f);
    }

    void Death()
    {
        Destroy(transform.parent.parent.gameObject);
    }
    void OnDestroy()
    {
        energyPlayer.OnDeath -= Death;
    }
}
