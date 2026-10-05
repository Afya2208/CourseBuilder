namespace API.Features.Users;

public interface IRoleService
{
    public Task<List<RoleDto>> ReadAllAsync();
}

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    public async Task<List<RoleDto>> ReadAllAsync()
    {
        return await roleRepository.ReadAllAsync();
    }
}