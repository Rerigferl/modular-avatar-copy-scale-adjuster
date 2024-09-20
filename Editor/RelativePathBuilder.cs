#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Buffers;

namespace Numeira
{
    internal ref struct RelativePathBuilder
    {
        private char[] buffer;
        private Span<char> span;
        private int idx;

        public RelativePathBuilder(Span<char> initialBuffer)
        {
            if (initialBuffer.IsEmpty)
            {
                span = buffer = ArrayPool<char>.Shared.Rent(2048);
            }
            else
            {
                span = initialBuffer;
                buffer = null;
            }
            idx = span.Length - 1;
        }

        public string GetRelativePath(GameObject gameObject, GameObject root, ReadOnlySpan<char> prefix = default, ReadOnlySpan<char> suffix = default)
        {
            var child = gameObject;
            if (root == child) return "";

            while (child != root && child != null)
            {
                Append(child.name, "/", prefix, suffix);
                child = child.transform.parent?.gameObject;
            }

            if (child == null && root != null) return null;

            return ToString();
        }
        public override string ToString()
        {
            return span[(idx + 1)..].ToString();
        }

        public void Reset()
        {
            idx = span.Length - 1;
        }

        public void Dispose()
        {
            if (buffer != null)
                ArrayPool<char>.Shared.Return(buffer);
        }

        private void Append(ReadOnlySpan<char> value, ReadOnlySpan<char> separator, ReadOnlySpan<char> prefix, ReadOnlySpan<char> suffix)
        {
            var length = value.Length + separator.Length + prefix.Length + suffix.Length;
            if (idx - length < 0)
            {
                var newBuffer = ArrayPool<char>.Shared.Rent(Math.Max(2048, span.Length + length));
                var segment = span[idx..];
                segment.CopyTo(newBuffer.AsSpan()[^segment.Length..]);
                idx += newBuffer.Length - span.Length;
                span = buffer = newBuffer;
            }
            idx -= length;
            {
                var segment = span[idx..];
                separator.CopyTo(segment);
                segment = segment[separator.Length..];
                prefix.CopyTo(segment);
                segment = segment[prefix.Length..];
                value.CopyTo(segment);
                segment = segment[value.Length..];
                suffix.CopyTo(segment);
            }
        }

    }
}
#endif