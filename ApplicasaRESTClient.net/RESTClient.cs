using System;
using System.Collections.Generic;

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace ApplicasaRESTClientnet
{
    public enum Currencies
    {
        Main,
        Secondary
    }
    public enum Platforms
    {
        iOS,
        Android,
        Windows,
        WebDevices
    }
    public class RESTClient : HttpClient, ApplicasaRESTClientnet.IRESTClient
    {
        public string ApplicationID { get; set; }
        public string ApplicationKey { get; set; }
        public string UserID { get; set; }
        public Dictionary<string, object> User { get { return _user; } }
        public Platforms Platform { get; set; }
        public float ApplicationVersion { get; set; }
        public bool IsSandbox { get; set; }

        protected Uri _baseAddress = new Uri("https://api.applicasa.com/3.0/");

        protected Dictionary<string, object> _user = new Dictionary<string, object>();
        protected HttpClient _client;
        public RESTClient()
        {

        }
        protected string transformPlatform(Platforms platform)
        {
            switch (platform)
            {
                case Platforms.iOS:
                    return "1";

                case Platforms.Android:
                    return "2";

                case Platforms.Windows:
                    return "3";
                case Platforms.WebDevices:
                    return "4";

                default:
                    return "-1";
            }
        }
        protected static string base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public async Task<Dictionary<string, object>> Initialize(string userID = null, string facebookToken = null)
        {
            
            var clientHandler = new HttpClientHandler() { Credentials = new NetworkCredential(ApplicationID, ApplicationKey)};
            _client = new HttpClient(clientHandler);
            _client.BaseAddress = _baseAddress;
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",base64Encode(String.Format("{0}:{1}",ApplicationID,ApplicationKey)));
            _client.DefaultRequestHeaders.Add("AppID", ApplicationID);
            _client.DefaultRequestHeaders.Add("Platform", transformPlatform(Platform));
            _client.DefaultRequestHeaders.Add("AppVersion", ApplicationVersion.ToString());
            _client.DefaultRequestHeaders.Add("IsSandbox", IsSandbox.ToString());

            StringContent postContent = null;
            if (facebookToken != null)
            {
                Dictionary<string, object> user = new Dictionary<string, object>();
                user["FacebookToken"] = userID;
                postContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            }
            else
                postContent = new StringContent("{}", Encoding.UTF8, "application/json");
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "User/Init");
            
            msg.Content = postContent;
            if (userID != null && facebookToken == null)
                msg.Headers.Add("UserID", userID);
            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                _user = userObject;
                UserID = userObject["UserID"] as string;
                return userObject;
            }
        }
        #region Login methods
        public async Task<Dictionary<string, object>> RegisterWithUserNameAndPassword(string userName, string passwordHash)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(userName))
                throw new Exception("Username has to be set");
            if (passwordHash == null)
            {
                throw new Exception("Password has to be empty string when using no password authentication");
            }
            Dictionary<string, object> user = new Dictionary<string, object>();
            user["UserName"] = userName;
            //hash
            user["UserPassword"] = passwordHash;
            postContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "User/RegisterWithUserNameAndPassword");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                _user = userObject;
                UserID = userObject["UserID"] as string;
                return userObject;
            }
        }
        public async Task<Dictionary<string, object>> LoginWithUserNameAndPassword(string userName, string passwordHash)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(userName))
                throw new Exception("Username has to be set");
            if (passwordHash == null)
            {
                throw new Exception("Password has to be empty string when using no password authentication");
            }

            Dictionary<string, object> user = new Dictionary<string, object>();
            user["UserName"] = userName;
            //hash
            user["UserPassword"] = passwordHash;
            postContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "User/LoginWithUserNameAndPassword");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                _user = userObject;
                UserID = userObject["UserID"] as string;
                return userObject;
            }
        }
        public async Task<bool> ResetPassword(string userName)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(userName))
                throw new Exception("Username has to be set");


            Dictionary<string, object> user = new Dictionary<string, object>();
            user["UserName"] = userName;

            postContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "User/ResetPassword");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            return true;
        }
        public async Task<bool> ChangeUserName(string currentUserName, string newUserName, string passwordHash)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(newUserName) || String.IsNullOrWhiteSpace(currentUserName))
                throw new Exception("Username has to be set");
            if (passwordHash == null)
            {
                throw new Exception("Password has to be empty string when using no password authentication");
            }

            Dictionary<string, object> user = new Dictionary<string, object>();
            user["CurrentUserName"] = currentUserName;
            user["NewUserName"] = newUserName;
            //hash
            user["UserPassword"] = passwordHash;
            postContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "User/ChangeUserName");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            return true;
        }
        public async Task<bool> ChangeUserPassword(string userName, string passwordHash, string newPasswordHash)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(userName))
                throw new Exception("Username has to be set");
            if (passwordHash == null || newPasswordHash == null)
            {
                throw new Exception("Password has to be empty string when using no password authentication");
            }

            Dictionary<string, object> user = new Dictionary<string, object>();
            user["UserName"] = userName;

            //hash
            user["CurrentUserPassword"] = passwordHash;
            user["NewUserPassword"] = newPasswordHash;
            postContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "User/ChangeUserPassword");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            return true;
        }
        public async Task<bool> RemoveUserNameAndPassword(string userName, string passwordHash)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(userName))
                throw new Exception("Username has to be set");
            if (passwordHash == null)
            {
                throw new Exception("Password has to be empty string when using no password authentication");
            }

            Dictionary<string, object> user = new Dictionary<string, object>();
            user["UserName"] = userName;
            //hash
            user["UserPassword"] = passwordHash;
            postContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "User/RemoveUserNameAndPassword");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            return true;
        }
        #endregion
        #region Facebook integration
        /*
         * Important : Make sure to check if the UserID in the response matches the UserID that was returned. 
         * If a different UserID is returned, it means there is an existing user with that Facebook account. 
         * In that case, you need to decide which of the users you want to use. 
         * If you still want to assign the current user to that Facebook account, 
         * you’ll have to run the method “DisconnectFacebook” on the user that was returned, 
         * and then run this method again on the current user.
         */

        public async Task<Dictionary<string, object>> LoginWithFacebook(string facebookToken)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(facebookToken))
                throw new Exception("Facebook token has to be set");


            Dictionary<string, object> user = new Dictionary<string, object>();
            user["FacebookToken"] = facebookToken;
            //hash

            postContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "User/LoginWithFacebook");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                _user = userObject;
                return userObject;
            }
        }
        public async Task<Dictionary<string, object>> FindFacebookFriends(string facebookToken)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(facebookToken))
                throw new Exception("Facebook token has to be set");


            Dictionary<string, object> user = new Dictionary<string, object>();
            user["FacebookToken"] = facebookToken;
            //hash

            postContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "User/FindFacebookFriends");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        public async Task<Dictionary<string, object>> DisconnectFacebook()
        {
            StringContent postContent = null;


            postContent = new StringContent("", Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "User/DisconnectFacebook");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        #endregion
        #region Backend
        public async Task<Dictionary<string, object>> CreateObject(string objectName, Dictionary<string, object> valueObject)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(objectName))
                throw new Exception("object name has to be set");
            if (valueObject == null)
                throw new Exception("value object has to be set");

            postContent = new StringContent(JsonConvert.SerializeObject(valueObject), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, objectName);
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        public async Task<bool> UpdateObject(string objectName, string recordID, Dictionary<string, object> changedValuesObject)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(objectName))
                throw new Exception("object name has to be set");
            if (String.IsNullOrWhiteSpace(recordID))
                throw new Exception("record id has to be set");
            if (changedValuesObject == null)
                throw new Exception("value object has to be set");

            postContent = new StringContent(JsonConvert.SerializeObject(changedValuesObject), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Put, objectName + "/" + recordID);
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                return true;
            }
        }
        public async Task<bool> DeleteObject(string objectName, string recordID)
        {

            if (String.IsNullOrWhiteSpace(objectName))
                throw new Exception("object name has to be set");
            if (String.IsNullOrWhiteSpace(recordID))
                throw new Exception("record id has to be set");


            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Delete, objectName + "/" + recordID);

            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                return true;
            }
        }
        public async Task<Dictionary<string, object>> GetObject(string objectName, string recordID, List<string> foreignKeys)
        {
            return await getObject(objectName, recordID, foreignKeys);
        }
        protected async Task<Dictionary<string, object>> getObject(string objectName, string recordID, List<string> foreignKeys, Dictionary<string, object> additionalParameters = null)
        {

            if (String.IsNullOrWhiteSpace(objectName))
                throw new Exception("object name has to be set");
            if (String.IsNullOrWhiteSpace(recordID))
                throw new Exception("record id has to be set");

            string queryString = "";
            if (foreignKeys != null && foreignKeys.Count > 0)
            {
                queryString = "? foreignKeys=" + String.Join(",", foreignKeys);
            }
            foreach (KeyValuePair<string, object> kvp in additionalParameters)
            {
                queryString += "&" + kvp.Key + "=" + kvp.Value.ToString();
            }
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, objectName + "/" + recordID + queryString);

            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }

            //read response
            var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
            return userObject;

        }
        public async Task<Dictionary<string, object>> GetObject(string objectName, Dictionary<string, object> query = null, int limit = 100, int offset = 0, List<string> foreignKeys = null, List<string> orderBy = null)
        {
            return await getObject(objectName, query, limit, offset, foreignKeys, orderBy);
        }
        public async Task<Dictionary<string, object>> GetObject(string objectName, Dictionary<string, object> query = null, int limit = 100, int offset = 0, List<string> foreignKeys = null, List<string> orderBy = null, Dictionary<string, object> additionalParameters = null)
        {
            return await getObject(objectName, query, limit, offset, foreignKeys, orderBy,additionalParameters);
        }
        protected async Task<Dictionary<string, object>> getObject(string objectName, Dictionary<string, object> query = null, int limit = 100, int offset = 0, List<string> foreignKeys = null, List<string> orderBy = null, Dictionary<string, object> additionalParameters = null)
        {

            if (String.IsNullOrWhiteSpace(objectName))
                throw new Exception("object name has to be set");


            string queryString = "?";
            if (query == null)
                queryString += "query={}";
            else
                queryString += "query=" + JsonConvert.SerializeObject(query);
            queryString += "&limit=" + limit.ToString();
            queryString += "&offset=" + offset.ToString();
            if (foreignKeys != null && foreignKeys.Count > 0)
            {
                queryString += "&foreignkeys=" + String.Join(",", foreignKeys);
            }
            if (orderBy != null && orderBy.Count > 0)
            {
                queryString += "&orderBy=" + String.Join(",", orderBy);
            }
            if (additionalParameters != null)
            {
                foreach (KeyValuePair<string, object> kvp in additionalParameters)
                {
                    queryString += "&" + kvp.Key + "=" + kvp.Value.ToString();
                }
            }
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, objectName + queryString);

            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        #endregion
        #region virtual currency
        public async Task<Dictionary<string, object>> GetVirtualCurrency(Dictionary<string, object> query = null, int limit = 100, int offset = 0, List<string> foreignKeys = null, List<string> orderBy = null)
        {
            return await GetObject("VirtualCurrency", query, limit, offset, foreignKeys, orderBy);
        }
        public async Task<Dictionary<string, object>> BuyVirtualCurrency(string currencyId, string receipt, string inAppPurchaseData = null)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(currencyId))
                throw new Exception("currency id has to be set");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");
            if (String.IsNullOrWhiteSpace(inAppPurchaseData) && Platform == Platforms.Android)
                throw new Exception("inAppPurchaseData has to be set for the android platform");
            Dictionary<string, object> currency = new Dictionary<string, object>();
            currency["VirtualCurrencyID"] = currencyId;
            currency["Receipt"] = receipt;
            if (Platform == Platforms.Android)
                currency["InAppPurchaseData"] = inAppPurchaseData;


            postContent = new StringContent(JsonConvert.SerializeObject(currency), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "VirtualCurrency/Buy");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                //update userObject
                foreach(var objectKey in userObject.Keys)
                {
                    _user[objectKey] = userObject[objectKey];
                }
                return userObject;
            }
        }
        public async Task<Dictionary<string, object>> GiveVirtualCurrency(Currencies currency, int amount, string receipt)
        {
            StringContent postContent = null;
            if (amount <= 0)
                throw new Exception("amount has to be greater 0");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");

            Dictionary<string, object> currencyObject = new Dictionary<string, object>();

            currencyObject["VirtualCurrencyKind"] = currency == Currencies.Main ? 1 : 2;
            currencyObject["Receipt"] = receipt;
            currencyObject["Amount"] = amount;


            postContent = new StringContent(JsonConvert.SerializeObject(currencyObject), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "VirtualCurrency/Give");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                //update userObject
                foreach (var objectKey in userObject.Keys)
                {
                    _user[objectKey] = userObject[objectKey];
                }
                return userObject;
            }
        }
        public async Task<Dictionary<string, object>> UseVirtualCurrency(Currencies currency, int amount, string receipt)
        {
            StringContent postContent = null;
            if (amount <= 0)
                throw new Exception("amount has to be greater 0");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");

            Dictionary<string, object> currencyObject = new Dictionary<string, object>();

            currencyObject["VirtualCurrencyKind"] = currency == Currencies.Main ? 1 : 2;
            currencyObject["Receipt"] = receipt;
            currencyObject["Amount"] = amount;


            postContent = new StringContent(JsonConvert.SerializeObject(currencyObject), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "VirtualCurrency/Use");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                //update userObject
                foreach (var objectKey in userObject.Keys)
                {
                    _user[objectKey] = userObject[objectKey];
                }
                return userObject;
            }
        }
        #endregion
        #region virtual goods
        public async Task<Dictionary<string, object>> GetVirtualGood(bool includeUserInventory = false, int? byCategoryPosition = null, Dictionary<string, object> query = null, int limit = 100, int offset = 0, List<string> foreignKeys = null, List<string> orderBy = null)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("includeUserInventory", includeUserInventory);
            if (byCategoryPosition.HasValue)
            {
                param.Add("byCategoryPosition", byCategoryPosition.Value);
            }
            return await getObject("VirtualGood", query, limit, offset, foreignKeys, orderBy, param);
        }

        public async Task<Dictionary<string, object>> BuyVirtualGoodWithCurrency(string virtualGoodId, Currencies currency, int amount, string receipt)
        {
            StringContent postContent = null;
            if (amount <= 0)
                throw new Exception("amount has to be greater 0");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");
            if (String.IsNullOrWhiteSpace(virtualGoodId))
                throw new Exception("virtual good id has to be set");

            Dictionary<string, object> currencyObject = new Dictionary<string, object>();
            currencyObject["VirtualGoodID"] = virtualGoodId;
            currencyObject["VirtualCurrencyKind"] = currency == Currencies.Main ? 1 : 2;
            currencyObject["Receipt"] = receipt;
            currencyObject["Amount"] = amount;


            postContent = new StringContent(JsonConvert.SerializeObject(currencyObject), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "VirtualGood/Buy");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                //update userObject
                foreach (var objectKey in userObject.Keys)
                {
                    _user[objectKey] = userObject[objectKey];
                }
                return userObject;
            }
        }
        public async Task<Dictionary<string, object>> BuyVirtualGood(string virtualGoodId, string receipt, string inAppPurchaseData = null)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(virtualGoodId))
                throw new Exception("virtual good id has to be set");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");
            if (String.IsNullOrWhiteSpace(inAppPurchaseData) && Platform == Platforms.Android)
                throw new Exception("inAppPurchaseData has to be set for the android platform");
            Dictionary<string, object> currency = new Dictionary<string, object>();
            currency["VirtualCurrencyID"] = virtualGoodId;
            currency["VirtualCurrencyKind"] = 3; //real money
            currency["Receipt"] = receipt;
            if (Platform == Platforms.Android)
                currency["InAppPurchaseData"] = inAppPurchaseData;


            postContent = new StringContent(JsonConvert.SerializeObject(currency), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "VirtualGood/Buy");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        public async Task<Dictionary<string, object>> GiveVirtualGood(string virtualGoodId, int amount, string receipt)
        {
            StringContent postContent = null;
            if (amount <= 0)
                throw new Exception("amount has to be greater 0");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");
            if (String.IsNullOrWhiteSpace(virtualGoodId))
                throw new Exception("virtual good id has to be set");

            Dictionary<string, object> currencyObject = new Dictionary<string, object>();

            currencyObject["VirtualGoodID"] = virtualGoodId;
            currencyObject["Receipt"] = receipt;
            currencyObject["Amount"] = amount;


            postContent = new StringContent(JsonConvert.SerializeObject(currencyObject), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "VirtualGood/Give");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        public async Task<Dictionary<string, object>> UseVirtualGood(string virtualGoodId, int amount, string receipt)
        {
            StringContent postContent = null;
            if (amount <= 0)
                throw new Exception("amount has to be greater 0");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");
            if (String.IsNullOrWhiteSpace(virtualGoodId))
                throw new Exception("virtual good id has to be set");

            Dictionary<string, object> currencyObject = new Dictionary<string, object>();

            currencyObject["VirtualGoodID"] = virtualGoodId;
            currencyObject["Receipt"] = receipt;
            currencyObject["Amount"] = amount;

            postContent = new StringContent(JsonConvert.SerializeObject(currencyObject), Encoding.UTF8, "application/json");

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "VirtualGood/Use");
            msg.Content = postContent;
            msg.Headers.Add("UserID", UserID);

            var response = await _client.SendAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicasaException(response, await response.Content.ReadAsStringAsync());
            }
            else
            {
                //read response
                var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        #endregion
        #region virtual good categories
        public async Task<Dictionary<string, object>> GetVirtualGoodCategory(bool includeUserInventory = false, bool includeVGetItems = false, Dictionary<string, object> query = null, int limit = 100, int offset = 0, List<string> foreignKeys = null, List<string> orderBy = null)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("includeVGetItems", includeVGetItems);
            param.Add("includeUserInventory", includeUserInventory);
            return await getObject("VirtualGoodCategory", query, limit, offset, foreignKeys, orderBy, param);
        }
        public async Task<Dictionary<string, object>> GetVirtualGoodCategory(string categoryId, bool includeUserInventory = false, bool includeVGetItems = false, List<string> foreignKeys = null)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("includeVGetItems", includeVGetItems);
            param.Add("includeUserInventory", includeUserInventory);
            return await getObject("VirtualGoodCategory", categoryId, foreignKeys, param);
        }
        #endregion
    }
}

