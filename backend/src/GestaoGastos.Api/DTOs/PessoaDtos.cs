using System.ComponentModel.DataAnnotations;

namespace GestaoGastos.Api.DTOs;

public class CriarPessoaDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(0, 150, ErrorMessage = "A idade deve estar entre 0 e 150.")]
    public int Idade { get; set; }
}

public class PessoaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
}