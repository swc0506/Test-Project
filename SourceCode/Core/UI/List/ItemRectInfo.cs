using System;
using UnityEngine;

namespace Core.UI
{
    public enum Arrangement
    {
        Horizontal,
        Vertical
    }

    internal struct ItemRectInfo : IEquatable<ItemRectInfo>
    {
        public int index;
        public Vector2 position;
        public Vector2 size;

        //左上角为原点
        public float xMin
        {
            get { return position.x; }
        }

        public float xMax
        {
            get { return position.x + size.x; }
        }

        public float yMax
        {
            get { return position.y; }
        }

        public float yMin
        {
            get { return position.y - size.y; }
        }

        public bool Equals(ItemRectInfo other)
        {
            return index == other.index && position.Equals(other.position) && size.Equals(other.size);
        }

        public override bool Equals(object obj)
        {
            return obj is ItemRectInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = index;
                hashCode = (hashCode * 397) ^ position.GetHashCode();
                hashCode = (hashCode * 397) ^ size.GetHashCode();
                return hashCode;
            }
        }
    }
}