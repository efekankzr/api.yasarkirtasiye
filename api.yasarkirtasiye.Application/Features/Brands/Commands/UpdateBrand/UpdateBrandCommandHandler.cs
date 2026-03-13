using api.yasarkirtasiye.Application.Features.Brands.DTOs;
using api.yasarkirtasiye.Application.Interfaces;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Application.Interfaces.Services;
using api.yasarkirtasiye.Domain.Entities;
using MediatR;

namespace api.yasarkirtasiye.Application.Features.Brands.Commands.UpdateBrand;

public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, BrandDto>
{
    private readonly IRepository<Brand> _brandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;

    public UpdateBrandCommandHandler(IRepository<Brand> brandRepository, IUnitOfWork unitOfWork, IFileService fileService)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<BrandDto> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(request.Id, cancellationToken);

        if (brand == null)
            throw new Exception("Brand not found.");

        if (request.RemoveImage && !string.IsNullOrEmpty(brand.ImageUrl))
        {
            _fileService.DeleteFile(brand.ImageUrl);
            brand.ImageUrl = null;
        }

        if (request.Image != null)
        {
            if (!string.IsNullOrEmpty(brand.ImageUrl))
            {
                _fileService.DeleteFile(brand.ImageUrl);
            }
            brand.ImageUrl = await _fileService.UploadFileAsync(request.Image, "brands", cancellationToken);
        }

        brand.Name = request.Name;
        brand.IsShowMainPage = request.IsShowMainPage;

        await _brandRepository.UpdateAsync(brand, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BrandDto
        {
            Id = brand.Id,
            Name = brand.Name,
            ImageUrl = brand.ImageUrl,
            IsShowMainPage = brand.IsShowMainPage
        };
    }
}
