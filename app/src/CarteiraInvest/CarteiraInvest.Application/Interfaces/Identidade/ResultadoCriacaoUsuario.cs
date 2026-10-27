namespace CarteiraInvest.Application.Interfaces.Identidade
{
    public class ResultadoCriacaoUsuario
    {
        public bool Sucesso { get; init; }
        public String? UsuarioId { get; init; }

        public IReadOnlyCollection<string> Erros { get; init; } = Array.Empty<string>();
    }
}