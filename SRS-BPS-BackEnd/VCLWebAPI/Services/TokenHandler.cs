using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace VCLWebAPI.Services
{
    class TokenHandler
    {
        private readonly bool _isStaging = false;

        private readonly string _stagingCredentials = "clickdummy:clickme";

        private static readonly string _defaultPasskeyStaging = "1tn43528igd12vx5b452897xhn8m62l3a";
        //private static readonly string _defaultPasskeyProduction = "1tn43528igd12vx5b452897xhn8m62l3a"; //"4tz43528igk42vy5b452897xhn8m66l3x";
        private static readonly string _defaultPasskeyProduction = "4tz43528igk42vy5b452897xhn8m66l3x";

        private static readonly string _validationUrlStaging = "https://cmpreview.schueco.com/blueprint/servlet/generic/sso/validate";
        private static readonly string _validationUrlProduction = "https://www.schueco.com/blueprint/servlet/generic/sso/validate";

        /**
         * Encodes {@code plainText} with the default passkey into a hex string.
         *
         * @see #EncodeToHexString(String, String)
         * @see #DecodeHexString(String)
         */
        public string EncodeToHexString(string plainText)
        {
            var passkey = _defaultPasskeyStaging;
            if (!_isStaging)
            {
                passkey = _defaultPasskeyProduction;
            }
            return EncodeToHexString(plainText, passkey);
        }

        /**
         * Encodes {@code plainText} with {@code passkey} into a hex string.
         *
         * @see #Encode(String, String)
         * @see #DecodeHexString(String, String)
         */
        public string EncodeToHexString(string plainText, string passkey)
        {
            var cipherText = Encode(plainText, passkey);
            var encoded = BitConverter.ToString(cipherText).Replace("-", "");
            return encoded;
        }

        /**
         * Encodes {@code plainText} with the default passkey into a byte array.
         *
         * @see #Encode(String, String)
         */
        public byte[] Encode(string plainText)
        {
            var passkey = _defaultPasskeyStaging;
            if (!_isStaging)
            {
                passkey = _defaultPasskeyProduction;
            }
            return Encode(plainText, passkey);
        }

        /**
         * Encodes {@code plainText} with {@code passkey} into a byte array.
         *
         * @see #Decode(byte[], String)
         */
        public byte[] Encode(string plainText, string passkey)
        {
            byte[] cipherText = null;
            try
            {
                var engine = new TwofishEngine();

                BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine));

                var key = Encoding.UTF8.GetBytes(passkey);
                var input = Encoding.UTF8.GetBytes(plainText);

                var keyParam = new KeyParameter(key);

                cipher.Init(true, keyParam);

                cipherText = new byte[cipher.GetOutputSize(input.Length)];

                var outputLen = cipher.ProcessBytes(input, 0, input.Length, cipherText, 0);
                cipher.DoFinal(cipherText, outputLen);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.StackTrace);
            }
            return cipherText;
        }


        /**
         * Validates user with {@code id} and security token {@code token}.
         *
         */
        public async Task<HttpStatusCode> RequestValidation(string id, string token)
        {

            using (var client = new HttpClient())
            {

                var validationUrl = _validationUrlStaging;
                if (!_isStaging)
                {
                    validationUrl = _validationUrlProduction;
                }

                if (_isStaging)
                {
                    var byteArray = Encoding.ASCII.GetBytes(_stagingCredentials);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                }

                var dict = new Dictionary<string, string>();
                dict.Add("id", id);
                dict.Add("token", token);
                var req = new HttpRequestMessage(HttpMethod.Post, validationUrl) { Content = new FormUrlEncodedContent(dict) };
                HttpResponseMessage response = await client.SendAsync(req);

                return response.StatusCode;
            }

        }

        /**
         * Decodes a {@code hecString} with the default passkey.
         *
         * @see #DecodeHexString(String, String)
         * @see #EncodeToHexString(String)
         */
        public string DecodeHexString(string hexString)
        {

            var passkey = _defaultPasskeyStaging;
            if (!_isStaging)
            {
                passkey = _defaultPasskeyProduction;
            }

            return DecodeHexString(hexString, passkey);
        }

        /**
         * Decodes a {@code hecString} with {@code passkey}.
         *
         * @see #Decode(byte[], String)
         * @see #EncodeToHexString(String, String)
         */
        public string DecodeHexString(string hexString, string passkey)
        {
            var bytes = Enumerable.Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                .ToArray();

            return Decode(bytes, passkey);
        }

        /**
         * Decodes a byte array with the default passkey.
         *
         * @see #Decode(byte[], String)
         */
        public string Decode(byte[] bytes)
        {
            var passkey = _defaultPasskeyStaging;
            if (!_isStaging)
            {
                passkey = _defaultPasskeyProduction;
            }
            return Decode(bytes, passkey);
        }

        /**
         * Decodes {@code bytes} with {@code passkey}.
         *
         * @see #Encode(String, String)
         */
        public string Decode(byte[] bytes, string passkey)
        {
            byte[] decodedText = null;
            try
            {
                var engine = new TwofishEngine();
                BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine));

                var input = bytes;
                var key = Encoding.UTF8.GetBytes(passkey);

                cipher.Init(false, new KeyParameter(key));

                decodedText = new byte[cipher.GetOutputSize(input.Length)];

                var outputLen = cipher.ProcessBytes(input, 0, input.Length, decodedText, 0);
                cipher.DoFinal(decodedText, outputLen);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.StackTrace);
            }

            return Encoding.UTF8.GetString(decodedText).Trim();
        }
    }
}