using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{   

    public EnemySpawnerController enemy_spawner;
    private Vector3 _direction;
    private GameObject cannon_base;
    private Quaternion _lookRotation;

    void Start()
    {
        cannon_base = GameObject.Find("CannonBase");
    }

    void Update()
    {   
        
        GameObject enemies = enemy_spawner.enemies[0];
        _direction = (enemies.transform.position - cannon_base.transform.position).normalized;
        _lookRotation = Quaternion.LookRotation(_direction, Vector3.up);
        cannon_base.transform.rotation = Quaternion.Slerp(cannon_base.transform.rotation, _lookRotation, Time.deltaTime * cannon_base.GetComponent<CannonBaseController>().rotationSpeed);

    }
}
