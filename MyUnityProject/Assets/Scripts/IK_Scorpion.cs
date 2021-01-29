using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OctopusController;
using UnityEngine.UI;

public class IK_Scorpion : MonoBehaviour
{
    MyScorpionController _myController= new MyScorpionController();

    public IK_tentacles _myOctopus;

    [Header("Body")]
    float animTime;
    public float animDuration = 5;
    bool animPlaying = false;
    public Transform Body;
    public Transform StartPos;
    public Transform EndPos;

    [Header("Tail")]
    public Transform tailTarget;
    public Transform tail;
    public Image fillter;
    [Header("Legs")]
    public Transform[] legs;
    public Transform[] legTargets;
    public Transform[] futureLegBases;

    bool upOrDown = true;
    // Cambiar nombres variables
    [Header("Body Movement")]
    public Vector3 initialPosition;
    public Transform[] joints;
    public Vector3 tmp;
    public Vector3 newBodyPosition;
    public float initialY;
    public Slider slider;
    float timer = 0.5f;
    bool animTimer = false;
    bool canPress = true;
    // Start is called before the first frame update
    void Start()
    {
        _myController.InitLegs(legs, futureLegBases, legTargets);
        _myController.InitTail(tail);
        initialY = joints[0].transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (animTimer)
        {
            startTimer();
        }
        if (timer < 0)
        {
           UpdateBodyRotation();
        }
        if (animPlaying)
            animTime += Time.deltaTime;

        NotifyTailTarget();

        if (Input.GetKey(KeyCode.Space))
        {
            if (canPress == true)
            {
                if (slider.value == 100)
                {
                    upOrDown = false;
                }
                if (slider.value == 0)
                {
                    upOrDown = true;
                }
                if (upOrDown == true)
                {
                    slider.value++;
                }
                else
                {
                    slider.value--;
                }
                if (slider.value < 30)
                {
                    fillter.color = Color.yellow;
                }
                else if (slider.value < 70)
                {
                    fillter.color = Color.green;

                }
                else
                {
                    fillter.color = Color.red;

                }
            }

        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            NotifyStartWalk();
            animTime = 0;
            animTimer = true;
            newBodyPosition = Body.position;
            animPlaying = true;
            canPress = false;
        }
        if (animTime < animDuration)
        {
            Body.position = Vector3.Lerp(StartPos.position, EndPos.position, animTime / animDuration);
            updateBodyPosition();
        }
        else if (animTime >= animDuration && animPlaying)
        {
            Body.position = EndPos.position;
            animPlaying = false;
        }
        _myController.UpdateIK();
    }

    //Function to send the tail target transform to the dll
    public void NotifyTailTarget()
    {
        _myController.NotifyTailTarget(tailTarget);
    }

    //Trigger Function to start the walk animation
    public void NotifyStartWalk()
    {
        _myController.NotifyStartWalk();
    }

    private void updateBodyPosition()
    {
        if (initialPosition.y != joints[0].transform.localPosition.y)
        {
            tmp.y = joints[0].transform.position.y - initialY;
            initialPosition = joints[0].transform.localPosition;
        }
        newBodyPosition = Body.localPosition;
        newBodyPosition.y = tmp.y;
        Body.localPosition = newBodyPosition;
    }

    private void UpdateBodyRotation()
    {
        Vector3 rightLegs = new Vector3(0,0,0);
        Vector3 leftLegs = new Vector3(0,0,0);

        for (int i = 0; i < 6; i++)
        {
            if (i == 0 || i == 2 || i == 4)
            {
                rightLegs += joints[i].transform.position;
            }
            else if (i == 1 || i == 3 || i == 5)
            {
                leftLegs += joints[i].transform.position;
            }
        }

        if (rightLegs.y == leftLegs.y)
        {
            Body.transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, 0);
        }
        else if (leftLegs.y > rightLegs.y)
        {
            float legHeight = leftLegs.y - rightLegs.y;
            Body.transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z + legHeight * 5);
        }
        else if (rightLegs.y > leftLegs.y)
        {
            float legHeight = rightLegs.y - leftLegs.y;
            Body.transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z - legHeight * 5);
        }
    }

    private void startTimer()
    {
        timer -= Time.deltaTime;
    }
}
