using System.Collections.Generic;
using UnityEngine;

namespace myField.Utils
{
    public struct Transformation
    {
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }

        public Transformation(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public Transformation(Transform transform)
        {
            Position = transform.position;
            Rotation = transform.rotation;
        }

        public Vector3 Transform(Vector3 vector)
        {
            return Rotation * vector + Position;
        }

        public List<Vector3> Transform(IList<Vector3> vectors)
        {
            var newPos = new List<Vector3>(vectors.Count);
            for (int i = 0; i < vectors.Count; i++)
                newPos.Add(Transform(vectors[i]));

            return newPos;
        }
    }
}