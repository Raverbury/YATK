using UnityEngine;

public class BruhBullet : MonoBehaviour {
    private void Update() {
        if (transform.position.y < -448) {
            transform.position += new Vector3(0f, 500f, 0f);
        }
        transform.position += transform.right * 2f;
    }
}