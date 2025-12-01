using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Globalization;

// ===== DESAFIO 1 - CÁLCULO DE COMISSÃO =====
public class Venda
{
    public string vendedor { get; set; }
    public decimal valor { get; set; }
}

public class VendasData
{
    public List<Venda> vendas { get; set; }
}

public class ComissaoVendedor
{
    public string Vendedor { get; set; }
    public decimal TotalVendas { get; set; }
    public decimal TotalComissao { get; set; }
}

public static class CalculoComissao
{
    public static void CalcularComissoes()
    {
        Console.Clear();
        string json = @"{
            ""vendas"": [
                { ""vendedor"": ""João Silva"", ""valor"": 1200.50 },
                { ""vendedor"": ""João Silva"", ""valor"": 950.75 },
                { ""vendedor"": ""João Silva"", ""valor"": 1800.00 },
                { ""vendedor"": ""João Silva"", ""valor"": 1400.30 },
                { ""vendedor"": ""João Silva"", ""valor"": 1100.90 },
                { ""vendedor"": ""João Silva"", ""valor"": 1550.00 },
                { ""vendedor"": ""João Silva"", ""valor"": 1700.80 },
                { ""vendedor"": ""João Silva"", ""valor"": 250.30 },
                { ""vendedor"": ""João Silva"", ""valor"": 480.75 },
                { ""vendedor"": ""João Silva"", ""valor"": 320.40 },
                
                { ""vendedor"": ""Maria Souza"", ""valor"": 2100.40 },
                { ""vendedor"": ""Maria Souza"", ""valor"": 1350.60 },
                { ""vendedor"": ""Maria Souza"", ""valor"": 950.20 },
                { ""vendedor"": ""Maria Souza"", ""valor"": 1600.75 },
                { ""vendedor"": ""Maria Souza"", ""valor"": 1750.00 },
                { ""vendedor"": ""Maria Souza"", ""valor"": 1450.90 },
                { ""vendedor"": ""Maria Souza"", ""valor"": 400.50 },
                { ""vendedor"": ""Maria Souza"", ""valor"": 180.20 },
                { ""vendedor"": ""Maria Souza"", ""valor"": 90.75 },
                
                { ""vendedor"": ""Carlos Oliveira"", ""valor"": 800.50 },
                { ""vendedor"": ""Carlos Oliveira"", ""valor"": 1200.00 },
                { ""vendedor"": ""Carlos Oliveira"", ""valor"": 1950.30 },
                { ""vendedor"": ""Carlos Oliveira"", ""valor"": 1750.80 },
                { ""vendedor"": ""Carlos Oliveira"", ""valor"": 1300.60 },
                { ""vendedor"": ""Carlos Oliveira"", ""valor"": 300.40 },
                { ""vendedor"": ""Carlos Oliveira"", ""valor"": 500.00 },
                { ""vendedor"": ""Carlos Oliveira"", ""valor"": 125.75 },
                
                { ""vendedor"": ""Ana Lima"", ""valor"": 1000.00 },
                { ""vendedor"": ""Ana Lima"", ""valor"": 1100.50 },
                { ""vendedor"": ""Ana Lima"", ""valor"": 1250.75 },
                { ""vendedor"": ""Ana Lima"", ""valor"": 1400.20 },
                { ""vendedor"": ""Ana Lima"", ""valor"": 1550.90 },
                { ""vendedor"": ""Ana Lima"", ""valor"": 1650.00 },
                { ""vendedor"": ""Ana Lima"", ""valor"": 75.30 },
                { ""vendedor"": ""Ana Lima"", ""valor"": 420.90 },
                { ""vendedor"": ""Ana Lima"", ""valor"": 315.40 }
            ]
        }";

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var vendasData = JsonSerializer.Deserialize<VendasData>(json, options);
            
            if (vendasData?.vendas == null)
            {
                Console.WriteLine("Erro: Dados de vendas não puderam ser carregados.");
                return;
            }

            var comissoes = vendasData.vendas
                .GroupBy(v => v.vendedor)
                .Select(g => new ComissaoVendedor
                {
                    Vendedor = g.Key,
                    TotalVendas = g.Sum(v => v.valor),
                    TotalComissao = g.Sum(v => CalcularComissaoVenda(v.valor))
                })
                .ToList();

            Console.WriteLine("=== RELATÓRIO DE COMISSÕES ===\n");
            
            foreach (var comissao in comissoes)
            {
                Console.WriteLine($"Vendedor: {comissao.Vendedor}");
                Console.WriteLine($"Total de Vendas: R$ {comissao.TotalVendas:F2}");
                Console.WriteLine($"Comissão Total: R$ {comissao.TotalComissao:F2}");
                Console.WriteLine("---");
            }

            // Detalhamento por venda
            Console.WriteLine("\n=== DETALHAMENTO POR VENDA ===");
            foreach (var venda in vendasData.vendas.Take(5)) // Mostra apenas as 5 primeiras para não poluir
            {
                var comissao = CalcularComissaoVenda(venda.valor);
                Console.WriteLine($"{venda.vendedor} - R$ {venda.valor:F2} - Comissão: R$ {comissao:F2}");
            }
            
            Console.WriteLine($"... e mais {vendasData.vendas.Count - 5} vendas");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar vendas: {ex.Message}");
            Console.WriteLine($"Detalhes: {ex.InnerException?.Message}");
        }
    }

    private static decimal CalcularComissaoVenda(decimal valor)
    {
        if (valor < 100.00m)
            return 0;
        else if (valor < 500.00m)
            return valor * 0.01m;
        else
            return valor * 0.05m;
    }
}

// ===== DESAFIO 2 - CONTROLE DE ESTOQUE =====
public class Produto
{
    public int codigoProduto { get; set; }
    public string descricaoProduto { get; set; }
    public int estoque { get; set; }
}

public class EstoqueData
{
    public List<Produto> estoque { get; set; }
}

public class Movimentacao
{
    public int Id { get; set; }
    public int CodigoProduto { get; set; }
    public string Descricao { get; set; }
    public int Quantidade { get; set; }
    public DateTime DataMovimentacao { get; set; }
    public string Tipo { get; set; }
}

public class GerenciadorEstoque
{
    private List<Produto> produtos;
    private List<Movimentacao> movimentacoes;
    private int proximoId = 1;

    public GerenciadorEstoque()
    {
        string json = @"{
            ""estoque"": [
                {
                    ""codigoProduto"": 101,
                    ""descricaoProduto"": ""Caneta Azul"",
                    ""estoque"": 150
                },
                {
                    ""codigoProduto"": 102,
                    ""descricaoProduto"": ""Caderno Universitário"",
                    ""estoque"": 75
                },
                {
                    ""codigoProduto"": 103,
                    ""descricaoProduto"": ""Borracha Branca"",
                    ""estoque"": 200
                },
                {
                    ""codigoProduto"": 104,
                    ""descricaoProduto"": ""Lápis Preto HB"",
                    ""estoque"": 320
                },
                {
                    ""codigoProduto"": 105,
                    ""descricaoProduto"": ""Marcador de Texto Amarelo"",
                    ""estoque"": 90
                }
            ]
        }";

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var estoqueData = JsonSerializer.Deserialize<EstoqueData>(json, options);
            produtos = estoqueData?.estoque ?? new List<Produto>();
            movimentacoes = new List<Movimentacao>();
            
            if (!produtos.Any())
            {
                Console.WriteLine("Aviso: Nenhum produto foi carregado do JSON.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar estoque: {ex.Message}");
            produtos = new List<Produto>();
            movimentacoes = new List<Movimentacao>();
        }
    }

    public void Executar()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== SISTEMA DE CONTROLE DE ESTOQUE ===");
            Console.WriteLine("1. Listar Produtos");
            Console.WriteLine("2. Realizar Movimentação");
            Console.WriteLine("3. Histórico de Movimentações");
            Console.WriteLine("4. Voltar ao Menu Principal");
            Console.Write("Escolha uma opção: ");

            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    ListarProdutos();
                    break;
                case "2":
                    RealizarMovimentacao();
                    break;
                case "3":
                    ListarMovimentacoes();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }

    private void ListarProdutos()
    {
        Console.WriteLine("\n=== PRODUTOS EM ESTOQUE ===");
        if (!produtos.Any())
        {
            Console.WriteLine("Nenhum produto cadastrado.");
            return;
        }

        foreach (var produto in produtos)
        {
            Console.WriteLine($"Código: {produto.codigoProduto} | " +
                            $"Descrição: {produto.descricaoProduto} | " +
                            $"Estoque: {produto.estoque}");
        }
    }

    private void RealizarMovimentacao()
    {
        Console.WriteLine("\n=== NOVA MOVIMENTAÇÃO ===");
        
        ListarProdutos();
        Console.WriteLine();
        
        Console.Write("Código do Produto: ");
        if (!int.TryParse(Console.ReadLine(), out int codigo))
        {
            Console.WriteLine("Código inválido!");
            return;
        }

        var produto = produtos.FirstOrDefault(p => p.codigoProduto == codigo);
        if (produto == null)
        {
            Console.WriteLine("Produto não encontrado!");
            return;
        }

        Console.Write("Tipo (E=Entrada, S=Saída): ");
        var tipoInput = Console.ReadLine()?.ToUpper();
        if (string.IsNullOrEmpty(tipoInput) || (tipoInput != "E" && tipoInput != "S"))
        {
            Console.WriteLine("Tipo inválido! Use E para Entrada ou S para Saída.");
            return;
        }

        Console.Write("Quantidade: ");
        if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
        {
            Console.WriteLine("Quantidade inválida! Deve ser um número maior que zero.");
            return;
        }

        Console.Write("Descrição da movimentação: ");
        var descricao = Console.ReadLine();

        if (tipoInput == "S" && quantidade > produto.estoque)
        {
            Console.WriteLine($"Quantidade indisponível em estoque! Estoque atual: {produto.estoque}");
            return;
        }

        // Realizar movimentação
        if (tipoInput == "E")
        {
            produto.estoque += quantidade;
        }
        else
        {
            produto.estoque -= quantidade;
        }

        // Registrar movimentação
        var movimentacao = new Movimentacao
        {
            Id = proximoId++,
            CodigoProduto = codigo,
            Descricao = descricao ?? "Sem descrição",
            Quantidade = quantidade,
            DataMovimentacao = DateTime.Now,
            Tipo = tipoInput == "E" ? "ENTRADA" : "SAÍDA"
        };

        movimentacoes.Add(movimentacao);

        Console.WriteLine($"\n✅ Movimentação realizada com sucesso!");
        Console.WriteLine($"📦 Estoque atual de {produto.descricaoProduto}: {produto.estoque}");
    }

    private void ListarMovimentacoes()
    {
        Console.WriteLine("\n=== HISTÓRICO DE MOVIMENTAÇÕES ===");
        if (!movimentacoes.Any())
        {
            Console.WriteLine("Nenhuma movimentação registrada.");
            return;
        }

        foreach (var mov in movimentacoes.OrderByDescending(m => m.DataMovimentacao))
        {
            var produto = produtos.FirstOrDefault(p => p.codigoProduto == mov.CodigoProduto);
            var nomeProduto = produto?.descricaoProduto ?? "Produto não encontrado";
            
            Console.WriteLine($"ID: {mov.Id} | " +
                            $"Produto: {nomeProduto} | " +
                            $"Tipo: {mov.Tipo} | " +
                            $"Quantidade: {mov.Quantidade} | " +
                            $"Data: {mov.DataMovimentacao:dd/MM/yyyy HH:mm} | " +
                            $"Descrição: {mov.Descricao}");
        }
    }
}

// ===== DESAFIO 3 - CALCULADORA DE JUROS =====
public static class CalculadoraJuros
{
    public static void CalcularJuros()
    {
        Console.Clear();
        Console.WriteLine("=== CALCULADORA DE JUROS POR ATRASO ===\n");
        
        Console.Write("Digite o valor original: R$ ");
        if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorOriginal) || valorOriginal <= 0)
        {
            Console.WriteLine("Valor inválido!");
            return;
        }

        Console.Write("Digite a data de vencimento (dd/mm/aaaa): ");
        if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataVencimento))
        {
            Console.WriteLine("Data inválida! Use o formato dd/mm/aaaa.");
            return;
        }

        DateTime dataAtual = DateTime.Today;
        
        if (dataAtual <= dataVencimento)
        {
            Console.WriteLine("✅ A data atual não é posterior à data de vencimento. Não há juros a calcular.");
            return;
        }

        int diasAtraso = (dataAtual - dataVencimento).Days;
        decimal taxaDiaria = 0.025m; // 2,5% ao dia
        decimal valorJuros = valorOriginal * taxaDiaria * diasAtraso;
        decimal valorTotal = valorOriginal + valorJuros;

        Console.WriteLine("\n=== RESULTADO DO CÁLCULO ===");
        Console.WriteLine($"📅 Data de Vencimento: {dataVencimento:dd/MM/yyyy}");
        Console.WriteLine($"📅 Data de Hoje: {dataAtual:dd/MM/yyyy}");
        Console.WriteLine($"⏰ Dias em Atraso: {diasAtraso}");
        Console.WriteLine($"💰 Valor Original: R$ {valorOriginal:F2}");
        Console.WriteLine($"📊 Taxa de Juros Diária: {taxaDiaria * 100}%");
        Console.WriteLine($"💸 Valor dos Juros: R$ {valorJuros:F2}");
        Console.WriteLine($"💳 Valor Total a Pagar: R$ {valorTotal:F2}");
    }
}

// ===== PROGRAMA PRINCIPAL =====
class Program
{
    static void Main(string[] args)
    {
        // Configurar encoding para suportar caracteres especiais no Linux
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== 🚀 MENU PRINCIPAL - DESAFIOS C# ===");
            Console.WriteLine("1. 📊 Cálculo de Comissão de Vendedores");
            Console.WriteLine("2. 📦 Sistema de Controle de Estoque");
            Console.WriteLine("3. 💰 Calculadora de Juros por Atraso");
            Console.WriteLine("4. ❌ Sair");
            Console.Write("Escolha uma opção: ");

            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    CalculoComissao.CalcularComissoes();
                    break;
                case "2":
                    var gerenciador = new GerenciadorEstoque();
                    gerenciador.Executar();
                    break;
                case "3":
                    CalculadoraJuros.CalcularJuros();
                    break;
                case "4":
                    Console.WriteLine("Saindo... Até logo! 👋");
                    return;
                default:
                    Console.WriteLine("Opção inválida! Tente novamente.");
                    break;
            }

            if (opcao != "2") // O estoque já tem seu próprio loop
            {
                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }
    }
}
