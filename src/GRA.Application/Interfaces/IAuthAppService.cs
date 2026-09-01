using GRA.Application.DTOs;
using GRA.Application.Wrappers;

namespace GRA.Application.Interfaces;

public interface IAuthAppService
{
    Task<ApiResponse<LoginResponseDto>> LoginFuncionarioAsync(LoginFuncionarioDto dto);

    Task<ApiResponse<LoginResponseDto>> LoginClienteAsync(LoginClienteDto dto);
}