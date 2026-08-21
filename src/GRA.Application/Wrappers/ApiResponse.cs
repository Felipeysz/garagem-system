using System.Text.Json.Serialization;

namespace GRA.Application.Wrappers;

public enum StatusResultado
{
    Sucesso,
    NaoEncontrado,
    ValidacaoFalhou,
    ErroInterno
}

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public List<string> Erros { get; set; } = [];

    [JsonIgnore]
    public StatusResultado Status { get; private set; }

    public ApiResponse()
    {
    }

    private ApiResponse(T data)
    {
        Data = data;
        Status = StatusResultado.Sucesso;
    }

    private ApiResponse(List<string> erros, StatusResultado status)
    {
        Erros = erros;
        Status = status;
    }

    public static ApiResponse<T> ComSucesso(T data) => new(data);

    public static ApiResponse<T> ComErros(IEnumerable<string> erros) => new(erros.ToList(), StatusResultado.ValidacaoFalhou);

    public static ApiResponse<T> ComErro(string erro) => new(new List<string> { erro }, StatusResultado.ErroInterno);

    public static ApiResponse<T> NaoEncontrado(string erro) => new(new List<string> { erro }, StatusResultado.NaoEncontrado);
}