using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Configuration;

namespace OAuth {
    public class BobOAuth : OAuth.OAuthBase {
        readonly static string baseOAuthURL = "https://api.login.yahoo.com/oauth/v2/";
        readonly static string getRequestTokenURLSuffix = "get_request_token";
        readonly static string requestAuthURLSuffix = "request_auth";
        readonly static string getTokenURLSuffix = "get_token";

        string getRequestTokenURL = baseOAuthURL + getRequestTokenURLSuffix;
        string requestAuthURL = baseOAuthURL + requestAuthURLSuffix;
        string getTokenURL = baseOAuthURL + getTokenURLSuffix;

        string consumerKey = string.Empty;
        string consumerSecret = string.Empty;
        string token = string.Empty;
        string tokenSecret = string.Empty;

        readonly string authCallback = "oob";

        public void SetConsumerProperties() {
            consumerKey = ConfigurationManager.AppSettings[Settings.ConsumerKeyKey];
            consumerSecret = ConfigurationManager.AppSettings[Settings.ConsumerSecretKey];
        }

        public void SetTokenProperties() {
            token = ConfigurationManager.AppSettings[Settings.TokenKeyKey];
            tokenSecret = ConfigurationManager.AppSettings[Settings.TokenSecretKey];
        }

        public void SetTokenProperties(string response) {
            var parameters = ParseParameters(response);
            token = GetParameter(OAuthTokenKey, parameters);
            tokenSecret = GetParameter(OAuthTokenSecretKey, parameters);
            var handle = GetParameter(OAuthSessionHandle, parameters);
            var yahooId = GetParameter(OAuthYahooGuid, parameters);

            ConfigurationPersister.PersistGetTokenResponse(token, tokenSecret, handle, yahooId);
        }

        string GenerateSignature(Uri uri, SignatureTypes signatureType, out string nURL, out string nParameters){
            return GenerateSignature(
                uri,
                consumerKey, consumerSecret, 
                token, tokenSecret, 
                "GET", 
                GenerateTimeStamp(), GenerateNonce(),
                signatureType, out nURL, out nParameters);
        }


        public string GenerateGetRequestTokenPlainTextSignedURL() {
            var nURL = "";
            var nParameters = "";

            var uri = new Uri(string.Format("{0}?{1}", getRequestTokenURL, GenerateParameter(OAuthCallbackKey, authCallback)));
            var requestSignature = GenerateSignature( uri,SignatureTypes.PLAINTEXT, out nURL, out nParameters);
            GenerateSignatureBase(uri, consumerKey,token, tokenSecret, "GET",  GenerateTimeStamp(), GenerateNonce(),  PlainTextSignatureType, 
                out nURL, out nParameters);
            
            return string.Format("{0}?{1}&{2}", nURL, nParameters, GenerateParameter(OAuthSignatureKey, requestSignature));
        }

        public string GenerateGetRequestTokenHMACSHA1SignedURL() {
            var nURL = "";
            var nParameters = "";

            var uri = new Uri(string.Format("{0}?{1}", getRequestTokenURL, GenerateParameter(OAuthCallbackKey, authCallback)));
            var requestSignature = GenerateSignature(uri, SignatureTypes.HMACSHA1, out nURL, out nParameters);

            return string.Format("{0}?{1}&{2}", nURL, nParameters, GenerateParameter(OAuthSignatureKey, UrlEncode(requestSignature)));
        }

        public string GenerateRequestAuthURL(string response) {
            var parameters = ParseParameters(response);
            token = GetParameter(OAuthTokenKey, parameters);
            tokenSecret = GetParameter(OAuthTokenSecretKey, parameters);

            return string.Format("{0}?{1}", requestAuthURL, GenerateParameter(OAuthTokenKey, token));
        }

        public string GenerateGetTokenURL(string verifier) {
            var nURL = "";
            var nParameters = "";

            var uri = new Uri(string.Format("{0}?{1}", getTokenURL, GenerateParameter(OAuthVerifierKey, UrlEncode(verifier))));
            var requestSignature = GenerateSignature(uri, SignatureTypes.HMACSHA1, out nURL, out nParameters);


            return string.Format("{0}?{1}&{2}", nURL, nParameters, GenerateParameter(OAuthSignatureKey, UrlEncode(requestSignature)));
        }

        public string GenerateRefreshGetTokenURL() {
            var nURL = "";
            var nParameters = "";

            SetTokenProperties();

            var uri = new Uri(string.Format("{0}?{1}", getTokenURL, GenerateParameter(OAuthSessionHandle, UrlEncode(ConfigurationManager.AppSettings[Settings.SessionHandleKey]))));
            var requestSignature = GenerateSignature(uri, SignatureTypes.HMACSHA1, out nURL, out nParameters);

            return string.Format("{0}?{1}&{2}", nURL, nParameters, GenerateParameter(OAuthSignatureKey, UrlEncode(requestSignature)));
        }

        string GenerateParameter(string key, string value) {
            return string.Format("{0}={1}", key, value);
        }

        string GetParameter(string key, List<QueryParameter> parameterList) {
            return parameterList.ToDictionary(p=>p.Name)[key].Value;
        }

        List<QueryParameter> ParseParameters(string parameters) {
            return GetQueryParameters(parameters, true);
        }
    }
}
