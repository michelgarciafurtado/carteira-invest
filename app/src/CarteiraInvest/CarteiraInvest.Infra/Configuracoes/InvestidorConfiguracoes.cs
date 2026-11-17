using CarteiraInvest.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Infra.Configuracoes;

public class InvestidorConfiguracoes : IEntityTypeConfiguration<Investidor>
{
    public void Configure(EntityTypeBuilder<Investidor> builder)
    {
        builder.ToTable("Investidores");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.IdentityUserId)
            .IsRequired();

        builder.Property(i => i.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(i => i.Sobrenome)
            .HasMaxLength(100)
            .IsRequired();

        // Endereco será salvo nas colunas da tabela Investidores.
        builder.OwnsOne(i => i.Endereco, endereco =>
        {
            endereco.Property(e => e.Logradouro)
                .HasColumnName("Logradouro")
                .IsRequired();

            endereco.Property(e => e.Numero)
                .HasColumnName("Numero")
                .IsRequired();

            endereco.Property(e => e.Cidade)
                .HasColumnName("Cidade")
                .IsRequired();

            endereco.Property(e => e.Estado)
                .HasColumnName("Estado")
                .IsRequired();

            endereco.Property(e => e.CEP)
                .HasColumnName("CEP")
                .IsRequired();

            endereco.Property(e => e.Complemento)
                .HasColumnName("Complemento");

        });

        builder.Ignore(i => i.Carteira);
        builder.Ignore(i => i.AcoesMonitoradas);


    }
}
