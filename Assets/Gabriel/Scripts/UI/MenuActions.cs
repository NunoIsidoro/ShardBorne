using UnityEngine;

public class MenuActions : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] private SkillIcon doubleJumpIcon;
    [SerializeField] private SkillIcon wallJumpIcon;
    [SerializeField] private SkillIcon heavyAttackIcon;
    [SerializeField] private SkillIcon healIcon;

    [Header("Abilities References (Auto-Filled)")]
    private MultipleJumpAbility multipleJump;
    private JumpAbility jump;
    private WallJumpAbility wallJump;
    private RecoverLifeAbility healAbility;

    // Referência externa
    public DialogueVariables dialogueVars;
    private void Start()
    {
        if (Player.Instance != null)
        {
            multipleJump = Player.Instance.GetComponent<MultipleJumpAbility>();
            jump = Player.Instance.GetComponent<JumpAbility>();
            wallJump = Player.Instance.GetComponent<WallJumpAbility>();
            healAbility = Player.Instance.GetComponent<RecoverLifeAbility>();
        }
    }
    public void UnlockDoubleJump()
    {
        multipleJump.SetMaxJumpNumber(2);
        doubleJumpIcon.OnSkillClicked();
        Debug.Log("Double Jump ativdo");
        // Aqui colocas a lógica que deve acontecer
    }

    public void UnlockWallJump()
    {
        wallJump.isPermitted = true;
        wallJumpIcon.OnSkillClicked();
        Debug.Log("Wall Jump ativado");
    }

    public void UnlockHeavyAtack()
    {
        heavyAttackIcon.OnSkillClicked();
        Debug.Log("Heavy Attack ativado");


        // Atualiza a flag no sistema de variáveis
        dialogueVars.flags["ability_HeavyStrike"] = true;

        SaveSystem.SaveAll(dialogueVars);
    }

    public void UnlockHeal()
    {
        healAbility.isPermitted = true;
        healIcon.OnSkillClicked();
        Debug.Log("Heal ativado");
    }
}
