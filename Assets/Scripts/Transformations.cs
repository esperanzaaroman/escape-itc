using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Transformations
{
    public enum AXIS
    {
        AX_X,
        AX_Y,
        AX_Z
    }

    public static Matrix4x4 TranslateM(float tx, float ty, float tz)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 3] = tx;
        m[1, 3] = ty;
        m[2, 3] = tz;
        return m;
    }

    public static Matrix4x4 ScaleM(float sx, float sy, float sz)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = sx;
        m[1, 1] = sy;
        m[2, 2] = sz;
        return m;
    }

    public static Matrix4x4 RotateM(float angle, AXIS axis)
    {
        Matrix4x4 m = Matrix4x4.identity;
        float rad = angle * Mathf.Deg2Rad;
        
        if (axis == AXIS.AX_X)
        {
            m[1, 1] = Mathf.Cos(rad);
            m[1, 2] = -Mathf.Sin(rad);
            m[2, 1] = Mathf.Sin(rad);
            m[2, 2] = Mathf.Cos(rad);
        }
        else if (axis == AXIS.AX_Y)
        {
            m[0, 0] = Mathf.Cos(rad);
            m[0, 2] = Mathf.Sin(rad);
            m[2, 0] = -Mathf.Sin(rad);
            m[2, 2] = Mathf.Cos(rad);
        }
        else if (axis == AXIS.AX_Z)
        {
            m[0, 0] = Mathf.Cos(rad);
            m[0, 1] = -Mathf.Sin(rad);
            m[1, 0] = Mathf.Sin(rad);
            m[1, 1] = Mathf.Cos(rad);
        }
        
        return m;
    }

    public static Vector4 Translate(Vector4 original, float x, float y, float z)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 3] = x;
        m[1, 3] = y;
        m[2, 3] = z;
        return m * original;
    }

    public static Vector4 Translate(Vector4 original, Vector3 translation)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 3] = translation.x;
        m[1, 3] = translation.y;
        m[2, 3] = translation.z;
        return m * original;
    }

    public static Vector4 Scale(Vector4 original, float x, float y, float z)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = x;
        m[1, 1] = y;
        m[2, 2] = z;
        return m * original;
    }

    public static Vector4 Scale(Vector4 original, Vector3 scale)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m[0, 0] = scale.x;
        m[1, 1] = scale.y;
        m[2, 2] = scale.z;
        return m * original;
    }

    public static Vector4 RotateX(Vector4 original, float deg)
    {
        Matrix4x4 m = Matrix4x4.identity;
        float rad = Mathf.Deg2Rad * deg;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        m[1, 1] = cos;
        m[1, 2] = -sin;
        m[2, 1] = sin;
        m[2, 2] = cos;
        return m * original;
    }

    public static Vector4 RotateY(Vector4 original, float deg)
    {
        Matrix4x4 m = Matrix4x4.identity;
        float rad = Mathf.Deg2Rad * deg;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        m[0, 0] = cos;
        m[0, 2] = sin;
        m[2, 0] = -sin;
        m[2, 2] = cos;
        return m * original;
    }

    public static Vector4 RotateZ(Vector4 original, float deg)
    {
        Matrix4x4 m = Matrix4x4.identity;
        float rad = Mathf.Deg2Rad * deg;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        m[0, 0] = cos;
        m[0, 1] = -sin;
        m[1, 0] = sin;
        m[1, 1] = cos;
        return m * original;
    }

}
