namespace GRA.Application.Wrappers;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public List<string> Erros { get; set; } = [];

    public ApiResponse()
    {
    }

    private ApiResponse(T data)
    {
        Data = data;
    }

    private ApiResponse(List<string> erros)
    {
        Erros = erros;
    }

    public static ApiResponse<T> ComSucesso(T data) => new(data);

    public static ApiResponse<T> ComErros(IEnumerable<string> erros) => new(erros.ToList());

    public static ApiResponse<T> ComErro(string erro) => new(new List<string> { erro });
}