using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Utils
{
    public static class TransformUtil
    {
        public static Matrix4x4 GetMatrix(this Transform trans)
        {
            return trans.localToWorldMatrix;
        }

        public static Matrix4x4 GetRelativeMatrix(this Transform trans, Transform relativeTo)
        {
            var m = trans.localToWorldMatrix;
            return relativeTo.worldToLocalMatrix * m;
        }

        public static Matrix4x4 GetLocalMatrix(this Transform trans)
        {
            return Matrix4x4.TRS(trans.localPosition, trans.localRotation, trans.localScale);
        }



        #region Matrix Methods

        public static  void   GetTransformFromMatrix(this Transform trans, Matrix4x4 m)
        {
            trans.localPosition = m.GetTranslation();
            trans.localRotation = m.GetRotation();
            trans.localScale = m.GetScale();
        }

        public static Vector3 GetTranslation(this Matrix4x4 m)
        {
            var col = m.GetColumn(3);
            return new Vector3(col.x, col.y, col.z);
        }

        public static Quaternion GetRotation(this Matrix4x4 matrix)
        {
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            /*
            Quaternion q = new Quaternion();
            q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
            q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
            q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
            q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
            q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
            q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
            q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
            return q;
            */
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }

        public static Vector3 GetScale(this Matrix4x4 matrix)
        {
            //var xs = m.GetColumn(0);
            //var ys = m.GetColumn(1);
            //var zs = m.GetColumn(2);

            //var sc = new Vector3();
            //sc.x = Vector3.Magnitude(new Vector3(xs.x, xs.y, xs.z));
            //sc.y = Vector3.Magnitude(new Vector3(ys.x, ys.y, ys.z));
            //sc.z = Vector3.Magnitude(new Vector3(zs.x, zs.y, zs.z));

            //return sc;

            //return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
            Vector3 scale = new Vector3(
                  matrix.GetColumn(0).magnitude,
                  matrix.GetColumn(1).magnitude,
                  matrix.GetColumn(2).magnitude
                  );
            if (Vector3.Cross(matrix.GetColumn(0), matrix.GetColumn(1)).normalized != (Vector3)matrix.GetColumn(2).normalized)
            {
                scale.x *= -1;
            }
            
            return scale;
        }

        #endregion

        #region GetAxis
        /*
        public static Vector3 GetAxis(CartesianAxis axis)
        {
            switch (axis)
            {
                case CartesianAxis.X:
                    return Vector3.right;
                case CartesianAxis.Y:
                    return Vector3.up;
                case CartesianAxis.Z:
                    return Vector3.forward;
            }

            return Vector3.zero;
        }

        public static Vector3 GetAxis(this Transform trans, CartesianAxis axis)
        {
            if (trans == null) throw new System.ArgumentNullException("trans");

            switch (axis)
            {
                case CartesianAxis.X:
                    return trans.right;
                case CartesianAxis.Y:
                    return trans.up;
                case CartesianAxis.Z:
                    return trans.forward;
            }

            return Vector3.zero;
        }

        public static Vector3 GetAxis(this Transform trans, CartesianAxis axis, bool inLocalSpace)
        {
            if (trans == null) throw new System.ArgumentNullException("trans");

            Vector3 v = Vector3.zero;
            switch (axis)
            {
                case CartesianAxis.X:
                    v = trans.right;
                    break;
                case CartesianAxis.Y:
                    v = trans.up;
                    break;
                case CartesianAxis.Z:
                    v = trans.forward;
                    break;
            }

            return (inLocalSpace) ? trans.InverseTransformDirection(v) : v;
        }
        */
        #endregion

        #region Parent

        public static Vector3 ParentTransformPoint(this Transform t, Vector3 pnt)
        {
            if (t.parent == null) return pnt;
            return t.parent.TransformPoint(pnt);
        }

        public static Vector3 ParentInverseTransformPoint(this Transform t, Vector3 pnt)
        {
            if (t.parent == null) return pnt;
            return t.parent.InverseTransformPoint(pnt);
        }

        #endregion

        #region Transform Methods

        public static Vector3 GetRelativePosition(this Transform trans, Transform relativeTo)
        {
            return relativeTo.InverseTransformPoint(trans.position);
        }

        public static Quaternion GetRelativeRotation(this Transform trans, Transform relativeTo)
        {
            //return trans.rotation * Quaternion.Inverse(relativeTo.rotation);
            return Quaternion.Inverse(relativeTo.rotation) * trans.rotation;
            //return Quaternion.Inverse(trans.rotation) * relativeTo.rotation;
        }

        /// <summary>
        /// Multiply a vector by only the scale part of a transformation
        /// </summary>
        /// <param name="m"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 ScaleVector(this Matrix4x4 m, Vector3 v)
        {
            var sc = m.GetScale();
            return Matrix4x4.Scale(sc).MultiplyPoint(v);
        }

      

        public static Vector3 ScaleVector(this Transform t, Vector3 v)
        {
            var sc = t.localToWorldMatrix.GetScale();
            return Matrix4x4.Scale(sc).MultiplyPoint(v);
        }

        /// <summary>
        /// Inverse multiply a vector by on the scale part of a transformation
        /// </summary>
        /// <param name="m"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 InverseScaleVector(this Matrix4x4 m, Vector3 v)
        {
            var sc = m.inverse.GetScale();
            return Matrix4x4.Scale(sc).MultiplyPoint(v);
        }



        public static Vector3 InverseScaleVector(this Transform t, Vector3 v)
        {
            var sc = t.worldToLocalMatrix.GetScale();
            return Matrix4x4.Scale(sc).MultiplyPoint(v);
        }

        public static Quaternion TranformRotation(this Matrix4x4 m, Quaternion rot)
        {
            return rot * m.GetRotation();
        }

 

        public static Quaternion TransformRotation(this Transform t, Quaternion rot)
        {
            return rot * t.rotation;
        }

        public static Quaternion InverseTranformRotation(this Matrix4x4 m, Quaternion rot)
        {
            //return rot * Quaternion.Inverse(m.GetRotation());
            return Quaternion.Inverse(m.GetRotation()) * rot;
        }



        public static Quaternion InverseTransformRotation(this Transform t, Quaternion rot)
        {
            //return rot * Quaternion.Inverse(t.rotation);
            return Quaternion.Inverse(t.rotation) * rot;
        }

     

        /// <summary>
        /// Transform a ray by a transformation
        /// </summary>
        /// <param name="m"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Ray TransformRay(this Matrix4x4 m, Ray r)
        {
            return new Ray(m.MultiplyPoint(r.origin), m.MultiplyVector(r.direction));
        }


        public static Ray TransformRay(this Transform t, Ray r)
        {
            return new Ray(t.TransformPoint(r.origin), t.TransformDirection(r.direction));
        }

        /// <summary>
        /// Inverse transform a ray by a transformation
        /// </summary>
        /// <param name="m"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Ray InverseTransformRay(this Matrix4x4 m, Ray r)
        {
            m = m.inverse;
            return new Ray(m.MultiplyPoint(r.origin), m.MultiplyVector(r.direction));
        }



        public static Ray InverseTransformRay(this Transform t, Ray r)
        {
            return new Ray(t.InverseTransformPoint(r.origin), t.InverseTransformDirection(r.direction));
        }

        /// <summary>
        /// Transform ray cast info by a transformation
        /// </summary>
        /// <param name="m"></param>
        /// <param name="r"></param>
        /// <returns></returns>
      

        #endregion

        #region Transpose Methods

        /// <summary>
        /// Set the position and rotation of a Transform as if its origin were that of 'anchor'. 
        /// Anchor should be in world space.
        /// </summary>
        /// <param name="trans">The transform to transpose.</param>
        /// <param name="anchor">The point around which to transpose in world space.</param>
        /// <param name="position">The new position in world space.</param>
        /// <param name="rotation">The new rotation in world space.</param>
        public static void TransposeAroundGlobalAnchor(this Transform trans, Vector3 anchor, Vector3 position, Quaternion rotation)
        {
            anchor = trans.InverseTransformPoint(anchor);
            if (trans.parent != null)
            {
                position = trans.parent.InverseTransformPoint(position);
                rotation = trans.parent.InverseTransformRotation(rotation);
            }

            LocalTransposeAroundAnchor(trans, anchor, position, rotation);
        }

        /// <summary>
        /// Set the position and rotation of a Transform as if its origin were that of 'anchor'. 
        /// Anchor should be in world space.
        /// </summary>
        /// <param name="trans">The transform to transpose.</param>
        /// <param name="anchor">The point around which to transpose in world space.</param>
        /// <param name="position">The new position in world space.</param>
        /// <param name="rotation">The new rotation in world space.</param>

        /// <summary>
        /// Set the position and rotation of a Transform as if its origin were that of 'anchor'. 
        /// Anchor should be local to the Transform where <0,0,0> would be the same as its true origin.
        /// </summary>
        /// <param name="trans">The transform to transpose.</param>
        /// <param name="anchor">The point around which to transpose in local space.</param>
        /// <param name="position">The new position in world space.</param>
        /// <param name="rotation">The new rotation in world space.</param>
        public static void TransposeAroundAnchor(this Transform trans, Vector3 anchor, Vector3 position, Quaternion rotation)
        {
            if (trans.parent != null)
            {
                position = trans.parent.InverseTransformPoint(position);
                rotation = trans.parent.InverseTransformRotation(rotation);
            }

            LocalTransposeAroundAnchor(trans, anchor, position, rotation);
        }

        /// <summary>
        /// Set the position and rotation of a Transform as if its origin were that of 'anchor'. 
        /// Anchor should be local to the Transform where <0,0,0> would be the same as its true origin.
        /// </summary>
        /// <param name="trans">The transform to transpose.</param>
        /// <param name="anchor">The point around which to transpose in local space.</param>
        /// <param name="position">The new position in world space.</param>
        /// <param name="rotation">The new rotation in world space.</param>
      

        /// <summary>
        /// Set the position and rotation of a Transform as if its origin were that of 'anchor'. 
        /// Anchor should be local to the Transform where <0,0,0> would be the same as its true origin.
        /// </summary>
        /// <param name="trans">The transform to transpose.</param>
        /// <param name="anchor">The point around which to transpose in world space.</param>
        /// <param name="position">The new position in world space.</param>
        /// <param name="rotation">The new rotation in world space.</param>
        public static void TransposeAroundAnchor(this Transform trans, Transform anchor, Vector3 position, Quaternion rotation)
        {
            if (trans.parent != null)
            {
                position = trans.parent.InverseTransformPoint(position);
                rotation = trans.parent.InverseTransformRotation(rotation);
            }

            //LocalTransposeAroundAnchor(trans, anchor.ToRelativeTrans(trans), position, rotation);
        }

        /// <summary>
        /// Set the localPosition and localRotation of a Transform as if its origin were that of 'anchor'. 
        /// Anchor should be local to the Transform where <0,0,0> would be the same as its true origin.
        /// </summary>
        /// <param name="trans">The transform to transpose.</param>
        /// <param name="anchor">The point around which to transpose relative to the transform.</param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public static void LocalTransposeAroundAnchor(this Transform trans, Vector3 anchor, Vector3 position, Quaternion rotation)
        {
            anchor = rotation * Vector3.Scale(anchor, trans.localScale);
            trans.localPosition = position - anchor;
            trans.localRotation = rotation;
        }

     

        public static void LocalTransposeAroundAnchor(this Transform trans, Transform anchor, Vector3 position, Quaternion rotation)
        {
            var m = anchor.GetRelativeMatrix(trans);

            var anchorPos = rotation * Vector3.Scale(m.GetTranslation(), trans.localScale);
            trans.localPosition = position - anchorPos;
            trans.localRotation = m.GetRotation() * rotation;
        }

        #endregion

    }
}