using UnityEngine;

[CreateAssetMenu(fileName = "LoopableBGM", menuName = "ScriptableObjects/LoopableBGM", order = 0)]
public class LoopableBGM : ScriptableObject
{
    [Tooltip("The AudioClip containing the BGM.")]
    public AudioClip bgmClip;
    [Tooltip("The start point of the loop, in samples.")]
    public int loopStartPoint;
    [Tooltip("The end point of the loop, in samples.")]
    public int loopEndPoint;
}