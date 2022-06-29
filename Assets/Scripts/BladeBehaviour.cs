using UnityEngine;

public class BladeBehaviour : MonoBehaviour
{

    public bool isActive = false;

    public void setActive()
    {
        isActive = true;
    }

    public void setInactive()
    {
        isActive = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player"){
            return;
        } 
        if (isActive){
            if (other.transform.tag  == "Enemy" || other.transform.tag == "EnemyHead"){
                other.GetComponent<EnemyBehaviour>().TakeDamage(20);
            }  else if (other.transform.tag == "RangedEnemy"  || other.transform.tag == "RangedEnemyHead"){
                other.GetComponent<RangedEnemyBehaviour>().TakeDamage(20);
            }
        }
    }
}
