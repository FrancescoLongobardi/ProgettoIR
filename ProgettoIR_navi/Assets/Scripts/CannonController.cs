using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    private GameObject ball_spawner;
    public float verticalInput;
    private const float max_elevation = 45f;
    private const float min_elevation = 100f;
    public bool rot_completed = false;
    public float rotationSpeed = 30f;
    private LaunchProjectile ball_spawner_script;
    private float target_angle = float.NaN;

    void Start(){
        ball_spawner = GameObject.Find("FirePoint");
        ball_spawner_script = ball_spawner.GetComponent<LaunchProjectile>();
    }

    void FixedUpdate()
    {
        verticalInput = Input.GetAxis("Vertical");

        if(verticalInput < 0 && transform.rotation.eulerAngles.z <= min_elevation)
            transform.Rotate(Vector3.back, verticalInput * rotationSpeed * Time.deltaTime);
        if(verticalInput > 0 && transform.rotation.eulerAngles.z >= max_elevation)
            transform.Rotate(Vector3.back, verticalInput * rotationSpeed * Time.deltaTime);

    }

    public GameObject Shoot(){
        return ball_spawner_script.Shoot();
    }

    public void LookWithSlerp(GameObject enemy){
        LaunchProjectile ball_spawner_script = ball_spawner.GetComponent<LaunchProjectile>();
        float angle = 0.5f * (Mathf.Asin((Physics.gravity.y * Vector3.Distance(ball_spawner.transform.position, enemy.transform.position)) / (ball_spawner_script.speed * ball_spawner_script.speed)) * Mathf.Rad2Deg);

        if(!float.IsNaN(angle)){
            float possible_angle = 90f+angle;
            //float fixed_angle = -Mathf.Clamp(-possible_angle, -min_elevation, -max_elevation);
            float rot_input = Mathf.Sign(possible_angle - transform.localEulerAngles.z);
            float next_angle = transform.localEulerAngles.z + rotationSpeed * Time.deltaTime * rot_input;
            
            if (next_angle >= max_elevation && next_angle <= min_elevation)
                transform.Rotate(Vector3.forward, rot_input * rotationSpeed * Time.deltaTime);
            else if(next_angle < max_elevation)
                transform.localEulerAngles = new Vector3(0f, 90f, max_elevation);
            else if(next_angle > min_elevation)
                transform.localEulerAngles = new Vector3(0f, 90f, min_elevation);
        }
        else
            transform.localEulerAngles = new Vector3(0f, 90f, max_elevation);
    }

    public float CalculateInputForAimbot(GameObject enemy){
        LaunchProjectile ball_spawner_script = ball_spawner.GetComponent<LaunchProjectile>();
        float angle = 0.5f * (Mathf.Asin((Physics.gravity.y * Vector3.Distance(ball_spawner.transform.position, enemy.transform.position)) / (ball_spawner_script.speed * ball_spawner_script.speed)) * Mathf.Rad2Deg);
        float rot_input = 0;

        if(!float.IsNaN(angle)){
            float possible_angle = 90f+angle;
            target_angle = possible_angle;
            //float fixed_angle = -Mathf.Clamp(-possible_angle, -min_elevation, -max_elevation);
            rot_input = Mathf.Sign(possible_angle - transform.localEulerAngles.z);
        }
        
        return rot_input;
    }

    public bool CheckRotationCompleted(){
        return transform.localEulerAngles.z == target_angle || transform.localEulerAngles.z == min_elevation || transform.localEulerAngles.z == max_elevation;
    }

    public void rotateCannon(float elevate){
        float angle = transform.localEulerAngles.z + elevate * rotationSpeed * Time.deltaTime;
        //float possible_angle = 90f+angle;
        if (angle >= max_elevation && angle <= min_elevation){
            transform.Rotate(Vector3.forward, elevate * rotationSpeed * Time.deltaTime);
        }
        else if(angle < max_elevation){
            transform.localEulerAngles = new Vector3(0f, 90f, max_elevation);
        }
        else if(angle > min_elevation){
            transform.localEulerAngles = new Vector3(0f, 90f, min_elevation);
        }
    }

    public float GetShootingCooldownLeft(){
        return ball_spawner_script.GetShootingCooldownLeft();
    }
}
