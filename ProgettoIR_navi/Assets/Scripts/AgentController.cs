using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor;
using System.IO;

public class AgentController : Agent
{   
    public EnemySpawnerController enemy_spawner;
    private CannonBaseController cannon_base;
    private CannonController cannon;
    public GameObject plane;
    private Vector3 cannon_starting_pos;
    private Quaternion cannon_base_starting_rot;
    private float boundary_limit = 8.5f;
    private float distance_offset = float.NaN;
    public bool shot = false;
    private RayPerceptionSensorComponent3D raycast;
    private Vector3 cannon_base_offset = new Vector3(-0.3449993f, 0.2330005f, -0.01311016f); // Offset della cannon base dalla posizione dell'agente
    private int episodes_count = 0;     // Per dimostrazione
    private int max_episodes = 500;     // Per dimostrazione
    private float z_noise, x_noise, speed_noise;
    private float cannon_base_target_angle;
    private float cannon_target_angle;
    //private int max_shots = 5;
    //private int num_shots = 0;
    //private StreamWriter writer;
    public override void Initialize()
    {
        cannon_base = transform.Find("CannonBase").GetComponent<CannonBaseController>();
        cannon = cannon_base.gameObject.transform.Find("Cannon").GetComponent<CannonController>();
        cannon_starting_pos = cannon.transform.localPosition;
        cannon_base_starting_rot = cannon_base.transform.localRotation;
        //writer = new StreamWriter("Assets/rewards2.txt", true);
        //Time.timeScale = 30F;
        //Debug.Log(MaxStep);
    }

    private float Get180Angle(float angle){
        if(angle > 180f)
            return -(180f - (angle - 180f));
        else if(angle < -180f)
            return 180f + (angle + 180f);
        else return angle;
    }

    //Per dimostrazione
    /*
    private void CheckEpisodesCount(){
        if(++episodes_count > max_episodes){
            Debug.Log("Fatto");
            EditorApplication.isPlaying = false;
        }
        Debug.Log("Episodio " + episodes_count);
    }
    */

    public override void OnEpisodeBegin(){
        
        // Per dimostrazione
        //CheckEpisodesCount();
        
        //num_shots = 0;
        //cannon.ResetCooldown();
        //Debug.Log(episodes_count + " di " + max_episodes);
        shot = false;
        distance_offset = Random.Range(0f, (cannon.GetMaxDistance()*3)/4);
        //RandomAgentPositionTraining();
        AgentPositionForCurriculum();
        cannon_base.transform.localRotation =  Quaternion.Euler(0f, -90f, 0f);
        cannon.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
        //Debug.Log(cannon_base.transform.localRotation.eulerAngles);
        /*
        while(!enemy_spawner.spawned){
            continue;
        }
        */

        //raycast.RayLength
        //enemy_spawner.SpawnForDemonstration(transform.localPosition + cannon_base_offset, cannon_base.transform.localRotation, cannon.GetMaxDistance(), -180f, 180f);
        enemy_spawner.SpawnForCurriculum(transform.localPosition + cannon_base_offset, cannon_base.transform.localRotation, cannon.GetMaxDistance(), -180f, 180f);
        //cannon_base_target_angle = cannon_base.CalculateAndGetTargetAngle(enemy_spawner.enemies[0], Get180Angle(transform.rotation.eulerAngles.y));
        //cannon_target_angle = cannon.CalculateAndGetTargetAngle(enemy_spawner.enemies[0]);
        //enemy_spawner.SpawnForTraining();
    }

    float Get360Angle(float angle){
        if (angle >360 || angle < -360)
            angle = angle % 360;
        return (angle < 0)? 360f - Mathf.Abs(angle) : angle;
    }

    float MoveTowardsTarget(Vector3 enemy_pos){
        Vector3 direction = (enemy_pos - transform.localPosition).normalized;
        direction.y = 0;
        return Quaternion.FromToRotation(transform.forward, direction).eulerAngles.y;
    }

    void AgentPositionForCurriculum(){
        transform.localPosition = new Vector3(0.0f, 0.22f, 0.0f);
    }

    void RandomAgentPositionTraining(){
        Vector3 bounds = plane.GetComponent<MeshRenderer>().localBounds.size;
        float min_x = -1 * plane.transform.localScale.x * (bounds.x / 2) + boundary_limit;
        float min_z = -1 * plane.transform.localScale.z * (bounds.z / 2) + boundary_limit;
        float max_x = plane.transform.localScale.x * (bounds.x / 2) - boundary_limit;
        float max_z = plane.transform.localScale.z * (bounds.z / 2) - boundary_limit;
        float pos_x = Random.Range(min_x, max_x);
        float pos_z = Random.Range(min_z, max_z);
        //transform.localPosition = new Vector3(pos_x, 0.51f, pos_z);
        transform.localPosition = new Vector3(pos_x, 0.22f, pos_z);
        transform.localEulerAngles = new Vector3(0f, Random.Range(0f,360f), 0f);
    }
    
    public override void CollectObservations(VectorSensor sensor){
        //Agent position
        //sensor.AddObservation(transform.localPosition.x);
        //sensor.AddObservation(transform.localPosition.z);
        //Enemy position and speed
        //sensor.AddObservation(enemy_spawner.permanent_enemies[0].transform.localPosition.x /*+ x_noise*/);
        //sensor.AddObservation(enemy_spawner.permanent_enemies[0].transform.localPosition.z /*+ z_noise*/);
        //sensor.AddObservation(enemy_spawner.permanent_enemies[0].GetComponent<EnemyController>().speed /*+ speed_noise*/);
        //Agent rotations and cooldown
        //sensor.AddObservation(transform.localEulerAngles.y);
        sensor.AddObservation(cannon_base.gameObject.transform.localEulerAngles.y);
        sensor.AddObservation(cannon.gameObject.transform.localEulerAngles.z);
        //sensor.AddObservation(cannon.GetShootingCooldownLeft());

        //sensor.AddObservation(cannon_base.rotationSpeed);
        //sensor.AddObservation(cannon.rotationSpeed);
    }
    
    public static float SampleGaussian(float mean, float stddev)
    {
            // The method requires sampling from a uniform random of (0,1]
            // but Random.NextDouble() returns a sample of [0,1).
            float x1 = 1 - Random.Range(0.0f + float.Epsilon, 1.0f);
            float x2 = 1 - Random.Range(0.0f + float.Epsilon, 1.0f);

            float y1 = Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Cos(2.0f * Mathf.PI * x2);
            return y1 * stddev + mean;
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

    private void ExecuteActions_Demo(float cannon_base_rot, float cannon_elev, int shoot){
        cannon_base.rotateCannonBase(cannon_base_rot);
        cannon.rotateCannon(cannon_elev);
        
        if(shoot == 1 && !shot){
            if(FireProjectile())
                shot = true;
        }
    }

    private void ExecuteActions_Training(float cannon_base_rot, float cannon_elev, int shoot){
        cannon_base.rotateCannonBase_training(cannon_base_rot);
        cannon.rotateCannon_training(cannon_elev);
        if(shoot == 1 && !shot){
            if(FireProjectile())
                shot = true;
        }
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        float cannon_elev = convertActionFromIntToFloat(actions.DiscreteActions[0]);
        float cannon_base_rot = convertActionFromIntToFloat(actions.DiscreteActions[1]);
        
        // Per dimostrazione
        //ExecuteActions_Demo(cannon_base_rot, cannon_elev, actions.DiscreteActions[2]);

        // Per training
        ExecuteActions_Training(cannon_base_rot, cannon_elev, actions.DiscreteActions[2]);
        
        AddReward(-1f/MaxStep);
        
        /*
        float delta_angle_cannon_base = Mathf.Abs(Mathf.DeltaAngle(cannon_base.GetLocalYAngle(), cannon_base_target_angle));
        float delta_angle_cannon = Mathf.Abs(cannon_target_angle - cannon.transform.localEulerAngles.z);
        //AddReward(0.0001f * (180f - delta_angle_cannon_base)/180f);
        //AddReward(0.0001f * (45f - delta_angle_cannon)/45f);
        //AddReward(-0.0000005f * step_count >= 0.0015f ? 0.0015f : -0.0000005f * step_count);
        
        AddReward(-0.0001f * (delta_angle_cannon_base/180f));
        AddReward(-0.0001f * (delta_angle_cannon/45f));
        */
        
    }
    
    private void AddRewardDistance(){
        float dist = Vector3.Distance(cannon_base.transform.localPosition, enemy_spawner.enemies[0].transform.localPosition);
        if(dist >= cannon.GetMaxDistance()/4 && dist <= cannon.GetMaxDistance())
            AddReward(0.005f/dist);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
       // ActionSegment<float> continous_action = actionsOut.ContinuousActions;
        ActionSegment<int> discrete_action = actionsOut.DiscreteActions;
        discrete_action[0] = 0;
        discrete_action[1] = 0;
        //Debug.Log(movement_finished);
        //Debug.Log(CheckRotationCompleted()+ " "+ Get360Angle(GetYAngle()) + " " +Get360Angle(target_angle)+ " "+ Mathf.DeltaAngle(Get360Angle(GetYAngle()), Get360Angle(target_angle)));
        //Debug.Log(movement_finished + " " + CheckRotationCompleted() + " " + (Vector3.Distance(transform.localPosition + cannon_base_offset, enemy_spawner.enemies[0].transform.localPosition) > cannon.GetMaxDistance()-distance_offset));

        discrete_action[0] = convertActionFromFloatToInt(cannon.CalculateInputForAimbot(enemy_spawner.enemies[0]));
        discrete_action[1] = convertActionFromFloatToInt(cannon_base.CalculateInputForAimbot(enemy_spawner.enemies[0], Get180Angle(transform.rotation.eulerAngles.y)));

        if (Input.GetKey(KeyCode.W))
        {
            discrete_action[2] = 1;
        }
        if(cannon.CheckRotationCompleted() && cannon_base.CheckRotationCompleted() && !shot){
            discrete_action[2] = 1;
        }
        else{
            //Debug.Log(cannon.CheckRotationCompleted()+ " "+ cannon_base.CheckRotationCompleted());
            discrete_action[2] = 0;

        }
        
    }

    public void SetShot(bool value){
        shot = value;
    }

    int convertActionFromFloatToInt(float action){
        if (action == 0f)
            return 0;
        else if (action == 1f)
            return 1;
        else if (action == -1f)
            return 2;
        else return 0;
    }

    float convertActionFromIntToFloat(int action){
        if (action ==  0)
            return 0f;
        else if (action == 1)
            return 1f;
        else if (action == 2)
            return -1f;
        else return 0;
    }

    bool FireProjectile(){
        GameObject proj = cannon_base.Shoot();
        
        if(proj != null){
            Object[] parametersConstruct = new Object[2];
            parametersConstruct[0] = this;
            parametersConstruct[1] = enemy_spawner;

            proj.SendMessage("Construct", parametersConstruct);
            //AddReward(-0.05f);
            //num_shots++;
            return true;
        }
        else
            return false;
        
    }

    public void enemy_miss(float min_dist){
        
        float penalty = (-0.02f * min_dist) >= -1.0f ? (-0.02f * min_dist) : -1.0f;
        /*
        if(min_dist < 5f)
            penalty = -0.01f * min_dist;
        else if(min_dist >= 5f && min_dist < 10f)
            penalty = -0.02f * min_dist;
        else
            penalty = (-0.03f * min_dist) >= -0.5f ? (-0.03f * min_dist) : -0.5f;
        */
        /*
        AddReward(-1.0f/max_shots);
        Debug.Log("Enemy miss at shot "+num_shots+": "+GetCumulativeReward());
        if(num_shots == max_shots){
            EndEpisode();
        }
        */
        Debug.Log(min_dist);
        if(GetCumulativeReward() - penalty > -1.0f)
            AddReward(penalty);
        else
            SetReward(-1.0f);
        Debug.Log(GetCumulativeReward());
        EndEpisode();
    }

    public void enemy_hit(GameObject other){
        /*
        for(int i = 0; i < GetStoredActionBuffers().DiscreteActions.Array.Length; i++)
            Debug.Log(GetStoredActionBuffers().DiscreteActions.Array[i]);
        */
        AddReward(3.0f);
        Debug.Log(GetCumulativeReward());
        enemy_spawner.RemoveEnemyFromList(other);
        
        if(enemy_spawner.enemies.Count == 0){
            EndEpisode();
        }
        
    }

    void FixedUpdate(){
        if(!shot)
            RequestDecision();
    }


    void Update()
    {   
        /*
        string print = "";
        foreach (float obs in GetObservations()){
            print = print + " " + obs;
        }
        Debug.Log(print);
        /*
        Vector3 bounds = plane.GetComponent<MeshRenderer>().localBounds.size;
        float min_x = -1 * plane.transform.localScale.x * (bounds.x / 2) + boundary_limit;
        float min_z = -1 * plane.transform.localScale.z * (bounds.z / 2) + boundary_limit;
        float max_x = plane.transform.localScale.x * (bounds.x / 2) - boundary_limit;
        float max_z = plane.transform.localScale.z * (bounds.z / 2) - boundary_limit;
        float pos_x = Random.Range(min_x, max_x);
        float pos_z = Random.Range(min_z, max_z);
        Debug.DrawLine(new Vector3(min_x, 0, min_z), new Vector3(max_x, 0, min_z), Color.red);
        Debug.DrawLine(new Vector3(min_x, 0, min_z), new Vector3(min_x, 0, max_z), Color.red);
        Debug.DrawLine(new Vector3(min_x, 0, max_z), new Vector3(max_x, 0, max_z), Color.red);
        Debug.DrawLine(new Vector3(max_x, 0, min_z), new Vector3(max_x, 0, max_z), Color.red);
        for(int i = 0; i < 361 ; i++){
            Debug.DrawRay(transform.localPosition, Quaternion.Euler(0, i, 0) * transform.forward * cannon.GetMaxDistance(), Color.blue);
            Debug.DrawRay(transform.localPosition, Quaternion.Euler(0, i, 0) * transform.forward * 5, Color.green);
        }
        
        //Debug.Log(Time.deltaTime);
        Quaternion max_right = cannon_base_starting_rot * Quaternion.Euler(0, 25, 0);
        Quaternion max_left = cannon_base_starting_rot * Quaternion.Euler(0, -25, 0);
        Vector3 max_dist = transform.forward * cannon.GetMaxDistance();
        //Debug.DrawRay(transform.position, transform.forward, Color.blue);
        Debug.DrawRay(transform.localPosition + cannon_base_offset, max_right * max_dist, Color.green);
        Debug.DrawRay(transform.localPosition + cannon_base_offset, max_left * max_dist, Color.green);
        //CalculateShipRotationAngle(enemy_spawner.enemies[0]);
        //Debug.Log(Get180Angle(transform.localEulerAngles.y));
        Debug.DrawRay(transform.localPosition, transform.forward * 15f, Color.blue);
        
        //Debug.Log(transform.localPosition);
        if(enemy_spawner.enemies.Count > 0){
            Vector3 direction = (enemy_spawner.enemies[0].transform.localPosition - transform.localPosition).normalized;
            Debug.DrawRay(transform.localPosition, direction*20f, Color.red);
        }
        
        //Debug.Log(SampleGaussian(0, 1)  +  "  "  + SampleGaussian(0, 0.4f));
        */
    }

    private float GetYAngle(){
        return Get360Angle(transform.localEulerAngles.y);
    }

    public void OnTriggerEnter(Collider other){
        if (other.TryGetComponent<EnemyController>(out EnemyController enemy)){
            AddReward(-1.0f);
            Debug.Log(GetCumulativeReward());
            enemy_spawner.RemoveEnemyFromList(other.gameObject);
            cannon.ResetCooldown();
            EndEpisode();
        }
    }

}