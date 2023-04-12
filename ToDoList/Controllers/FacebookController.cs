using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ToDoList.Controllers
{
    public class FacebookController : Controller
    {
        // Inject the Facebook options in your controller or service
        private readonly IOptionsMonitor<FacebookOptions> _facebookOptions;
        public FacebookController(IOptionsMonitor<FacebookOptions> facebookOptions)
        {
            _facebookOptions = facebookOptions;
        }

        // Use the Facebook SDK to initiate the authentication flow
        // Use the ChallengeAsync method to initiate the Facebook authentication flow
        public async Task<IActionResult> FacebookLogin()
        {
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = "/signin-facebook?auth_type=reauthenticate"
            };
            authProperties.Parameters.Add("auth_type", "reauthenticate"); // Add auth_type parameter
            authProperties.Parameters.Add("prompt", "login"); // Add prompt parameter

            await HttpContext.ChallengeAsync("Facebook", authProperties); // Use ChallengeAsync with "Facebook" as the scheme

            return RedirectToAction("Index", "Home"); // Redirect to a different action or controller after the challenge
        }


    }
}
