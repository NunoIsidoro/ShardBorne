// SpawnPoint2D.cs
using UnityEngine;

public enum FaceDir { Unchanged, Left, Right }

public class SpawnPoint2D : MonoBehaviour
{
    [Tooltip("ID único deste ponto (ex.: Start, PortaSul, Praca).")]
    public string spawnId = "Start";

    [Header("Orientação")]
    public FaceDir face = FaceDir.Unchanged;

    [Header("Ajustes")]
    [Tooltip("Offset adicional em unidades de mundo (x,y).")]
    public Vector2 spawnOffset = Vector2.zero;

    [Tooltip("Tentar encostar o player ao chão ao nascer.")]
    public bool snapToGround = true;

    [Tooltip("Distância para procurar o chão abaixo do spawn.")]
    public float groundSnapDistance = 0.8f;

    [Tooltip("Layer(s) considerados chão.")]
    public LayerMask groundMask;
}
