using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float life_time = 5f;
    public AgentController agent;
    public EnemySpawnerController enemy_spawner;

    void Update()
    {
        life_time -= Time.deltaTime;
        if(life_time < 0){
            Destroy(this.gameObject);
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
            Destroy(gameObject); 
        }
        else if(other.gameObject.tag == "enemy"){
            //enemy_spawner.RemoveEnemyFromList(other.gameObject);
            agent.enemy_hit();
        }
    }

    float find_nearest_enemy(Vector3 contact_point){
        float min = float.MaxValue;
        for(int i = 0; i < enemy_spawner.enemies.Count; i++){
            float dist = Vector3.Distance(enemy_spawner.enemies[i].transform.position, contact_point);
            if(dist < min)
                min = dist;
        }
        return min;
    }
}
