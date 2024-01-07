using System;
using System.Security.Cryptography;
using System.Text;
using TeamCatalyst.Catalyst.Abstractions.AssemblyRewriting;

namespace TeamCatalyst.Catalyst.Abstractions.Hashing;

public static class Hasher {
    public static void HashBuffer(this ICryptoTransform hash, byte[] buffer) {
        hash.TransformBlock(buffer, 0, buffer.Length, buffer, 0);
    }

    public static void HashString(this ICryptoTransform hash, string value) {
        HashBuffer(hash, Encoding.UTF8.GetBytes(value));
    }

    public static void HashInt32(this ICryptoTransform hash, int value) {
        HashBuffer(hash, BitConverter.GetBytes(value));
    }

    public static void HashBoolean(this ICryptoTransform hash, bool value) {
        HashBuffer(hash, BitConverter.GetBytes(value));
    }

    public static string ComputeHash(byte[] bytes, params IAssemblyRewriter[] hashers) {
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
