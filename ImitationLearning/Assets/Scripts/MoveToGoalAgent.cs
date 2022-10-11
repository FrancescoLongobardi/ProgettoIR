using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MoveToGoalAgent : Agent
{   
    private float speed = 5f;
    public GameObject door;

    public override void Initialize()
    {
        //Time.timeScale = 3f;
    }


    public override void OnEpisodeBegin(){
        float pos_x_pl = UnityEngine.Random.Range(-9.0f,9.0f);
        float pos_z_pl = UnityEngine.Random.Range(-9.0f,9.0f);
        transform.localPosition = new Vector3(pos_x_pl,0.1f,pos_z_pl);
        transform.rotation = Quaternion.identity;
        float pos_x_door, pos_z_door, distance;
        do{
            pos_x_door = UnityEngine.Random.Range(-8.0f,8.0f);
            pos_z_door = UnityEngine.Random.Range(-8.0f,8.0f);
            distance = Vector3.Distance(transform.localPosition, new Vector3(pos_x_door,0.1f,pos_z_door));
        }while(distance < 0.3f);
        
        door.transform.localPosition = new Vector3(pos_x_door,0.1f,pos_z_door);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if(transform.localPosition.y < 0){
            AddReward(-1.0f);
            Debug.Log(GetCumulativeReward());
            EndEpisode();
        }
        float move_x = actions.ContinuousActions[0];
        float move_z = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(move_x, 0, move_z) * Time.deltaTime * speed;

        float distance = Vector3.Distance(transform.localPosition, door.transform.localPosition);
        //AddReward((0.001f/distance)-0.0002f);
        AddReward(-0.001f * distance);
        Debug.Log(GetCumulativeReward());
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continous_action = actionsOut.ContinuousActions;
        continous_action[0] = Input.GetAxis("Horizontal");
        continous_action[1] = Input.GetAxis("Vertical");

    }

    public void OnTriggerEnter(Collider other){
        if (other.TryGetComponent<Goal>(out Goal goal)){
            AddReward(1.0f);
            Debug.Log(GetCumulativeReward());
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall)){
            AddReward(-1.0f);
            Debug.Log(GetCumulativeReward());
            EndEpisode();
        }
    }
    
}
