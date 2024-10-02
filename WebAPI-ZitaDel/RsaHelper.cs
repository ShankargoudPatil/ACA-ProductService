using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System;
using System.IO;
using System.Security.Cryptography;
namespace WebAPI_ZitaDel;

public class RsaHelper
{
    // Function to load the RSA private key from PEM format using BouncyCastle
    public static RSAParameters LoadRsaPrivateKey(string pem)
    {
        var privateKeyPem = pem.Replace("-----BEGIN RSA PRIVATE KEY-----", "")
                               .Replace("-----END RSA PRIVATE KEY-----", "")
                               .Replace("\n", "")
                               .Replace("\r", "");

        var privateKeyBytes = Convert.FromBase64String(privateKeyPem);

        using (var reader = new StringReader(pem))
        {
            var pemReader = new PemReader(reader);
            var keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
            var privateKeyParams = (RsaPrivateCrtKeyParameters)keyPair.Private;

            return new RSAParameters
            {
                Modulus = privateKeyParams.Modulus.ToByteArrayUnsigned(),
                Exponent = privateKeyParams.PublicExponent.ToByteArrayUnsigned(),
                D = privateKeyParams.Exponent.ToByteArrayUnsigned(),
                P = privateKeyParams.P.ToByteArrayUnsigned(),
                Q = privateKeyParams.Q.ToByteArrayUnsigned(),
                DP = privateKeyParams.DP.ToByteArrayUnsigned(),
                DQ = privateKeyParams.DQ.ToByteArrayUnsigned(),
                InverseQ = privateKeyParams.QInv.ToByteArrayUnsigned()
            };
        }
    }
}

