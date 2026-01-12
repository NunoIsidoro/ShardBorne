using UnityEngine;

public static class SceneTransit
{
    public static string nextSpawnId = null;

    // Anti-bounce: tempo até o portal poder voltar a disparar
    public static float portalCooldownUntil = 0f;

    public static void ArmCooldown(float seconds)
    {
        portalCooldownUntil = Time.unscaledTime + Mathf.Max(0f, seconds);
    }
}
