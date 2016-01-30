using UnityEngine;
using System.Collections;

public class MissileControllerJoystick : MonoBehaviour {

    [Header("Basic")]
    public int controllerNb = 1;
    public float speed = 2.0f;
    public SpriteRenderer ghostSpriteRenderer;
    public float EnergyToLoseWhenHitWall = 10f;
    public float EnergyToLoseWhenHitOtherGhost = 10f;

    [Header("Dash")]
    public float dashSpeed = 8.0f;
    public float dashDuration = 0.5f;
    public float cooldownDash = 4f;

    [Header("Booster")]
    public float boosterSpeed = 4.0f;
    public float boostTimeBeforeInvincible = 2f;

    [Header("LimitCoordinate")]
    public float limitLeftX = -8.38f;
    public float limitRightX = 8.41f;
    public float limitTopY = 4.5f;
    public float limitBottomY = -4.53f;

    [Header("Vortex")]
    public float vortexImmunityTime = 0.5f;

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
    private bool vortexImmune = false;

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
                timeBoosting = 0f;
                currentSpeed = speed;
                ghostSpriteRenderer.color = Color.white;
            }
        }
    }

    private bool _isInvincible;
    private float timeBoosting = 0f;
    private Vector3 newPos;

    void Update() {

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


        newPos = transform.position + transform.right * Time.deltaTime * (currentSpeed);

        if (
            newPos.x > limitLeftX &&
            newPos.x < limitRightX &&
            newPos.y < limitTopY &&
            newPos.y > limitBottomY 
            )
        {
            transform.Translate(transform.right * Time.deltaTime * (currentSpeed), Space.World);
        }
        
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

        if (other.tag == "vortex" && !vortexImmune)
        {
            Vortex vortex = other.GetComponent<Vortex>();
            vortexImmune = true;
            StartCoroutine(vortexImmunityTimer());
            transform.position = vortex.teleportationPoint.position;
        }

        MissileControllerJoystick controller = other.GetComponent<MissileControllerJoystick>();
        if (controller != null)
        {
            if (controller.isInvincible && isInvincible)
            {
                isInvincible = false;
                controller.isInvincible = false;
                return;
            }

            if (controller.isInvincible && !isInvincible)
            {
                energy.currentEnergy = 0;
                return;
            }

            if (!controller.isInvincible && !isInvincible)
            {
                energy.currentEnergy -= EnergyToLoseWhenHitOtherGhost;
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

    IEnumerator vortexImmunityTimer()
    {
        float timer = 0f;

        while (timer < vortexImmunityTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        vortexImmune = false;
    }
}
