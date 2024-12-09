using Jose;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using STDApp.Models;
using System;
using System.IO;

namespace STDApp.Common
{
    public class APIUtil
    {
        private static byte[] hexStringToBytes(String s)
        {
            byte[] ans = new byte[s.Length / 2];

            for (int i = 0; i < ans.Length; i++)
            {
                int index = i * 2;
                int val = Convert.ToInt32(s.Substring(index, 2), 16);
                ans[i] = (byte)val;
            }

            return ans;
        }
        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static string doEncryptJWE(string payload, string symmetricKey)
        {
            byte[] key = hexStringToBytes(symmetricKey);
            // Encrypt the payload using JWE
            string jwe = Jose.JWT.Encode(payload, key, JweAlgorithm.A256KW, JweEncryption.A128GCM);

            string[] parts = jwe.Split('.');

            Recipient recipient = new Recipient();
            recipient.header = new Header();
            recipient.encrypted_key = parts[1];

            Recipient[] recipients = new Recipient[1];
            recipients[0] = recipient;

            var jwePayload = new Req_000_JWEBIDVInquiryRequest();
            jwePayload.recipients = recipients;
            jwePayload.protectedField = parts[0];
            jwePayload.ciphertext = parts[3];
            jwePayload.iv = parts[2];
            jwePayload.tag = parts[4];

            var payloadRequest = JsonConvert.SerializeObject(jwePayload);          
            return payloadRequest;
        }

        public static string DoSignatureJWS(string payloadRequest)
        {
            string privateKeyPem = loadRsaPrivateKeyPem();
            AsymmetricKeyParameter keyParameter;
            using (var reader = new System.IO.StringReader(privateKeyPem))
            {
                var pemReader = new PemReader(reader);
                keyParameter = pemReader.ReadObject() as AsymmetricKeyParameter;
                if (keyParameter == null)
                {
                    throw new InvalidOperationException("Invalid private key format.");
                }
            }
            
            var rsa = DotNetUtilities.ToRSA((Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)keyParameter);
            string jwsToken = Jose.JWT.Encode(payloadRequest, rsa, Jose.JwsAlgorithm.RS256);
            char[] delimiterChars = { '.' };
            string[] words = jwsToken.Split(delimiterChars);
            string xJwsSignature = words[0] + ".." + words[2];

            //Console.WriteLine(xJwsSignature);
            return xJwsSignature;
        }


        static string Base64Encoding(byte[] input)
        {
            return Convert.ToBase64String(input);
        }

        static byte[] Base64Decoding(String input)
        {
            return Convert.FromBase64String(input);
        }

        private static byte[] getRsaPrivateKeyEncodedFromPem(string rsaPrivateKeyPem)
        {
            string rsaPrivateKeyHeaderPem = "-----BEGIN PRIVATE KEY-----";
            string rsaPrivateKeyFooterPem = "-----END PRIVATE KEY-----";
            string rsaPrivateKeyDataPem = rsaPrivateKeyPem.Replace(rsaPrivateKeyHeaderPem, "").Replace(rsaPrivateKeyFooterPem, "").Replace("\n", "");
            return Base64Decoding(rsaPrivateKeyDataPem);
        }

        private static string loadRsaPrivateKeyPem()
        {
            var certPath = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";
            var text = File.ReadAllText(certPath);
            return text;
           
        }
    }
}
