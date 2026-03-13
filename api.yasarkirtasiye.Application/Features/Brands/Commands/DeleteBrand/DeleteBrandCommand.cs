using MediatR;

namespace api.yasarkirtasiye.Application.Features.Brands.Commands.DeleteBrand;

public class DeleteBrandCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public DeleteBrandCommand(Guid id)
    {
        Id = id;
    }
}
