using UnityEngine;

public class BruhBullet : MonoBehaviour {
    public float Speed;
    public int FramesToLive;

    private void Update() {
        transform.Translate(Vector3.right * Speed);
        FramesToLive -= 1;
        if (FramesToLive <= 0) {
            gameObject.SetActive(false);
        }
    }
}