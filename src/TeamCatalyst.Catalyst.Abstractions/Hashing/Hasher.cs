using System;
using System.Security.Cryptography;
using System.Text;

namespace TeamCatalyst.Catalyst.Abstractions.Hashing;

public static class Hasher {
    public static void HashBuffer(ICryptoTransform hash, byte[] buffer) {
        hash.TransformBlock(buffer, 0, buffer.Length, buffer, 0);
    }

    public static void HashString(ICryptoTransform hash, string value) {
        HashBuffer(hash, Encoding.UTF8.GetBytes(value));
    }

    public static void HashInt32(ICryptoTransform hash, int value) {
        HashBuffer(hash, BitConverter.GetBytes(value));
    }

    public static void HashBoolean(ICryptoTransform hash, bool value) {
        HashBuffer(hash, BitConverter.GetBytes(value));
    }

    public static string ComputeHash(byte[] bytes, params IHasher[] hashers) {
        using var md5 = MD5.Create();

        // TODO: Hash versions here.

        foreach (var hasher in hashers)
            hasher.Hash(md5);

        md5.TransformFinalBlock(bytes, 0, bytes.Length);
        return ByteArrayToString(md5.Hash);
    }

    private static string ByteArrayToString(byte[] data) {
        var sb = new StringBuilder(data.Length * 2);

        foreach (var datum in data)
            sb.Append(datum.ToString("x2"));

        return sb.ToString();
    }
}
