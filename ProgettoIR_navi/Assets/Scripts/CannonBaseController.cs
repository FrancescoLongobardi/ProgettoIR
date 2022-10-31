using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBaseController : MonoBehaviour
{
    private GameObject cannon;
    public float horizontalInput;
    public float rotationSpeed = 10f;
    private const float max_left = -115f;
    private const float max_right = -65f;
    // Start is called before the first frame update
    private float target_angle = float.NaN;

    void Start(){
        cannon = GameObject.Find("Cannon");
    }

    // Update is called once per frame
    /*
    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, horizontalInput * Time.deltaTime * rotationSpeed);
    }
    */
    public void LookWithSlerp(GameObject enemy){
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        direction.y = 0f;
        Quaternion _lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        float rotation_angle = (_lookRotation.eulerAngles.y > 180f) ? _lookRotation.eulerAngles.y -360f : _lookRotation.eulerAngles.y;
        
        float possible_rotation = rotation_angle;
        float local_y_angle = (transform.localEulerAngles.y > 180f) ? transform.localEulerAngles.y - 360f : transform.localEulerAngles.y;
        //Debug.Log(possible_rotation);
        float rot_input = Mathf.Sign(possible_rotation - local_y_angle);
        //Debug.Log(rot_input);
        
        float next_angle =  local_y_angle + rotationSpeed * Time.deltaTime * rot_input;

        //Debug.Log(next_angle);
        if (next_angle >= max_left && next_angle <= max_right)
            transform.Rotate(Vector3.up, rot_input * rotationSpeed * Time.deltaTime);
        else if(next_angle > max_right){
            transform.localEulerAngles = new Vector3(0f, max_right, 0f);
        }
        else if(next_angle < max_left){
            transform.localEulerAngles = new Vector3(0f, max_left, 0f);
        }

        cannon.GetComponent<CannonController>().LookWithSlerp(enemy);
    }

    public void rotateCannonBase(float rot_input){
        float local_y_angle = (transform.localEulerAngles.y > 180f) ? transform.localEulerAngles.y - 360f : transform.localEulerAngles.y;
        
        if (Mathf.Abs(GetLocalYAngle() - target_angle) < 0.01f){
            //Debug.Log("<color=green>Equal angles cannon base, RETURN </color>");
            return;
        }

        if(rot_input == 0){
            //Debug.Log("<color=red> Diff angles cannon base</color>"+ target_angle+ " "+ GetLocalYAngle());
            if(target_angle > max_right)
                transform.localEulerAngles = new Vector3(0f, max_right, 0f);
            else if(target_angle < max_left)
                transform.localEulerAngles = new Vector3(0f, max_left, 0f);
            else if (target_angle >= max_left && target_angle <= max_right)
                transform.localEulerAngles = new Vector3(0f, target_angle, 0f);
            
            return;
        }

        float next_angle =  local_y_angle + rotationSpeed * Time.deltaTime * rot_input;
        if(next_angle >= max_left && next_angle <= max_right)
            transform.Rotate(Vector3.up, rot_input * rotationSpeed * Time.deltaTime);
        else if(next_angle > max_right){
            transform.localEulerAngles = new Vector3(0f, max_right, 0f);
        }
        else if(next_angle < max_left){
            transform.localEulerAngles = new Vector3(0f, max_left, 0f);
        }
    }

    public void rotateCannonBase_training(float rot_input){
        float local_y_angle = (transform.localEulerAngles.y > 180f) ? transform.localEulerAngles.y - 360f : transform.localEulerAngles.y;
        float next_angle =  local_y_angle + rotationSpeed * Time.deltaTime * rot_input;
        if(next_angle >= max_left && next_angle <= max_right)
            transform.Rotate(Vector3.up, rot_input * rotationSpeed * Time.deltaTime);
        else if(next_angle > max_right){
            transform.localEulerAngles = new Vector3(0f, max_right, 0f);
        }
        else if(next_angle < max_left){
            transform.localEulerAngles = new Vector3(0f, max_left, 0f);
        }
    }

    /*
    private float Get180Angle(float angle){
        return (angle > 180f ? angle - 360f : angle); 
    }
    */
    
    private float Get180Angle(float angle){
        if(angle > 180f)
            return -(180f - (angle - 180f));
        else if(angle < -180f)
            return 180f + (angle + 180f);
        else return angle;
    }

    public float CalculateInputForAimbot(GameObject enemy, float agent_y_rot){
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        direction.y = 0f;
        Quaternion _lookRotation = Quaternion.LookRotation(direction, transform.forward);
        //Debug.DrawRay(transform.position, transform.forward*10, Color.red);
        //Debug.DrawRay(transform.position, direction*10, Color.blue);    
        float possible_rotation = Get180Angle(Get180Angle(_lookRotation.eulerAngles.y) - agent_y_rot);
        target_angle = possible_rotation;
        float local_y_angle = GetLocalYAngle();
        
        float rot_input = 0;
        //Debug.Log(possible_rotation + " " + local_y_angle + " " + Time.deltaTime*rotationSpeed);
        if(Mathf.Abs(possible_rotation - local_y_angle) >= Time.deltaTime*rotationSpeed)
            rot_input = Mathf.Sign(possible_rotation - local_y_angle);
        
        return rot_input;
    }

    private float GetLocalYAngle(){
        return Get180Angle(transform.localEulerAngles.y);
    }

    public bool CheckRotationCompleted(){
        return Mathf.Abs(GetLocalYAngle() - target_angle) < 0.01f ||
               (target_angle < max_left && GetLocalYAngle() == max_left) ||
               (target_angle > max_right && GetLocalYAngle() == max_right);
    }

    public GameObject Shoot(){
        return cannon.GetComponent<CannonController>().Shoot();
    }
}
