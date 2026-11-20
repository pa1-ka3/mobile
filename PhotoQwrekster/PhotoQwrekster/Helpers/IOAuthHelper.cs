using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoQwrekster.Helpers
{
    interface IOAuthHelper
    {
        void GetRequestTokenQueryNonStatic(bool isRefresh, Action<List<object>> callback);
        void RefreshTokenCodeNonStatic(Action<List<object>> callback);
        Uri GetLoginUrl();
        Uri GetLogoutUrl();
        void SetTokenCode(string value);
        string GetAccountKey();
        void SetAccountKey(string key);
        string GetStorage();
        void ClearAccessToken();
        bool IsCancelOrLogoutRedirectUrl(string url);
    }
}
