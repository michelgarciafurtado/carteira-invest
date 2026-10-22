# CarteiraInvest

Sistema fullstack de gerenciamento e acompanhamento de carteira de investimentos.
O projeto contará com uma API para atender o front feito em Angular e fará integração com API da Brapi para consultar 
os symbols, nome dado aos códigos das ações pela API externa.
Contara com cache Redis, PostgreSQL para armazenar informacoes das carteiras e IdentityDB para servir de base de dados para armazenar os dados de usuários.
[Link para documentação no miro:](https://miro.com/app/board/uXjVEYtcPi0=/?share_link_id=678945879539)

## 🛠️ Tecnologias Utilizadas

- **Frontend:** Angular (SPA com SCSS)
- **Backend:** .NET 10 Web API
- **Banco de Dados:** PostgreSQL
- **Cache:** Redis
- **Infraestrutura:** Docker, Docker Compose e Nginx
- **Hospedagem:** VM Linux

## 📁 Estrutura do Projeto

```text
PrjCarteiraInvest/
├── app/          # API Backend em .NET
├── deploy/       # Docker Compose e configurações de Nginx
├── front/        # Aplicação Frontend Angular
└── testes/       # Testes automatizados
```

## 📁 Estrutura de app (onde fica a API em .Net)

```text
app/
└── src/
    └── CarteiraInvest/
        ├── CarteiraInvest.Api/
        ├── CarteiraInvest.Application/
        ├── CarteiraInvest.Domain/
        ├── CarteiraInvest.Infra/
        └── CarteiraInvest.slnx
```

## Diagrama UML de Domain

# Diagrama UML — CarteiraInvest.Domain

Diagrama baseado nos 10 tipos declarados em `app/src/CarteiraInvest/CarteiraInvest.Domain`, nas pastas `Aggregates`, `Entities` e `ValueObject` (nome usado no código).

## Classes e relacionamentos

```mermaid
classDiagram
    direction LR

    namespace Aggregates {
        class BaseEntity {
            <<abstract>>
            +Guid Id
            +DateTime CreatedAt
            #BaseEntity()
        }
        class Investidor {
            +string IdentityUserId
            +string Nome
            +string Sobrenome
            -List~AcaoCarteira~ _carteira
            +IReadOnlyCollection~AcaoCarteira~ Carteira
            -List~AcaoMonitorada~ _acoesMonitoradas
            +IReadOnlyCollection~AcaoMonitorada~ AcoesMonitoradas
            +Endereco Endereco
            +Investidor(string identityUserId, string nome, string sobrenome, Endereco endereco)
            +AtualizarEndereco(Endereco endereco) void
            +AtualizarNome(string nome, string sobrenome) void
            +MonitorarAcao(AcaoMonitorada acaoMonitorada) void
            +PararMonitoramentoAcao(string symbol) void
            +AdicionarAcaoCarteira(AcaoCarteira acaoCarteira) void
            +ObterAcaoCarteira(string symbol, Guid corretoraId) AcaoCarteira?
        }
    }

    namespace Entities {
        class Acao {
            +string Symbol
            +string LongName
            +Acao(string symbol, string longName)
        }
        class AcaoCarteira {
            +string Symbol
            +Acao Acao
            +DateTimeOffset? DataUltimaCotacao
            +Money? PrecoUltimaCotacao
            +Guid InvestidorId
            +Investidor Investidor
            +Guid CorretoraId
            +Corretora Corretora
            +Money PrecoMedio
            +int Quantidade
            +Money ValorInvestido
            +Money ValorAtual
            +Money LucroPrejuizo
            +decimal PercentualLucroPrejuizo
            -List~AcaoTransacao~ _transacoes
            +IReadOnlyCollection~AcaoTransacao~ Transacoes
            +AcaoCarteira(string symbol, Guid investidorId, Guid corretoraId)
            +AtualizarCotacao(DateTimeOffset dataCotacao, decimal precoUltimaCotacao) void
            +AdicionarTransacao(AcaoTransacao transacao) void
            +RecalcularPosicao() void
            +RecalcularValoresMercado() void
            -CalcularPrecoMedio() Money
            -CalcularQuantidade() int
            -CalcularValorInvestido() Money
            -CalcularValorAtual() Money
            -CalcularLucroPrejuizo() Money
            -CalcularPercentualLucroPrejuizo() decimal
        }
        class AcaoMonitorada {
            +string Symbol
            +Acao Acao
            +Guid InvestidorId
            +Investidor Investidor
            +decimal? PrecoAlvo
            +bool EmitirAlerta
            +AcaoMonitorada(string symbol, Guid investidorId, decimal? precoAlvo, bool emitirAlerta)
        }
        class AcaoTransacao {
            +string AcaoSymbol
            +Acao? Acao
            +Guid InvestidorId
            +Investidor? Investidor
            +TipoOperacaoEnum TipoOperacao
            +DateTime DataOperacao
            +Money PrecoUnitario
            +int Quantidade
            +AcaoTransacao(string acaoSymbol, Guid investidorId, TipoOperacaoEnum tipoOperacao, DateTime dataOperacao, decimal precoUnitario, int quantidade)
        }
        class Corretora {
            +string Nome
            +Corretora(string nome)
        }
    }

    namespace ValueObject {
        class Endereco {
            <<record>>
            +string Logradouro
            +string Numero
            +string complemento
            +string Cidade
            +string Estado
            +string CEP
            +Endereco(string Logradouro, string Numero, string complemento, string Cidade, string Estado, string CEP)
        }
        class Money {
            <<readonly struct>>
            +decimal Valor
            +Money Zero$
            +Money(decimal valor)
            +Criar(decimal valor) Money$
            +operator +(Money a, Money b) Money$
            +operator -(Money a, Money b) Money$
            +operator *(Money a, decimal fator) Money$
            +operator /(Money a, decimal divisor) Money$
            +implicit operator decimal(Money money) decimal$
            +ToString() string
            +Equals(Money other) bool
            +Equals(object? obj) bool
            +GetHashCode() int
        }
        class TipoOperacaoEnum {
            <<enumeration>>
            Compra = 0
            Venda = 1
        }
    }

    class IEquatableMoney["IEquatable<Money>"] {
        <<interface>>
        +Equals(Money other) bool
    }

    BaseEntity <|-- Investidor
    BaseEntity <|-- AcaoCarteira
    BaseEntity <|-- AcaoMonitorada
    BaseEntity <|-- AcaoTransacao
    BaseEntity <|-- Corretora
    IEquatableMoney <|.. Money

    Investidor "1" --> "0..*" AcaoCarteira : Carteira / Investidor
    Investidor "1" --> "0..*" AcaoMonitorada : AcoesMonitoradas / Investidor
    Investidor --> "1" Endereco : Endereco
    AcaoCarteira "0..*" --> "1" Acao : Acao
    AcaoMonitorada "0..*" --> "1" Acao : Acao
    AcaoCarteira "0..*" --> "1" Corretora : Corretora
    AcaoCarteira --> "0..*" AcaoTransacao : Transacoes
    AcaoTransacao "0..*" --> "0..1" Acao : Acao
    AcaoTransacao "0..*" --> "0..1" Investidor : Investidor
    AcaoCarteira --> "0..1" Money : PrecoUltimaCotacao
    AcaoCarteira --> "1" Money : PrecoMedio
    AcaoCarteira --> "1" Money : ValorInvestido
    AcaoCarteira --> "1" Money : ValorAtual
    AcaoCarteira --> "1" Money : LucroPrejuizo
    AcaoTransacao --> "1" Money : PrecoUnitario
    AcaoTransacao --> "1" TipoOperacaoEnum : TipoOperacao
    AcaoCarteira ..> TipoOperacaoEnum : calcula compras e vendas
```

## Convenções

- `+`: público; `-`: privado; `#`: protegido; `$`: membro estático.
- `?`: tipo anulável; `0..1`: referência opcional; `0..*`: coleção que pode estar vazia.
- Triângulo com linha contínua: herança; triângulo com linha tracejada: implementação de interface; seta contínua: associação; seta tracejada: dependência.
- As propriedades mostram a visibilidade de leitura. Os setters são privados, exceto `BaseEntity.Id` e `CreatedAt`, que possuem setters protegidos. `Money.Valor` possui somente getter; `Endereco` é um record posicional com propriedades `init` geradas pelo compilador.
- Os relacionamentos representam propriedades e coleções declaradas no código. Não pressupõem configuração de banco de dados, exclusão em cascata ou composição exclusiva. A interface `IEquatable<Money>` pertence ao .NET e aparece apenas para mostrar sua implementação.
- Construtores e membros explicitamente declarados foram incluídos; membros gerados pelo compilador para o record e membros herdados de `object` não são repetidos.


