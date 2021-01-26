using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OctopusController;

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

    [Header("Legs")]
    public Transform[] legs;
    public Transform[] legTargets;
    public Transform[] futureLegBases;


    // Cambiar nombres variables
    [Header("Body Movement")]
    public Vector3 initialPosition;
    public Transform[] joints;
    public Vector3 tmp;
    public Vector3 newBodyPosition;
    public float initialY;
    float timer = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        _myController.InitLegs(legs,futureLegBases,legTargets);
        _myController.InitTail(tail);
        initialY = joints[0].transform.position.y;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (animTime < 0)
        {
            UpdateBodyRotation();
        }
        if (animPlaying)
            animTime += Time.deltaTime;

        NotifyTailTarget();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NotifyStartWalk();
            animTime = 0;
            newBodyPosition = Body.position;
            animPlaying = true;
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

    // Funcion joseada cambiar cosas
    private void updateBodyPosition()
    {
        //Avoid the first jump
        if (initialPosition.y != joints[0].transform.localPosition.y)
        {
            //ESTO NO SETOCA
            Debug.Log("CAMBIO!");
            tmp.y = joints[0].transform.localPosition.y - initialY;
            initialPosition = joints[0].transform.localPosition;

            //Sum or decrement to the body
        }
        newBodyPosition = Body.localPosition;
        Body.localPosition = newBodyPosition;
    }

    private void UpdateBodyRotation()
    {
        float side1Legs = 0f;
        float side2Legs = 0f;
        
        for (int i = 0; i < 6; i++)
        {
            if (i < 3)
            {
                side1Legs += joints[i].transform.position.y;
            }
            else if (i > 2 && i < 6)
            {
                side2Legs += joints[i].transform.position.y;
            }
        }
        if (side1Legs == side2Legs)
        {
            Debug.Log("PSOE!");
            Body.transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, 0);
        }
        else if (side1Legs > side2Legs)
        {
            Debug.Log("PODEMOS!");
            float diff = side1Legs - side2Legs;
            Body.transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z + diff * 15);
        }
        else if (side2Legs > side1Legs)
        {
            Debug.Log("PP!");
            float diff = side2Legs - side1Legs;
            Body.transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z - diff * 15);
        }
    }

    private void startTimer()
    {
        timer -= Time.deltaTime;
    }
}
