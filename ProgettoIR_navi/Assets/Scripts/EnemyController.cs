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
        speed = Random.Range(5f, 10f);
        direction = dirs[Random.Range(0, 2)];
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += new Vector3(0, 0, direction) * Time.deltaTime * speed;
    }

    void OnCollisionEnter(Collision other)
    {   
        if(other.gameObject.tag == "wall"){
            direction = -direction;
        }
    }

}
