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
    private float target_angle = float.NaN;
    public override void Initialize()
    {
        cannon_base = GameObject.Find("CannonBase").GetComponent<CannonBaseController>();
        cannon = GameObject.Find("Cannon").GetComponent<CannonController>();
        cannon_starting_pos = cannon.transform.position;
        cannon_base_starting_rot = cannon_base.transform.rotation;
    }

    private float Get180Angle(float angle){
        if(angle > 180f)
            return -(180f - (angle - 180f));
        else if(angle < -180f)
            return 180f + (angle + 180f);
        else return angle;
    }

    public override void OnEpisodeBegin(){
        RandomAgentPositionTraining();
        cannon_base.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
        cannon.transform.localEulerAngles = new Vector3(0f, 90f, 90f);
        enemy_spawner.SpawnForDemonstration(cannon.transform.position, cannon_base.transform.rotation, cannon.GetMaxDistance(), -180f, 180f);
        float angle = CalculateShipRotationAngle(enemy_spawner.enemies[0]);
        angle = Get180Angle(angle);
        Debug.Log(angle);
        Debug.Log("Range: " + (GetYAngle() + angle - 24) + ", " + (GetYAngle() + angle + 24));
        target_angle = Random.Range(GetYAngle() + angle - 24, GetYAngle() + angle + 24);
        //Debug.Log(GetYAngle());
        Debug.Log(target_angle);
        target_angle = Get180Angle(target_angle);
        Debug.Log(target_angle);
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
        transform.localEulerAngles = new Vector3(0f, Random.Range(0f,360f), 0f);
    }

    void RandomAgentPositionDemonstration(){
        float min_x = 5f, max_x = 32f, min_z = -26.5f, max_z = 25; 
        float pos_x = Random.Range(min_x, max_x);
        float pos_z = Random.Range(min_z, max_z);
        transform.localPosition = new Vector3(pos_x, 0.51f, pos_z);
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localEulerAngles.y);
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
        rotateAgent(steer_y);
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
        continous_action[1] = CalculateInputForAgentRotation();
        continous_action[2] = cannon.CalculateInputForAimbot(enemy_spawner.enemies[0]);
        continous_action[3] = cannon_base.CalculateInputForAimbot(enemy_spawner.enemies[0], transform.rotation.eulerAngles.y);
        if(cannon.CheckRotationCompleted() && cannon_base.CheckRotationCompleted() && CheckRotationCompleted()){
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
        Vector3 max_dist = transform.forward * -cannon.GetMaxDistance();
        //Debug.DrawRay(transform.position, transform.forward, Color.blue);
        Debug.DrawRay(cannon.transform.position, max_right * max_dist, Color.green);
        Debug.DrawRay(cannon.transform.position, max_left * max_dist, Color.green);
        //CalculateShipRotationAngle(enemy_spawner.enemies[0]);
        //Debug.Log(Get180Angle(transform.localEulerAngles.y));
    }

    private float CalculateShipRotationAngle(GameObject enemy){
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        direction.y = 0f;
        //Debug.DrawRay(transform.position, direction*10, Color.green);
        //Debug.DrawRay(transform.position, -transform.right*10, Color.blue);
        //Debug.DrawRay(cannon_base.transform.position, cannon_base.transform.forward, Color.red);
        float rotation_angle = Quaternion.FromToRotation(-transform.right, direction).eulerAngles.y;

        return rotation_angle;
        //Debug.Log(rotation_angle);
    }

    private float GetYAngle(){
        return Get180Angle(transform.localEulerAngles.y);
    }

    private float CalculateInputForAgentRotation(){
        float rot_input = 0;
        if(Mathf.Abs(target_angle - GetYAngle()) >= Time.deltaTime*rotation_speed)
            rot_input = Mathf.Sign(target_angle - GetYAngle());
        return rot_input;
    }

    private bool CheckRotationCompleted(){
        return Mathf.Abs(GetYAngle() - target_angle) < 0.01f;
    }

    private void rotateAgent(float rot_input){
        if (Mathf.Abs(GetYAngle() - target_angle) < 0.01f){
            //Debug.Log("<color=green>Equal angles cannon base, RETURN </color>");
            return;
        }
        
        if(rot_input == 0)
            transform.localEulerAngles = new Vector3(0f, target_angle, 0f);
        else
            transform.Rotate(Vector3.up, rot_input * rotation_speed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other){
        if (other.TryGetComponent<EnemyController>(out EnemyController enemy)){
            AddReward(-1.0f);
            //Debug.Log(GetCumulativeReward());
            EndEpisode();
        }
    }

}
