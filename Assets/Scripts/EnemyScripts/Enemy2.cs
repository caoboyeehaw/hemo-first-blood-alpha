using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Experimental.U2D.Animation;
using System.Linq;

public class Enemy2 : MonoBehaviour
{
    [System.Serializable]
    public struct ItemDrops
    {
        public bool isProtein;
        public GameObject drop;
        public float DropPercentage;
        public int NumOfDrop;
    }

    public GameObject DamageText;
    private SpriteRenderer sprite;

    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    [Header("Enemy Stats")]

    public float contactDamage;
    public float HP = 100;
    public float speed = 0;
    public int EXPWorth = 50;

    [Header("Movement Settings")]
    public float stoppingDistance = 0;
    //public float retreatDistance = 0;
    public bool followPlayer;
    //public bool retreat;
    public bool randomMovement;
    public float timeRunningTwdPlayer;
    private float runningTimeOut;

    [Header("Random Movement Settings")]
    public float circleRadius;
    public float timeTillNextMove;

    //public bool DodgeWhileCharging;



    private float NextMoveCoolDown;
    private bool reachedDestination;
    private Vector2 randPos;

    private float knockbackForce = 0;
    [Header("KnockBack Settings")]
    //public float knockForcePlayerContact;
    private bool knockback = false;
    public float knockbackstartrange = 0.4f;
    public float knockbackendrange = 1.0f;
    private float knockbacktime;




    public Transform player;
    private Rigidbody2D rb;


    [Header("Melee Settings")]
    //public float DistanceToPlayer;
    

    public float beginningrangetomove = 0;
    public float endingrangetomove = 0;
    private float chaseCoolDown;

    public float RetreatSpeed;
    public float retreatTime = 0.1f;
    private float chaseCoolDownTimer;
    private bool hitPlayer;

    [Header("Line Of Sight")]
    public bool LineOfSight;


    //DEATH VARIABLE
    public bool isDead = false;

    //ANIMATION VARIABLES
    public LayerMask IgnoreMe;
    Vector2 direction;
    float a;

    [Header("Drops")]
    public ItemDrops[] Drops;
    public GameObject[] Currency;

    [Header("SkinModule")]
    [SerializeField]
    private SpriteLibrary spriteLibrary = default;
    [SerializeField]
    private SpriteResolver targetResolver = default;
    [SerializeField]
    private string targetCategory = default;

    private string[] currSprite;

    private float critRate = 0;
    private float critDMG = 0;

    private float easeOM;

    private float relaMouseAngle;

    private float facing;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (GlobalPlayerVariables.GameOver == false)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        else
            player = this.transform;
        sprite = GetComponent<SpriteRenderer>();
        attack();

        currSprite = spriteLibrary.spriteLibraryAsset.GetCategoryLabelNames(targetCategory).ToArray();
    }

    void variation()
    {
        chaseCoolDown = Random.Range(beginningrangetomove, endingrangetomove);
        chaseCoolDownTimer = chaseCoolDown;
    }


    private void FixedUpdate()
    {


        
        if (knockback == true)
        {

            //Debug.Log("KNOCKBACK");
            if (hitPlayer == false)
                transform.position = Vector2.MoveTowards(transform.position, player.position, -knockbackForce * speed * Time.deltaTime);
            else
            {
                //Vector2 Offset = player.position;

                transform.position = Vector2.MoveTowards(transform.position, randPos, RetreatSpeed * speed * Time.deltaTime);
            }
            getDirection(player);

        }
        else
        {

            if (Vector2.Distance(transform.position, player.position) > stoppingDistance && followPlayer == true) //follow player
            {
                easeOM = 1;
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime * easeOM);
                //go to position 
                getDirection(player);

                //Debug.Log(transform);
            }
            else
            {
                if (randomMovement == false)
                    transform.position = this.transform.position;
                else //RANDOM MOVEMENT
                {
                    float disToRandPos = (new Vector2(transform.position.x, transform.position.y) - randPos).magnitude;
                    if (disToRandPos < speed)
                    {
                        if (easeOM > 0)
                            easeOM -= Time.fixedDeltaTime;
                        else
                            easeOM = 0;
                    }
                    else
                    {
                        easeOM = 1;
                    }
                    //Debug.Log(easeOM);
                    //Debug.Log("RANDOMPOS");
                    if (reachedDestination == false)
                        transform.position = Vector2.MoveTowards(transform.position, randPos, speed * Time.deltaTime * easeOM);
                    else
                        transform.position = this.transform.position;

                    if (transform.position.x == randPos.x && transform.position.y == randPos.y)
                    {
                        reachedDestination = true;
                    }
                    direction = randPos;
                    a = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                }
            }
            /*
            else if (Vector2.Distance(transform.position, player.position) < retreatDistance && retreat == true) //retreat
            {
                reachedDestination = true;
                transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
            }
            */

        }
    }

    void getDirection(Transform objectpos)
    {
        direction = objectpos.position - transform.position;
        a = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    // Update is called once per frame
    void Update()
    {
        Animate(facing);
        if (GlobalPlayerVariables.GameOver != false)
        {
            player = this.transform;
        }
        knockbacktime -= Time.deltaTime;
        if (knockbacktime <= 0)
        {
            knockbacktime = 0;
            knockback = false;
        }

        if (player != null && player != this.transform)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, Mathf.Infinity, ~IgnoreMe);
            //var rayDirection = player.position - transform.position;
            //Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.green);
            if (hit.collider.gameObject.tag == "Player")
            {
                LineOfSight = true;
                //Debug.Log("Player is Visable");
                // enemy can see the player!

                //Debug.Log("Player is Visable");
            }
            else
            {
                LineOfSight = false;
                //Debug.Log("Player is NOT Visable");
            }
        }


        if (LineOfSight == true)
        {
            if (followPlayer == true)
            {
                runningTimeOut += Time.deltaTime;
                if (runningTimeOut >= timeRunningTwdPlayer)
                {
                    randomRetreatPos();
                    reachedDestination = true;
                    NextMoveCoolDown = timeTillNextMove;
                    knockbacktime = retreatTime;
                    knockback = true;
                    attack();
                    runningTimeOut = 0;
                }
            }
            //Debug.Log(a); //ANGLE




            if (chaseCoolDownTimer <= 0)
            {
                facing = a;
                resumeFollow();
            }
            if (NextMoveCoolDown <= 0 && reachedDestination == true && hitPlayer == true)
            {
                Vector2 temp = randPos;
                randomPos();
                facing = Mathf.Atan2((temp - randPos).x, (temp - randPos).y) * Mathf.Rad2Deg;
            }
            NextMoveCoolDown -= Time.deltaTime;
            chaseCoolDownTimer -= Time.deltaTime;
        }
        else
        {
            //reachedDestination = true;
            //NextMoveCoolDown = timeTillNextMove;
            if (NextMoveCoolDown <= 0)
            {
                Vector2 temp = randPos;
                randomPos();
                facing = Mathf.Atan2((temp - randPos).x, (temp - randPos).y) * Mathf.Rad2Deg;
            }
            NextMoveCoolDown -= Time.deltaTime;
        }





    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        reachedDestination = true;
        NextMoveCoolDown = timeTillNextMove;

        if (collision.gameObject.tag != "Enemy")
        {
            //knockbackForce = knockForcePlayerContact;
            knockbacktime = retreatTime;
            knockback = true;
            //reachedDestination = true;
            attack();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Bullet")
        {
            float damage = collision.gameObject.GetComponent<Bullet>().damage;
            float speed = collision.gameObject.GetComponent<Bullet>().speed;
            critRate = collision.gameObject.GetComponent<Bullet>().critRate;
            critDMG = collision.gameObject.GetComponent<Bullet>().critDMG;
            knockbackForce = collision.gameObject.GetComponent<Bullet>().knockbackForce;

            takeDamage(damage, collision.transform, speed);

            //reachedDestination = true;

            knockbacktime = Random.Range(knockbackstartrange, knockbackendrange);
            knockback = true;
            hitPlayer = false;

        }



    }

    public void takeDamage(float damage, Transform impact, float speed)
    {
        
        //Debug.Log(damage);
        bool iscrit = false;
        float chance2crit = Random.Range(0f, 1f);
        if (chance2crit <= critRate)
        {
            iscrit = true;
            damage *= critDMG;
        }

        HP -= damage;
        showDamage(damage, impact, speed, iscrit);
        StartCoroutine(FlashRed());
        if (HP <= 0)
        {
            Die();
        }
    }


    void showDamage(float damage, Transform impact, float speed, bool crit)
    {
        damage = Mathf.Round(damage);
        if (damage > 1)
        {
            Vector3 direction = (transform.position - impact.transform.position).normalized;

            //might add to impact to make it go past enemy
            var go = Instantiate(DamageText, impact.position, Quaternion.identity);
            if (crit == false)
            {
                go.GetComponent<TextMeshPro>().text = damage.ToString();
            }
            else
            {
                Color colorTop = new Color(0.83529f, 0.06667f, 0.06667f);
                Color colorBottom = new Color(0.98824f, 0.33725f, .90196f);
                //Debug.Log("CRIT");
                go.GetComponent<TextMeshPro>().text = damage.ToString();
                //go.GetComponent<TextMeshPro>().color = new Color(1.0f, 0.0f, 0.7f);
                go.GetComponent<TextMeshPro>().colorGradient = new VertexGradient(colorTop, colorTop, colorBottom, colorBottom);                         
                go.GetComponent<TextMeshPro>().fontSize *= 1.2f;

             //rgb colors
        //private float r = 236;  // red component
        //private float g = 35;  // green component
        //private float b = 101;  // blue component

}
            go.GetComponent<DestroyText>().spawnPos(direction.x, direction.y, speed / 5);
        }
    }


    public IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }


    void attack()
    {

        hitPlayer = true;
        followPlayer = false;
        NextMoveCoolDown = 0;
        variation();
        //reachedDestination = true;
        //randomPos();
        //NextMoveCoolDown = timeTillNextMove;
    }

    void resumeFollow()
    {
        reachedDestination = true;
        hitPlayer = false;
        followPlayer = true;
    }

    void randomRetreatPos()
    {
        randPos = transform.position;
        randPos += Random.insideUnitCircle * circleRadius;
    }

    void randomPos()
    {

        NextMoveCoolDown = timeTillNextMove;
        randPos = transform.position;
        randPos += Random.insideUnitCircle * circleRadius;
        reachedDestination = false;


    }


    void Die()
    {
        if (isDead == false)
        {
            isDead = true;
            GlobalPlayerVariables.expToDistribute += EXPWorth;
            if (OnEnemyKilled != null)
            {
                OnEnemyKilled();
            }
            foreach (ItemDrops id in Drops)
            {
                if (Random.Range(0, 100) <= id.DropPercentage)
                {
                    if (id.isProtein == false)
                    {
                        for (int i = 0; i < id.NumOfDrop; i++)
                            Instantiate(id.drop, transform.position, Quaternion.identity);
                    }
                    else if (id.isProtein == true)
                    {
                        // if(id.NumOfDrop % 10 != 0)
                        //Instantiate(Currency[0], transform.position, Quaternion.identity);
                        int ones = 0;
                        int tens = 0;
                        int hundy = 0;
                        int thous = 0;
                        int tenthous = 0;

                        ones = id.NumOfDrop % 10;
                        for (int i = 0; i < ones; i++)
                        {
                            Instantiate(Currency[0], transform.position, Quaternion.identity);
                        }
                        tens = id.NumOfDrop / 10 % 10;
                        for (int i = 0; i < tens; i++)
                        {
                            Instantiate(Currency[1], transform.position, Quaternion.identity);
                        }
                        hundy = id.NumOfDrop / 100 % 10;
                        for (int i = 0; i < hundy; i++)
                        {
                            Instantiate(Currency[2], transform.position, Quaternion.identity);
                        }
                        thous = id.NumOfDrop / 1000 % 10;
                        for (int i = 0; i < thous; i++)
                        {
                            Instantiate(Currency[3], transform.position, Quaternion.identity);
                        }
                        tenthous = id.NumOfDrop / 10000 % 10;
                        for (int i = 0; i < tenthous; i++)
                        {
                            Instantiate(Currency[4], transform.position, Quaternion.identity);
                        }
                    }


                }
            }
            if (transform.Find("StickyGrenade(Clone)") != null)
            {
                transform.Find("StickyGrenade(Clone)").GetComponent<StickyGrenade>().stuck = false;
                transform.Find("StickyGrenade(Clone)").GetComponent<StickyGrenade>().landed = true;
                transform.Find("StickyGrenade(Clone)").parent = null;
            }

            GameObject.Destroy(gameObject);
        }
    }

    public void Animate(float angle)
    {
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(player.Stats.Direction.y, player.Stats.Direction.x) * Mathf.Rad2Deg - 180));
        //relaMouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        relaMouseAngle = angle;
        if (relaMouseAngle < 0)
            relaMouseAngle = relaMouseAngle + 360;
        //Debug.Log(relaMouseAngle);
        //Debug.Log(relaMouseAngle);
        //New 8 directions system
        /*[0]Down
         *[1]Up
         *[2]Left
         *[3]Right
         *[4]TopLeft
         *[5]TopRight
         *[6]BotLeft
         *[7]BotRight
         */
        if (relaMouseAngle <= 22.5 || relaMouseAngle > 337.5) //Right
        {
            targetResolver.SetCategoryAndLabel(targetCategory, currSprite[3]);
        }
        else if (relaMouseAngle > 22.5 && relaMouseAngle <= 67.5) //TopRight
        {
            targetResolver.SetCategoryAndLabel(targetCategory, currSprite[5]);
        }
        else if (relaMouseAngle > 67.5 && relaMouseAngle <= 112.5) //Up
        {
            targetResolver.SetCategoryAndLabel(targetCategory, currSprite[1]);
        }
        else if (relaMouseAngle > 112.5 && relaMouseAngle <= 157.5) //TopLeft
        {
            targetResolver.SetCategoryAndLabel(targetCategory, currSprite[4]);
        }
        else if (relaMouseAngle > 157.5 && relaMouseAngle <= 202.5) //Left
        {
            targetResolver.SetCategoryAndLabel(targetCategory, currSprite[2]);
        }
        else if (relaMouseAngle > 202.5 && relaMouseAngle <= 247.5) //BotLeft
        {
            targetResolver.SetCategoryAndLabel(targetCategory, currSprite[6]);
        }
        else if (relaMouseAngle > 247.5 && relaMouseAngle <= 292.5) //Down
        {
            targetResolver.SetCategoryAndLabel(targetCategory, currSprite[0]);
        }
        else if (relaMouseAngle > 292.5 && relaMouseAngle <= 337.5) //BotRight
        {
            targetResolver.SetCategoryAndLabel(targetCategory, currSprite[7]);
        }
    }


}
