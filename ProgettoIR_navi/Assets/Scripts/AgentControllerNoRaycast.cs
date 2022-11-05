using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor;

public class AgentControllerNoRaycast : Agent
{   
    public EnemySpawnerController enemy_spawner;
    private CannonBaseController cannon_base;
    private CannonController cannon;
    public GameObject plane;
    private float speed = 6f;
    private float rotation_speed = 7f;
    private Vector3 cannon_starting_pos;
    private Quaternion cannon_base_starting_rot;
    private float boundary_limit = 3f;
    private float target_angle = float.NaN;
    private bool movement_finished = false;
    private float distance_offset = float.NaN;
    private bool shot = false;
    private RayPerceptionSensorComponent3D raycast;
    private Vector3 cannon_base_offset = new Vector3(-0.3449993f, 0.2330005f, -0.01311016f); // Offset della cannon base dalla posizione dell'agente
    private int step_count = 0;
    private int episodes_count = 0;     // Per dimostrazione
    private int max_episodes = 100;     // Per dimostrazione
    private int max_step_episodes = 25000;
    private float z_noise, x_noise, speed_noise;


    public override void Initialize()
    {
        cannon_base = transform.Find("CannonBase").GetComponent<CannonBaseController>();
        cannon = cannon_base.gameObject.transform.Find("Cannon").GetComponent<CannonController>();
        cannon_starting_pos = cannon.transform.localPosition;
        cannon_base_starting_rot = cannon_base.transform.localRotation;
        raycast = GetComponent<RayPerceptionSensorComponent3D>();
        //Time.timeScale = 30F;
    }

    private float Get180Angle(float angle){
        if(angle > 180f)
            return -(180f - (angle - 180f));
        else if(angle < -180f)
            return 180f + (angle + 180f);
        else return angle;
    }

    public override void OnEpisodeBegin(){
        if(episodes_count >= max_episodes)
            EditorApplication.isPlaying = false;
        episodes_count++;
        Debug.Log("Episodio " + episodes_count);
        x_noise = SampleGaussian(0f, 1f);
        z_noise = SampleGaussian(0f, 1f);
        speed_noise = SampleGaussian(0, 0.4f);
        //Debug.Log(episodes_count + " di " + max_episodes);
        shot = false;
        distance_offset = Random.Range(0f, (cannon.GetMaxDistance()*3)/4);
        RandomAgentPositionTraining();
        cannon_base.transform.localRotation =  Quaternion.Euler(0f, -90f, 0f);
        cannon.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
        //Debug.Log(cannon_base.transform.localRotation.eulerAngles);
        /*
        while(!enemy_spawner.spawned){
            continue;
        }
        */

        //raycast.RayLength
        //enemy_spawner.SpawnForDemonstration(transform.localPosition + cannon_base_offset, cannon_base.transform.localRotation, 100, cannon.GetMaxDistance(), -180f, 180f);
        enemy_spawner.SpawnForTraining();

        // Per dimostrazione
        
        if (Vector3.Distance(transform.localPosition + cannon_base_offset, enemy_spawner.enemies[0].transform.localPosition) > cannon.GetMaxDistance()){
            target_angle = MoveTowardsTarget(enemy_spawner.enemies[0].transform.localPosition);
            target_angle = Get360Angle(target_angle)+GetYAngle();
            movement_finished = false;
        }
        else{
            movement_finished = true;
            float angle = CalculateShipRotationAngle(enemy_spawner.enemies[0]);
            angle = Get360Angle(angle);
            //Debug.Log(angle);
            //Debug.Log("Range: " + (GetYAngle() + angle - 24) + ", " + (GetYAngle() + angle + 24));
            target_angle = Random.Range(GetYAngle() + angle - 24, GetYAngle() + angle + 24);
            //Debug.Log(GetYAngle());
            //Debug.Log(target_angle);
            target_angle = Get360Angle(target_angle);
            //Debug.Log(target_angle);
            //enemy_spawner.SpawnForTraining();
        }
        
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

    void RandomAgentPositionTraining(){
        Vector3 bounds = plane.GetComponent<MeshRenderer>().localBounds.size;
        float min_x = -1 * plane.transform.localScale.x * (bounds.x / 2) + boundary_limit;
        float min_z = -1 * plane.transform.localScale.z * (bounds.z / 2) + boundary_limit;
        float max_x = plane.transform.localScale.x * (bounds.x / 2) - boundary_limit;
        float max_z = plane.transform.localScale.z * (bounds.z / 2) - boundary_limit;
        float pos_x = Random.Range(min_x, max_x);
        float pos_z = Random.Range(min_z, max_z);
        //transform.localPosition = new Vector3(pos_x, 0.51f, pos_z);
        transform.localPosition = new Vector3(pos_x, -0.939f, pos_z);
        transform.localEulerAngles = new Vector3(0f, Random.Range(0f,360f), 0f);
    }

    void RandomAgentPositionDemonstration(){
        float min_x = 5f, max_x = 32f, min_z = -26.5f, max_z = 25; 
        float pos_x = Random.Range(min_x, max_x);
        float pos_z = Random.Range(min_z, max_z);
        //transform.localPosition = new Vector3(pos_x, 0.51f, pos_z);
        transform.localPosition = new Vector3(pos_x, -0.939f, pos_z);
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localEulerAngles.y);
        sensor.AddObservation(cannon.GetShootingCooldownLeft());
        sensor.AddObservation(cannon_base.gameObject.transform.localEulerAngles.y);
        //sensor.AddObservation(cannon_base.rotationSpeed);
        sensor.AddObservation(cannon.gameObject.transform.localEulerAngles.z);
        //sensor.AddObservation(cannon.rotationSpeed);
        sensor.AddObservation(enemy_spawner.permanent_enemies[0].transform.localPosition.x + x_noise);
        sensor.AddObservation(enemy_spawner.permanent_enemies[0].transform.localPosition.z + z_noise);
        sensor.AddObservation(enemy_spawner.permanent_enemies[0].GetComponent<EnemyController>().speed + speed_noise);
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
    public override void OnActionReceived(ActionBuffers actions)
    {
        float move_z = convertActionFromIntToFloat(actions.DiscreteActions[0]); 
        float steer_y = convertActionFromIntToFloat(actions.DiscreteActions[1]);
        float cannon_elev = convertActionFromIntToFloat(actions.DiscreteActions[2]);
        float cannon_base_rot = convertActionFromIntToFloat(actions.DiscreteActions[3]);

        transform.localPosition += transform.forward * Time.deltaTime * speed * move_z;
        
        // Per dimostrazione
        
        if(movement_finished && CheckRotationCompleted()){
            //Debug.Log(cannon_base_rot);
            cannon_base.rotateCannonBase(cannon_base_rot);
            cannon.rotateCannon(cannon_elev);
        }
        rotateAgent(steer_y);
        

        // Per training
        /*
        cannon_base.rotateCannonBase_training(cannon_base_rot);
        cannon.rotateCannon_training(cannon_elev);
        rotateAgent_training(steer_y);
        */

        //per dimostrazione
        //Debug.Log(actions.DiscreteActions[0]);
        if(actions.DiscreteActions[4] == 1 && !shot){
            FireProjectile();
            shot = true;
        }
        
        
        //per training
        /*
        if(actions.DiscreteActions[4] == 1){
            FireProjectile();
        }*/
        

        //AddReward(-0.001f);
        //AddRewardDistance();
    }

    void FixedUpdate(){
        //Debug.Log(episodes_count);
        if(step_count > max_step_episodes){
            Debug.Log("Step terminati");
            step_count = 0;
            EndEpisode();   
        }
        else
            step_count++;
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
        discrete_action[1] = convertActionFromFloatToInt(CalculateInputForAgentRotation());
        discrete_action[2] = 0;
        discrete_action[3] = 0;
        //Debug.Log(movement_finished);
        //Debug.Log(CheckRotationCompleted()+ " "+ Get360Angle(GetYAngle()) + " " +Get360Angle(target_angle)+ " "+ Mathf.DeltaAngle(Get360Angle(GetYAngle()), Get360Angle(target_angle)));
        //Debug.Log(movement_finished + " " + CheckRotationCompleted() + " " + (Vector3.Distance(transform.localPosition + cannon_base_offset, enemy_spawner.enemies[0].transform.localPosition) > cannon.GetMaxDistance()-distance_offset));
        if(movement_finished == false && CheckRotationCompleted() && Vector3.Distance(transform.localPosition + cannon_base_offset, enemy_spawner.enemies[0].transform.localPosition) > cannon.GetMaxDistance()-distance_offset){
            discrete_action[0] = 1;
        }

        if(movement_finished == false && CheckRotationCompleted() && Vector3.Distance(transform.localPosition + cannon_base_offset, enemy_spawner.enemies[0].transform.localPosition) <= cannon.GetMaxDistance()-distance_offset){
            discrete_action[0] = 0;
            float angle = CalculateShipRotationAngle(enemy_spawner.enemies[0]);
            angle = Get360Angle(angle);
            //Debug.Log(angle);
            //Debug.Log("Range: " + (GetYAngle() + angle - 24) + ", " + (GetYAngle() + angle + 24));
            target_angle = Random.Range(GetYAngle() + angle - 24, GetYAngle() + angle + 24);
            target_angle = Get360Angle(target_angle);
            movement_finished = true;
            discrete_action[1] = convertActionFromFloatToInt(CalculateInputForAgentRotation());
        }

        if(movement_finished && CheckRotationCompleted()){
            discrete_action[2] = convertActionFromFloatToInt(cannon.CalculateInputForAimbot(enemy_spawner.enemies[0]));
            discrete_action[3] = convertActionFromFloatToInt(cannon_base.CalculateInputForAimbot(enemy_spawner.enemies[0], Get180Angle(transform.rotation.eulerAngles.y)));
        }
        
        if(cannon.CheckRotationCompleted() && cannon_base.CheckRotationCompleted() && CheckRotationCompleted() && movement_finished){
            discrete_action[4] = 1;
        }
        else{
            //Debug.Log(cannon.CheckRotationCompleted()+ " "+ cannon_base.CheckRotationCompleted());
            discrete_action[4] = 0;

        }
        
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

    void FireProjectile(){
        GameObject proj = cannon_base.Shoot();
        
        if(proj != null){
            Object[] parametersConstruct = new Object[2];
            parametersConstruct[0] = this;
            parametersConstruct[1] = enemy_spawner;

            proj.SendMessage("ConstructNoRaycast", parametersConstruct);
        }
        AddReward(-0.1f);
    }

    public void enemy_miss(float min_dist){
        float penalty = (-0.01f * min_dist) >= -0.5f ? (-0.01f * min_dist) : -0.5f;
        AddReward(penalty);
    }
    public void enemy_hit(GameObject other){
        AddReward(1.0f);
        enemy_spawner.RemoveEnemyFromList(other);
        //Debug.Log(enemy_spawner.enemies.Count);
        if(enemy_spawner.enemies.Count == 0){
            //Debug.Log(GetCumulativeReward());
            step_count = 0;
            EndEpisode();
        }
    }

    void Update()
    {   
        //Debug.Log(GetCumulativeReward());
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
    }

    private float CalculateShipRotationAngle(GameObject enemy){
        Vector3 direction = (enemy.transform.localPosition - transform.localPosition).normalized;
        direction.y = 0f;
        //Debug.DrawRay(transform.position, direction*10, Color.green);
        //Debug.DrawRay(transform.position, -transform.right*10, Color.blue);
        //Debug.DrawRay(cannon_base.transform.position, cannon_base.transform.forward, Color.red);
        float rotation_angle = Quaternion.FromToRotation(-transform.right, direction).eulerAngles.y;

        return rotation_angle;
        //Debug.Log(rotation_angle);
    }

    private float GetYAngle(){
        return Get360Angle(transform.localEulerAngles.y);
    }

    private float CalculateInputForAgentRotation(){
        float rot_input = 0;
        //if(Mathf.Abs(Get360Angle(target_angle) - Get360Angle(GetYAngle())) >= Time.deltaTime*rotation_speed)
        if(Mathf.Abs(Mathf.DeltaAngle(Get360Angle(GetYAngle()), Get360Angle(target_angle))) >= Time.deltaTime*rotation_speed)
            //rot_input = Mathf.Sign(Get360Angle(target_angle) - Get360Angle(GetYAngle()) );
            rot_input = Mathf.Sign(Mathf.DeltaAngle(Get360Angle(GetYAngle()), Get360Angle(target_angle)));
        return rot_input;
    }

    private bool CheckRotationCompleted(){
        return Mathf.Abs(Get360Angle(GetYAngle()) - Get360Angle(target_angle)) < 0.01f;
    }

    private void rotateAgent(float rot_input){
        if (Mathf.Abs(Get360Angle(GetYAngle()) - Get360Angle(target_angle)) < 0.01f){
            //Debug.Log("<color=green>Equal angles cannon base, RETURN </color>");
            return;
        }
        
        if(rot_input == 0)
            transform.localEulerAngles = new Vector3(0f, target_angle, 0f);
        else
            transform.Rotate(Vector3.up, rot_input * rotation_speed * Time.deltaTime);
    }

    private void rotateAgent_training(float rot_input){
        transform.Rotate(Vector3.up, rot_input * rotation_speed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other){
        if (other.TryGetComponent<EnemyController>(out EnemyController enemy)){
            AddReward(-1.0f);
            //Debug.Log(GetCumulativeReward());
            Debug.Log(GetCumulativeReward());
            enemy_spawner.RemoveEnemyFromList(other.gameObject);
            step_count = 0;
            EndEpisode();
        }
    }

}
