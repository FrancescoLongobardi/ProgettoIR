using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{   
    public EnemySpawnerController enemy_spawner;
    private CannonBaseController cannon_base;
    private CannonController cannon;
    private float speed = 8f;
    private float rotation_speed = 5f;

    public override void Initialize()
    {
        cannon_base = GameObject.Find("CannonBase").GetComponent<CannonBaseController>();
        cannon = GameObject.Find("Cannon").GetComponent<CannonController>();
        
    }


    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(cannon.GetShootingCooldownLeft());
        sensor.AddObservation(cannon_base.gameObject.transform.localEulerAngles.y);
        sensor.AddObservation(cannon_base.rotationSpeed);
        sensor.AddObservation(cannon.gameObject.transform.localEulerAngles.z);
        sensor.AddObservation(cannon.rotationSpeed);
    }
    /*
        Vector ContinouousActions:
            0 - forward/backward movement (agent z-axis traslation)
            1 - steering (agent y-axis rotation)
            2 - cannon elevation (cannon z-axis rotation)
            3 - cannon base rotation (cannon base y-axis rotation)

        Vector DiscreteActions:
            0 - shoot/no shoot
    */
    public override void OnActionReceived(ActionBuffers actions)
    {
        float move_z = actions.ContinuousActions[0]; 
        float steer_y = actions.ContinuousActions[1];
        float cannon_elev = actions.ContinuousActions[2];
        float cannon_base_rot = actions.ContinuousActions[3];

        transform.localPosition += new Vector3(0f, 0f, move_z) * Time.deltaTime * speed;
        transform.Rotate(Vector3.up, steer_y * rotation_speed * Time.deltaTime);
        cannon_base.rotateCannonBase(cannon_base_rot);
        cannon.rotateCannon(cannon_elev);
        if(actions.DiscreteActions[0] == 1)
            FireProjectile();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continous_action = actionsOut.ContinuousActions;
        ActionSegment<int> discrete_action = actionsOut.DiscreteActions;

        continous_action[0] = 0;
        continous_action[1] = 0;
        continous_action[2] = cannon.CalculateInputForAimbot(enemy_spawner.enemies[0]);
        continous_action[3] = cannon_base.CalculateInputForAimbot(enemy_spawner.enemies[0]);
        if(cannon.CheckRotationCompleted() && cannon_base.CheckRotationCompleted())
            discrete_action[0] = 1;
        else
            discrete_action[0] = -1;
        

    }

    void FireProjectile(){
        GameObject proj = cannon_base.Shoot();
        
        if(proj != null){
            Object[] parametersConstruct = new Object[2];
            parametersConstruct[0] = this;
            parametersConstruct[1] = enemy_spawner;

            proj.SendMessage("Construct", parametersConstruct);
        }
    }

    public void enemy_miss(float min_dist){
        AddReward(-0.01f * min_dist);
    }
    public void enemy_hit(){
        AddReward(1.0f);
        //Debug.Log(enemy_spawner.enemies.Count);
        //if(enemy_spawner.enemies.Count == 0)
        //    EndEpisode();
    }

    void Start()
    {
        
    }

    void Update()
    {   
        //Quaternion randAng = Quaternion.Euler(0, Random.Range(-45,45), 0);

        Quaternion randAng = cannon.transform.rotation * Quaternion.Euler(0, 0, 0);
        Debug.DrawRay(cannon.transform.position, Vector3.up, Color.green);
    }


    public void OnTriggerEnter(Collider other){
        if (other.TryGetComponent<EnemyController>(out EnemyController enemy)){
            AddReward(-1.0f);
            //Debug.Log(GetCumulativeReward());
            EndEpisode();
        }
    }

}
