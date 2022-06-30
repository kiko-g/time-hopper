using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform playerTransform;

    private Animator animator;

    private float moveSpeed;

    Vector3 offset;

    [Header("Enemy Stats")]
    public float baseHealth = 100f;
    public float baseDamage = 5f;
    public float healthIncreasePerRound = 5f;
    public float damageIncreasePerRound = 2f;
    private float health;
    private float damage;
    public int dropPercentage;

    bool dropped = false, alreadyAttacked = false, registeredHit = false;
    
    [SerializeField]
    private GameObject currencyPrefab;

    [SerializeField]
    private GameObject rumblePrefab;

    private GameObject currencyHolder;
    
    private GameObject enemies;

    private NavMeshAgent navMeshAgent;

    public ObstacleAvoidanceType AvoidanceType;

    // textmeshprougui with damage on hit
    [SerializeField]
    private TextMeshProUGUI damageText;

    private WaveSpawner waveSpawner;
    private RumbleSpawner rumbleSpawner;

    private bool enableNavMesh = false;

    private Rigidbody body;

    private string[] sentences = new string[] {"voicerecording_gladiator_sentence1_1", "voicerecording_gladiator_sentence1_2", "voicerecording_gladiator_sentence1_3", "voicerecording_gladiator_sentence2_1", "voicerecording_gladiator_sentence2_2", "voicerecording_gladiator_sentence3_1", "voicerecording_gladiator_sentence3_2", "voicerecording_gladiator_sentence4_1", "voicerecording_gladiator_sentence4_2"};

    private string[] sentencesAlt = new string[] {"voicerecording_zombie_sentence1_1", "voicerecording_zombie_sentence1_2", "voicerecording_zombie_sentence1_3", "voicerecording_zombie_sentence2_1", "voicerecording_zombie_sentence2_2", "voicerecording_zombie_sentence2_3", "voicerecording_zombie_sentence2_4"};

    private string footstepsGladiatorBase = "Gladiator/footstep_coliseu_gladiator_";
    private string footstepsZombieBase = "Zombie/footstep_newworld_zombie_";

    private string footstepsBase;

    private int footstepsLowerBound = 1;
    private int footstepsHigherBound = 7;

    public float sentenceLowerBoundTimeout = 5f;
    public float sentenceHigherBoundTimeout = 20f;

    private float lastDamageTime = 0f;
    private bool addDamage = false;

    private IEnumerator speakingCoroutine;

    private int type = 1;

    // Start is called before the first frame update
    void Start()
    {
        
        speakingCoroutine = sentencesCoroutine();
        body = GetComponent<Rigidbody>();
        if (SceneManager.GetActiveScene().name == "Rumble"){
            rumbleSpawner = GameObject.Find("RumbleSpawner").GetComponent<RumbleSpawner>();
        } else {
            waveSpawner = GameObject.Find("WaveSpawner").GetComponent<WaveSpawner>();
        }
        if (rumbleSpawner != null){
            health = baseHealth + healthIncreasePerRound * (rumbleSpawner.roundNr);
            damage = baseDamage + damageIncreasePerRound * (rumbleSpawner.roundNr);
        } else {
            health = baseHealth + healthIncreasePerRound * (waveSpawner.roundNr);
            damage = baseDamage + damageIncreasePerRound * (waveSpawner.roundNr);
        }
        
        //navMeshAgent = GetComponent<NavMeshAgent>();
        //health = baseHealth;
        //damage = baseDamage;
        playerTransform = GameObject.Find("PlayerArmature").transform;
        animator = GetComponentInChildren<Animator>();
        currencyHolder = GameObject.Find("CurrencyHolder");
        
        // Generate random float move speed between 1 and 3 with different random seed for each enemy
        moveSpeed = Random.Range(1f,3f);

        footstepsBase = footstepsGladiatorBase;

        StartCoroutine(speakingCoroutine);

        //Debug.Log("Start Coroutine");
        
    }

    void initNavMeshAgent()
    {
        navMeshAgent.obstacleAvoidanceType = AvoidanceType;
        navMeshAgent.avoidancePriority = Random.Range(0, 100);
        navMeshAgent.speed = moveSpeed;
    }

    public void setDropPercentage(int dropP)
    {
        dropPercentage = dropP;
        Debug.Log(dropPercentage);
    }

    IEnumerator sentencesCoroutine(){
        while (true){
            int sentenceIndex = Random.Range(0, sentences.Length);
            string sentence = sentences[sentenceIndex];
            if (type == 0)
            {
                sentenceIndex = Random.Range(0, sentencesAlt.Length);
                sentence = sentencesAlt[sentenceIndex];
            }
            float timeout = Random.Range(sentenceLowerBoundTimeout, sentenceHigherBoundTimeout+1);
            //Debug.Log("Timeout: " + timeout);
            yield return new WaitForSeconds(timeout);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Voice Recording/" + sentence, transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Health: " + health);
        Debug.Log("Damage: " + damage);
        if (transform == null)
            return;
        if (transform.position.y <= 3 && !enableNavMesh && IsOnNavMesh()){
            gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            initNavMeshAgent();
            enableNavMesh = true;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            body.isKinematic = true;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death")){
            if (enableNavMesh)
                navMeshAgent.enabled = false;
            moveSpeed = 0;
            float animTime = animator.GetCurrentAnimatorStateInfo(0).length;
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f){
                DropCurrency();
            }
            Destroy(transform.parent.gameObject, animTime - 0.5f);
            return;
        }
        if(Time.time - lastDamageTime > 0.5f){
            addDamage = false;
        }
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Attack") || animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("atack"))
        {
            transform.LookAt(playerTransform);
            if (enableNavMesh){
                navMeshAgent.enabled = false;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.1f )
            {
                registeredHit = false;
            }
            
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                alreadyAttacked = false;
                registeredHit = false;
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f && !registeredHit)
            {
                if (Vector3.Distance(transform.position, playerTransform.position) < 5)
                {
                    if (transform.GetChild(0).name == "Gladiador"){
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Punch and Shot/Punch Shot/punch_gladiator", transform.position);
                        playerTransform.GetComponent<StarterAssets.ThirdPersonController>().TakeDamage(damage, "Gladiador");
                    } else {
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Punch and Shot/Punch Shot/punch_zombie", transform.position);
                        playerTransform.GetComponent<StarterAssets.ThirdPersonController>().TakeDamage(damage, "Zombie");
                    }
                }
                registeredHit = true;

            }
        } else {
            //transform.LookAt(playerTransform);
            if (enableNavMesh){
                navMeshAgent.enabled = true;
            }
        }

        animator.speed = 1;
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Run") && moveSpeed > 0)
        {
            animator.speed = moveSpeed / 3.0f;
        }

        if (Vector3.Distance(transform.position, playerTransform.position) > 1.5){
            if (enableNavMesh){
                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(playerTransform.position);
                //Debug.Log("NavMeshAgent: " + navMeshAgent.enabled);
            }
                        
            transform.LookAt(playerTransform);

            Vector3 eulerAngles = transform.rotation.eulerAngles;
            eulerAngles = new Vector3(0, eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(eulerAngles);

            if (!enableNavMesh)
                transform.position += transform.forward * moveSpeed * Time.deltaTime;

            animator.SetBool("is_attacking", false);
            animator.SetBool("is_running", true);
            
        } else {
            if (enableNavMesh)
                navMeshAgent.enabled = false; 
            animator.SetBool("is_running", false);
            animator.SetBool("is_attacking", true);
        }

        // damage text position equal to transform position with y offset of 0.5
        damageText.transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z);

        // damage text rotation equal to transform rotation with 180 degree offset
        damageText.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 180, 0));
    }

    public void TakeDamage(float damage)
    {
        if(!animator.GetBool("is_dead")){
            health -= damage;

            // change text o textmeshprougui with damage on hit
            if(damageText.text != "" && addDamage){
                damageText.text = (int.Parse(damageText.text) + damage).ToString();
            } else {
                damageText.text = damage.ToString();
            }
            addDamage = true;
            lastDamageTime = Time.time;
            damageText.GetComponent<Animator>().Play("EnemyDamageOnHit", -1, 0f);

            if (transform.GetChild(0).name == "Gladiador"){
                int id = Random.Range(1, 4);
                FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Punch and Shot/Scream/ah_gladiator_" + id, transform.position);
            } else {
                int id = Random.Range(1, 4);
                FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Punch and Shot/Scream/ah_zombie_" + id, transform.position);
            }

            if (health <= 0){
                Die();
            }
        }
    }

    void OnParticleCollision(){
            TakeDamage(1);
    }

    void Step(){
        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Footstep/" + footstepsBase + Random.Range(footstepsLowerBound, footstepsHigherBound+1), transform.position);
    }

    public void Die()
    {
        StopCoroutine(speakingCoroutine);
        moveSpeed = 0;
        animator.SetBool("is_dead", true);
        // deactivate the colliders
        GetComponent<CapsuleCollider>().enabled = false;
        //GetComponent<SphereCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        StarterAssets.ThirdPersonController player = playerTransform.GetComponent<StarterAssets.ThirdPersonController>();
        player.AddWeaponCurrency(5);
        if(player.waveSpawner != null)
            player.waveSpawner.decreaseEnemiesToDefeat();
        else if(player.rumbleSpawner != null){
            player.rumbleSpawner.increaseEnemiesKilled();
        }
    }

    private void DropCurrency(){
        if(!dropped){
            dropped = true;
            int dropRng = Random.Range(1, 101);
            if(dropRng <= dropPercentage){
                Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);
                GameObject currency = Instantiate(currencyPrefab, spawnPos, new Quaternion(0, 0, 0, 0));
                Debug.Log("Dropped currency");
                currency.transform.SetParent(currencyHolder.transform);
            }
            // check if the current scene is named Rumble
            if(SceneManager.GetActiveScene().name == "Rumble"){
                dropRng = Random.Range(1,101);
                if(dropRng <= 10){
                    Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
                    GameObject currency = Instantiate(rumblePrefab, spawnPos, new Quaternion(0, 0, 0, 0));
                    Debug.Log("Dropped legendary currency");
                    currency.transform.SetParent(currencyHolder.transform);
                }
            }
        }

    }

    //on collision enter, if tag is "Player" then ignore collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }

    public bool IsOnNavMesh()
    {
        NavMeshHit hit;

        // Check for nearest point on navmesh to agent, within onMeshThreshold
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
        {
            // Check if the positions are vertically aligned
            if (Mathf.Approximately(transform.position.x, hit.position.x)
                && Mathf.Approximately(transform.position.z, hit.position.z))
            {
                // Lastly, check if object is below navmesh
                return transform.position.y >= hit.position.y;
            }
        }

        return false;
    }

    public void setType(int t){
        // 0 = zombie, 1 = gladiator
        type = t;
        if (t == 0){
            footstepsBase = footstepsZombieBase;
        }
    }

    public void setStats(int roundNum, int t){
        //health = baseHealth + healthIncreasePerRound * (roundNum - 1);
        //damage = baseDamage + damageIncreasePerRound * (roundNum - 1);
        
        // 0 = zombie, 1 = gladiator
        type = t;
        if (t == 0){
            footstepsBase = footstepsZombieBase;
        }
        Debug.Log("Health: " + health);
        Debug.Log("Damage: " + damage);
    }
}
