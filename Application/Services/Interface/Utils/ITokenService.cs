using Domain.Entities.UserAgg;
using Domain.Entities.UserEntities;

namespace Application.Services.Interface.Utils;

public interface ITokenService
{
    Task<string> CreateToken(ApplicationUser user);
}