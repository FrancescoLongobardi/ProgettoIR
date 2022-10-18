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

    public void enemy_miss(float min_dist){
        AddReward(-0.01f * min_dist);
    }
    public void enemy_hit(){
        AddReward(1.0f);
    }

    void Start()
    {
        
    }

    void Update()
    {   
        
        GameObject enemies = enemy_spawner.enemies[0];
       
        cannon_base.LookWithSlerp(enemies);
        
        cannon_base.Shoot();
    }

}
