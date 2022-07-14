using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using NetShop.IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace NetShop.IdentityService.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        UserManager<ApplicationUser> userManager;
        public ResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
         this.userManager = userManager;   
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            ApplicationUser user = await this.userManager.FindByEmailAsync(context.UserName);// .Users.ToList().FirstOrDefault(x => x.Username == context.UserName && x.Password == context.Password);
            if (user != null){
                //if(this.userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, context.Password) != PasswordVerificationResult.Failed)
                if(await this.userManager.CheckPasswordAsync(user, context.Password))
                {
                   context.Result = new GrantValidationResult(user.Id, OidcConstants.AuthenticationMethods.Password);
                } 
            }
        }
    }
}