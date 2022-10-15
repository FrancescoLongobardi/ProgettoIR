using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{   
    public EnemySpawnerController enemy_spawner;
    private GameObject cannon_base;

    void Start()
    {
        cannon_base = GameObject.Find("CannonBase");
    }

    void Update()
    {   
        
        GameObject enemies = enemy_spawner.enemies[0];
       
        cannon_base.GetComponent<CannonBaseController>().LookWithSlerp(enemies);
        
        cannon_base.GetComponent<CannonBaseController>().Shoot();
    }

}
