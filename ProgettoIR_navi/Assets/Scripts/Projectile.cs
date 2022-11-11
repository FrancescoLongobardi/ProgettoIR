using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class Projectile : MonoBehaviour
{
    private float life_time = 5f;
    public AgentController agent;
    public EnemySpawnerController enemy_spawner;

    void Update()
    {
        /*
        life_time -= Time.deltaTime;
        if(life_time < 0){
            Destroy(this.gameObject);
        }
        */
        if(transform.localPosition.y < 0){
            agent.SetShot(false);
            agent.enemy_miss(find_nearest_enemy(transform.localPosition));
            Destroy(gameObject);
        }
    }

    void Construct(Object[] parametersConstruct){
        enemy_spawner = (EnemySpawnerController) parametersConstruct[1];
        agent = (AgentController) parametersConstruct[0];
    }


    void OnCollisionEnter(Collision other)
    {   
        if(other.gameObject.tag == "water"){
            Vector3 contact_point = other.contacts[0].point;
            agent.enemy_miss(find_nearest_enemy(contact_point));
            //agent.SetShot(false);
            Destroy(gameObject);
 
        }
        else if(other.gameObject.tag == "enemy"){
            Destroy(gameObject);
            agent.enemy_hit(other.gameObject);
        }
    }

    float find_nearest_enemy(Vector3 contact_point){
        float min = float.MaxValue;
        for(int i = 0; i < enemy_spawner.enemies.Count; i++){
            float dist = Vector3.Distance(enemy_spawner.enemies[i].transform.localPosition, contact_point);
            if(dist < min)
                min = dist;
        }
        return min;
    }
}
