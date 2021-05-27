using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension {

	public static Vector3 Vec4XYZ (this Vector4 vec4)
	{
		return new Vector3(vec4.x, vec4.y, vec4.z);
	}

	public static Vector3 CosVec3 (this Vector3 vec)
	{
		return new Vector3(Mathf.Cos(vec.x), Mathf.Cos(vec.y), Mathf.Cos(vec.z));
	}

	public static Vector3 FloatAdd(this Vector3 vec, float flo)
	{
		return new Vector3(vec.x + flo, vec.y + flo, vec.z + flo);
	}

	public static Vector3 Mul(this Vector3 vec, Vector3 vec2)
	{
		return new Vector3(vec.x * vec2.x, vec.y * vec2.y, vec.z * vec2.z);
	}

	public static float RemapValue(this float value, Vector2 inRange, Vector2 outRange, bool clamped = true)
	{
		float temp = (value - inRange.x) / (inRange.y - inRange.x);
		if(clamped)
			return Mathf.Lerp(outRange.x, outRange.y, temp);
		else
			return Mathf.LerpUnclamped(outRange.x, outRange.y, temp);
	}

	public static float RandomVec(this Vector2 vec)
	{
		return Random.Range(vec.x, vec.y);
	}

	public static int RandomVec(this Vector2Int vec)
	{
		return Random.Range(vec.x, vec.y);
	}

	public static Vector3 ClampVec(this Vector3 vec, Vector2 clamp)
	{
		Vector3 temp = vec;
		temp.x = Mathf.Clamp(temp.x, clamp.x, clamp.y);
		temp.y = Mathf.Clamp(temp.y, clamp.x, clamp.y);
		temp.z = Mathf.Clamp(temp.z, clamp.x, clamp.y);
		return temp;
	}
}
