using System;

namespace SCDH.Models
{
    public class ContratoHabitacional
    {
        public Guid Id { get; set; }
        public string? NumeroContrato { get; set; }
        public string? CpfCliente { get; set; }
        public decimal ValorImovel { get; set; }
        public string? CaminhoArquivo { get; set; }
    }
}