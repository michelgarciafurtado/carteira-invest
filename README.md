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
