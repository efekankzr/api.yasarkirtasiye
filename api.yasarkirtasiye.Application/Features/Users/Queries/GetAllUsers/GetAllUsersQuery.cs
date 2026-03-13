using api.yasarkirtasiye.Application.Features.Users.DTOs;
using MediatR;

namespace api.yasarkirtasiye.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
{
}
