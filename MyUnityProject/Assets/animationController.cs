using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationController : MonoBehaviour
{
    public MovingBall _ball;
    Animator _animator;
    bool animationRunning = false;
    public GameObject Robot1;
    public GameObject Robot2;
    public GameObject Robot3;
    public GameObject Robot4;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.Play("Talking");
        
 
    }
    // Update is called once per frame
    void Update()
    {
        if (!animationRunning)
        {
           
            if (_ball.gol && _ball.shot)
            {
                _animator.SetBool("isGoal", true);
                _animator.SetBool("change2VictoryOrFail", true);
                _animator.Play("Victory");
            
            }
            if (_ball.shot && !_ball.gol)
            {
                _animator.SetBool("isGoal", false);
                _animator.SetBool("change2VictoryOrFail", true);
                _animator.Play("triste");
            }
        }
    }
}