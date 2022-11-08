using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    private GameObject ball_spawner;
    public float verticalInput;
    private const float max_elevation = 45f;
    private const float min_elevation = 117f;
    public bool rot_completed = false;
    public float rotationSpeed = 10f;
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

    public float GetMaxDistance(){
        return (ball_spawner_script.speed * ball_spawner_script.speed)/ -Physics.gravity.y;
    }

    private void CalculateTargetAngle(GameObject enemy){
        LaunchProjectile ball_spawner_script = ball_spawner.GetComponent<LaunchProjectile>();
        float angle = CalculateThrowAngle(ball_spawner.transform.position, enemy.transform.position, ball_spawner_script.speed);
        if(!float.IsNaN(angle)){
            float possible_angle = 90f+angle;
            target_angle = possible_angle;
        }
        else
            target_angle = transform.localEulerAngles.z;
    }

    public float CalculateAndGetTargetAngle(GameObject enemy){
        CalculateTargetAngle(enemy);
        return target_angle;
    }

    public float CalculateInputForAimbot(GameObject enemy){
        //CalculateTargetAngle(enemy);   Perché già chiamata in AgentController all'inizio di ogni episodio

        float rot_input = 0;

        if(Mathf.Abs(target_angle - transform.localEulerAngles.z) >= Time.deltaTime*rotationSpeed)
                rot_input = Mathf.Sign(target_angle - transform.localEulerAngles.z);
    
        return rot_input;
    }

     public static float CalculateThrowAngle(Vector3 from, Vector3 to, float speed)
     {
         float xx = to.x - from.x;
         float xz = to.z - from.z;
         float x = Mathf.Sqrt(xx * xx + xz * xz);
         float y = from.y - to.y;
 
         float v = speed;
         float g = Physics.gravity.y;
 
         float sqrt = (v * v * v * v) - (g * (g * (x * x) + 2 * y * (v * v)));
 
         // Not enough range
         if (sqrt < 0)
         {
             return 0.0f;
         }
 
         float angle = Mathf.Atan(((v * v) + Mathf.Sqrt(sqrt)) / (g * x));
         angle = (angle * 360) / (float)(2 * Mathf.PI); // Conversion from radian to degrees
         angle = 90 + angle; // Idk why but thats needed
         angle *= -1; // Unity negative is upward, positive is pointing downard
         return angle;
     }

    public bool CheckRotationCompleted(){
        return Mathf.Abs(transform.localEulerAngles.z - target_angle) < 0.01f ||
               (target_angle < max_elevation && transform.localEulerAngles.z == max_elevation) ||
               (target_angle > min_elevation && transform.localEulerAngles.z == min_elevation);
    }

    public void rotateCannon(float elevate){
        float angle = transform.localEulerAngles.z + elevate * rotationSpeed * Time.deltaTime;
        
        if (Mathf.Abs(transform.localEulerAngles.z - target_angle) < 0.01f){
            //Debug.Log("<color=green>Equal angles cannon, RETURN </color>");
            return;
        }

        if(elevate == 0){
            //Debug.Log("<color=red> Diff angles cannon</color>"+ target_angle+ " "+ transform.localEulerAngles.y);
            if(target_angle > min_elevation)
                transform.localEulerAngles = new Vector3(0f, 90f, min_elevation);
            else if(target_angle < max_elevation)
                transform.localEulerAngles = new Vector3(0f, 90f, max_elevation);
            else if (target_angle <= min_elevation && target_angle >= max_elevation)
                transform.localEulerAngles = new Vector3(0f, 90f, target_angle);
            
            return;
        }
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

    public void rotateCannon_training(float elevate){
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

    public void ResetCooldown(){
        ball_spawner_script.ResetCooldown();
    }
}
