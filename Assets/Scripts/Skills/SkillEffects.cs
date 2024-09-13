using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillEffects : MonoBehaviour
{
    [Header("Debug Info")]
    [SerializeField]
    public bool cooldownActive;
    public float cooldownRemaining;
    public bool durationActive;
    public float durationRemaining;
    private List<Skill> skillList = new List<Skill>();
    void Awake()
    {
        Load();
    }
    public void RunSkill(InputAction.CallbackContext context)
    {
        if (context.performed && !cooldownActive)
        {   
            //depending on active skill run the code that controls it, start the duration, and start the cooldown
            switch (GameSettings.activeSkill)
            {
                case SkillEnum.dash:
                {
                    Debug.Log("Player has Dashed!");
                    //get the position of the cursor on the screen and make a vector of its direction from the player
                    //add some velocity to the player and push them some distance towards that direction
                    break;
                }
                case SkillEnum.vanish:
                {
                    //disable the players collision for the duration
                    //add a darkness to the player or screen
                    //stop all enemy movement towards the player
                    //revert those once the duration has finished
                    Debug.Log("Player has Vanished!");
                    break;
                }
                case SkillEnum.decoy:
                {
                    //spawn another duck prefab
                    //make all enemies target that one for the duration
                    //after duration all eneimes go back to targeting the player
                    Debug.Log("Player has Decoyed!");
                    break;
                }
            }
            StartDuration();
            StartCooldown();
        }
    }
    private void Load()
    {
        string json = Resources.Load<TextAsset>("skills").text;
        SkillList skillsJson = JsonUtility.FromJson<SkillList>(json);
        skillList = skillsJson.skills;
    }

    void Update()
    {
        if (durationActive)
        {
            CheckDuration();
        }
        if (cooldownActive)
        {
            CheckCooldown();
        }
    }

    private void StartCooldown()
    {
        cooldownRemaining = skillList[(int)GameSettings.activeSkill].cooldown;//this restricts the ability to make one conjoined method of StartCooldown, StartDuration, etc. etc.
        cooldownActive = true;
    }

    private void StartDuration()
    {
        durationRemaining = skillList[(int)GameSettings.activeSkill].duration;
        durationActive = true;
    }

    private void CheckDuration()
    {
        if (durationRemaining > 0)
        {
            durationRemaining -= Time.deltaTime;
        }
        else
        {
            Debug.Log("Duration ended");
            durationActive = false;
        }
    }

    private void CheckCooldown()
    {
        if (cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;
        }
        else
        {
            Debug.Log("Cooldown ended");
            cooldownActive = false;
        }
    }

}
