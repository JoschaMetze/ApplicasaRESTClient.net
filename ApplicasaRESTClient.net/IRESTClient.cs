using System;
namespace ApplicasaRESTClientnet
{
    interface IRESTClient
    {
        string ApplicationID { get; set; }
        string ApplicationKey { get; set; }
        float ApplicationVersion { get; set; }
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> BuyVirtualCurrency(string currencyId, string receipt, string inAppPurchaseData = null);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> BuyVirtualGood(string virtualGoodId, string receipt, string inAppPurchaseData = null);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> BuyVirtualGoodWithCurrency(string virtualGoodId, Currencies currency, int amount, string receipt);
        System.Threading.Tasks.Task<bool> ChangeUserName(string currentUserName, string newUserName, string passwordHash);
        System.Threading.Tasks.Task<bool> ChangeUserPassword(string userName, string passwordHash, string newPasswordHash);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> CreateObject(string objectName, System.Collections.Generic.Dictionary<string, object> valueObject);
        System.Threading.Tasks.Task<bool> DeleteObject(string objectName, string recordID);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> DisconnectFacebook();
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> FindFacebookFriends(string facebookToken);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> GetObject(string objectName, System.Collections.Generic.Dictionary<string, object> query = null, int limit = 100, int offset = 0, System.Collections.Generic.List<string> foreignKeys = null, System.Collections.Generic.List<string> orderBy = null);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> GetObject(string objectName, string recordID, System.Collections.Generic.List<string> foreignKeys);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> GetVirtualCurrency(System.Collections.Generic.Dictionary<string, object> query = null, int limit = 100, int offset = 0, System.Collections.Generic.List<string> foreignKeys = null, System.Collections.Generic.List<string> orderBy = null);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> GetVirtualGood(bool includeUserInventory = false, int? byCategoryPosition = null, System.Collections.Generic.Dictionary<string, object> query = null, int limit = 100, int offset = 0, System.Collections.Generic.List<string> foreignKeys = null, System.Collections.Generic.List<string> orderBy = null);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> GetVirtualGoodCategory(bool includeUserInventory = false, bool includeVGetItems = false, System.Collections.Generic.Dictionary<string, object> query = null, int limit = 100, int offset = 0, System.Collections.Generic.List<string> foreignKeys = null, System.Collections.Generic.List<string> orderBy = null);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> GetVirtualGoodCategory(string categoryId, bool includeUserInventory = false, bool includeVGetItems = false, System.Collections.Generic.List<string> foreignKeys = null);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> GiveVirtualCurrency(Currencies currency, int amount, string receipt);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> GiveVirtualGood(string virtualGoodId, int amount, string receipt);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> Initialize(string userID = null, string facebookToken = null);
        bool IsSandbox { get; set; }
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> LoginWithFacebook(string facebookToken);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> LoginWithUserNameAndPassword(string userName, string passwordHash);
        Platforms Platform { get; set; }
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> RegisterWithUserNameAndPassword(string userName, string passwordHash);
        System.Threading.Tasks.Task<bool> RemoveUserNameAndPassword(string userName, string passwordHash);
        System.Threading.Tasks.Task<bool> ResetPassword(string userName);
        System.Threading.Tasks.Task<bool> UpdateObject(string objectName, string recordID, System.Collections.Generic.Dictionary<string, object> changedValuesObject);
        System.Collections.Generic.Dictionary<string, object> User { get; }
        string UserID { get; set; }
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> UseVirtualCurrency(Currencies currency, int amount, string receipt);
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, object>> UseVirtualGood(string virtualGoodId, int amount, string receipt);
    }
}
