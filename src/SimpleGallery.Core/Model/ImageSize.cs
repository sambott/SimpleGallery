using System;

namespace SimpleGallery.Core.Model
{
    public struct ImageSize : IEquatable<ImageSize>
    {
        public readonly int Width;
        public readonly int Height;

        public ImageSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public bool Equals(ImageSize other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ImageSize size && Equals(size);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width * 397) ^ Height;
            }
        }
    }
}