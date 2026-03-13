using api.yasarkirtasiye.Application.Interfaces;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Domain.Entities;
using MediatR;

namespace api.yasarkirtasiye.Application.Features.Brands.Commands.DeleteBrand;

public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, Unit>
{
    private readonly IRepository<Brand> _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBrandCommandHandler(IRepository<Brand> brandRepository, IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(request.Id, cancellationToken);

        if (brand == null)
        {
            throw new Exception("Brand not found."); 
        }

        await _brandRepository.DeleteAsync(brand, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
