using MediatR;
using api.yasarkirtasiye.Application.Features.Categories.DTOs;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Application.Wrappers;
using api.yasarkirtasiye.Domain.Entities;

namespace api.yasarkirtasiye.Application.Features.Categories.Queries;

public class GetAllCategoriesQuery : IRequest<PagedResult<CategoryDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, PagedResult<CategoryDto>>
{
    private readonly IRepository<Category> _categoryRepository;

    public GetAllCategoriesQueryHandler(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var pagedData = await _categoryRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var dtos = pagedData.Items.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name
        });

        return new PagedResult<CategoryDto>(dtos, pagedData.TotalCount, request.PageNumber, request.PageSize);
    }
}
