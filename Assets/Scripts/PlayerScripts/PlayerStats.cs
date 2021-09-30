using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public Vector2 Direction { get; set; }

    public float Angle { get; set; }

    public Vector2 Position { get; set; }

    public float Speed { get; set; }

    public float Health { get; set; }

    public float Stamina { get; set; }

    public float MaxStamina { get; set; }
    public float StaminaRegenRate { get; set; }
    public float TimeBeforeStamRegen { get; set; }

    public int NumofHeal { get; set; }

    public bool IsDualWield = false;

    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float sprintSpeed;
    [SerializeField]
    private float HP;
    [SerializeField]
    private float playerStamina;
    [SerializeField]
    private float maxPlayerStamina;
    [SerializeField]
    public float staminaRegenRate;
    [SerializeField]
    private float TimeBfrStamRegen;
    [SerializeField]
    private int NumOfHeal;

    public float WalkSpeed { get => walkSpeed; }

    public float SprintSpeed { get => sprintSpeed; }

    public float hp { get => HP; }

    public float stamina { get => playerStamina; }

    public float maxplayerstamina { get => maxPlayerStamina; }

    public float staminaregenrate { get => staminaRegenRate; }

    public float StaminaRegen { get => TimeBfrStamRegen; }

    public int numofheal { get => NumOfHeal; }
}
