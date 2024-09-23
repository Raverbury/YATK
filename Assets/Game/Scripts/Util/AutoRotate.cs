using UnityEngine;

public class AutoRotate : PausableMono
{
    public float rotateSpeed = 14;

    // Update is called once per frame
    protected override void PausableUpdate()
    {
        transform.eulerAngles += new Vector3(0, 0, rotateSpeed);
    }
}
