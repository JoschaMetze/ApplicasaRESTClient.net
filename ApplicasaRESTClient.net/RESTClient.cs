using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        Windows
    }
    public class RESTClient : HttpClient
    {
        public string ApplicationID { get; set; }
        public string UserID { get; set; }
        public Platforms Platform { get; set; }
        public float ApplicationVersion { get; set; }
        public bool IsSandbox { get; set; }

        protected Uri _baseAddress = new Uri("http://api.applicasa.com/3.0/");

        protected HttpClient _client;
        public RESTClient()
        {
            _client = new HttpClient();
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

                default:
                    return "-1";
            }
        }
        public async Task<dynamic> Initialize(string userID = null, string facebookToken = null)
        {
            _client.BaseAddress = _baseAddress;
            _client.DefaultRequestHeaders.Add("AppID", ApplicationID);
            _client.DefaultRequestHeaders.Add("Platform", transformPlatform(Platform));
            _client.DefaultRequestHeaders.Add("AppVersion", ApplicationVersion.ToString());
            _client.DefaultRequestHeaders.Add("IsSandbox", IsSandbox.ToString());

            StringContent postContent = null;
            if (facebookToken != null)
            {
                dynamic user = new ExpandoObject();
                user.FacebookToken = userID;
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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                UserID = userObject.UserID;
                return userObject;
            }
        }
        #region Login methods
        public async Task<dynamic> RegisterWithUserNameAndPassword(string userName, string passwordHash)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(userName))
                throw new Exception("Username has to be set");
            if (passwordHash == null)
            {
                throw new Exception("Password has to be empty string when using no password authentication");
            }
            dynamic user = new ExpandoObject();
            user.UserName = userName;
            //hash
            user.UserPassword = passwordHash;
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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                UserID = userObject.UserID;
                return userObject;
            }
        }
        public async Task<dynamic> LoginWithUserNameAndPassword(string userName, string passwordHash)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(userName))
                throw new Exception("Username has to be set");
            if (passwordHash == null)
            {
                throw new Exception("Password has to be empty string when using no password authentication");
            }

            dynamic user = new ExpandoObject();
            user.UserName = userName;
            //hash
            user.UserPassword = passwordHash;
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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                UserID = userObject.UserID;
                return userObject;
            }
        }
        public async Task<bool> ResetPassword(string userName)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(userName))
                throw new Exception("Username has to be set");


            dynamic user = new ExpandoObject();
            user.UserName = userName;

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

            dynamic user = new ExpandoObject();
            user.CurrentUserName = currentUserName;
            user.NewUserName = newUserName;
            //hash
            user.UserPassword = passwordHash;
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

            dynamic user = new ExpandoObject();
            user.UserName = userName;

            //hash
            user.CurrentUserPassword = passwordHash;
            user.NewUserPassword = newPasswordHash;
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

            dynamic user = new ExpandoObject();
            user.UserName = userName;
            //hash
            user.UserPassword = passwordHash;
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

        public async Task<dynamic> LoginWithFacebook(string facebookToken)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(facebookToken))
                throw new Exception("Facebook token has to be set");


            dynamic user = new ExpandoObject();
            user.FacebookToken = facebookToken;
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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        public async Task<dynamic> FindFacebookFriends(string facebookToken)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(facebookToken))
                throw new Exception("Facebook token has to be set");


            dynamic user = new ExpandoObject();
            user.FacebookToken = facebookToken;
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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        public async Task<dynamic> DisconnectFacebook()
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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        #endregion
        #region Backend
        public async Task<dynamic> CreateObject(string objectName, dynamic valueObject)
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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        public async Task<bool> UpdateObject(string objectName, string recordID, dynamic changedValuesObject)
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
        public async Task<dynamic> GetObject(string objectName, string recordID, List<string> foreignKeys)
        {
            return await getObject(objectName, recordID, foreignKeys);
        }
        protected async Task<dynamic> getObject(string objectName, string recordID, List<string> foreignKeys, Dictionary<string, object> additionalParameters = null)
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
            var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            return userObject;

        }
        public async Task<dynamic> GetObject(string objectName, dynamic query = null, int limit = 100, int offset = 0, List<string> foreignKeys = null, List<string> orderBy = null)
        {
            return await getObject(objectName, query, limit, offset, foreignKeys, orderBy);
        }
        protected async Task<dynamic> getObject(string objectName, dynamic query = null, int limit = 100, int offset = 0, List<string> foreignKeys = null, List<string> orderBy = null, Dictionary<string, object> additionalParameters = null)
        {

            if (String.IsNullOrWhiteSpace(objectName))
                throw new Exception("object name has to be set");


            string queryString = "?";
            if (query == null)
                queryString += "query={}";
            else
                queryString += "query=" + JsonConvert.SerializeObject(query);
            queryString += "& limit=" + limit.ToString();
            queryString += "& offset=" + offset.ToString();
            if (foreignKeys != null && foreignKeys.Count > 0)
            {
                queryString += "& foreignkeys=" + String.Join(",", foreignKeys);
            }
            if (orderBy != null && orderBy.Count > 0)
            {
                queryString += "& orderBy=" + String.Join(",", orderBy);
            }
            foreach (KeyValuePair<string, object> kvp in additionalParameters)
            {
                queryString += "&" + kvp.Key + "=" + kvp.Value.ToString();
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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        #endregion
        #region virtual currency
        public async Task<dynamic> GetVirtualCurrency(dynamic query = null, int limit = 100, int offset = 0, List<string> foreignKeys = null, List<string> orderBy = null)
        {
            return await GetObject("VirtualCurrency", query, limit, offset, foreignKeys, orderBy);
        }
        public async Task<dynamic> BuyVirtualCurrency(string currencyId, string receipt, string inAppPurchaseData = null)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(currencyId))
                throw new Exception("currency id has to be set");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");
            if (String.IsNullOrWhiteSpace(inAppPurchaseData) && Platform == Platforms.Android)
                throw new Exception("inAppPurchaseData has to be set for the android platform");
            dynamic currency = new ExpandoObject();
            currency.VirtualCurrencyID = currencyId;
            currency.Receipt = receipt;
            if (Platform == Platforms.Android)
                currency.InAppPurchaseData = inAppPurchaseData;


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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        public async Task<dynamic> GiveVirtualCurrency(Currencies currency, int amount, string receipt)
        {
            StringContent postContent = null;
            if (amount <= 0)
                throw new Exception("amount has to be greater 0");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");

            dynamic currencyObject = new ExpandoObject();

            currencyObject.VirtualCurrencyKind = currency == Currencies.Main ? 1 : 2;
            currencyObject.Receipt = receipt;
            currencyObject.Amount = amount;


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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        public async Task<dynamic> UseVirtualCurrency(Currencies currency, int amount, string receipt)
        {
            StringContent postContent = null;
            if (amount <= 0)
                throw new Exception("amount has to be greater 0");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");

            dynamic currencyObject = new ExpandoObject();

            currencyObject.VirtualCurrencyKind = currency == Currencies.Main ? 1 : 2;
            currencyObject.Receipt = receipt;
            currencyObject.Amount = amount;


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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        #endregion
        #region virtual goods
        public async Task<dynamic> GetVirtualGood(bool includeUserInventory = false, int? byCategoryPosition = null, dynamic query = null, int limit = 100, int offset = 0, List<string> foreignKeys = null, List<string> orderBy = null)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("includeUserInventory", includeUserInventory);
            if (byCategoryPosition.HasValue)
            {
                param.Add("byCategoryPosition", byCategoryPosition.Value);
            }
            return await getObject("VirtualGood", query, limit, offset, foreignKeys, orderBy, param);
        }

        public async Task<dynamic> BuyVirtualGoodWithCurrency(string virtualGoodId, Currencies currency, int amount, string receipt)
        {
            StringContent postContent = null;
            if (amount <= 0)
                throw new Exception("amount has to be greater 0");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");
            if (String.IsNullOrWhiteSpace(virtualGoodId))
                throw new Exception("virtual good id has to be set");

            dynamic currencyObject = new ExpandoObject();
            currencyObject.VirtualGoodID = virtualGoodId;
            currencyObject.VirtualCurrencyKind = currency == Currencies.Main ? 1 : 2;
            currencyObject.Receipt = receipt;
            currencyObject.Amount = amount;


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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        public async Task<dynamic> BuyVirtualGood(string virtualGoodId, string receipt, string inAppPurchaseData = null)
        {
            StringContent postContent = null;
            if (String.IsNullOrWhiteSpace(virtualGoodId))
                throw new Exception("virtual good id has to be set");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");
            if (String.IsNullOrWhiteSpace(inAppPurchaseData) && Platform == Platforms.Android)
                throw new Exception("inAppPurchaseData has to be set for the android platform");
            dynamic currency = new ExpandoObject();
            currency.VirtualCurrencyID = virtualGoodId;
            currency.VirtualCurrencyKind = 3; //real money
            currency.Receipt = receipt;
            if (Platform == Platforms.Android)
                currency.InAppPurchaseData = inAppPurchaseData;


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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        public async Task<dynamic> GiveVirtualGood(string virtualGoodId, int amount, string receipt)
        {
            StringContent postContent = null;
            if (amount <= 0)
                throw new Exception("amount has to be greater 0");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");
            if (String.IsNullOrWhiteSpace(virtualGoodId))
                throw new Exception("virtual good id has to be set");

            dynamic currencyObject = new ExpandoObject();

            currencyObject.VirtualGoodID = virtualGoodId;
            currencyObject.Receipt = receipt;
            currencyObject.Amount = amount;


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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        public async Task<dynamic> UseVirtualGood(string virtualGoodId, int amount, string receipt)
        {
            StringContent postContent = null;
            if (amount <= 0)
                throw new Exception("amount has to be greater 0");
            if (String.IsNullOrWhiteSpace(receipt))
                throw new Exception("receipt has to be set");
            if (String.IsNullOrWhiteSpace(virtualGoodId))
                throw new Exception("virtual good id has to be set");

            dynamic currencyObject = new ExpandoObject();

            currencyObject.VirtualGoodID = virtualGoodId;
            currencyObject.Receipt = receipt;
            currencyObject.Amount = amount;

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
                var userObject = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return userObject;
            }
        }
        #endregion
        #region virtual good categories
        public async Task<dynamic> GetVirtualGoodCategory(bool includeUserInventory = false, bool includeVGetItems = false, dynamic query = null, int limit = 100, int offset = 0, List<string> foreignKeys = null, List<string> orderBy = null)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("includeVGetItems", includeVGetItems);
            param.Add("includeUserInventory", includeUserInventory);
            return await getObject("VirtualGoodCategory", query, limit, offset, foreignKeys, orderBy, param);
        }
        public async Task<dynamic> GetVirtualGoodCategory(string categoryId, bool includeUserInventory = false, bool includeVGetItems = false, List<string> foreignKeys = null)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("includeVGetItems", includeVGetItems);
            param.Add("includeUserInventory", includeUserInventory);
            return await getObject("VirtualGoodCategory", categoryId, foreignKeys, param);
        }
        #endregion
    }
}

