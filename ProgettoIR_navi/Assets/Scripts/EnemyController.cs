using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{   
    private float speed;
    private int direction;
    // Start is called before the first frame update
    void Start()
    {
        int[] dirs = {1, -1};
        speed = Random.Range(8f, 12f);
        direction = dirs[Random.Range(0, 2)];
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localPosition += direction * transform.forward * Time.deltaTime * speed;
    }

    void OnCollisionEnter(Collision other)
    {   
        if(other.gameObject.tag == "wall"){
            direction = -direction;
        }
    }

}
