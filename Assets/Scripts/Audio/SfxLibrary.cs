using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SFX Library")]
public class SfxLibrary : ScriptableObject
{
    [Header("Gameplay")]
    public AudioClip pellet;
    public AudioClip powerPellet;
    public AudioClip ghostEaten;
    public AudioClip death;
    public AudioClip start;
    public AudioClip extraLife;

    [Header("Music / Loops")]
    public AudioClip bgmLoop;
    public AudioClip frightenedLoop;
}
