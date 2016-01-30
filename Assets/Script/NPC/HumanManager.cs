using UnityEngine;
using System.Collections;

public class HumanManager : MonoBehaviour {

    public float nbEnergyToGive = 15f;
    public float respawnTime = 10f;
    private SpriteRenderer spriteRenderer;


    public bool humanUse
    {
        get
        {
            return _humanUse;
        }

        set
        {
            _humanUse = value;
            if (_humanUse)
            {
                spriteRenderer.enabled = false;
                StartCoroutine(respawnTimer());
            }
            else
            {
                spriteRenderer.enabled = true;
            }
        }
    }

    private bool _humanUse = false;

    IEnumerator respawnTimer()
    {
        float Timer = 0f;
        while (Timer < respawnTime)
        {
            Timer += Time.deltaTime;
            yield return null;
        }

        humanUse = false;
    }

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
