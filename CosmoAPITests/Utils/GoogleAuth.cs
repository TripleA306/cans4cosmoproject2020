using System.Net.Http;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using System.Threading;

namespace CosmoAPITests.Utils
{
    public class GoogleAuth
    {
        public static string getGoogleAuth(HttpClient _client)
        {
            string[] scopes = new string[] { Oauth2Service.Scope.UserinfoEmail };

            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = "314266744370-h687lj78ug1msrod8lktk6sqmavd61dk.apps.googleusercontent.com",
                ClientSecret = "-cCRO6nbbrbYu_-Q4l6QSqeR"
            }, scopes, "token", CancellationToken.None).Result;

            return credential.Token.IdToken;
        }
    }
}
