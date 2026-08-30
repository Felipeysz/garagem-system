using System.Text.RegularExpressions;

namespace GRA.Domain.ValueObjects;

public readonly struct Cpf : IEquatable<Cpf>
{
    public string Valor { get; }

    private Cpf(string valor)
    {
        Valor = valor;
    }

    public static bool TryCreate(string? input, out Cpf cpf)
    {
        cpf = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var valor = input.Trim();

        if (!Regex.IsMatch(valor, @"^\d{11}$"))
            return false;

        if (!EhValido(valor))
            return false;

        cpf = new Cpf(valor);
        return true;
    }

    public static Cpf Parse(string valor)
    {
        if (!TryCreate(valor, out var cpf))
            throw new FormatException($"CPF inválido: '{valor}'.");

        return cpf;
    }

    private static bool EhValido(string cpf)
    {
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

    public override string ToString() => Valor;

    public bool Equals(Cpf other) => Valor == other.Valor;

    public override bool Equals(object? obj) => obj is Cpf other && Equals(other);

    public override int GetHashCode() => Valor?.GetHashCode() ?? 0;

    public static bool operator ==(Cpf left, Cpf right) => left.Equals(right);

    public static bool operator !=(Cpf left, Cpf right) => !left.Equals(right);

    public static implicit operator string(Cpf cpf) => cpf.Valor;
}