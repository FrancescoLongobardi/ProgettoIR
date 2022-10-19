using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{

    public List<GameObject> enemies;
    public GameObject plane;
    public GameObject enemy_prefab;
    private int n_enemies = 1; //TODO vedere se si può cancellare
    private float min_x, max_x, min_z, max_z;
    private const float min_distance = 5f;
    private float boundary_limit_x = 1;
    private float boundary_limit_z = 3;
    public bool spawned = false;

    void Start(){
        Vector3 bounds = plane.GetComponent<MeshRenderer>().localBounds.size;
        min_x = -1 * plane.transform.localScale.x * (bounds.x / 2) + boundary_limit_x;
        min_z = -1 * plane.transform.localScale.z * (bounds.z / 2) + boundary_limit_z;
        max_x = plane.transform.localScale.x * (bounds.x / 2) - boundary_limit_x;
        max_z = plane.transform.localScale.z * (bounds.z / 2) - boundary_limit_z;
        enemies = new List<GameObject>();
        SpawnEnemies();
        spawned = true;
    }

    void Update()
    {      
        
        
    }
    
    public void RemoveEnemyFromList(GameObject enemy){
        enemies.Remove(enemy);
        Destroy(enemy);
    }

    private void SpawnEnemies(){
        bool ok_coords = false;
        float pos_x, pos_z;
        for(int i = 0; i < n_enemies; i++){
            Vector3 enemy_pos = Vector3.zero;
            Vector3 enemy_rot = Vector3.zero;
            while(!ok_coords){
                pos_x = Random.Range(min_x, max_x);
                pos_z = Random.Range(min_z, max_z);
                enemy_pos = new Vector3(pos_x, plane.transform.localPosition.y + 0.51f, pos_z);
                enemy_rot = new Vector3(0f, Random.Range(0f,360f), 0f);
                bool ok_enemy = true;
                for(int j = 0; j < i; j++){
                    Debug.Log("Distanza: " + Vector3.Distance(enemy_pos, enemies[j].transform.position));
                    if(Vector3.Distance(enemy_pos, enemies[j].transform.position) < min_distance*2){
                        ok_enemy = false;
                        break;
                    }
                }
                if(ok_enemy)
                    ok_coords = true;
                
            }
            enemies.Add(Instantiate(enemy_prefab, enemy_pos, Quaternion.Euler(enemy_rot)));
            ok_coords = false;
        }

        /*
        for(int i = 0; i < n_enemies; i++){
            for(int j = 0; j < n_enemies; j++){
                Debug.Log(Vector3.Distance(enemies[i].transform.position, enemies[j].transform.position));
            }
        }
        */
    }
}
