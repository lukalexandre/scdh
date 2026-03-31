using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SCDH.Data;
using SCDH.Models;

namespace SCDH.Controllers
{
    [ApiController]
    [Route("api/contratos")]
    public class ContratosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ContratosController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] string numeroContrato, [FromForm] string cpfCliente, [FromForm] decimal valorImovel, IFormFile arquivo)
        {
            // Validações
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest(new { erro = "Arquivo inválido ou não enviado." });

            if (arquivo.ContentType != "application/pdf" && !arquivo.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { erro = "Apenas arquivos PDF são permitidos." });

            if (arquivo.Length > 5242880)
                return BadRequest(new { erro = "O arquivo excede o limite de 5MB." });

            // Correção para o Checklist: Pasta "uploads"
            string pasta = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(pasta);

            string nomeUnico = Guid.NewGuid().ToString() + ".pdf";
            string caminhoFinal = Path.Combine(pasta, nomeUnico);

            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var contrato = new ContratoHabitacional
            {
                Id = Guid.NewGuid(),
                NumeroContrato = numeroContrato,
                CpfCliente = cpfCliente,
                ValorImovel = valorImovel,
                CaminhoArquivo = nomeUnico
            };

            _context.Contratos.Add(contrato);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Contrato salvo com sucesso!", id = contrato.Id });
        }

        [HttpGet("listar")]
        public IActionResult Listar()
        {
            var contratos = _context.Contratos.ToList();
            return Ok(contratos);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato == null) return NotFound("Contrato não encontrado.");

            // Correção para o Checklist: Pasta "uploads"
            string caminhoFisico = Path.Combine(_env.WebRootPath, "uploads", contrato.CaminhoArquivo);
            if (!System.IO.File.Exists(caminhoFisico)) return NotFound("O PDF sumiu do servidor!");

            var stream = System.IO.File.OpenRead(caminhoFisico);
            return File(stream, "application/pdf", contrato.CaminhoArquivo);
        }
    }
}