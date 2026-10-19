using CarteiraInvest.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Domain.Entities;

public class Corretora(string nome) : BaseEntity
{
    public string Nome { get; private set; } = !string.IsNullOrWhiteSpace(nome) 
        ? nome.Trim() :
        throw new ArgumentException("Nome da corretora não pode ser nulo ou vazio.", nameof(nome));
}
