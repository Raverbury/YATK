
using UnityEngine;

public interface ISingle
{
    public int GetLife();
    public int GetTimer();
    public int GetScore();
    public string GetName();
    public bool IsTimeout();
}
