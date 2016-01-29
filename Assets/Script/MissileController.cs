using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour {

    
    public float speed = 2.0f;
    public float bufferSpeed = 2.0f;
    public float loseControlTime = 0.5f;
    public float forceBounce = 200.0f;

    Rigidbody2D rigidBody;

    // Use this for initialization
    void Start () {
        controllerDirection = transform.right;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    Vector3 controllerDirection;
    float controllerIntensity;

    bool noControl = false;
    
    void Update () {
        if (!noControl)
        {
            controllerIntensity = Vector3.Magnitude(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f));

            if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
            {
                controllerDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
            }

            Vector3 vectorToTarget = (transform.position + controllerDirection * speed) - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            transform.Translate(transform.right * Time.deltaTime * (speed + (bufferSpeed * controllerIntensity)), Space.World);
        }

    }

    IEnumerator temporaryLostControl()
    {
        float timer = 0f;
        while (timer < loseControlTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        noControl = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!noControl)
        {
            rigidBody.constraints = RigidbodyConstraints2D.None;
            rigidBody.AddForce(controllerDirection * (speed + (bufferSpeed * controllerIntensity)) * forceBounce * -1f);
            noControl = true;
            StartCoroutine("temporaryLostControl");
        }

    }
}
