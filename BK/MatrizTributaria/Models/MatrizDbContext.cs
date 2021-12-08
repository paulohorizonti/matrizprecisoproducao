using System.Data.Entity;

namespace MatrizTributaria.Models
{
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
    public class MatrizDbContext : DbContext
    {

        public MatrizDbContext() : base("name=MatrizDbContext") { }

        public virtual DbSet<CategoriaProduto> CategoriaProdutos { get; set; }
        public virtual DbSet<ClassificacaoNatReceita> ClassificacaoNatReceitas { get; set; }
        public virtual DbSet<Produto> Produtos { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Nivel> Niveis { get; set; }
        public virtual DbSet<Empresa> Empresas { get; set; }

        public virtual DbSet<Versao> Versoes { get; set; }

        public virtual DbSet<Legislacao> Legislacoes { get; set; }
        public virtual DbSet<NaturezaReceita> NaturezaReceitas { get; set; }
        public virtual DbSet<Tributacao> Tributacoes { get; set; }

        public virtual DbSet<CstIcmsGeral> CstIcmsGerais { get; set; }
        public virtual DbSet<CstPisCofinsEntrada> CstPisCofinsEntradas { get; set; }
        public virtual DbSet<CstPisCofinsSaida> CstPisCofinsSaidas { get; set; }

        public virtual DbSet<SetorProdutos> SetorProdutos { get; set; }

        public virtual DbSet<TributacaoEmpresa> TributacaoEmpresas { get; set; }

        public virtual DbSet<TributacaoGeralView> Tributacao_GeralView { get; set; }

        public virtual DbSet<MatrizTributaria.Areas.Cliente.Models.AnaliseTributaria> Analise_Tributaria { get; set; } //Vitor
        public virtual DbSet<MatrizTributaria.Areas.Cliente.Models.AnaliseTributaria2> Analise_Tributaria_2 { get; set; } //Vitor


    }
}