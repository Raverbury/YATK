using STG;
using UnityEngine;

public class FakeTransform
{
    public Vector3 translate;
    public Vector3 eulerAngles;
    public Vector3 scale;
    public FakeTransform parent = null;
    public static float perspectiveCameraZ = -200f;

    public FakeTransform() : this(Vector3.zero, Vector3.zero, Vector3.one) { }

    public FakeTransform(Vector3 translate) : this(translate, Vector3.zero, Vector3.one) { }

    public FakeTransform(Vector3 translate, FakeTransform parentFakeTransform) : this(translate, Vector3.zero, Vector3.one)
    {
        parent = parentFakeTransform;
    }

    public FakeTransform(Vector3 translate, Vector3 eulerAngles, Vector3 scale)
    {
        this.translate = translate;
        this.eulerAngles = eulerAngles;
        this.scale = scale;
    }

    public enum ProjectionType
    {
        Orthographic,
        Perspective,
    }

    public void ApplyTo(GameObject gameObject, ProjectionType projectionType)
    {
        ApplyTo(gameObject.transform, projectionType);
    }

    public void ApplyTo(Transform transform, ProjectionType projectionType)
    {
        // calculate transform matrix
        Matrix4x4 transformMatrix = CalculateTransformMatrix();
        // only set positions since we're only trying to recreate the 3d in 2d effect, not replicate the entire transform
        // in essence, only uses scale and rotate to calculate pos when stuff has parent(s), while the object manages its own scale and rotate
        transform.position = projectionType switch
        {
            ProjectionType.Perspective => GetPerspectivePos(transformMatrix),
            _ => GetOrthographicPos(transformMatrix),
        };
    }

    private Matrix4x4 CalculateTransformMatrix()
    {
        Matrix4x4 parentTransform = (parent != null) ? parent.CalculateTransformMatrix() : Matrix4x4.identity;
        Matrix4x4 translate = Matrix4x4.Translate(this.translate);
        Matrix4x4 scale = Matrix4x4.Scale(this.scale);
        Matrix4x4 rotate = Matrix4x4.Rotate(Quaternion.Euler(this.eulerAngles));
        return parentTransform * (translate * rotate * scale);
    }

    private Vector3 GetOrthographicPos(Matrix4x4 transformMatrix)
    {
        return new Vector3(transformMatrix.m03, transformMatrix.m13, 0);
    }

    private Vector3 GetPerspectivePos(Matrix4x4 transformMatrix)
    {
        Vector3 screenCenter = new Vector3(Constant.GAME_CENTER_X, Constant.GAME_CENTER_Y);
        return (GetOrthographicPos(transformMatrix) - screenCenter) * Mathf.Abs(perspectiveCameraZ) / Mathf.Abs(transformMatrix.m23 - perspectiveCameraZ) + screenCenter;
    }
}