using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoQwrekster.Helpers
{
    interface IOAuthV1Helper : IOAuthHelper
    {
        void SetOAuthTokenAndSecret(string token, string token_secret);

        Uri GetRequestTokenUrl(string method);

        void SetVerifier(string verifier);
    }
}
