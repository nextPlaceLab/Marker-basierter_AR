﻿namespace nextPlace.ARVideoPlayer {
	using UnityEngine;
	using System.Collections;

	public static class MatrixHelper {

		public static Matrix4x4 GetMatrix(Quaternion q, Vector3 pos)
		{
			var m = new Matrix4x4();
            m.SetTRS(pos, q.normalized, Vector3.one);
			return m;
		}
		public static Quaternion GetQuaternion(this Matrix4x4 matrix) {
			Vector3 forward = new Vector3 (matrix.m02, matrix.m12, matrix.m22);
			Vector3 upwards = new Vector3 (matrix.m01, matrix.m11, matrix.m21);
			return Quaternion.LookRotation (forward, upwards);
		}

		public static Vector3 GetPosition(this Matrix4x4 matrix) {
			return new Vector3 (matrix.m03, matrix.m13, matrix.m23); 
		}

		public static Vector3 GetScale(this Matrix4x4 matrix) {
			Vector3 scale;
			scale.x = new Vector4 (matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
			scale.y = new Vector4 (matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
			scale.z = new Vector4 (matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
			return scale;
		}
     
    }
}