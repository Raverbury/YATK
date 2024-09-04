using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AtuneRing : MonoBehaviour
{
    public float rotateSpeed = 14;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, rotateSpeed);
    }
}
