using api.yasarkirtasiye.Application.Features.Brands.DTOs;
using api.yasarkirtasiye.Application.Interfaces;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Application.Interfaces.Services;
using api.yasarkirtasiye.Domain.Entities;
using MediatR;

namespace api.yasarkirtasiye.Application.Features.Brands.Commands.CreateBrand;

public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, BrandDto>
{
    private readonly IRepository<Brand> _brandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;

    public CreateBrandCommandHandler(IRepository<Brand> brandRepository, IUnitOfWork unitOfWork, IFileService fileService)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<BrandDto> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        string? imageUrl = null;
        if (request.Image != null)
        {
            imageUrl = await _fileService.UploadFileAsync(request.Image, "brands", cancellationToken);
        }

        var brand = new Brand
        {
            Name = request.Name,
            ImageUrl = imageUrl,
            IsShowMainPage = request.IsShowMainPage
        };

        await _brandRepository.AddAsync(brand, cancellationToken);
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
