using UnityEngine;
using System.Collections;

public class MissileControllerJoystick : MonoBehaviour {


    public int controllerNb = 1;
    public float speed = 2.0f;
    public float boosterSpeed = 4.0f;
    public float dashSpeed = 8.0f;

    //[Header "Dash"]
    public float dashDuration = 0.5f;
    public float cooldownDash = 4f;

    //[Header "Dash"]
    public float boostTimeBeforeInvincible = 2f;

    //[Header "Dash"]
    public SpriteRenderer ghostSpriteRenderer;

    public float EnergyToLoseWhenHitWall = 10f;

    private Energy energy;
    // Use this for initialization
    void Start () {
        controllerDirection = transform.right;
        energy = GetComponent<Energy>();
        energy.OnDeath += Death;
    }

    // Update is called once per frame
    private Vector3 controllerDirection;
    private float controllerIntensity;
    private bool dashMode = false;
    private bool dashOnCooldown = false;
    private float currentSpeed;

    public bool isInvincible
    {
        get
        {
            return _isInvincible;
        }
        set
        {
            _isInvincible = value;
            if (_isInvincible)
            {
                ghostSpriteRenderer.color = Color.red;
            }
            else
            {
                ghostSpriteRenderer.color = Color.white;
            }
        }
    }
    private bool _isInvincible;

    private float timeBoosting = 0f;

    void Update () {

        if (currentSpeed == boosterSpeed)
        {
            timeBoosting += Time.deltaTime;
            if (timeBoosting > boostTimeBeforeInvincible && !isInvincible)
            {
                isInvincible = true;
            }
        }
        else
        {
            isInvincible = false;
            timeBoosting = 0f;
        }

        if (Input.GetButtonDown("Dash" + controllerNb.ToString()) && !dashOnCooldown)
        {
            dashMode = true;
            dashOnCooldown = true;
            currentSpeed = dashSpeed;
            StartCoroutine(DashTimer());
        }

        if (!dashMode) {

            if (Input.GetAxis("Boost" + controllerNb.ToString()) > 0f)
            {
                currentSpeed = boosterSpeed;
                energy.toggleBoostMode(true);
            }
            else
            {
                currentSpeed = speed;
                energy.toggleBoostMode(false);
            }

            if (Input.GetAxis("Horizontal" + controllerNb.ToString()) != 0f || Input.GetAxis("Vertical" + controllerNb.ToString()) != 0f)
            {
                controllerDirection = new Vector3(Input.GetAxis("Horizontal" + controllerNb.ToString()), Input.GetAxis("Vertical" + controllerNb.ToString()), 0f);
            }

            Vector3 vectorToTarget = (transform.position + controllerDirection * speed) - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        transform.Translate(transform.right * Time.deltaTime * (currentSpeed), Space.World);
    }


    void Death()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        energy.OnDeath -= Death;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        MissileControllerJoystick controller = other.GetComponent<MissileControllerJoystick>();
        if (controller != null)
        {
            if (controller.isInvincible && isInvincible)
            {
                isInvincible = false;
                return;
            }

            if (controller.isInvincible && !isInvincible)
            {
                energy.currentEnergy = 0;
                return;
            }
            return;
        }

        HumanManager human = other.GetComponent<HumanManager>();
        if (human != null && !human.humanUse)
        {
            energy.currentEnergy += human.nbEnergyToGive;
            human.humanUse = true;
            return;
        }

        if (other.tag == "mur")
        {
            energy.currentEnergy -= EnergyToLoseWhenHitWall;
            return;
        }

        //if () { }
    }


    IEnumerator DashTimer()
    {
        float timer = 0f;
        while (timer < dashDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        dashMode = false;

        while (timer < cooldownDash)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        dashOnCooldown = false;
    }



}
