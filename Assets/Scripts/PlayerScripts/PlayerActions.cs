using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions
{
    private Player player;
    public float defaultSpeed;
    private float DashDistance;
    private float DashSpeed;
    private Transform leftArm;
    private Transform rightArm;
    private Image HealthBar;
    private Image ARBar;
    public Text HealCounts;
    public Text VaccineCounts;
    public Text MollyCounts;
    public Text HPNumber;
    public Text MaxHPNUmber;
    private GameObject LeftAmmo;
    private GameObject RWeaponIcon;
    private Image VaccineCooldownDisplay;
    private Image HealCooldownDisplay;
    private float relaMouseAngle;

    private Vector3 OriLeftArmPos;
    private Vector3 OriRightArmPos;

    private string currSpriteCategory;

    private string[] CurrHemoSprite;

    private float ease;

    private float stopSpeed;

    private float hdir;
    private float vdir;



    //private bool isWaiting = false;

    public PlayerActions(Player player)
    {
        this.player = player;
        leftArm = player.transform.Find("LeftArm");
        OriLeftArmPos = leftArm.position;
        rightArm = player.transform.Find("RightArm");
        OriRightArmPos = rightArm.position;
        HealthBar = GameObject.Find("HP").GetComponent<Image>();
        ARBar = GameObject.Find("AR").GetComponent<Image>();
        LeftAmmo = GameObject.Find("LeftAmmo");
        RWeaponIcon = GameObject.Find("WeaponBorderR");
        HealCounts = GameObject.Find("HealCounts").GetComponent<Text>();
        VaccineCounts = GameObject.Find("VaccineCounts").GetComponent<Text>();
        MollyCounts = GameObject.Find("MollyCounts").GetComponent<Text>();
        HPNumber = GameObject.Find("HPNumber").GetComponent<Text>();
        MaxHPNUmber = GameObject.Find("MaxHPNumber").GetComponent<Text>();
        LeftAmmo.gameObject.SetActive(false);
        DashDistance = player.Stats.DashDistance;
        DashSpeed = player.Stats.DashSpeed;
        VaccineCooldownDisplay = GameObject.Find("VaccineCoolDownIndicator").GetComponent<Image>();
        HealCooldownDisplay = GameObject.Find("HealCoolDownIndicator").GetComponent<Image>();
        player.Components.PlayerTrailRenderer.endColor = new Color(184/255f, 59/255f, 60/255f); // new Color(184, 59, 60);
        foreach (Transform rw in rightArm)
        {
            if (rw.gameObject.activeSelf)
            {
                foreach (Transform WIcon in RWeaponIcon.transform)
                    WIcon.gameObject.SetActive(false);
                if (rw.name.Contains("Protocal"))
                {
                    RWeaponIcon.transform.Find("ProtocalIcon").gameObject.SetActive(true);
                }
                else if (rw.name.Contains("Sedator"))
                {
                    RWeaponIcon.transform.Find("SedatorIcon").gameObject.SetActive(true);
                }
                else if (rw.name.Contains("Neutralizer"))
                {
                    RWeaponIcon.transform.Find("NeutralizerIcon").gameObject.SetActive(true);
                }
                else if (rw.name.Contains("Precision"))
                {
                    RWeaponIcon.transform.Find("PrecisionIcon").gameObject.SetActive(true);
                }
                else if (rw.name.Contains("Umpilical"))
                {
                    RWeaponIcon.transform.Find("UmpilicalIcon").gameObject.SetActive(true);
                }
                else if (rw.name.Contains("Disruptor"))
                {
                    RWeaponIcon.transform.Find("DisruptorIcon").gameObject.SetActive(true);
                }
                else if (rw.name.Contains("Vaccinator2.0"))
                {
                    RWeaponIcon.transform.Find("Vaccinator2.0Icon").gameObject.SetActive(true);
                }

            }
        }
        currSpriteCategory = player.Components.PlayerTargetCategory;
        CurrHemoSprite = player.Components.PlayerSpriteLibrary.spriteLibraryAsset.GetCategoryLabelNames(currSpriteCategory + (player.Stats.ArmorLevel).ToString()).ToArray();
    }

    public void Move(Transform transform)
    {
        /*
        if(player.Stats.Direction.magnitude > 0)
        {
            hdir = player.Stats.Direction.x;
            vdir = player.Stats.Direction.y;
            if (ease < 1)
                ease += Time.fixedDeltaTime * player.Stats.PFrictionz / stopSpeed;
            else
                ease = 1;
        }
        else
        {
            stopSpeed = player.Stats.Speed / player.Stats.WalkSpeed;
            if (ease > 0)
                ease -= Time.fixedDeltaTime * player.Stats.PFrictionz / stopSpeed;
            else
                ease = 0;
        }
        */
        if (player.Stats.Direction.magnitude <= 0)
        {
            stopSpeed = player.Stats.Speed / player.Stats.WalkSpeed;
            if (ease > 0)
                ease -= Time.fixedDeltaTime * player.Stats.PFrictionz / stopSpeed;
            else
                ease = 0;
        }
        else
        {
            hdir = player.Stats.Direction.x;
            vdir = player.Stats.Direction.y;
            ease = 1;
        }

        //Debug.Log(ease);
        player.Components.PlayerRidgitBody.velocity = ease * (new Vector2(hdir * player.Stats.Speed * Time.deltaTime, vdir * player.Stats.Speed * Time.deltaTime));
    }

    public void Sprint()
    {
        if(player.Stats.Stamina >= 0)
            player.Stats.Speed = player.Stats.SprintSpeed * (GlobalPlayerVariables.BaseWeaponWeight - GlobalPlayerVariables.weaponWeight + 0.001f);
        if(GlobalPlayerVariables.BaseWeaponWeight - GlobalPlayerVariables.weaponWeight + 0.001f == 0.001)
        {
            Debug.Log("Weapon too heavy!");
        }
    }

    public void Walk()
    {
        player.Stats.Speed = player.Stats.WalkSpeed * (GlobalPlayerVariables.BaseWeaponWeight - GlobalPlayerVariables.weaponWeight + 0.001f);
        if (GlobalPlayerVariables.BaseWeaponWeight - GlobalPlayerVariables.weaponWeight + 0.001f == 0.001)
        {
            Debug.Log("Weapon too heavy!");
        }
    }

    public void Dash(Vector2 Dir)
    {
        player.Components.PlayerRidgitBody.MovePosition(player.Stats.Position + Dir * DashSpeed * (GlobalPlayerVariables.BaseWeaponWeight - GlobalPlayerVariables.weaponWeight) * Time.fixedDeltaTime);
    }

    public void Animate()
    {

        relaMouseAngle = player.Stats.Angle;

        if (relaMouseAngle < 0)
            relaMouseAngle = relaMouseAngle + 360;

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
        if(relaMouseAngle <= 22.5 || relaMouseAngle > 337.5) //Right
        {
            player.Components.PlayerSpriteResolver.SetCategoryAndLabel(currSpriteCategory + (player.Stats.ArmorLevel).ToString(), CurrHemoSprite[3]);
        }
        else if(relaMouseAngle > 22.5 && relaMouseAngle <= 67.5) //TopRight
        {
            player.Components.PlayerSpriteResolver.SetCategoryAndLabel(currSpriteCategory + (player.Stats.ArmorLevel).ToString(), CurrHemoSprite[5]);
        }
        else if (relaMouseAngle > 67.5 && relaMouseAngle <= 112.5) //Up
        {
            player.Components.PlayerSpriteResolver.SetCategoryAndLabel(currSpriteCategory + (player.Stats.ArmorLevel).ToString(), CurrHemoSprite[1]);
        }
        else if (relaMouseAngle > 112.5 && relaMouseAngle <= 157.5) //TopLeft
        {
            player.Components.PlayerSpriteResolver.SetCategoryAndLabel(currSpriteCategory + (player.Stats.ArmorLevel).ToString(), CurrHemoSprite[4]);
        }
        else if (relaMouseAngle > 157.5 && relaMouseAngle <= 202.5) //Left
        {
            player.Components.PlayerSpriteResolver.SetCategoryAndLabel(currSpriteCategory + (player.Stats.ArmorLevel).ToString(), CurrHemoSprite[2]);
        }
        else if (relaMouseAngle > 202.5 && relaMouseAngle <= 247.5) //BotLeft
        {
            player.Components.PlayerSpriteResolver.SetCategoryAndLabel(currSpriteCategory + (player.Stats.ArmorLevel).ToString(), CurrHemoSprite[6]);
        }
        else if (relaMouseAngle > 247.5 && relaMouseAngle <= 292.5) //Down
        {
            player.Components.PlayerSpriteResolver.SetCategoryAndLabel(currSpriteCategory + (player.Stats.ArmorLevel).ToString(), CurrHemoSprite[0]);
        }
        else if (relaMouseAngle > 292.5 && relaMouseAngle <= 337.5) //BotRight
        {
            player.Components.PlayerSpriteResolver.SetCategoryAndLabel(currSpriteCategory + (player.Stats.ArmorLevel).ToString(), CurrHemoSprite[7]);
        }

        if (player.Components.PlayerRidgitBody.velocity.magnitude == 0)
        {
            player.Components.PlayerAnimator.SetFloat("BounceRate", 0.5f);
        }
        else
        {
            player.Components.PlayerAnimator.SetFloat("BounceRate", player.Stats.Speed / defaultSpeed);
        }
        
    }

    public void ToggleDual()
    {
        
        if(!leftArm.gameObject.activeSelf)
        {
            if (player.Stats.Angle < 0)
            {
                leftArm.position = new Vector2(player.Stats.Position.x + 0.3f, player.Stats.Position.y - 0.15f);
                rightArm.position = new Vector2(player.Stats.Position.x - 0.3f, player.Stats.Position.y - 0.15f);
            }
            else
            {
                leftArm.position = new Vector2(player.Stats.Position.x - 0.3f, player.Stats.Position.y - 0.15f);
                rightArm.position = new Vector2(player.Stats.Position.x + 0.3f, player.Stats.Position.y - 0.15f);
            }

            leftArm.gameObject.SetActive(true);
            LeftAmmo.gameObject.SetActive(true);
            player.Stats.IsDualWield = true;
        }
        else
        {
            foreach (Transform lw in leftArm)
            {
                if (lw.gameObject.activeSelf == true)
                {
                    Weapon lWeapon = lw.GetComponent<Weapon>();
                    lWeapon.transform.position = leftArm.transform.position;
                    lWeapon.transform.rotation = leftArm.transform.rotation;
                }
            }

            if (player.Stats.Angle < 0)
            {
                leftArm.position = new Vector2(player.Stats.Position.x + 0.05f, player.Stats.Position.y - 0.15f);
                rightArm.position = new Vector2(player.Stats.Position.x - 0.05f, player.Stats.Position.y - 0.15f);
            }
            else
            {
                leftArm.position = new Vector2(player.Stats.Position.x - 0.05f, player.Stats.Position.y - 0.15f);
                rightArm.position = new Vector2(player.Stats.Position.x + 0.05f, player.Stats.Position.y - 0.15f);
            }

            leftArm.gameObject.SetActive(false);
            LeftAmmo.gameObject.SetActive(false);
            player.Stats.IsDualWield = false;


        }
        
    }

    public void SwapWeapon()
    {
        float input;
        if(float.TryParse(Input.inputString, out input))
        {
            foreach (Transform rw in rightArm)
            {
                //Debug.Log(rw.gameObject.activeSelf);
                if (rw.GetComponent<Weapon>().Slot == input)
                {
                    rw.gameObject.SetActive(true);
                    foreach (Transform WIcon in RWeaponIcon.transform)
                        WIcon.gameObject.SetActive(false);
                    if (rw.name.Contains("Protocal"))
                    {
                        RWeaponIcon.transform.Find("ProtocalIcon").gameObject.SetActive(true);
                    }
                    else if (rw.name.Contains("Sedator"))
                    {
                        RWeaponIcon.transform.Find("SedatorIcon").gameObject.SetActive(true);
                    }
                    else if (rw.name.Contains("Neutralizer"))
                    {
                        RWeaponIcon.transform.Find("NeutralizerIcon").gameObject.SetActive(true);
                    }
                    else if (rw.name.Contains("Precision"))
                    {
                        RWeaponIcon.transform.Find("PrecisionIcon").gameObject.SetActive(true);
                    }
                    else if (rw.name.Contains("Umpilical"))
                    {
                        RWeaponIcon.transform.Find("UmpilicalIcon").gameObject.SetActive(true);
                    }
                    else if (rw.name.Contains("Disruptor"))
                    {
                        RWeaponIcon.transform.Find("DisruptorIcon").gameObject.SetActive(true);
                    }
                    else if (rw.name.Contains("Vaccinator2.0"))
                    {
                        RWeaponIcon.transform.Find("Vaccinator2.0Icon").gameObject.SetActive(true);
                    }
                    //start swap counter???
                }
                else
                {
                    Weapon RWeapon = rw.GetComponent<Weapon>();
                    RWeapon.transform.position = rightArm.transform.position;
                    RWeapon.transform.rotation = rightArm.transform.rotation;
                    rw.gameObject.SetActive(false);
                }
            }
        }
    }

    public void Phizer()
    {
        player.Stats.NumofPhizer -= 1;
        ParticleSystem.ColorOverLifetimeModule pSsettings = player.Components.PlayerParticleSystem.colorOverLifetime;
        pSsettings.color = new ParticleSystem.MinMaxGradient(new Color(255, 255, 255), new Color(0, 78, 137));
        player.Components.PlayerParticleSystem.Play();
        float tempMH = player.Stats.MaxHealth;
        float tempMS = player.Stats.MaxStamina;
        player.Stats.MaxHealth += player.Stats.MaxHealth * player.Stats.HPAdd / 100;
        //player.Stats.Health += player.Stats.MaxHealth - tempMH;
        player.Stats.MaxStamina += player.Stats.MaxStamina * player.Stats.StamAdd / 100;
        //player.Stats.Stamina += player.Stats.MaxStamina - tempMS;
        player.Stats.HPRegen += player.Stats.HPRegenAdd;
        player.Stats.StaminaRegenRate += player.Stats.StamRegenAdd;
        player.Components.PlayerTrailRenderer.endColor = new Color(0, 76 / 255f, 134 / 255f);
        currSpriteCategory = "PhizerHemo";
    }

    public void Heal()
    {
        //Debug.Log(player.References.numOfHeal);
        if (player.Stats.MaxHealth - player.Stats.Health < player.Stats.TylenolHealAmount)
        {
            player.Stats.Health = player.Stats.MaxHealth;
            player.Stats.NumofHeal--;
            player.Components.PlayerStatusIndicator.StartFlash(0.5f, 0.25f, Color.green, 0f, Color.red, 2);
            //player.Components.PlayerStatusIndicator.ChangeTransparency((player.Stats.hp - player.Stats.Health) / player.Stats.hp);
        }
        else
        {
            player.Stats.Health += player.Stats.TylenolHealAmount;
            player.Stats.NumofHeal--;
            player.Components.PlayerStatusIndicator.StartFlash(0.25f, ((player.Stats.MaxHealth - player.Stats.Health) / player.Stats.MaxHealth), Color.green, ((player.Stats.MaxHealth - player.Stats.Health) / player.Stats.MaxHealth) /2f, Color.red, 1);
            //player.Components.PlayerStatusIndicator.ChangeTransparency((player.Stats.hp - player.Stats.Health) / player.Stats.hp);
        }
    }

    public void UpdateCountsUI()
    {
        HealCounts.text = player.Stats.NumofHeal.ToString();
        VaccineCounts.text = player.Stats.NumofPhizer.ToString();
        MollyCounts.text = player.Stats.NumofMolly.ToString();
        HPNumber.text = ((int)player.Stats.Health).ToString();
        MaxHPNUmber.text = "/ " + ((int)player.Stats.MaxHealth).ToString();
        ARBar.fillAmount = player.Stats.Armorz / ((player.Stats.ArmorPerArmorLevelz * 4) - 1);
        switch (player.Stats.ArmorLevel)
        {
            case 1:
                ARBar.color = Color.white;
                break;
            case 2:
                ARBar.color = new Color(55 / 255f, 195 / 255f, 255 / 255f);
                break;
            case 3:
                ARBar.color = new Color(166 / 255f, 50 / 255f, 168 / 255f);
                break;
            case 4:
                ARBar.color = Color.yellow;
                break;
                /*
            case 5:
                ARBar.color = Color.green;
                break;*/
        }

    }

    public void Regen()
    {
        if (player.Stats.Health < player.Stats.MaxHealth)
        {
            player.Stats.Health += player.Stats.HPRegen * Time.deltaTime;
            player.Components.PlayerStatusIndicator.ChangeTransparency((player.Stats.MaxHealth - player.Stats.Health) / player.Stats.MaxHealth);
        }
        HealthBar.fillAmount = player.Stats.Health / player.Stats.MaxHealth;
    }

    
    public void ResetPlayerStats()
    {
        player.Components.PlayerParticleSystem.Stop();
        player.Stats.HPRegen -= player.Stats.HPRegenAdd; //HEALTH REGEN FUNCTION GOES HERE
        player.Stats.StaminaRegenRate -= player.Stats.StamRegenAdd; //STAMINA REGEN FUNCTION GOES HERE
        //HEALTH FUNCTION IMPLEMENTED HERE
        player.Stats.MaxHealth = player.healthGrowthRate * player.Currentlevel + GlobalPlayerVariables.baseMaxHealth; //player.Stats.maxhp; 
        if (player.Stats.Health > player.Stats.MaxHealth)
            player.Stats.Health = player.Stats.MaxHealth;
        //STAMINA FUNCTION NEEDS TO BE IMPLEMENTED HERE
        player.Stats.MaxStamina = player.Stats.maxplayerstamina;
        if (player.Stats.Stamina > player.Stats.Stamina)
            player.Stats.Stamina = player.Stats.MaxStamina;


        player.Components.PlayerTrailRenderer.endColor = new Color(184 / 255f, 59 / 255f, 60 / 255f);
        player.Components.PlayerStatusIndicator.ChangeTransparency((player.Stats.MaxHealth - player.Stats.Health) / player.Stats.MaxHealth);
        currSpriteCategory = "DefaultHemo";
    }

    public void LeftSlotCooldownDisplayUpdate(float LeftSlotFillAmount)
    {
        VaccineCooldownDisplay.fillAmount = LeftSlotFillAmount;
    }

    public void RightSlotCooldownDisplayUpdate(float RightSlotFillAmount)
    {
        HealCooldownDisplay.fillAmount = RightSlotFillAmount;
    }



    /*

    private IEnumerator wait(float duration)
    {
        isWaiting = true;
        yield return new WaitForSeconds(duration);
        isWaiting = false;
    }
    */

}