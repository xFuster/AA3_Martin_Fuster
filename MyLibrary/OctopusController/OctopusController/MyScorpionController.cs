using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace OctopusController
{

    public class MyScorpionController
    {
        //TAIL
        Transform tailTarget;
        Vector3 auxito;
        Transform tailEndEffector;
        MyTentacleController _tail;

        float animationRange;
        float animTime = 0;
        bool isPlaying = false;
        float distanceBetweenFutureBases = 1f;

        //LEGS
        Transform[] legTargets = new Transform[6];
        Transform[] legFutureBases = new Transform[6];
        MyTentacleController[] _legs = new MyTentacleController[6];

        Vector3[] auxiliarPositionInitial = new Vector3[6];
        Vector3[] auxiliarPositionFinal = new Vector3[6];
        bool[] firstTime = new bool[6];
        float[] personalTimer = new float[6];
        float[] personalFinisher = new float[6];
        private Vector3[] copy;
        private float[] distances;
        float threeshold = 0.05f;
        float rate = 120.0f;

        #region public
        public void InitLegs(Transform[] LegRoots, Transform[] LegFutureBases, Transform[] LegTargets)
        {
            _legs = new MyTentacleController[LegRoots.Length];
            //Legs init
            for (int i = 0; i < LegRoots.Length; i++)
            {
                _legs[i] = new MyTentacleController();
                _legs[i].LoadTentacleJoints(LegRoots[i], TentacleMode.LEG);
                legFutureBases[i] = LegFutureBases[i];
                legTargets[i] = LegTargets[i];
                //TODO: initialize anything needed for the FABRIK implementation
            }
            distances = new float[_legs[0].Bones.Length - 1];
            copy = new Vector3[_legs[0].Bones.Length];
        }

        public void InitTail(Transform TailBase)
        {
            _tail = new MyTentacleController();
            _tail.LoadTentacleJoints(TailBase, TentacleMode.TAIL);
            //TODO: Initialize anything needed for the Gradient Descent implementation
            // we get the tailEndEffector from the tail bones
            tailEndEffector = _tail.Bones[_tail.Bones.Length - 1];
        }

        //TODO: Check when to start the animation towards target and implement Gradient Descent method to move the joints.
        public void NotifyTailTarget(Transform target)
        {
            tailTarget = target;
        }

        //TODO: Notifies the start of the walking animation
        public void NotifyStartWalk()
        {
            isPlaying = true;
            animationRange = 5;
            animTime = 0;
        }

        //TODO: create the apropiate animations and update the IK from the legs and tail

        public void UpdateIK()
        {
            if (Vector3.Distance(tailEndEffector.transform.position, tailTarget.transform.position) > threeshold)
            {
                for (int i = 0; i < _tail.Bones.Length - 2; i++)
                {
                    float slope = CalculateSlope(_tail.Bones[i]);
                    _tail.Bones[i].transform.Rotate((Vector3.forward * -slope) * rate);
                }
            }
            if (isPlaying == true)
            {
                animTime += Time.deltaTime;
                if (animTime < animationRange)
                {
                    updateLegPos();
                }
                else
                {
                    isPlaying = false;
                }
            }
        }
        #endregion

        #region private
        //TODO: Implement the leg base animations and logic
        private void updateLegPos()
        {
            //check for the distance to the futureBase, then if it's too far away start moving the leg towards the future base position
            for (int j = 0; j < 6; j++)
            {
                RaycastHit suelo;
                if (Vector3.Distance(_legs[j].Bones[0].position, legFutureBases[j].position) > distanceBetweenFutureBases)
                {
                    if (Physics.Raycast(legFutureBases[j].position + Vector3.up * 10.0f, Vector3.down, out suelo, Mathf.Infinity))
                    {
                        if (suelo.transform.CompareTag("Suelo"))
                        {
                            _legs[j].Bones[0].position = Vector3.Lerp(_legs[j].Bones[0].position, suelo.point, 1.4f);
                        }
                        else
                        {
                            _legs[j].Bones[0].position = Vector3.Lerp(_legs[j].Bones[0].position, legFutureBases[j].position, 1.4f);
                        }
                    }
                }
                updateLegs(j);
            }
        }
        //TODO: implement Gradient Descent method to move tail if necessary
        private void updateTail()
        {

        }
        private float CalculateSlope(Transform actualJoint)
        {
            float deltaZ = 0.01f;
            float distanceBetweenEndEffectorAndTarget = Vector3.Distance(tailEndEffector.transform.position, tailTarget.transform.position);
            actualJoint.transform.Rotate(Vector3.forward * deltaZ);
            float distanceBetweenEndEffectorAndTarget2 = Vector3.Distance(tailEndEffector.transform.position, tailTarget.transform.position);
            actualJoint.transform.Rotate(Vector3.forward * -deltaZ);
            return (distanceBetweenEndEffectorAndTarget2 - distanceBetweenEndEffectorAndTarget) / deltaZ;
        }

        //TODO: implement fabrik method to move legs 
        private void updateLegs(int idPata)
        {
            // Save the position of the bones in copy
            for (int i = 0; i <= _legs[0].Bones.Length - 1; i++)
            {
                copy[i] = _legs[idPata].Bones[i].position;
            }
            // Calculate the distance between the bones
            for (int i = 0; i <= _legs[idPata].Bones.Length - 2; i++)
            {
                distances[i] = Vector3.Distance(_legs[idPata].Bones[i].position, _legs[idPata].Bones[i + 1].position);
            }

            float targetRootDist = Vector3.Distance(copy[0], legTargets[idPata].position);
            if (targetRootDist < distances.Sum())
            {
                // A loop that checks if the bones separate
                while (Vector3.Distance(copy[copy.Length - 1], legTargets[idPata].position) != 0 || Vector3.Distance(copy[0], _legs[idPata].Bones[0].position) != 0)
                {
                    copy[copy.Length - 1] = legTargets[idPata].position;
                    // First stage of Fabrik forwardReaching
                    for (int i = _legs[idPata].Bones.Length - 2; i >= 0; i--)
                    {
                        Vector3 vectorDirector = (copy[i + 1] - copy[i]).normalized;
                        Vector3 movementVector = vectorDirector * distances[i];
                        copy[i] = copy[i + 1] - movementVector;
                    }

                    copy[0] = _legs[idPata].Bones[0].position;
                    // Second stage of Fabrik backwardReaching
                    for (int i = 1; i < _legs[idPata].Bones.Length - 1; i++)
                    {
                        Vector3 vectorDirector = (copy[i - 1] - copy[i]).normalized;
                        Vector3 movementVector = vectorDirector * distances[i - 1];
                        copy[i] = copy[i - 1] - movementVector;

                    }
                }
                // Update original rotations
                for (int i = 0; i <= _legs[idPata].Bones.Length - 2; i++)
                {
                    Vector3 direction = (copy[i + 1] - copy[i]).normalized;
                    Vector3 antDir = (_legs[idPata].Bones[i + 1].position - _legs[idPata].Bones[i].position).normalized;
                    Quaternion rot = Quaternion.FromToRotation(antDir, direction);
                    _legs[idPata].Bones[i].rotation = rot * _legs[idPata].Bones[i].rotation;
                }
            }
            #endregion
        }
    }

}
