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
    }

    public void enemy_miss(float min_dist){
        AddReward(-0.01f * min_dist);
    }
    public void enemy_hit(){
        AddReward(1.0f);
        if(enemy_spawner.enemies.Count == 0)
            EndEpisode();
    }

    void Start()
    {
        
    }

    void Update()
    {   
        
        GameObject enemies = enemy_spawner.enemies[0];
       
        cannon_base.LookWithSlerp(enemies);
        
        GameObject proj = cannon_base.Shoot();
        
        if(proj != null){
            Object[] parametersConstruct = new Object[2];
            parametersConstruct[0] = this;
            parametersConstruct[1] = enemy_spawner;

            proj.SendMessage("Construct", parametersConstruct);
        }
    }


    public void OnTriggerEnter(Collider other){
        if (other.TryGetComponent<EnemyController>(out EnemyController enemy)){
            AddReward(-1.0f);
            //Debug.Log(GetCumulativeReward());
            EndEpisode();
        }
    }

}
