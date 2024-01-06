using System.Security.Cryptography;

namespace TeamCatalyst.Catalyst.Abstractions.Hashing;

public interface IHasher {
    void Hash(ICryptoTransform hash);
}
