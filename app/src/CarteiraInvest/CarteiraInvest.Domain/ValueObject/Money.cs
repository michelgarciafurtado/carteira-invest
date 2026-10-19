using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Domain.ValueObject;

public readonly struct Money:IEquatable<Money>
{
    public decimal Valor { get; }

    public Money(decimal valor)
    {
        if(valor == null)
            throw new ArgumentNullException(nameof(valor), "O valor não pode ser nulo.");

        //este MidpointRounding.AwayFromZero é aconselhavel para aplicacoes financeiras porque arredonda para o valor mais distante de zero, evitando viés em cálculos financeiros
        Valor = Math.Round(valor, 2, MidpointRounding.AwayFromZero);
    }
    //para instanciar um Money com valor zero
    public static Money Zero => new(0m);
    //para instanciar um Money com valor
    public static Money Criar(decimal valor) => new(valor);

    //metodos para operações matemáticas com Money
    public static Money operator +(Money a, Money b) => new(a.Valor + b.Valor);
    public static Money operator -(Money a, Money b) => new(a.Valor - b.Valor);
    public static Money operator *(Money a, decimal fator) => new(a.Valor * fator);
    public static Money operator /(Money a, decimal divisor)
        => divisor == 0 ? Zero : new(a.Valor / divisor);

    public static implicit operator decimal(Money money) => money.Valor;

    public override string ToString() => Valor.ToString("C2");

    // Implementação de IEquatable<Money>
    public bool Equals(Money other) => Valor == other.Valor;

    public override bool Equals(object? obj) => obj is Money other && Equals(other);

    public override int GetHashCode() => Valor.GetHashCode();
}
