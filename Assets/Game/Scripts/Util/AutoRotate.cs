using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float rotateSpeed = 14;

    // Update is called once per frame
    private void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, rotateSpeed);
    }
}
