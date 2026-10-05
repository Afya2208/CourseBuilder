using Domain.Entities;

namespace API.Features.Themes;

public record ThemeDto(int Id, string Name)
{
    public Theme MapToEntity()
    {
        return new Theme()
        {
            Id = Id, Name = Name
        };
    }
}


public static class ThemesDtos
{
    
}