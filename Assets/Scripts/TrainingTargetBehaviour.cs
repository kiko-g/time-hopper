
using UnityEngine;
using TMPro;

public class TrainingTargetBehaviour : MonoBehaviour
{

    float maxY = 29.2f;
    float startingY;

    bool moveUp = true;
    bool moveDown = false;

    bool dying = false;
    
    float spawnTime;

    float deathTime;
    public float health;

    public int maxDuration;
    public int minDuration;
    
    [SerializeField]
    private TextMeshProUGUI damageText;

    int duration;

    Vector3 pivotPoint;

    private float lastDamageTime = 0f;
    private bool addDamage = false;


    // Start is called before the first frame update
    void Start()
    {
        startingY = transform.position.y;
        duration = Random.Range(minDuration, maxDuration);
        pivotPoint = new Vector3(transform.position.x, 29, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastDamageTime > 0.5f){
            addDamage = false;
        }
        if(moveUp){
            if(transform.position.y >= maxY){
                transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
                spawnTime = Time.time;
                moveUp = false;
            }
            else{
                transform.position += transform.up * 2 * Time.deltaTime;
            }
        }
        else{
            if(moveDown){
                if(transform.position.y <= startingY){
                    moveDown = false;
                    Destroy(this.gameObject);
                }
                else{
                    transform.position += transform.up * -2 * Time.deltaTime;
                }
            }
            else{
                if(Time.time - spawnTime >= duration){
                    moveDown = true;
                }
            }
        }
        
        if(dying){
            transform.RotateAround(pivotPoint, Vector3.forward, -180 * Time.deltaTime);
            if(Time.time - deathTime >= 0.5f){
                Destroy(this.gameObject);
            }
        }

        damageText.transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z);

        // damage text rotation equal to transform rotation with 180 degree offset
        damageText.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 90, 0));
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if(damageText.text != "" && addDamage){
                damageText.text = (int.Parse(damageText.text) + damage).ToString();
            } else {
                damageText.text = damage.ToString();
        }
        addDamage = true;
        lastDamageTime = Time.time;
        damageText.GetComponent<Animator>().Play("EnemyDamageOnHit", -1, 0f);

        if (health <= 0){
            Die();
        }
    }

    private void Die()
    {
        dying = true;
        deathTime = Time.time;
    }

}
