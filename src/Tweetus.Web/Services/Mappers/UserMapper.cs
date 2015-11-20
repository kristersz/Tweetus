using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetus.Web.Models;
using Tweetus.Web.ViewModels;

namespace Tweetus.Web.Services.Mappers
{
    public static class UserMapper
    {
        public static IList<UserVM> MapUsersToViewModels(IList<ApplicationUser> users)
        {
            var result = new List<UserVM>();

            foreach (var user in users)
            {
                result.Add(MapUserToViewModel(user));
            }

            return result;
        }

        public static UserVM MapUserToViewModel(ApplicationUser user)
        {
            return new UserVM()
            {
                UserId = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                UserAbout = user.About,
                ProfilePictureBase64 = (user.ProfilePicture != null) ? Convert.ToBase64String(user.ProfilePicture) : string.Empty
            };
        }
    }
}
