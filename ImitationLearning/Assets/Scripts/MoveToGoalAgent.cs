using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{   
    private float speed = 3f;

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move_x = actions.ContinuousActions[0];
        float move_z = actions.ContinuousActions[1];

        transform.position += new Vector3(move_x, 0, move_z) * Time.deltaTime * speed;

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continous_action = actionsOut.ContinuousActions;
        continous_action[0] = Input.GetAxis("Horizontal");
        continous_action[1] = Input.GetAxis("Vertical");

    }

    public void OnTriggerEnter(Collider other){
        Debug.Log("Tocco");
    }
    
}
