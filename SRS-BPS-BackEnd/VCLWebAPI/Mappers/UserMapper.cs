using VCLWebAPI.Models;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Mappers
{
    public class UserMapper
    {
        public UserApiModel MapUserDbModelToApiModel(User user)
        {
            UserApiModel userApiModel = new UserApiModel
            {
                UserId = user.UserId,
                UserGuid = user.UserGuid.ToString(),
                UserName = user.UserName,
                NameFirst = user.NameFirst,
                NameLast = user.NameLast,
                Email = user.Email,
                Company = user.Company,
                Hash = user.Hash,
                Salt = user.Salt,
                Language = user.Language
            };

            foreach (AccessRole accessRole in user.AccessRole)
            {
                userApiModel.AccessRole.Add(new AccessRoleApiModel
                {
                    AccessRoleId = accessRole.AccessRoleId,
                    AccessRoleName = accessRole.AccessRoleName
                });
            }

            return userApiModel;
        }

        //public User MapUserApiModelToDbModel(UserApiModel user)
        //{
        //    //User userDbModel = new User
        //    //{
        //    //    UserId = user.UserId,
        //    //    UserName = user.UserName,
        //    //    NameFirst = user.NameFirst,
        //    //    NameLast = user.NameLast,
        //    //    Email = user.Email,
        //    //    Company = user.Company,
        //    //    Hash = user.Hash,
        //    //    Salt = user.Salt,
        //    //};
        //    //return userDbModel;
        //}
    }
}