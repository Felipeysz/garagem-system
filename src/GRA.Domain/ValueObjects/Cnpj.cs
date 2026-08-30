using System.Text.RegularExpressions;

namespace GRA.Domain.ValueObjects;

public readonly struct Cnpj : IEquatable<Cnpj>
{
    public string Valor { get; }

    private Cnpj(string valor)
    {
        Valor = valor;
    }

    public static bool TryCreate(string? input, out Cnpj cnpj)
    {
        cnpj = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var valor = input.Trim().ToUpperInvariant();

        if (!Regex.IsMatch(valor, @"^[A-Z0-9]{12}\d{2}$"))
            return false;

        if (!EhValido(valor))
            return false;

        cnpj = new Cnpj(valor);
        return true;
    }

    public static Cnpj Parse(string valor)
    {
        if (!TryCreate(valor, out var cnpj))
            throw new FormatException($"CNPJ inválido: '{valor}'.");

        return cnpj;
    }

    private static bool EhValido(string cnpj)
    {
        int[] pesos1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] pesos2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var valores = cnpj.Select(c => (int)c - 48).ToArray();

        var soma1 = 0;
        for (var i = 0; i < 12; i++)
            soma1 += valores[i] * pesos1[i];

        var resto1 = soma1 % 11;
        var dv1 = resto1 < 2 ? 0 : 11 - resto1;

        if (valores[12] != dv1)
            return false;

        var soma2 = 0;
        for (var i = 0; i < 13; i++)
            soma2 += valores[i] * pesos2[i];

        var resto2 = soma2 % 11;
        var dv2 = resto2 < 2 ? 0 : 11 - resto2;

        return valores[13] == dv2;
    }

    public override string ToString() => Valor;

    public bool Equals(Cnpj other) => Valor == other.Valor;

    public override bool Equals(object? obj) => obj is Cnpj other && Equals(other);

    public override int GetHashCode() => Valor?.GetHashCode() ?? 0;

    public static bool operator ==(Cnpj left, Cnpj right) => left.Equals(right);

    public static bool operator !=(Cnpj left, Cnpj right) => !left.Equals(right);

    public static implicit operator string(Cnpj cnpj) => cnpj.Valor;
}