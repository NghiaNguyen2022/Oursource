using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using PN.SmartLib.Helper;
using RestSharp;
using SAPCore;
using SAPCore.Config;
using STD.DataReader;
using STDApp.Models;
using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace STDApp.Common
{
    public class APIHelper
    {
        private static RSAParameters ConvertToRSAParameters(RsaPrivateCrtKeyParameters rsaKey)
        {
            RSAParameters result = default(RSAParameters);
            result.Modulus = rsaKey.Modulus.ToByteArrayUnsigned();
            result.Exponent = rsaKey.PublicExponent.ToByteArrayUnsigned();
            result.D = rsaKey.Exponent.ToByteArrayUnsigned();
            result.P = rsaKey.P.ToByteArrayUnsigned();
            result.Q = rsaKey.Q.ToByteArrayUnsigned();
            result.DP = rsaKey.DP.ToByteArrayUnsigned();
            result.DQ = rsaKey.DQ.ToByteArrayUnsigned();
            result.InverseQ = rsaKey.QInv.ToByteArrayUnsigned();
            return result;
        }
        private static RSA LoadPrivateKey(string filePath)
        {
            try
            {
                string text = File.ReadAllText(filePath);
                text = text.Replace("-----BEGIN PRIVATE KEY-----", "").Replace("-----END PRIVATE KEY-----", "").Replace("\n", "")
                    .Replace("\r", "")
                    .Trim();
                var obj = Convert.FromBase64String(text);
                var instance = PrivateKeyInfo.GetInstance(obj);
                var asymmetricKeyParameter = PrivateKeyFactory.CreateKey(instance);
                if (asymmetricKeyParameter is RsaPrivateCrtKeyParameters rsaKey)
                {
                    RSA rSA = RSA.Create();
                    rSA.ImportParameters(ConvertToRSAParameters(rsaKey));
                    return rSA;
                }

                throw new ArgumentException("Key is not an RSA private key.");
            }
            catch (Exception innerException)
            {
                throw new Exception("Error loading private key", innerException);
            }
        }
        static byte[] SignData(RSA rsa, string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            return rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        public static string GenSignature()
        {

            // Header (base64 encoded JSON)
            var header = new
            {
                alg = "RS256", // RSA with SHA-256
                typ = "JWT"
            };
            var headerJson = JsonSerializer.Serialize(header);
            var headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            var detachedPayload = string.Empty;

            var signingInput = $"{headerBase64}.{detachedPayload}";
            var certPath = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";

            var rsa = LoadPrivateKey(certPath);
            var signature = SignData(rsa, signingInput);

            var signatureBase64 = Base64UrlEncode(signature);
            var jws = $"{headerBase64}.{detachedPayload}.{signatureBase64}";
            return jws;
            // Console.WriteLine("Detached JWS:");
            //Console.WriteLine(jws);

            // Example: Verification
            //bool isValid = VerifySignature(rsa, signingInput, signature);
            //Console.WriteLine("Signature valid: " + isValid);
        }
        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
        public static string GetToken()
        {
            try
            {
                var query = string.Format(QueryString.GetAPICode);
                var data = DataProvider.QuerySingle(CoreSetting.DataConnection, query);
                if (data == null)
                {
                    return string.Empty;
                }
                var code = data["Code"].ToString();
                if (string.IsNullOrEmpty(code))
                {
                    return string.Empty;
                }

                var options = new RestClientOptions(ConfigurationManager.AppSettings["LinkBIDVAPI"])
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest(ConfigurationManager.AppSettings["AuthenBIDV"], Method.Post);

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("client_id", ConfigurationManager.AppSettings["ClientIDBIDV"]);
                request.AddParameter("client_secret", ConfigurationManager.AppSettings["ClientSecretBIDV"]);
                request.AddParameter("code", code);
                request.AddParameter("redirect_uri", ConfigurationManager.AppSettings["redirect_uri"]);

                var response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var error = JsonSerializer.Deserialize<BIDVAccesstokenResponseErrro>(response.Content);
                    if (error.error_description.Contains("code expired"))
                    {
                        UIHelper.LogMessage($"Code để lấy token đã bị hết hạn, vui lòng lấy lại code mới và thử lại", UIHelper.MsgType.StatusBar, true);

                        return string.Empty;
                    }
                    else
                    {
                        UIHelper.LogMessage($"Lỗi {error.error_description}", UIHelper.MsgType.StatusBar, true);

                        return string.Empty;
                    }
                }
                else
                {
                    var result = JsonSerializer.Deserialize<BIDVAccesstokenResponse>(response.Content);
                    if (result != null)
                        return result.access_token;
                }
            }
            catch (Exception ex)
            { }
            return string.Empty;
        }
    }
}
