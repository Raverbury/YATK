using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float acceleration;
    public float speedCap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * speed * 60 * Time.deltaTime);
        transform.eulerAngles += Vector3.forward * rotationSpeed * 60 * Time.deltaTime;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if ((int)THKO.Constant.CollisionLayer.PlayableArea ==  other.gameObject.layer) {
            gameObject.SetActive(false);
        }
    }
}
