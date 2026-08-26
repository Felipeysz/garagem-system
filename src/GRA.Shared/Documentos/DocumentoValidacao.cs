namespace GRA.Shared.Documentos;

public static class DocumentoValidacao
{
    public static bool CpfEhValido(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11 || !cpf.All(char.IsDigit))
            return false;

        if (cpf.Distinct().Count() == 1)
            return false;

        int[] pesos1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] pesos2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        var digitos = cpf.Select(c => c - '0').ToArray();

        var soma1 = 0;
        for (var i = 0; i < 9; i++)
            soma1 += digitos[i] * pesos1[i];

        var resto1 = soma1 % 11;
        var dv1 = resto1 < 2 ? 0 : 11 - resto1;

        if (digitos[9] != dv1)
            return false;

        var soma2 = 0;
        for (var i = 0; i < 10; i++)
            soma2 += digitos[i] * pesos2[i];

        var resto2 = soma2 % 11;
        var dv2 = resto2 < 2 ? 0 : 11 - resto2;

        return digitos[10] == dv2;
    }

    public static bool CnpjEhValido(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14 || !cnpj.All(char.IsDigit))
            return false;

        if (cnpj.Distinct().Count() == 1)
            return false;

        int[] pesos1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] pesos2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var digitos = cnpj.Select(c => c - '0').ToArray();

        var soma1 = 0;
        for (var i = 0; i < 12; i++)
            soma1 += digitos[i] * pesos1[i];

        var resto1 = soma1 % 11;
        var dv1 = resto1 < 2 ? 0 : 11 - resto1;

        if (digitos[12] != dv1)
            return false;

        var soma2 = 0;
        for (var i = 0; i < 13; i++)
            soma2 += digitos[i] * pesos2[i];

        var resto2 = soma2 % 11;
        var dv2 = resto2 < 2 ? 0 : 11 - resto2;

        return digitos[13] == dv2;
    }
}