using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MovingBall : MonoBehaviour
{
    [SerializeField]
    IK_tentacles _myOctopus;

    //movement speed in units per second
    [Range(-1.0f, 1.0f)]
    [SerializeField]
    private float _movementSpeed = 5f;
    Rigidbody rb;
    Vector3 _dir;
    public Slider fuerzaSlider;
    public GameObject ballTarget;
    public bool gol = false;
    public bool shot = false;
    public float Timer;
    public bool timerActivated;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;

        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");

        if(timerActivated == true)
        {
            Timer += Time.deltaTime;
        }
        //update the position
        // transform.position = transform.position + new Vector3(-horizontalInput * _movementSpeed * Time.deltaTime, verticalInput * _movementSpeed * Time.deltaTime, 0);
        if(Timer > 3)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
               
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "blueTarget")
        {
            gol = true;
        }
        if (collision.gameObject.tag == "ball")
        {
            rb.useGravity = true;
            shot = true;
            _dir = ballTarget.transform.position - this.transform.position;
            _dir = _dir.normalized;
            rb.AddForceAtPosition(_dir * fuerzaSlider.value, this.transform.position, ForceMode.Impulse);
            timerActivated = true;

        }
        _myOctopus.NotifyShoot();
    }
}
