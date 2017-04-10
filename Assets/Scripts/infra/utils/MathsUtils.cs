using UnityEngine;
using System;

namespace Infra.Utils {
public static class MathsUtils {
    /// <summary>
    /// Returns the angle between the vector and positive X axis (right).
    /// Angle is in range (-180, 180].
    /// Positive is counter clock wise.
    /// </summary>
    public static float GetAngle(this Vector2 direction) {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle > 180f) {
            angle -= 360f;
        } else if (angle <= -180f) {
            angle += 360f;
        }
        return angle;
    }

    /// <summary>
    /// Returns the angle between the vector and an axis.
    /// Angle is in range (-180, 180].
    /// Positive is counter clock wise.
    /// </summary>
    public static float GetAngle(this Vector2 direction, Vector2 axis) {
        float angle = direction.GetAngle() - axis.GetAngle();
        if (angle > 180f) {
            angle -= 360f;
        } else if (angle <= -180f) {
            angle += 360f;
        }
        return angle;
    }

    /// <summary>
    /// Rotate a vector by specified degrees (in radians).
    /// </summary>
    public static Vector2 Rotate(this Vector2 v, float radians) {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = (cos * v.x) - (sin * v.y);
        float ty = (sin * v.x) + (cos * v.y);

        return new Vector2(tx, ty);
    }

    public static Vector2 GetWithMagnitude(this Vector2 v, float magnitude) {
        return v.normalized * magnitude;
    }

    public static Vector3 MultiplyChannels(this Vector3 lhs, Vector3 rhs) {
        lhs.x *= rhs.x;
        lhs.y *= rhs.y;
        lhs.z *= rhs.z;
        return lhs;
    }

    public static Vector2 MultiplyChannels(this Vector2 lhs, Vector2 rhs) {
        lhs.x *= rhs.x;
        lhs.y *= rhs.y;
        return lhs;
    }

    public static Vector2 MultiplyChannels(this Vector2 lhs, float x, float y) {
        lhs.x *= x;
        lhs.y *= y;
        return lhs;
    }

    public static void Set(this Transform transform, Vector2 value) {
        var position = transform.position;
        position.x = value.x;
        position.y = value.y;
        transform.position = position;
    }

    public static void SetX(this Transform transform, float value) {
        var position = transform.position;
        position.x = value;
        transform.position = position;
    }

    public static void SetY(this Transform transform, float value) {
        var position = transform.position;
        position.y = value;
        transform.position = position;
    }

    public static void SetZ(this Transform transform, float value) {
        var position = transform.position;
        position.z = value;
        transform.position = position;
    }

    public static void SetZRotation(this Transform transform, float value) {
        var rotation = transform.eulerAngles;
        rotation.z = value;
        transform.eulerAngles = rotation;
    }

    public static void SetLocal(this Transform transform, Vector2 value) {
        var position = transform.localPosition;
        position.x = value.x;
        position.y = value.y;
        transform.localPosition = position;
    }

    public static void SetLocalX(this Transform transform, float value) {
        var position = transform.localPosition;
        position.x = value;
        transform.localPosition = position;
    }

    public static void SetLocalY(this Transform transform, float value) {
        var position = transform.localPosition;
        position.y = value;
        transform.localPosition = position;
    }

    public static void SetLocalZ(this Transform transform, float value) {
        var position = transform.localPosition;
        position.z = value;
        transform.localPosition = position;
    }

    public static void SetScale(this Transform transform, float value) {
        var scale = transform.localScale;
        scale.x = value;
        scale.y = value;
        scale.z = value;
        transform.localScale = scale;
    }

    public static void SetScaleX(this Transform transform, float value) {
        var scale = transform.localScale;
        scale.x = value;
        transform.localScale = scale;
    }

    public static void SetScaleY(this Transform transform, float value) {
        var scale = transform.localScale;
        scale.y = value;
        transform.localScale = scale;
    }

    public static void SetScaleZ(this Transform transform, float value) {
        var scale = transform.localScale;
        scale.z = value;
        transform.localScale = scale;
    }

    public static void SetLocalZRotation(this Transform transform, float value) {
        var rotation = transform.localEulerAngles;
        rotation.z = value;
        transform.localEulerAngles = rotation;
    }

    public static bool Approximately(this Vector2 lhs, Vector2 rhs) {
        return Mathf.Approximately((lhs - rhs).sqrMagnitude, 0f);
    }

    public static bool Approximately(this Vector3 lhs, Vector3 rhs) {
        return Mathf.Approximately((lhs - rhs).sqrMagnitude, 0f);
    }

    /// <summary>
    /// C# modulus is strange and allows negative numbers. Use this to get
    /// only non-negative values.
    /// </summary>
    public static int Mod(int x, int m) {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    public static uint Max(uint lhs, uint rhs) {
        return lhs < rhs ? rhs : lhs;
    }
}
}
