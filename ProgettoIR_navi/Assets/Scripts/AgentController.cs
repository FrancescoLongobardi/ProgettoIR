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
    public GameObject plane;
    private float speed = 8f;
    private float rotation_speed = 5f;
    private Vector3 cannon_starting_pos;
    private Quaternion cannon_base_starting_rot;
    private float boundary_limit = 3f;

    public override void Initialize()
    {
        cannon_base = GameObject.Find("CannonBase").GetComponent<CannonBaseController>();
        cannon = GameObject.Find("Cannon").GetComponent<CannonController>();
        cannon_starting_pos = cannon.transform.position;
        cannon_base_starting_rot = cannon_base.transform.rotation;
    }

    public override void OnEpisodeBegin(){
        //RandomAgentPositionTraining();
        RandomAgentPositionDemonstration();
        cannon_base.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
        cannon.transform.localEulerAngles = new Vector3(0f, 90f, 90f);
        enemy_spawner.SpawnForDemonstration(cannon.transform.position, cannon_base.transform.rotation, cannon.GetMaxDistance());
        //enemy_spawner.SpawnForTraining();
    }

    void RandomAgentPositionTraining(){
        Vector3 bounds = plane.GetComponent<MeshRenderer>().localBounds.size;
        float min_x = -1 * plane.transform.localScale.x * (bounds.x / 2) + boundary_limit;
        float min_z = -1 * plane.transform.localScale.z * (bounds.z / 2) + boundary_limit;
        float max_x = plane.transform.localScale.x * (bounds.x / 2) - boundary_limit;
        float max_z = plane.transform.localScale.z * (bounds.z / 2) - boundary_limit;
        float pos_x = Random.Range(min_x, max_x);
        float pos_z = Random.Range(min_z, max_z);
        transform.localPosition = new Vector3(pos_x, 0.51f, pos_z);
    }

    void RandomAgentPositionDemonstration(){
        float min_x = 5f, max_x = 32f, min_z = -26.5f, max_z = 25; 
        float pos_x = Random.Range(min_x, max_x);
        float pos_z = Random.Range(min_z, max_z);
        transform.localPosition = new Vector3(pos_x, 0.51f, pos_z);
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
        if(cannon.CheckRotationCompleted() && cannon_base.CheckRotationCompleted()){
            discrete_action[0] = 1;
        }
        else{
            //Debug.Log(cannon.CheckRotationCompleted()+ " "+ cannon_base.CheckRotationCompleted());
            discrete_action[0] = -1;

        }
        

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
        if(enemy_spawner.enemies.Count == 0){
            GetCumulativeReward();
            EndEpisode();
        }
    }

    void Update()
    {   
       
        Quaternion max_right = cannon_base_starting_rot * Quaternion.Euler(0, 25, 0);
        Quaternion max_left = cannon_base_starting_rot * Quaternion.Euler(0, -25, 0);
        Vector3 max_dist = Vector3.forward * -cannon.GetMaxDistance();
        Debug.DrawRay(cannon.transform.position, max_right * max_dist, Color.green);
        Debug.DrawRay(cannon.transform.position, max_left * max_dist, Color.green);
        
    }


    public void OnTriggerEnter(Collider other){
        if (other.TryGetComponent<EnemyController>(out EnemyController enemy)){
            AddReward(-1.0f);
            //Debug.Log(GetCumulativeReward());
            EndEpisode();
        }
    }

}
