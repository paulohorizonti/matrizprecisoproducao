using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;

namespace MatrizTributaria.Controllers
{
    public class TributacaoController : Controller
    {
        //Objego context
       readonly MatrizDbContext db;
       List<Tributacao> trib;
      
       List<TributacaoGeralView> tribMTX = new List<TributacaoGeralView>();
        //Construtor
        public TributacaoController()
        {
            db = new MatrizDbContext();
        }

       
        // GET: Tributacao
        public ActionResult Index(string sortOrder, string searchString, string currentFilter,  int? page)
        {

            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ProdutoParam = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";
           

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var tributacao = from s in db.Tributacoes select s;

            //lista de produtos
            var listaProdutos = db.Produtos.ToList();
            int paginaTamanho = 4;
            int paginaNumero = 1;

            //Viewbag da lista de produtos
            ViewBag.Produtos = listaProdutos.ToPagedList(paginaNumero, paginaTamanho);

            var produtos = from s in db.Produtos select s; //variavel carregado de produtos
            ViewBag.ddlProdutos = new SelectList(produtos, "ProdutoId", "NomeDoProduto");//lista

            if (!String.IsNullOrEmpty(searchString))
            {
                tributacao = tributacao.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(searchString.ToUpper()) || s.id.ToString().ToUpper().Contains(searchString.ToUpper()));
                
            }
            
            switch (ViewBag.ProdutoParam)
            {
                case "Produto_desc":
                    tributacao = tributacao.OrderBy(s => s.id); //ordenar pelo id do registro da tabela
                    break;
                case "NatRec":
                    tributacao = tributacao.OrderBy(s => s.codNatReceita);
                    break;
                case "NatRec_desc":
                    tributacao = tributacao.OrderByDescending(s => s.codNatReceita);
                    break;
                default:
                    tributacao = tributacao.OrderByDescending(s => s.id);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            

            return View(tributacao.ToPagedList(pageNumber, pageSize));
        }

        //detalhe
        public ActionResult Detalhes(int? id)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tributacao tributacao = db.Tributacoes.Find(id);
            if (tributacao == null)
            {
                return HttpNotFound();
            }

            ViewBag.Produto = db.Produtos.Find(tributacao.idProduto).descricao;
            ViewBag.Setor = db.SetorProdutos.Find(tributacao.idSetor).descricao;
            ViewBag.CodInterno = db.Produtos.Find(tributacao.idProduto).codInterno;
            ViewBag.CodBarras = db.Produtos.Find(tributacao.idProduto).codBarras;
            ViewBag.Cest = db.Produtos.Find(tributacao.idProduto).cest;
            ViewBag.Ncm = db.Produtos.Find(tributacao.idProduto).ncm;


            //Condição para montar a viewbag: se existir natureza da receita cadastrada; caso contrario null
            ViewBag.NaturezaReceita = (tributacao.codNatReceita == null)?null:db.NaturezaReceitas.Find(tributacao.codNatReceita).descricao;
            ViewBag.FundLegalPC = (tributacao.idFundamentoLegal == null)?null: db.Legislacoes.Find(tributacao.idFundamentoLegal).fundLegal;
            ViewBag.FundLegalSaida = (tributacao.idFundLegalSaidaICMS ==null)?null: db.Legislacoes.Find(tributacao.idFundLegalSaidaICMS).fundLegal;
            ViewBag.FundLegalEndrada = (tributacao.idFundLelgalEntradaICMS == null)?null: db.Legislacoes.Find(tributacao.idFundLelgalEntradaICMS).fundLegal;


            
            //ViewBag.Legislacao = db.Legislacoes.Find(tributacao.idFundLelgalEntradaICMS).id;
            ViewBag.CreditoOutorgado = db.Tributacoes.Find(tributacao.id).creditoOutorgado;
            ViewBag.Regime2560 = db.Tributacoes.Find(tributacao.id).regime2560;
            ViewBag.DataInicio = db.Tributacoes.Find(tributacao.id).inicioVigenciaMVA;
            ViewBag.DataFim = db.Tributacoes.Find(tributacao.id).fimVigenciaMVA;

            return View(tributacao);
        }

        //Create
        public ActionResult Create()
        {

            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            if (Session["nivel"].Equals("USUARIO"))
            {
                int par = 1;
                return RedirectToAction("Erro", new { param = par });
            }

            /*Carregar as viewbags*/
            ViewBag.Produtos = db.Produtos;
            ViewBag.Setor = db.SetorProdutos;
            ViewBag.NatReceita = db.NaturezaReceitas;
            ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas;
            ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas;
            ViewBag.FundLegal = db.Legislacoes;
            ViewBag.CstIcms = db.CstIcmsGerais;
            ViewBag.FundLegalPC = db.Legislacoes;
            ViewBag.FundLegalSaida = db.Legislacoes;
            ViewBag.FundLegalEndrada = db.Legislacoes;
            ViewBag.Legislacao = db.Legislacoes;
            ViewBag.CstGeral = db.CstIcmsGerais;

            var prod = from s in db.Produtos select s; //variavel carregado de produtos

            ViewBag.ProdList = prod;
            ViewBag.DataAlt = DateTime.Now;
            ViewBag.DataCad = DateTime.Now;

            
            ViewBag.ProdutosComplete = AutoComplete();

            var model = new TributacaoViewModel();

            return View(model);
        }

        /*Implementação motivada por não vir o codigo do cst e sim a descrição
         do elemento dropdawn, assim a descrição é passada como parametro da 
        busca no banco e a variavel é atualizada com o codigo da cst*/
        [HttpPost]
        public ActionResult Create(TributacaoViewModel model, string cstEntradaPisCofins, string cstSaidaPisCofins, string cstVendaAtaCont, string cstVendaAtaSimpNacional, string cstVendaVarejoCont,
            string cstVendaVarejoConsFinal, string cstCompraDeInd, string cstdaNfedaIndFORN, string cstCompradeAta, string cstdaNfedeAtaFORn, string cstCompradeSimpNacional, 
            string CsosntdaNfedoSnFOR, int? idProduto)
        {
            /*verificar os dados de CST no inicio: caso estejam em branco o valor no model permanece NULL
             evitando assim o erro de busca por valores inexistentes na tabela de cst uma vez que o campo é
            uma chave estrangeira*/
            model.cstEntradaPisCofins = (cstEntradaPisCofins == "") ? null : (int?)(long)(from a in db.CstPisCofinsEntradas where a.descricao == cstEntradaPisCofins select a.codigo).FirstOrDefault();
            model.cstSaidaPisCofins = (cstSaidaPisCofins == "")? null: (int?)(long)(from a in db.CstPisCofinsSaidas where a.descricao == cstSaidaPisCofins select a.codigo).FirstOrDefault();
            model.cstVendaAtaCont = (cstVendaAtaCont == "")? null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstVendaAtaCont select a.codigo).FirstOrDefault();
            model.cstVendaAtaSimpNacional = (cstVendaAtaSimpNacional == "")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstVendaAtaSimpNacional select a.codigo).FirstOrDefault();
            model.cstVendaVarejoCont = (cstVendaVarejoCont == "")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstVendaVarejoCont select a.codigo).FirstOrDefault();
            model.cstVendaVarejoConsFinal = (cstVendaVarejoConsFinal =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstVendaVarejoConsFinal select a.codigo).FirstOrDefault();
            model.cstCompraDeInd = (cstCompraDeInd =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstCompraDeInd select a.codigo).FirstOrDefault();
            model.cstdaNfedaIndFORN = (cstdaNfedaIndFORN =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstdaNfedaIndFORN select a.codigo).FirstOrDefault();
            model.cstCompradeAta = (cstCompradeAta =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstCompradeAta select a.codigo).FirstOrDefault();
            model.cstdaNfedeAtaFORn = (cstdaNfedeAtaFORn =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstdaNfedeAtaFORn select a.codigo).FirstOrDefault();
            model.cstCompradeSimpNacional = (cstCompradeSimpNacional =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstCompradeSimpNacional select a.codigo).FirstOrDefault();
            model.CsosntdaNfedoSnFOR = (CsosntdaNfedoSnFOR =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CsosntdaNfedoSnFOR select a.codigo).FirstOrDefault();


           
            //validando campos
            if ( (idProduto == null) || (model.estado ==null) ||(model.idSetor.Equals(0)))
            {
               
                ViewBag.Obrigatorio = "Produto, Estado ou Setor do Produto vazio!. Revise os dados informados!!!";
                /*Carregar as View Bag*/
                var prod2 = from s in db.Produtos select s; //variavel carregado de produtos

                ViewBag.ProdList = prod2;
                ViewBag.Produtos = db.Produtos;
                ViewBag.Setor = db.SetorProdutos;
                ViewBag.NatReceita = db.NaturezaReceitas;
                ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas;
                ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas;
                ViewBag.FundLegal = db.Legislacoes;
                ViewBag.CstIcms = db.CstIcmsGerais;
                ViewBag.FundLegalPC = db.Legislacoes;
                ViewBag.FundLegalSaida = db.Legislacoes;
                ViewBag.FundLegalEndrada = db.Legislacoes;
                ViewBag.Legislacao = db.Legislacoes;
                ViewBag.CstGeral = db.CstIcmsGerais;

                return View(model);

            }

            try
            {
                model.idProduto = (int)idProduto; //recebe o id do produto
                //iformando a data do dia da criação do registro
                model.dataCad = DateTime.Now;
                model.dataAlt = DateTime.Now;

                var tributacao = new Tributacao()
                {
                    estado = model.estado,
                    idProduto = model.idProduto,
                    idSetor = model.idSetor,
                    fecp = model.fecp,
                    codNatReceita = model.codNatReceita,

                    cstEntradaPisCofins = model.cstEntradaPisCofins,
                    cstSaidaPisCofins = model.cstSaidaPisCofins,
                    aliqEntPis = model.aliqEntPis,
                    aliqSaidaPis = model.aliqSaidaPis,
                    aliqEntCofins = model.aliqEntCofins,
                    aliqSaidaCofins = model.aliqSaidaCofins,

                    idFundamentoLegal = model.idFundamentoLegal,

                    cstVendaAtaCont = model.cstVendaAtaCont,
                    aliqIcmsVendaAtaCont = model.aliqIcmsVendaAtaCont,
                    aliqIcmsSTVendaAtaCont = model.aliqIcmsSTVendaAtaCont,
                    redBaseCalcIcmsVendaAtaCont = model.redBaseCalcIcmsVendaAtaCont,
                    redBaseCalcIcmsSTVendaAtaCont = model.redBaseCalcIcmsSTVendaAtaCont,

                    cstVendaAtaSimpNacional = model.cstVendaAtaSimpNacional,
                    aliqIcmsVendaAtaSimpNacional = model.aliqIcmsVendaAtaSimpNacional,
                    aliqIcmsSTVendaAtaSimpNacional = model.aliqIcmsSTVendaAtaSimpNacional,
                    redBaseCalcIcmsVendaAtaSimpNacional = model.redBaseCalcIcmsVendaAtaSimpNacional,
                    redBaseCalcIcmsSTVendaAtaSimpNacional = model.redBaseCalcIcmsSTVendaAtaSimpNacional,

                    cstVendaVarejoCont = model.cstVendaVarejoCont,
                    aliqIcmsVendaVarejoCont = model.aliqIcmsVendaVarejoCont,
                    aliqIcmsSTVendaVarejo_Cont = model.aliqIcmsSTVendaVarejo_Cont,
                    redBaseCalcVendaVarejoCont = model.redBaseCalcVendaVarejoCont,
                    RedBaseCalcSTVendaVarejo_Cont = model.RedBaseCalcSTVendaVarejo_Cont,


                    cstVendaVarejoConsFinal = model.cstVendaVarejoConsFinal,
                    aliqIcmsVendaVarejoConsFinal = model.aliqIcmsVendaVarejoConsFinal,
                    aliqIcmsSTVendaVarejoConsFinal = model.aliqIcmsSTVendaVarejoConsFinal,
                    redBaseCalcIcmsVendaVarejoConsFinal = model.redBaseCalcIcmsVendaVarejoConsFinal,
                    redBaseCalcIcmsSTVendaVarejoConsFinal = model.redBaseCalcIcmsSTVendaVarejoConsFinal,


                    idFundLegalSaidaICMS = model.idFundLegalSaidaICMS,
                    idFundLelgalEntradaICMS = model.idFundLelgalEntradaICMS,

                    cstCompraDeInd = model.cstCompraDeInd,
                    aliqIcmsCompDeInd = model.aliqIcmsCompDeInd,
                    aliqIcmsSTCompDeInd = model.aliqIcmsSTCompDeInd,
                    redBaseCalcIcmsCompraDeInd = model.redBaseCalcIcmsCompraDeInd,
                    redBaseCalcIcmsSTCompraDeInd = model.redBaseCalcIcmsSTCompraDeInd,

                    cstCompradeAta = model.cstCompradeAta,
                    aliqIcmsCompradeAta = model.aliqIcmsCompradeAta,
                    aliqIcmsSTCompraDeAta = model.aliqIcmsSTCompraDeAta,
                    redBaseCalcIcmsCompraDeAta = model.redBaseCalcIcmsCompraDeAta,
                    redBaseCalcIcmsSTCompraDeAta = model.redBaseCalcIcmsSTCompraDeAta,

                    cstCompradeSimpNacional = model.cstCompradeSimpNacional,
                    aliqIcmsCompradeSimpNacional = model.aliqIcmsCompradeSimpNacional,
                    aliqIcmsSTCompradeSimpNacional = model.aliqIcmsSTCompradeSimpNacional,
                    redBaseCalcIcmsCompradeSimpNacional = model.redBaseCalcIcmsCompradeSimpNacional,
                    redBaseCalcIcmsSTCompradeSimpNacional = model.redBaseCalcIcmsSTCompradeSimpNacional,

                    cstdaNfedaIndFORN = model.cstdaNfedaIndFORN,
                    cstdaNfedeAtaFORn = model.cstdaNfedeAtaFORn,
                    CsosntdaNfedoSnFOR = model.CsosntdaNfedoSnFOR,
                    aliqIcmsNFE = model.aliqIcmsNFE,
                    aliqIcmsNfeAta = model.aliqIcmsNfeAta,
                    aliqIcmsNfeSN = model.aliqIcmsNfeSN,

                    tipoMVA = model.tipoMVA,
                    valorMVAInd = model.valorMVAInd,
                    inicioVigenciaMVA = model.inicioVigenciaMVA,
                    fimVigenciaMVA = model.fimVigenciaMVA,
                    creditoOutorgado = model.creditoOutorgado,
                    valorMVAAtacado = model.valorMVAAtacado,
                    regime2560 = model.regime2560,

                    dataCad = model.dataCad,
                    dataAlt = model.dataAlt

                };
                TempData["IDPRODUTO"] = null;

                db.Tributacoes.Add(tributacao);
                db.SaveChanges();
                return RedirectToAction("Index");//caso salve
            }
            catch(Exception e)
            {
               // Session["ERR_SALVAR"] = "ERRO AO SALVAR OS DADOS, POR FAVOR VERIFIQUE OS DADOS E TENTE NOVAMENTE: "+e.ToString();
                ViewBag.ErrSalvar = "ERRO AO SALVAR OS DADOS, POR FAVOR VERIFIQUE OS DADOS E TENTE NOVAMENTE: " + e.ToString();
            }


            /*Carregar as View Bag*/
            var prod = from s in db.Produtos select s; //variavel carregado de produtos

            ViewBag.ProdList = prod; //lista com os produtos
            ViewBag.Produtos = db.Produtos;
            ViewBag.Setor = db.SetorProdutos;
            ViewBag.NatReceita = db.NaturezaReceitas;
            ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas;
            ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas;
            ViewBag.FundLegal = db.Legislacoes;
            ViewBag.CstIcms = db.CstIcmsGerais;
            ViewBag.FundLegalPC = db.Legislacoes;
            ViewBag.FundLegalSaida = db.Legislacoes;
            ViewBag.FundLegalEndrada = db.Legislacoes;
            ViewBag.Legislacao = db.Legislacoes;
            ViewBag.CstGeral = db.CstIcmsGerais;


            return View(model);
        }

        public ActionResult ProcuraProduto(string sortOrder, string searchString, string currentFilter, int? page, string linhasNum)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            // ViewBag.NumLinhas = linhasNum;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ProdutoParam = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";
            ViewBag.CatProduto = sortOrder == "CatProd" ? "CatProd_desc" : "CatProd";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var produtos = from s in db.Produtos select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                produtos = produtos.Where(s => s.descricao.ToString().ToUpper().Contains(searchString.ToUpper()) || s.categoriaProduto.descricao.ToString().ToUpper().Contains(searchString.ToUpper()));

            }
            switch (sortOrder)
            {
                case "Produto_desc":
                    produtos = produtos.OrderByDescending(s => s.Id);
                    break;
                case "CatProd":
                    produtos = produtos.OrderBy(s => s.idCategoria);
                    break;
                case "CatProd_desc":
                    produtos = produtos.OrderByDescending(s => s.idCategoria);
                    break;
                default:
                    produtos = produtos.OrderBy(s => s.Id);
                    break;
            }
            int pageSize = 0;

            if (String.IsNullOrEmpty(linhasNum))
            {
                pageSize = 10;
            }
            else
            {

                ViewBag.Texto = linhasNum;
                pageSize = Int32.Parse(linhasNum);
            }


            int pageNumber = (page ?? 1);

            //var produtos = db.Produtos.ToList();
            return View(produtos.ToPagedList(pageNumber, pageSize)); //retorna a view com o numero de paginas e tamanho

        }

        /*Edição*/
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            /*Verificia a sessão  do usuário: caso nulo redireciona*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            /*Verifica o nível de acesso: passa um parametro caso seja igual USUARIO*/
            if (Session["nivel"].Equals("USUARIO"))
            {
                int par = 2;
                return RedirectToAction("../Erro/Erro", new { param = par });
            }

            /*Verifica se o id é nulo*/
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            /*Instancia um objeto de acordo com o Id passado como parametro*/
            Tributacao tributacao = db.Tributacoes.Find(id);

            /*Verifica se o objeto foi instanciado*/
            if (tributacao == null)
            {
                return HttpNotFound();
            }

            /*Verificando cst na tabela e Preencher a descrição dos csts
             Obs.: Quando o registro não possui algun desses valores a
            consulta ao banco retorna null, dando exceção por estar nulo
            dessa forma ele valida na condição e não permite valores
            nulos sejam tratados
             */
            CstIcmsGeral cstGeral1 = db.CstIcmsGerais.Find(tributacao.cstVendaAtaCont);
            CstIcmsGeral cstGeral2 = db.CstIcmsGerais.Find(tributacao.cstVendaAtaSimpNacional);
            CstIcmsGeral cstGeral3 = db.CstIcmsGerais.Find(tributacao.cstVendaVarejoCont);
            CstIcmsGeral cstGeral4 = db.CstIcmsGerais.Find(tributacao.cstVendaVarejoConsFinal);
            CstIcmsGeral cstGeral5 = db.CstIcmsGerais.Find(tributacao.cstCompraDeInd);
            CstIcmsGeral cstGeral6 = db.CstIcmsGerais.Find(tributacao.cstdaNfedaIndFORN);
            CstIcmsGeral cstGeral7 = db.CstIcmsGerais.Find(tributacao.cstCompradeAta);
            CstIcmsGeral cstGeral8 = db.CstIcmsGerais.Find(tributacao.cstdaNfedeAtaFORn);
            CstIcmsGeral cstGeral9 = db.CstIcmsGerais.Find(tributacao.cstCompradeSimpNacional);
            CstIcmsGeral cstGeral10 = db.CstIcmsGerais.Find(tributacao.CsosntdaNfedoSnFOR);

            if (cstGeral1 == null)
            {
                ViewBag.CstVendaAtaContDescricao = "";
            }
            else
            {
                ViewBag.CstVendaAtaContDescricao = cstGeral1.descricao;
            }

            if (cstGeral2 == null)
            {
                ViewBag.CstVendaSimpNacionalDescricao = "";
            }
            else
            {
                ViewBag.CstVendaSimpNacionalDescricao = cstGeral2.descricao;
            }

            if (cstGeral3 == null)
            {
                ViewBag.CstVendaVarCont = "";
            }
            else
            {
                ViewBag.CstVendaVarCont = cstGeral3.descricao;
            }

            if (cstGeral4 == null)
            {
                ViewBag.CstVendaVarejoCF = "";
            }
            else
            {
                ViewBag.CstVendaVarejoCF = cstGeral4.descricao;
            }

            if (cstGeral5 == null)
            {
                ViewBag.CstCompraDeInd = "";
            }
            else
            {
                ViewBag.CstCompraDeInd = cstGeral5.descricao;
            }

            if (cstGeral6 == null)
            {
                ViewBag.CstDaNfeIndForn = "";
            }
            else
            {
                ViewBag.CstDaNfeIndForn = cstGeral6.descricao;
            }

            if (cstGeral7 == null)
            {
                ViewBag.CstCompraATA = "";
            }
            else
            {
                ViewBag.CstCompraATA = cstGeral7.descricao;
            }

            if (cstGeral8 == null)
            {
                ViewBag.CstDaNfeAtaForn = "";
            }
            else
            {
                ViewBag.CstDaNfeAtaForn = cstGeral8.descricao;
            }

            if (cstGeral9 == null)
            {
                ViewBag.CstCompraSimNacional = "";
            }
            else
            {
                ViewBag.CstCompraSimNacional = cstGeral9.descricao;
            }
            if (cstGeral10 == null)
            {
                ViewBag.CstNfeSNfor = "";
            }
            else
            {
                ViewBag.CstNfeSNfor = cstGeral10.descricao;
            }


            /*ViewBagDiferente para pins cofins*/
            ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas;
            ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas;

            /*ViewBags com os dados necessários para preencher as dropbox na view*/
            ViewBag.Setor = db.SetorProdutos;
            ViewBag.NatReceita = db.NaturezaReceitas;

            ViewBag.FundLegal = db.Legislacoes;
            ViewBag.CstIcms = db.CstIcmsGerais;
            ViewBag.FundLegalPC = db.Legislacoes;
            ViewBag.FundLegalSaida = db.Legislacoes;
            ViewBag.FundLegalEndrada = db.Legislacoes;
            ViewBag.Legislacao = db.Legislacoes;
            ViewBag.CstGeral = db.CstIcmsGerais;

            ViewBag.DataAlt = DateTime.Now;

            return View(tributacao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id, estado, idProduto, idSetor, fecp, codNatReceita, aliqEntPis, aliqSaidaPis, aliqEntCofins, aliqSaidaCofins, idFundamentoLegal, aliqIcmsVendaAtaCont," +
            "aliqIcmsSTVendaAtaCont, redBaseCalcIcmsVendaAtaCont, redBaseCalcIcmsSTVendaAtaCont, aliqIcmsVendaAtaSimpNacional ,aliqIcmsSTVendaAtaSimpNacional ,redBaseCalcIcmsVendaAtaSimpNacional ," +
            "redBaseCalcIcmsSTVendaAtaSimpNacional ,aliqIcmsVendaVarejoCont ,aliqIcmsSTVendaAtaCont ,aliqIcmsSTVendaVarejo_Cont, redBaseCalcVendaVarejoCont ,RedBaseCalcSTVendaVarejo_Cont ,aliqIcmsVendaVarejoConsFinal ," +
            "aliqIcmsSTVendaVarejoConsFinal ,redBaseCalcIcmsVendaVarejoConsFinal ,redBaseCalcIcmsSTVendaVarejoConsFinal ,idFundLegalSaidaICMS ,aliqIcmsCompDeInd ,aliqIcmsSTCompDeInd ,redBaseCalcIcmsCompraDeInd ," +
            "redBaseCalcIcmsSTCompraDeInd ,aliqIcmsNFE ,redBaseCalcIcmsCompraDeAta ,redBaseCalcIcmsSTCompraDeAta ,aliqIcmsCompradeAta ,aliqIcmsSTCompraDeAta ,aliqIcmsNfeAta ,aliqIcmsCompradeSimpNacional ," +
            "aliqIcmsSTCompradeSimpNacional ,redBaseCalcIcmsCompradeSimpNacional ,redBaseCalcIcmsSTCompradeSimpNacional ,aliqIcmsNfeSN ,idFundLelgalEntradaICMS ,regime2560 ,tipoMVA ,valorMVAInd ,inicioVigenciaMVA ," +
            "valorMVAAtacado ,fimVigenciaMVA ,creditoOutorgado ,dataAlt")] string cstEntradaPisCofins, string cstSaidaPisCofins, string cstVendaAtaCont, string cstVendaAtaSimpNacional,
            string cstVendaVarejoCont, string cstVendaVarejoConsFinal, string cstCompraDeInd, string cstdaNfedaIndFORN, string cstCompradeAta, string cstdaNfedeAtaFORn, string cstCompradeSimpNacional,
            string CsosntdaNfedoSnFOR, Tributacao model)
        {
           

            if (cstEntradaPisCofins != "")
            {
                model.cstEntradaPisCofins = (from a in db.CstPisCofinsEntradas where a.descricao == cstEntradaPisCofins select a.codigo).FirstOrDefault();

            }
            if (cstSaidaPisCofins != "")
            {
                model.cstSaidaPisCofins = (from a in db.CstPisCofinsSaidas where a.descricao == cstSaidaPisCofins select a.codigo).FirstOrDefault();

            }
            if (cstVendaAtaCont != "")
            {
                model.cstVendaAtaCont = (from a in db.CstIcmsGerais where a.descricao == cstVendaAtaCont select a.codigo).FirstOrDefault();
            }
            if (cstVendaAtaSimpNacional != "")
            {
                model.cstVendaAtaSimpNacional = (from a in db.CstIcmsGerais where a.descricao == cstVendaAtaSimpNacional select a.codigo).FirstOrDefault();
            }
            if(cstVendaVarejoCont != "")
            {
                model.cstVendaVarejoCont = (from a in db.CstIcmsGerais where a.descricao == cstVendaVarejoCont select a.codigo).FirstOrDefault(); 
            }
            if(cstVendaVarejoConsFinal != "")
            {
                model.cstVendaVarejoConsFinal = (from a in db.CstIcmsGerais where a.descricao == cstVendaVarejoConsFinal select a.codigo).FirstOrDefault();
            }
            if(cstCompraDeInd != "")
            {
                model.cstCompraDeInd = (from a in db.CstIcmsGerais where a.descricao == cstCompraDeInd select a.codigo).FirstOrDefault();
            }
            if(cstdaNfedaIndFORN != "")
            {
                model.cstdaNfedaIndFORN = (from a in db.CstIcmsGerais where a.descricao == cstdaNfedaIndFORN select a.codigo).FirstOrDefault();
            }
            if(cstCompradeAta != "")
            {
                model.cstCompradeAta = (from a in db.CstIcmsGerais where a.descricao == cstdaNfedaIndFORN select a.codigo).FirstOrDefault();
            }
            if(cstdaNfedeAtaFORn != "")
            {
                model.cstdaNfedeAtaFORn = (from a in db.CstIcmsGerais where a.descricao == cstdaNfedeAtaFORn select a.codigo).FirstOrDefault();
            }
            if(cstCompradeSimpNacional  != "")
            {
                model.cstCompradeSimpNacional = (from a in db.CstIcmsGerais where a.descricao == cstCompradeSimpNacional select a.codigo).FirstOrDefault();
            }
            if(CsosntdaNfedoSnFOR != "")
            {
                model.CsosntdaNfedoSnFOR = (from a in db.CstIcmsGerais where a.descricao == CsosntdaNfedoSnFOR select a.codigo).FirstOrDefault();
            }

            /*Instanciar objeto*/
            var tributacao = db.Tributacoes.Find(model.id);

            /*Atribuir data local para alteração*/
            model.dataAlt = DateTime.Now;

            /*Verificar se o objeto está nulo*/
            if (tributacao == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            /*Atribuir valores ao objeto para alterações*/
            tributacao.idSetor = model.idSetor;
            tributacao.codNatReceita = model.codNatReceita;
            tributacao.fecp = model.fecp;

            /*Verificar estados de CSTs*/
            if (model.cstEntradaPisCofins != null)
            {
                tributacao.cstEntradaPisCofins = model.cstEntradaPisCofins;
            }

            if (model.cstSaidaPisCofins != null)
            {
                tributacao.cstSaidaPisCofins = model.cstSaidaPisCofins;
            }

            if (model.cstVendaAtaCont != null)
            {
                tributacao.cstVendaAtaCont = model.cstVendaAtaCont;
            }

            if (model.cstVendaAtaSimpNacional != null)
            {
                tributacao.cstVendaAtaSimpNacional = model.cstVendaAtaSimpNacional;
            }

            if(model.cstVendaVarejoCont != null)
            {
                tributacao.cstVendaVarejoCont = model.cstVendaVarejoCont;
            }

            if(model.cstVendaVarejoConsFinal != null)
            {
                tributacao.cstVendaVarejoConsFinal = model.cstVendaVarejoConsFinal;
            }

            if(model.cstCompraDeInd != null)
            {
                tributacao.cstCompraDeInd = model.cstCompraDeInd;
            }

            if(model.cstdaNfedaIndFORN != null)
            {
                tributacao.cstdaNfedaIndFORN = model.cstdaNfedaIndFORN;
            }

            if(model.cstCompradeAta != null)
            {
                tributacao.cstCompradeAta = model.cstCompradeAta;
            }

            if(model.cstdaNfedeAtaFORn != null)
            {
                tributacao.cstdaNfedeAtaFORn = model.cstdaNfedeAtaFORn;
            }

            if(model.cstCompradeSimpNacional != null)
            {
                tributacao.cstCompradeSimpNacional = model.cstCompradeSimpNacional;
            }

            if(model.CsosntdaNfedoSnFOR != null)
            {
                tributacao.CsosntdaNfedoSnFOR = model.CsosntdaNfedoSnFOR;
            }

            /*pis cofins*/
            tributacao.aliqEntPis = model.aliqEntPis;
            tributacao.aliqSaidaPis = model.aliqSaidaPis;
            tributacao.aliqEntCofins = model.aliqEntCofins;
            tributacao.aliqSaidaCofins = model.aliqSaidaCofins;
            tributacao.idFundamentoLegal = model.idFundamentoLegal;

            /*Venda atacado para contribuinte*/
            tributacao.aliqIcmsVendaAtaCont = model.aliqIcmsVendaAtaCont;
            tributacao.aliqIcmsSTVendaAtaCont = model.aliqIcmsSTVendaAtaCont;
            tributacao.redBaseCalcIcmsVendaAtaCont = model.redBaseCalcIcmsVendaAtaCont;
            tributacao.redBaseCalcIcmsSTVendaAtaCont = model.redBaseCalcIcmsSTVendaAtaCont;
         
            /*Venda atacado para simples nacional*/
            tributacao.aliqIcmsVendaAtaSimpNacional   = model.aliqIcmsVendaAtaSimpNacional;
            tributacao.aliqIcmsSTVendaAtaSimpNacional = model.aliqIcmsSTVendaAtaSimpNacional;
            tributacao.redBaseCalcIcmsVendaAtaSimpNacional = model.redBaseCalcIcmsVendaAtaSimpNacional;
            tributacao.redBaseCalcIcmsSTVendaAtaSimpNacional = model.redBaseCalcIcmsSTVendaAtaSimpNacional;

            /*Venda varejo para contribuinte*/
            tributacao.aliqIcmsVendaVarejoCont = model.aliqIcmsVendaVarejoCont;
            tributacao.aliqIcmsSTVendaVarejo_Cont = model.aliqIcmsSTVendaVarejo_Cont;
            tributacao.redBaseCalcVendaVarejoCont = model.redBaseCalcVendaVarejoCont;
            tributacao.RedBaseCalcSTVendaVarejo_Cont = model.RedBaseCalcSTVendaVarejo_Cont;
        
            /*Venda varejo para consumidor final*/
            tributacao.aliqIcmsVendaVarejoConsFinal = model.aliqIcmsVendaVarejoConsFinal;
            tributacao.aliqIcmsSTVendaVarejoConsFinal = model.aliqIcmsSTVendaVarejoConsFinal;
            tributacao.redBaseCalcIcmsVendaVarejoConsFinal = model.redBaseCalcIcmsVendaVarejoConsFinal;
            tributacao.redBaseCalcIcmsSTVendaVarejoConsFinal = model.redBaseCalcIcmsSTVendaVarejoConsFinal;

            /*Fundamento legal vendas icms*/
            tributacao.idFundLegalSaidaICMS = model.idFundLegalSaidaICMS;

            /*Compra de industria*/
            tributacao.aliqIcmsCompDeInd = model.aliqIcmsCompDeInd;
            tributacao.aliqIcmsSTCompDeInd = model.aliqIcmsSTCompDeInd;
            tributacao.redBaseCalcIcmsCompraDeInd = model.redBaseCalcIcmsCompraDeInd;
            tributacao.redBaseCalcIcmsSTCompraDeInd = model.redBaseCalcIcmsSTCompraDeInd;
            tributacao.aliqIcmsNFE = model.aliqIcmsNFE;

            /*Compra de atacado*/
            tributacao.redBaseCalcIcmsCompraDeAta = model.redBaseCalcIcmsCompraDeAta;
            tributacao.redBaseCalcIcmsSTCompraDeAta = model.redBaseCalcIcmsSTCompraDeAta;
            tributacao.aliqIcmsCompradeAta = model.aliqIcmsCompradeAta;
            tributacao.aliqIcmsSTCompraDeAta = model.aliqIcmsSTCompraDeAta;
            tributacao.aliqIcmsNfeAta = model.aliqIcmsNfeAta;

            /*Compra de simples nacional*/
            tributacao.aliqIcmsCompradeSimpNacional = model.aliqIcmsCompradeSimpNacional;
            tributacao.aliqIcmsSTCompradeSimpNacional = model.aliqIcmsSTCompradeSimpNacional;
            tributacao.redBaseCalcIcmsCompradeSimpNacional = model.redBaseCalcIcmsCompradeSimpNacional;
            tributacao.redBaseCalcIcmsSTCompradeSimpNacional = model.redBaseCalcIcmsSTCompradeSimpNacional;
            tributacao.aliqIcmsNfeSN = model.aliqIcmsNfeSN;

            /*Fundamento legal entrada(compras) icms*/
            tributacao.idFundLelgalEntradaICMS = model.idFundLelgalEntradaICMS;

            /*Outros atributos*/
            tributacao.regime2560 = model.regime2560;
            tributacao.tipoMVA = model.tipoMVA;
            tributacao.valorMVAInd = model.valorMVAInd;
            tributacao.inicioVigenciaMVA = model.inicioVigenciaMVA;
            tributacao.valorMVAAtacado = model.valorMVAAtacado;
            tributacao.fimVigenciaMVA = model.fimVigenciaMVA;
            tributacao.creditoOutorgado = model.creditoOutorgado;
            tributacao.dataAlt = model.dataAlt;

            db.SaveChanges();
            return RedirectToAction("Index");

        }

        //delte

        public ActionResult Delete(int? id)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            if (Session["nivel"].Equals("USUARIO"))
            {
                int par = 3;
                return RedirectToAction("../Erro/Erro", new { param = par });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Tributacao tributacao = db.Tributacoes.Find(id);
            if (tributacao == null)
            {
                return HttpNotFound();
            }



            /*Verificando cst na tabela e Preencher a descrição dos csts
            Obs.: Quando o registro não possui algun desses valores a
            consulta ao banco retorna null, dando exceção por estar nulo
            dessa forma ele valida na condição e não permite valores
            nulos sejam tratados
            */
            ViewBag.CstVendaAtaContDescricao = (db.CstIcmsGerais.Find(tributacao.cstVendaAtaCont) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstVendaAtaCont).descricao;
            ViewBag.CstVendaSimpNacionalDescricao = (db.CstIcmsGerais.Find(tributacao.cstVendaAtaSimpNacional) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstVendaAtaSimpNacional).descricao;
            ViewBag.CstVendaVarCont = (db.CstIcmsGerais.Find(tributacao.cstVendaVarejoCont) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstVendaVarejoCont).descricao;
            ViewBag.CstVendaVarejoCF = (db.CstIcmsGerais.Find(tributacao.cstVendaVarejoConsFinal) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstVendaVarejoConsFinal).descricao;
            ViewBag.CstCompraDeInd = (db.CstIcmsGerais.Find(tributacao.cstCompraDeInd) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstCompraDeInd).descricao;
            ViewBag.CstDaNfeIndForn = (db.CstIcmsGerais.Find(tributacao.cstdaNfedaIndFORN) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstdaNfedaIndFORN).descricao;
            ViewBag.CstCompraATA = (db.CstIcmsGerais.Find(tributacao.cstCompradeAta) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstCompradeAta).descricao;
            ViewBag.CstDaNfeAtaForn = (db.CstIcmsGerais.Find(tributacao.cstdaNfedeAtaFORn) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstdaNfedeAtaFORn).descricao;
            ViewBag.CstCompraSimNacional = (db.CstIcmsGerais.Find(tributacao.cstCompradeSimpNacional) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstCompradeSimpNacional).descricao;
            ViewBag.CstNfeSNfor = (db.CstIcmsGerais.Find(tributacao.CsosntdaNfedoSnFOR) == null) ? "" : db.CstIcmsGerais.Find(tributacao.CsosntdaNfedoSnFOR).descricao;



            /*ViewBagDiferente para pins cofins*/
            ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas;
            ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas;

            /*ViewBags com os dados necessários para preencher as dropbox na view*/
            ViewBag.Setor = db.SetorProdutos;
            ViewBag.NatReceita = db.NaturezaReceitas;
            ViewBag.FundLegal = db.Legislacoes;
            ViewBag.CstIcms = db.CstIcmsGerais;
            ViewBag.FundLegalPC = db.Legislacoes;
            ViewBag.FundLegalSaida = db.Legislacoes;
            ViewBag.FundLegalEndrada = db.Legislacoes;
            ViewBag.Legislacao = db.Legislacoes;
            ViewBag.CstGeral = db.CstIcmsGerais;

            ViewBag.DataAlt = DateTime.Now;

            return View(tributacao);

        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tributacao tributacao = db.Tributacoes.Find(id);
            db.Tributacoes.Remove(tributacao);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult AutoComplete()
        {
            //Carregar lista autocomplete
            List<Produto> produtos = db.Produtos.ToList();
            List<Object> resultado = new List<object>();


            produtos.ForEach(x => {
                resultado.Add(x.descricao);
          
            });

           // var prod = from s in db.Produtos select s.descricao; //variavel carregado de produtos


            return Json(resultado, JsonRequestBehavior.AllowGet);
        }


        //DASHBOARD
        [HttpGet]
        public ActionResult GraficoIcmsEntrada()
        {
            
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
          
            //chmar action auxiliar para verificar e carregar a tempdata com a lista
            VerificaTempData();

            /*Aliquota ICMS Compra industria*/
             ViewBag.AliqICMSEntradaInd = this.tribMTX.Count(a=>a.ALIQ_ICMS_COMP_DE_IND !=null);
             ViewBag.AliqICMSEntradaNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_COMP_DE_IND == null);

            /*Aliquota ICMS st Compra industria*/
            ViewBag.AliqICMSSTEntradaInd   = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_COMP_DE_IND != null);
            ViewBag.AliqICMSSTEntradaNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_COMP_DE_IND == null);


            /*Aliquota ICMS compra de atacado*/
            ViewBag.AliqICMSCompraAta = this.tribMTX.Count(a => a.ALIQ_ICMS_COMPRA_DE_ATA != null);
            ViewBag.AliqICMSCompraAtaNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_COMPRA_DE_ATA == null);

            /*Aliquota ICMS ST compra de atacado*/
            ViewBag.AliqICMSSTCompraAta      = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null);
            ViewBag.AliqICMSSTCompraAtaNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == null);

            /*Aliquota ICMS compra de Simples nacional*/
            ViewBag.AliqICMSCompraSN      = this.tribMTX.Count(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null);
            ViewBag.AliqICMSCompraSNNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null);

            /*Aliquota ICMS ST compra de Simples nacional*/
            ViewBag.AliqICMSSTCompraSN = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null);
            ViewBag.AliqICMSSTCompraSNNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null);


            /*Aliquota ICMS NFE Compra Industria*/
            ViewBag.AliqICMSNFEInd = this.tribMTX.Count(a => a.ALIQ_ICMS_NFE != null);
            ViewBag.AliqICMSNfeIndNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_NFE == null);

            /*Aliquota ICMS NFE Compra Ata*/
            ViewBag.AliqICMSNFEAta = this.tribMTX.Count(a => a.ALIQ_ICMS_NFE_FOR_ATA != null);
            ViewBag.AliqICMSNFEAtaNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_NFE_FOR_ATA == null);


            /*Aliquota ICMS NFE Compra SN*/
            ViewBag.AliqICMSNFESN = this.tribMTX.Count(a => a.ALIQ_ICMS_NFE_FOR_SN != null);
            ViewBag.AliqICMSNFESNNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_NFE_FOR_SN == null);


            return View();
        }

        [HttpGet]
        public ActionResult GraficoIcmsSaida()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //chmar action auxiliar para verificar e carregar a tempdata com a lista
            VerificaTempData();

            /*Aliquota ICMS Venda Varejo Consumidor Final*/
            ViewBag.AliqICMSVendaVarCF      = this.tribMTX.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null);
            ViewBag.AliqICMSVendaVarCFNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null);


            /*Aliquota ICMS ST Venda Varejo Consumidor Final*/
            ViewBag.AliqICMSSTVendaVarCF      = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null);
            ViewBag.AliqICMSSTVendaVarCFNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null);


            /*Aliquota ICMS Venda Varejo Para Contribuinte*/
            ViewBag.AliqICMSVendaVarCont      = this.tribMTX.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null);
            ViewBag.AliqICMSVendaVarContNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null);

            /*Aliquota ICMS ST Venda Varejo Para Contribuinte*/
            ViewBag.AliqICMSSTVendaVarCont      = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null);
            ViewBag.AliqICMSSTVendaVarContNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null);


            /*Aliquota ICMS Venda Ata Para Contribuinte*/
            ViewBag.AliqICMSVendaAtaCont      = this.tribMTX.Count(a => a.ALIQ_ICMS_VENDA_ATA_CONT != null);
            ViewBag.AliqICMSVendaAtaContNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_VENDA_ATA_CONT == null);

            /*Aliquota ICMS ST Venda Ata Para Contribuinte*/
            ViewBag.AliqICMSSTVendaAtaCont      = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_CONT != null);
            ViewBag.AliqICMSSTVendaAtaContNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_CONT == null);


            /*Aliquota ICMS Venda Ata Para Simples Nacional*/
            ViewBag.AliqICMSVendaAtaSN      = this.tribMTX.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null);
            ViewBag.AliqICMSVendaAtaNSNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null);

            /*Aliquota ICMS ST Venda Ata Para Simples Nacional*/
            ViewBag.AliqICMSSTVendaAtaSN     = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null);
            ViewBag.AliqICMSSTVendaAtaNSNulla = this.tribMTX.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null);



            return View();
        }

        [HttpGet]
        public ActionResult GfRedBCalcIcmsEntrada()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //chmar action auxiliar para verificar e carregar a tempdata com a lista
            VerificaTempData();


            /*Redução base calc icms compra de industria*/
            ViewBag.RedBasCalIcmsCompInd     = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null);
            ViewBag.RedBasCalIcmsCompIndNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == null);

            /*Redução base calc icms ST compra de industria*/
            ViewBag.RedBasCalIcmsSTCompInd     = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null);
            ViewBag.RedBasCalIcmsSTCompIndNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == null);


            /*Redução base calc icms compra de atacado*/
            ViewBag.RedBasCalIcmsCompAta     = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null);
            ViewBag.RedBasCalIcmsCompAtaNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == null);

            /*Redução base calc icms st compra de atacado*/
            ViewBag.RedBasCalIcmsSTCompAta     = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null);
            ViewBag.RedBasCalIcmsSTCompAtaNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == null);



            /*Redução base calc icms compra de  SIMPLES NACIONAL*/
            ViewBag.RedBasCalIcmsCompSN     = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL != null);
            ViewBag.RedBasCalIcmsCompSNNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL == null);

            /*Redução base calc icms compra de  SIMPLES NACIONAL*/
            ViewBag.RedBasCalIcmsSTCompSN     = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null);
            ViewBag.RedBasCalIcmsSTCompSNNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null);


            return View();
        }

        [HttpGet]
        public ActionResult GfRedBCalcIcmsSaida()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //chmar action auxiliar para verificar e carregar a tempdata com a lista
            VerificaTempData();

            /*Redução base calc icms venda varejo cf*/
            ViewBag.RedBasCalIcmsVendaVarCF     = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null);
            ViewBag.RedBasCalIcmsVendaVarCFNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null);

            /*Redução base calc icms ST venda varejo cf*/
            ViewBag.RedBasCalIcmsSTVendaVarCF     = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null);
            ViewBag.RedBasCalIcmsSTVendaVarCFNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null);


            /*Redução base de calculo ICMS venda varejo para Contribuinte*/
            ViewBag.RedBasCalIcmsVendaVarCont     = this.tribMTX.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null);
            ViewBag.RedBasCalIcmsVendaVarContNull = this.tribMTX.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null);

            /*Redução base de calculo ICMS ST venda varejo para Contribuinte*/
            ViewBag.RedBasCalIcmsSTVendaVarCont     = this.tribMTX.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null);
            ViewBag.RedBasCalIcmsSTVendaVarContNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null);


            /*Red Base Calc ICMS Venda Atacado para Contribuinte*/
            ViewBag.RedBasCalIcmsVendaAtaCont     = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT != null);
            ViewBag.RedBasCalIcmsVendaAtaContNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT == null);

            /*Red Base Calc ICMS ST Venda Atacado para Contribuinte*/
            ViewBag.RedBasCalIcmsSTVendaAtaCont =     this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT != null);
            ViewBag.RedBasCalIcmsSTVendaAtaContNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT == null);


            /*Red Base Calc ICMS Venda Atacado para Simples Nacional*/
            ViewBag.RedBasCalIcmsVendaAtaSN     = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null);
            ViewBag.RedBasCalIcmsVendaAtaSNNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null);

            /*Red Base Calc ICMS ST Venda Atacado para Simples Nacional*/
            ViewBag.RedBasCalIcmsSTVendaAtaSN = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null);
            ViewBag.RedBasCalIcmsSTVendaAtaSNNull = this.tribMTX.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null);


            return View();
        }
        [HttpGet]
        public ActionResult GraficoAliPisCofins()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //chmar action auxiliar para verificar e carregar a tempdata com a lista
            VerificaTempData();

            
            /*Aliq Entrada Pis*/
            ViewBag.AliEntradaPis     = this.tribMTX.Count(a => a.ALIQ_ENT_PIS != null);
            ViewBag.AliEntradaPisNull = this.tribMTX.Count(a => a.ALIQ_ENT_PIS == null);

            /*Aliq Saída Pis*/
            ViewBag.AliSaidaPis     = this.tribMTX.Count(a => a.ALIQ_SAIDA_PIS != null);
            ViewBag.AliSaidaPisNull = this.tribMTX.Count(a => a.ALIQ_SAIDA_PIS == null);

            /*Ali Entrada Cofins*/
            ViewBag.AliEntradaCofins     = this.tribMTX.Count(a => a.ALIQ_ENT_COFINS != null);
            ViewBag.AliEntradaCofinsNull = this.tribMTX.Count(a => a.ALIQ_ENT_COFINS == null);

            /*Ali Saída Cofins*/
            ViewBag.AliSaidaCofins     = this.tribMTX.Count(a => a.ALIQ_SAIDA_COFINS != null);
            ViewBag.AliSaidaCofinsNull = this.tribMTX.Count(a => a.ALIQ_SAIDA_COFINS == null);


            return View();
        }
        [HttpGet]
        public ActionResult GraficoCstEntrada() 
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //chmar action auxiliar para verificar e carregar a tempdata com a lista
            VerificaTempData();


           

            /*CST Entrada Pis/cofins */
            ViewBag.CstEntradaPisCofins     = this.tribMTX.Count(a => a.CST_ENTRADA_PISCOFINS != null);
            ViewBag.CstEntradaPisCofinsNull = this.tribMTX.Count(a => a.CST_ENTRADA_PISCOFINS == null);

            /*CST Compra de industria */
            ViewBag.CstCompraInd     = this.tribMTX.Count(a => a.CST_COMPRA_DE_IND != null);
            ViewBag.CstCompraIndNull = this.tribMTX.Count(a => a.CST_COMPRA_DE_IND == null);

            /*CST Compra de Atacado */
            ViewBag.CstCompraAta     = this.tribMTX.Count(a => a.CST_COMPRA_DE_ATA != null);
            ViewBag.CstCompraAtaNull = this.tribMTX.Count(a => a.CST_COMPRA_DE_ATA == null);

            /*CST Compra de Simpes Nacional */
            ViewBag.CstCompraSN     = this.tribMTX.Count(a => a.CST_COMPRA_DE_SIMP_NACIONAL != null);
            ViewBag.CstCompraSNNull = this.tribMTX.Count(a => a.CST_COMPRA_DE_SIMP_NACIONAL == null);


            /*CST NFe Industria*/
            ViewBag.CstNfeInd     = this.tribMTX.Count(a => a.CST_DA_NFE_DA_IND_FORN != null);
            ViewBag.CstNfeIndNull = this.tribMTX.Count(a => a.CST_DA_NFE_DA_IND_FORN == null);

            /*CST NFe Atacado*/
            ViewBag.CstNfeAta     = this.tribMTX.Count(a => a.CST_DA_NFE_DE_ATA_FORN != null);
            ViewBag.CstNfeAtaNull = this.tribMTX.Count(a => a.CST_DA_NFE_DE_ATA_FORN == null);

            /*CST NFe Simples Nacional*/
            ViewBag.CstNfeSN     = this.tribMTX.Count(a => a.CSOSNTDANFEDOSNFOR != null);
            ViewBag.CstNfeSNNull = this.tribMTX.Count(a => a.CSOSNTDANFEDOSNFOR == null);


            return View();
        }

        [HttpGet]
        public ActionResult GraficoCstSaida() 
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //chmar action auxiliar para verificar e carregar a tempdata com a lista
            VerificaTempData();

                                

            /*Cst Pis Cofins Saida*/
            ViewBag.CstSaidaPisCofins     = this.tribMTX.Count(a => a.CST_SAIDA_PISCOFINS != null);
            ViewBag.CstSaidaPisCofinsNull = this.tribMTX.Count(a => a.CST_SAIDA_PISCOFINS == null);


            /*Cst Venda Var Cons Final*/
            ViewBag.CstVendaVarCF     = this.tribMTX.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL != null);
            ViewBag.CstVendaVarCFNull = this.tribMTX.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL == null);

            /*CST Venda Varejo para Contribuinte*/
            ViewBag.CstVendaVarCont     = this.tribMTX.Count(a => a.CST_VENDA_VAREJO_CONT != null);
            ViewBag.CstVendaVarContNull = this.tribMTX.Count(a => a.CST_VENDA_VAREJO_CONT == null);


            /*CST Venda Atacado para Contribuinte*/
            ViewBag.CstVendaAtaCont     = this.tribMTX.Count(a => a.CST_VENDA_ATA_CONT != null);
            ViewBag.CstVendaAtaContNull = this.tribMTX.Count(a => a.CST_VENDA_ATA_CONT == null);

            /*CST Venda Atacado para Simples nacional*/
            ViewBag.CstVendaAtaSN     = this.tribMTX.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL != null);
            ViewBag.CstVendaAtaSNNull = this.tribMTX.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL == null);



            return View();


        }//fim da action


        /*Controller para edição em massa CStPisCofinsMassa - VERSÃO FINAL*/
        [HttpGet]
        public ActionResult EditCstPisCofinsSaidaMassa(string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor, string procurarCST,
            string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteCST, string filtroCorrenteNCM,
            string filtroCorrenteCEST, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //variavel auxiliar
            string resultado = param;
      
            //Variavel auxiliar para procurar por cst
            string cst = filtroCorrenteCST ?? procurarCST;

            //converte em int, caso o valor seja um numero
            bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;


            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            
            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;
            procurarCST = (procurarCST != null) ? procurarCST : null;



            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;
            TempData["opcao"] = (opcao != null) ? opcao : TempData["opcao"];

            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");


            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarCST != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarCST = (procurarCST == null) ? filtroCorrenteCST : procurarCST;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteCST = procurarCST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                    this.tribMTX = this.tribMTX.Where(s => s.CST_SAIDA_PISCOFINS != null).ToList();
                    break;
                case "Sem CST":
                    this.tribMTX = this.tribMTX.Where(s => s.CST_SAIDA_PISCOFINS == null).ToList();
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);

            //procurar por cst

            //procura por cst
            if (!String.IsNullOrEmpty(procurarCST))
            {

                this.tribMTX = this.tribMTX.Where(s => s.CST_SAIDA_PISCOFINS.ToString() == (cstInt.ToString())).ToList();


            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.tribMTX = this.tribMTX.OrderByDescending(s => s.DESCRICAO_PRODUTO).ToList();
                    break;

                default:
                    this.tribMTX = this.tribMTX.OrderBy(s => s.ID).ToList();
                    break;


            }


            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            ViewBag.CstGeral = db.CstPisCofinsSaidas.ToList();



            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist



        }//fim EditCstPisCofinsSaidaMassa

        [HttpGet]
        public ActionResult EditCstPisCofinsSaidaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));


            }
            //ViewBag.Tributos = trib;
            ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas.ToList();
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditCstPisCofinsSaidaMassaModalPost(string strDados, string cstPCSaida)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstSaidaPisCofins = (int)(from a in db.CstPisCofinsSaidas where a.descricao == cstPCSaida select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();


            try
            {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.cstSaidaPisCofins = cstSaidaPisCofins; //novo cst
                    db.SaveChanges();
                    regSalvos++;
                }
                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {

                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }



            //Redirecionar para a tela de graficos
            //Redirecionar para a tela de graficos
            return RedirectToAction("EditCstPisCofinsSaidaMassa", new { param = resultado, qtdSalvos = regSalvos });
           

        }

        //Edit em massa Venda Varejo Consumidor final - VERSÃO FINAL
        [HttpGet]
        public ActionResult EditCstVendaVarCFMassa(string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor, string procurarCST,
            string procuraNCM, string procuraCEST, string filtroCorrente,  string filtroCorrenteCST, string filtroCorrenteNCM,
            string filtroCorrenteCEST, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
            //variavel auxiliar
            string resultado = param;

           //Variavel auxiliar para procurar por cst
            string cst = filtroCorrenteCST ?? procurarCST;

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;



            //converte em int, verificando se não digitou pontos ou letras
            bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;
            procurarCST = (procurarCST != null) ? procurarCST : null;
           
            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;
            TempData["opcao"] = (opcao != null) ? opcao : TempData["opcao"];
            

            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarCST != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente     : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarCST = (procurarCST == null) ? filtroCorrenteCST  : procurarCST;
            procuraNCM  = (procuraNCM  == null) ? filtroCorrenteNCM  : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;

           
            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteCST = procurarCST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com CST":
                    this.tribMTX = this.tribMTX.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                    break;
                case "Sem CST":
                    this.tribMTX = this.tribMTX.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == null).ToList();
                    break;


            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);

            //procurar por cst

           //procura por cst
            if (!String.IsNullOrEmpty(procurarCST))
            {

                this.tribMTX = this.tribMTX.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL.ToString() == (cstInt.ToString())).ToList();


            }


            switch (ordenacao)
            {
                case "Produto_desc":
                    this.tribMTX = this.tribMTX.OrderByDescending(s => s.DESCRICAO_PRODUTO).ToList();
                    break;

                default:
                    this.tribMTX = this.tribMTX.OrderBy(s => s.ID).ToList();
                    break;


            }


            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);
           
            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            ViewBag.CstGeral = db.CstIcmsGerais.ToList();

         

            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist

        }

        [HttpGet]
        public ActionResult EditCstVendaVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));


            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais.ToList();
            return View(trib);

        }
        [HttpGet]
        public ActionResult EditCstVendaVarCFMassaModalPost(string strDados, string cstVendVarCF)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstVenVarCF = (int)(from a in db.CstIcmsGerais where a.descricao == cstVendVarCF select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            try {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.cstVendaVarejoConsFinal = cstVenVarCF; //novo cst
                    db.SaveChanges();
                    regSalvos++;
                }
                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e) {

                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }



            //Redirecionar para a tela de graficos
            return RedirectToAction("EditCstVendaVarCFMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        //Edit masss venda varejo contribuinte - VERSÃO FINAL
        [HttpGet]
        public ActionResult EditCstVendaVarContMassa(string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor, string procurarCST,
            string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteCST, string filtroCorrenteNCM,
            string filtroCorrenteCEST, int? page, int? numeroLinhas)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
            //variavel auxiliar
            string resultado = param;

            //Variavel auxiliar para procurar por cst
            string cst = filtroCorrenteCST ?? procurarCST;

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;


            //converte em int, verificando se não digitou pontos ou letras
            bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;
            procurarCST = (procurarCST != null) ? procurarCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente
            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;
            TempData["opcao"] = (opcao != null) ? opcao : TempData["opcao"];


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarCST != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarCST = (procurarCST == null) ? filtroCorrenteCST : procurarCST;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteCST = procurarCST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;



            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com CST":
                    this.tribMTX = this.tribMTX.Where(s => s.CST_VENDA_VAREJO_CONT != null).ToList();
                    break;
                case "Sem CST":
                    this.tribMTX = this.tribMTX.Where(s => s.CST_VENDA_VAREJO_CONT == null).ToList();
                    break;


            }


            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);

            //procurar por cst

            //procura por cst
            if (!String.IsNullOrEmpty(procurarCST))
            {

                this.tribMTX = this.tribMTX.Where(s => s.CST_VENDA_VAREJO_CONT.ToString() == (cstInt.ToString())).ToList();


            }
            switch (ordenacao)
            {
                case "Produto_desc":
                    this.tribMTX = this.tribMTX.OrderByDescending(s => s.DESCRICAO_PRODUTO).ToList();
                    break;

                default:
                    this.tribMTX = this.tribMTX.OrderBy(s => s.ID).ToList();
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            ViewBag.CstGeral = db.CstIcmsGerais.ToList();



            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditCstVendaVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais.ToList();
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditCstVendaVarContMassaModalPost(string strDados, string cstVendVarCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstVenVarCont = (int)(from a in db.CstIcmsGerais where a.descricao == cstVendVarCont select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();


            try
            {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.cstVendaVarejoCont = cstVenVarCont; //novo cst
                    db.SaveChanges();
                    regSalvos++;
                }
                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {

                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }





            //Redirecionar para a tela de graficos
            return RedirectToAction("EditCstVendaVarContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        //Edit massa venda atacado contribuinte -   VERSÃO FINAL
        [HttpGet]
        public ActionResult EditCstVendaAtaContMassa(string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor, string procurarCST,
            string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteCST, string filtroCorrenteNCM,
            string filtroCorrenteCEST, int? page, int? numeroLinhas)
        {
           
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //variavel auxiliar
            string resultado = param;

            //Variavel auxiliar para procurar por cst
            string cst = filtroCorrenteCST ?? procurarCST;

            //converte em int, caso o valor seja um numero

            bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;
            procurarCST = (procurarCST != null) ? procurarCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;
            TempData["opcao"] = (opcao != null) ? opcao : TempData["opcao"];


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarCST != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarCST = (procurarCST == null) ? filtroCorrenteCST : procurarCST;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteCST = procurarCST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                    this.tribMTX = this.tribMTX.Where(s => s.CST_VENDA_ATA_CONT != null).ToList();
                    break;
                case "Sem CST":
                    this.tribMTX = this.tribMTX.Where(s => s.CST_VENDA_ATA_CONT == null).ToList();
                    break;


            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);

            //procurar por cst

            //procura por cst
            if (!String.IsNullOrEmpty(procurarCST))
            {

                this.tribMTX = this.tribMTX.Where(s => s.CST_VENDA_ATA_CONT.ToString() == (cstInt.ToString())).ToList();


            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.tribMTX = this.tribMTX.OrderByDescending(s => s.DESCRICAO_PRODUTO).ToList();
                    break;

                default:
                    this.tribMTX = this.tribMTX.OrderBy(s => s.ID).ToList();
                    break;

            }



            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            ViewBag.CstGeral = db.CstIcmsGerais.ToList();



            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditCstVendaAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais.ToList();
            return View(trib);

        }

        [HttpGet]
        public ActionResult EditCstVendaAtaContMassaModalPost(string strDados, string cstVendAtaCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstVenAtaCont = (int)(from a in db.CstIcmsGerais where a.descricao == cstVendAtaCont select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();


            try
            {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.cstVendaAtaCont = cstVenAtaCont; //novo cst
                    db.SaveChanges();
                    regSalvos++;
                }
                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {

                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstVendaAtaCont = cstVenAtaCont; //novo cst
                db.SaveChanges();
            }
            
           //Redirecionar para a tela de graficos
            return RedirectToAction("EditCstVendaAtaContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }




        //Edit em massa venda atacado para simples nacional - VERSÃO FINAL
        [HttpGet]
        public ActionResult EditCstVendaAtaSNMassa(string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor, string procurarCST,
            string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteCST, string filtroCorrenteNCM,
            string filtroCorrenteCEST, int? page, int? numeroLinhas)
        {
           
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
            //variavel auxiliar
            string resultado = param;

            //Variavel auxiliar para procurar por cst
            string cst = filtroCorrenteCST ?? procurarCST;

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em int, verificando se não digitou pontos ou letras
            bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;
            procurarCST = (procurarCST != null) ? procurarCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;
            TempData["opcao"] = (opcao != null) ? opcao : TempData["opcao"];


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarCST != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarCST = (procurarCST == null) ? filtroCorrenteCST : procurarCST;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteCST = procurarCST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                    this.tribMTX = this.tribMTX.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                    break;
                case "Sem CST":
                    this.tribMTX = this.tribMTX.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                    break;


            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);

            //procurar por cst

            //procura por cst
            if (!String.IsNullOrEmpty(procurarCST))
            {

                this.tribMTX = this.tribMTX.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL.ToString() == (cstInt.ToString())).ToList();


            }


            switch (ordenacao)
            {
                case "Produto_desc":
                    this.tribMTX = this.tribMTX.OrderByDescending(s => s.DESCRICAO_PRODUTO).ToList();
                    break;

                default:
                    this.tribMTX = this.tribMTX.OrderBy(s => s.ID).ToList();
                    break;


            }



            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            ViewBag.CstGeral = db.CstIcmsGerais.ToList();

            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist

        }

        [HttpGet]
        public ActionResult EditCstVendaAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais.ToList();
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditCstVendaAtaSNMassaModalPost(string strDados, string cstVendAtaSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstVenAtaSN = (int)(from a in db.CstIcmsGerais where a.descricao == cstVendAtaSN select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;


            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();


            try
            {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.cstVendaAtaSimpNacional = cstVenAtaSN; //novo cst
                    db.SaveChanges();
                    regSalvos++;
                }
                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {

                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }

            
            //Redirecionar para a tela de graficos
            //Redirecionar para a tela de graficos
            return RedirectToAction("EditCstVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalvos });

        }


        /* Cst de Entrada */
        //Entrada Pis/cofins
        [HttpGet]
        public ActionResult EditCstPisCofinsEntradaMassa(string opcao, string ordenacao, string procurarPor, string filtroCorrente, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
            //Auxilia na conversão para fazer a busca pelo codigo de barras
            string cst = filtroCorrente ?? procurarPor;

            //converte em int, caso o valor seja um numero

            bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido
            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo


            ViewBag.FiltroCorrente = procurarPor;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com CST")
            {

                trib1 = trib1.Where(s => s.cstEntradaPisCofins != null);


                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.cstEntradaPisCofins.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }


            }
            else
            {
                trib1 = trib1.Where(s => s.cstEntradaPisCofins == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.cstEntradaPisCofins.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }


            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            

            int numeroPagina = (page ?? 1);

            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }//fim action
        [HttpGet]
        public ActionResult EditCstPisCofinsEntradaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas.ToList();
            return View(trib);
        }

        [HttpGet]           
        public ActionResult EditCstPisCofinsEntradaMassaModalPost(string strDados, string cstPisCofinsE)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstPCEntrada = (int)(from a in db.CstPisCofinsEntradas where a.descricao == cstPisCofinsE select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstEntradaPisCofins = cstPCEntrada; //novo cst
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //Compra de Industria
        [HttpGet]
        public ActionResult EditCstCompraIndustriaMassa(string opcao, string ordenacao, string procurarPor, string filtroCorrente, int? page, int? numeroLinhas)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
            //Auxilia na conversão para fazer a busca pelo codigo de barras
            string cst = filtroCorrente ?? procurarPor;

            //converte em int, caso o valor seja um numero

            bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido
            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo


            ViewBag.FiltroCorrente = procurarPor;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com CST")
            {

                trib1 = trib1.Where(s => s.cstCompraDeInd != null);


                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.cstCompraDeInd.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }


            }
            else
            {
                trib1 = trib1.Where(s => s.cstCompraDeInd == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.cstCompraDeInd.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);
            
            int numeroPagina = (page ?? 1);

            ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view

            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditCstCompraIndustriaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais.ToList();
            return View(trib);
        }
        [HttpGet]
        public ActionResult EditCstCompraIndustriaMassaModalPost(string strDados, string cstCompraInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstCPind = (int)(from a in db.CstIcmsGerais where a.descricao == cstCompraInd select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstCompraDeInd = cstCPind; //novo cst
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //Compra de atacado
        [HttpGet]
        public ActionResult EditCstCompraAtacadoMassa(string opcao, string ordenacao, string procurarPor, string filtroCorrente, int? page, int? numeroLinhas)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
            //Auxilia na conversão para fazer a busca pelo codigo de barras
            string cst = filtroCorrente ?? procurarPor;

            //converte em int, caso o valor seja um numero

            bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido
            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo


            ViewBag.FiltroCorrente = procurarPor;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com CST")
            {

                trib1 = trib1.Where(s => s.cstCompradeAta != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.cstCompradeAta.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }


            }
            else
            {
                trib1 = trib1.Where(s => s.cstCompradeAta == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.cstCompradeAta.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view

            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditCstCompraAtacadoMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais.ToList();
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditCstCompraAtacadoMassaModalPost(string strDados, string cstCompraAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstCPata = (int)(from a in db.CstIcmsGerais where a.descricao == cstCompraAta select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstCompradeAta = cstCPata; //novo cst
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //Compra SN
        [HttpGet]
        public ActionResult EditCstCompraSNMassa(string opcao, string ordenacao, string procurarPor, string filtroCorrente, int? page, int? numeroLinhas)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
            //Auxilia na conversão para fazer a busca pelo codigo de barras
            string cst = filtroCorrente ?? procurarPor;

            //converte em int, caso o valor seja um numero

            bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido
            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo


            ViewBag.FiltroCorrente = procurarPor;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com CST")
            {

                trib1 = trib1.Where(s => s.cstCompradeSimpNacional != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.cstCompradeSimpNacional.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }


            }
            else
            {
                trib1 = trib1.Where(s => s.cstCompradeSimpNacional == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.cstCompradeSimpNacional.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view

            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditCstCompraSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais.ToList();
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditCstCompraSNMassaModalPost(string strDados, string cstCompraSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstCPSn = (int)(from a in db.CstIcmsGerais where a.descricao == cstCompraSN select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstCompradeSimpNacional = cstCPSn; //novo cst

                
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //Nfe Ind
        [HttpGet]
        public ActionResult EditCstNfeIndMassa(string opcao, string ordenacao, string procurarPor, string filtroCorrente, int? page, int? numeroLinhas)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
            //Auxilia na conversão para fazer a busca pelo codigo de barras
            string cst = filtroCorrente ?? procurarPor;

            //converte em int, caso o valor seja um numero

            bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido
            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo


            ViewBag.FiltroCorrente = procurarPor;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com CST")
            {

                trib1 = trib1.Where(s => s.cstdaNfedaIndFORN != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.cstdaNfedaIndFORN.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }


            }
            else
            {
                trib1 = trib1.Where(s => s.cstdaNfedaIndFORN == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.cstdaNfedaIndFORN.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view

            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditCstNfeIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais.ToList();
            return View(trib);

        }

        [HttpGet]
        public ActionResult EditCstNfeIndMassaModalPost(string strDados, string cstCompraNfeInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstNfeInd = (int)(from a in db.CstIcmsGerais where a.descricao == cstCompraNfeInd select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstdaNfedaIndFORN = cstNfeInd; //novo cst


                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //Nfe Ata
        public ActionResult EditCstNfeAtaMassa(string opcao, string ordenacao, string procurarPor, string filtroCorrente, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
            //Auxilia na conversão para fazer a busca pelo codigo de barras
            string cst = filtroCorrente ?? procurarPor;

            //converte em int, caso o valor seja um numero

            bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido
            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo


            ViewBag.FiltroCorrente = procurarPor;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com CST")
            {

                trib1 = trib1.Where(s => s.cstdaNfedeAtaFORn != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.cstdaNfedeAtaFORn.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }


            }
            else
            {
                trib1 = trib1.Where(s => s.cstdaNfedeAtaFORn == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.cstdaNfedeAtaFORn.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view

            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditCstNfeAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais.ToList();
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditCstNfeAtaMassaModalPost(string strDados, string cstCompraNfeAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstNfeAta = (int)(from a in db.CstIcmsGerais where a.descricao == cstCompraNfeAta select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstdaNfedeAtaFORn = cstNfeAta; //novo cst


                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //NFe SN
        [HttpGet]
        public ActionResult EditCstNfeSNMassa(string opcao, string ordenacao, string procurarPor, string filtroCorrente, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
            //Auxilia na conversão para fazer a busca pelo codigo de barras
            string cst = filtroCorrente ?? procurarPor;

            //converte em int, caso o valor seja um numero

            bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido
            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo


            ViewBag.FiltroCorrente = procurarPor;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com CST")
            {

                trib1 = trib1.Where(s => s.CsosntdaNfedoSnFOR != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.CsosntdaNfedoSnFOR.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }


            }
            else
            {
                trib1 = trib1.Where(s => s.CsosntdaNfedoSnFOR == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (cstInt != 0) ? trib1.Where(s => s.CsosntdaNfedoSnFOR.ToString().Contains(cstInt.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view

            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditCstNfeSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais.ToList();
            return View(trib);
        }
        [HttpGet]
        public ActionResult EditCstNfeSNMassaModalPost(string strDados, string cstCompraNfeSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstNfeSN = (int)(from a in db.CstIcmsGerais where a.descricao == cstCompraNfeSN select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.CsosntdaNfedoSnFOR = cstNfeSN; //novo cst


                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //Aliq Entrada Pis
        [HttpGet]
        public ActionResult EditAliEntPisMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

           
            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

          
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) ||(procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqEntPis != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqEntPis.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqEntPis == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqEntPis.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view

            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist



        }

        [HttpGet]
        public ActionResult EditAliEntPisMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliEntPisMassaModalPost(string strDados, string aliqEntPis)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            aliqEntPis = aliqEntPis.Replace(".", ",");
           
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqEntPis = (aliqEntPis != "") ? trib.aliqEntPis = decimal.Parse(aliqEntPis) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoAliPisCofins", "Tributacao");
        }

        //Aliq Saida Pis
        [HttpGet]
        public ActionResult EditAliSaiPisMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqSaidaPis != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqSaidaPis.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqSaidaPis == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqSaidaPis.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view

            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliSaiPisMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliSaiPisMassaModalPost(string strDados, string aliqSaidaPis)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqSaidaPis = aliqSaidaPis.Replace(".", ","); 

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqSaidaPis = (aliqSaidaPis != "") ? trib.aliqSaidaPis = decimal.Parse(aliqSaidaPis) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoAliPisCofins", "Tributacao");
        }

        //Aliq entrada cofins
        [HttpGet]
        public ActionResult EditAliEntCofinsMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqEntCofins != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqEntCofins.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqEntCofins == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqEntCofins.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliEntCofinsMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliEntCofinsMassaModalPost(string strDados, string aliqEntCofins)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqEntCofins = aliqEntCofins.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqEntCofins = (aliqEntCofins != "") ? trib.aliqEntCofins = decimal.Parse(aliqEntCofins) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoAliPisCofins", "Tributacao");
        }

        //Aliq saida cofins
        [HttpGet]
        public ActionResult EditAliSaiCofinsMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqSaidaCofins != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqSaidaCofins.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqSaidaCofins == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqSaidaCofins.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliSaiCofinsMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliSaiCofinsMassaModalPost(string strDados, string aliqSaiCofins)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqSaiCofins = aliqSaiCofins.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqSaidaCofins = (aliqSaiCofins != "") ? trib.aliqSaidaCofins = decimal.Parse(aliqSaiCofins) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoAliPisCofins", "Tributacao");
        }

        //Red Base Calc Compra de Industria
        [HttpGet]
        public ActionResult EditRedBCIcmsCompIndMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.redBaseCalcIcmsCompraDeInd != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsCompraDeInd.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.redBaseCalcIcmsCompraDeInd == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsCompraDeInd.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsCompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsCompIndMassaModalPost(string strDados, string aliqRedBasCalcIcmsEntInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcIcmsEntInd = aliqRedBasCalcIcmsEntInd.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsCompraDeInd = (aliqRedBasCalcIcmsEntInd != "") ? trib.redBaseCalcIcmsCompraDeInd = decimal.Parse(aliqRedBasCalcIcmsEntInd) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsEntrada", "Tributacao");
        }

        //Red Base Calc ST compra de industria
        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompIndMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.redBaseCalcIcmsSTCompraDeInd != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsSTCompraDeInd.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.redBaseCalcIcmsSTCompraDeInd == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsSTCompraDeInd.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompIndMassaModalPost(string strDados, string aliqRedBasCalcIcmsSTEntInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcIcmsSTEntInd = aliqRedBasCalcIcmsSTEntInd.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsSTCompraDeInd = (aliqRedBasCalcIcmsSTEntInd != "") ? trib.redBaseCalcIcmsSTCompraDeInd = decimal.Parse(aliqRedBasCalcIcmsSTEntInd) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsEntrada", "Tributacao");
        }

        //Red Base de calc icms compra de atacado
        [HttpGet]
        public ActionResult EditRedBCIcmsCompAtaMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.redBaseCalcIcmsCompraDeAta != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsCompraDeAta.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.redBaseCalcIcmsCompraDeAta == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsCompraDeAta.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsCompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsCompAtaMassaModalPost(string strDados, string aliqRedBasCalcIcmsEntAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcIcmsEntAta = aliqRedBasCalcIcmsEntAta.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsCompraDeAta = (aliqRedBasCalcIcmsEntAta != "") ? trib.redBaseCalcIcmsCompraDeAta = decimal.Parse(aliqRedBasCalcIcmsEntAta) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsEntrada", "Tributacao");
        }

        //Red Base de Calc Icms ST compra de atacado
        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompAtaMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.redBaseCalcIcmsSTCompraDeAta != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsSTCompraDeAta.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.redBaseCalcIcmsSTCompraDeAta == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsSTCompraDeAta.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompAtaMassaModalPost(string strDados, string aliqRedBasCalcIcmsSTEntAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcIcmsSTEntAta = aliqRedBasCalcIcmsSTEntAta.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsSTCompraDeAta = (aliqRedBasCalcIcmsSTEntAta != "") ? trib.redBaseCalcIcmsSTCompraDeAta = decimal.Parse(aliqRedBasCalcIcmsSTEntAta) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsEntrada", "Tributacao");
        }

        //Red Base calc icms compra de Simples Nacional
        [HttpGet]
        public ActionResult EditRedBCIcmsCompSNMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.redBaseCalcIcmsCompradeSimpNacional != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsCompradeSimpNacional.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.redBaseCalcIcmsCompradeSimpNacional == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsCompradeSimpNacional.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsCompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsCompSNMassaModalPost(string strDados, string aliqRedBasCalcIcmsEntSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcIcmsEntSN = aliqRedBasCalcIcmsEntSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsCompradeSimpNacional = (aliqRedBasCalcIcmsEntSN != "") ? trib.redBaseCalcIcmsCompradeSimpNacional = decimal.Parse(aliqRedBasCalcIcmsEntSN) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsEntrada", "Tributacao");
        }

        //Red Base calc icms ST Compra de Simples Nacional
        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompSNMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.redBaseCalcIcmsSTCompradeSimpNacional != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsSTCompradeSimpNacional.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.redBaseCalcIcmsSTCompradeSimpNacional == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsSTCompradeSimpNacional.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompSNMassaModalPost(string strDados, string aliqRedBasCalcIcmsSTEntSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcIcmsSTEntSN = aliqRedBasCalcIcmsSTEntSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsSTCompradeSimpNacional = (aliqRedBasCalcIcmsSTEntSN != "") ? trib.redBaseCalcIcmsSTCompradeSimpNacional = decimal.Parse(aliqRedBasCalcIcmsSTEntSN) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsEntrada", "Tributacao");
        }

        //Red base calc venda varejo consumidor final
        [HttpGet]
        public ActionResult EditRedBCIcmsVendaVarCFMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.redBaseCalcIcmsVendaVarejoConsFinal != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsVendaVarejoConsFinal.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.redBaseCalcIcmsVendaVarejoConsFinal == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsVendaVarejoConsFinal.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsVendaVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        
        [HttpGet]
        public ActionResult EditRedBCIcmsVendaVarCFMassaModalPost(string strDados, string aliqRedBasCalcVendVarCF)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcVendVarCF = aliqRedBasCalcVendVarCF.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsVendaVarejoConsFinal = (aliqRedBasCalcVendVarCF != "") ? trib.redBaseCalcIcmsVendaVarejoConsFinal = decimal.Parse(aliqRedBasCalcVendVarCF) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }

        //Red bas calc icms ST venda vare consumidor final
        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaVarCFMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.redBaseCalcIcmsSTVendaVarejoConsFinal != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsSTVendaVarejoConsFinal.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.redBaseCalcIcmsSTVendaVarejoConsFinal == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsSTVendaVarejoConsFinal.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaVarCFMassaModalPost(string strDados, string aliqRedBasCalcSTVendVarCF)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcSTVendVarCF = aliqRedBasCalcSTVendVarCF.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsSTVendaVarejoConsFinal = (aliqRedBasCalcSTVendVarCF != "") ? trib.redBaseCalcIcmsSTVendaVarejoConsFinal = decimal.Parse(aliqRedBasCalcSTVendVarCF) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }

        //Red bas calc icms venda vare para contribuinte
        [HttpGet]
        public ActionResult EditRedBCIcmsVendaVarContMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.redBaseCalcVendaVarejoCont != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcVendaVarejoCont.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.redBaseCalcVendaVarejoCont == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcVendaVarejoCont.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsVendaVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsVendaVarContMassaModalPost(string strDados, string aliqRedBasCalcVendaVarCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcVendaVarCont = aliqRedBasCalcVendaVarCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcVendaVarejoCont = (aliqRedBasCalcVendaVarCont != "") ? trib.redBaseCalcVendaVarejoCont = decimal.Parse(aliqRedBasCalcVendaVarCont) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }


        //Red Bas Calc ICMS st Venda varejo contribuinte
        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaVarContMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.RedBaseCalcSTVendaVarejo_Cont != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.RedBaseCalcSTVendaVarejo_Cont.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.RedBaseCalcSTVendaVarejo_Cont == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.RedBaseCalcSTVendaVarejo_Cont.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaVarContMassaModalPost(string strDados, string aliqRedBasCalcSTVendaVarCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcSTVendaVarCont = aliqRedBasCalcSTVendaVarCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.RedBaseCalcSTVendaVarejo_Cont = (aliqRedBasCalcSTVendaVarCont != "") ? trib.RedBaseCalcSTVendaVarejo_Cont = decimal.Parse(aliqRedBasCalcSTVendaVarCont) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }

        //Red Base Calc Venda Ata Contribuinte
        [HttpGet]
        public ActionResult EditRedBCIcmsVendaAtaContMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.redBaseCalcIcmsVendaAtaCont != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsVendaAtaCont.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.redBaseCalcIcmsVendaAtaCont == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsVendaAtaCont.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsVendaAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsVendaAtaContMassaModalPost(string strDados, string aliqRedBasCalcVendaAtaCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcVendaAtaCont = aliqRedBasCalcVendaAtaCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsVendaAtaCont = (aliqRedBasCalcVendaAtaCont != "") ? trib.redBaseCalcIcmsVendaAtaCont = decimal.Parse(aliqRedBasCalcVendaAtaCont) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }


        //Red Base Calc ST Venda ata contribuinte
        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaAtaContMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.redBaseCalcIcmsSTVendaAtaCont != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsSTVendaAtaCont.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.redBaseCalcIcmsSTVendaAtaCont == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsSTVendaAtaCont.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaAtaContMassaModalPost(string strDados, string aliqRedBasCalcSTVendaAtaCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcSTVendaAtaCont = aliqRedBasCalcSTVendaAtaCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsSTVendaAtaCont = (aliqRedBasCalcSTVendaAtaCont != "") ? trib.redBaseCalcIcmsSTVendaAtaCont = decimal.Parse(aliqRedBasCalcSTVendaAtaCont) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }

        //Red Base Calc ST Venda Ata Simples nacional
        [HttpGet]
        public ActionResult EditRedBCIcmsVendaAtaSNMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.redBaseCalcIcmsVendaAtaSimpNacional != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsVendaAtaSimpNacional.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.redBaseCalcIcmsVendaAtaSimpNacional == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.redBaseCalcIcmsVendaAtaSimpNacional.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsVendaAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsVendaAtaSNMassaModalPost(string strDados, string aliqRedBasCalcVendaAtaSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcVendaAtaSN = aliqRedBasCalcVendaAtaSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsVendaAtaSimpNacional = (aliqRedBasCalcVendaAtaSN != "") ? trib.redBaseCalcIcmsVendaAtaSimpNacional = decimal.Parse(aliqRedBasCalcVendaAtaSN) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }



        //Edição em massa: Aliq Icms Compra de indutria - atualizado em 25112021
        [HttpGet]
        public ActionResult EditAliqIcmsCompIndMassa(string opcao, string param,  string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor, 
            string procurarPorAliq, string procuraNCM, string procuraCEST,  string filtroCorrente, string filtroCorrenteAliq, string filtroCorrenteNCM,
            string filtroCorrenteCEST, string filtroNulo,  int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //variavel auxiliar
            string resultado = param;

            
            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;


            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;


            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //parametro de ordenação da tabela e ordem
            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente




            /*Variavel do tipo demp data para guardar a opcao: tempo de vida maior com tempdata*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; // se a opçcao for diferente de nula a tempdata recebe seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null)? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            
            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            //Criar o temp data da lista
            VerificaTempData();

            //var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nennum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALIQUOTA
                    //switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_COMP_DE_IND != null).ToList();
                            break;
                        case "2": //sem aliquota
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_COMP_DE_IND == null).ToList();
                            break;
                    }
                    break;
                case "Sem aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nennum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALIQUOTA
                    //switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_COMP_DE_IND != null).ToList();
                            break;
                        case "2": //sem aliquota
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_COMP_DE_IND == null).ToList();
                            break;
                    }
                    break;

            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);
           
            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_COMP_DE_IND.ToString() == procurarPorAliq).ToList();
               

            }


            switch (ordenacao)
            {
                case "Produto_desc":
                    this.tribMTX = this.tribMTX.OrderByDescending(s => s.DESCRICAO_PRODUTO).ToList();
                    break;

                default:
                    this.tribMTX = this.tribMTX.OrderBy(s => s.ID).ToList();
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsCompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }
        [HttpGet]
        public ActionResult EditAliqIcmsCompIndMassaModalPost(string strDados, string aliqIcmsCompraInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsCompraInd = aliqIcmsCompraInd.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;


            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsCompDeInd = (aliqIcmsCompraInd != "") ? trib.aliqIcmsCompDeInd = decimal.Parse(aliqIcmsCompraInd) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsCompIndMassa", new { param = resultado, qtdSalvos = regSalvos });


           
        }





        //Edição em massa: Aliq Icms St Compra de Industria
        [HttpGet]
        public ActionResult EditAliqIcmsSTCompIndMassa(string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procurarPorAliq, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteAliq, string filtroCorrenteNCM,
            string filtroCorrenteCEST, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //variavel auxiliar
            string resultado = param;

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;


            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //ViewBag com a opcao vinda do grafico
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro) 
                    {
                        case "1":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_ST_COMP_DE_IND != null).ToList();
                            break;
                        case "2":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_ST_COMP_DE_IND == null).ToList();
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                        switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_ST_COMP_DE_IND != null).ToList();
                            break;
                        case "2":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_ST_COMP_DE_IND == null).ToList();
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_ST_COMP_DE_IND.ToString() == procurarPorAliq).ToList();


            }


            switch (ordenacao)
            {
                case "Produto_desc":
                    this.tribMTX = this.tribMTX.OrderByDescending(s => s.DESCRICAO_PRODUTO).ToList();
                    break;

                default:
                    this.tribMTX = this.tribMTX.OrderBy(s => s.ID).ToList();
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTCompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);

        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTCompIndMassaModalPost(string strDados, string aliqIcmsSTCompraInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTCompraInd = aliqIcmsSTCompraInd.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsSTCompDeInd = (aliqIcmsSTCompraInd != "") ? trib.aliqIcmsSTCompDeInd = decimal.Parse(aliqIcmsSTCompraInd) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsSTCompIndMassa", new { param = resultado, qtdSalvos = regSalvos });



           
        }



        //Edit Aliq Icms compra de atacado
        [HttpGet]
        public ActionResult EditAliqIcmsCompAtaMassa(string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procurarPorAliq, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteAliq, string filtroCorrenteNCM,
            string filtroCorrenteCEST, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //variavel auxiliar
            string resultado = param;

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "2":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA == null).ToList();
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "2":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA == null).ToList();
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA.ToString() == procurarPorAliq).ToList();

            }


            switch (ordenacao)
            {
                case "Produto_desc":
                    this.tribMTX = this.tribMTX.OrderByDescending(s => s.DESCRICAO_PRODUTO).ToList();
                    break;

                default:
                    this.tribMTX = this.tribMTX.OrderBy(s => s.ID).ToList();
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";


            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsCompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsCompAtaMassaModalPost(string strDados, string aliqIcmsCompraAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsCompraAta = aliqIcmsCompraAta.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;


            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsCompradeAta = (aliqIcmsCompraAta != "") ? trib.aliqIcmsCompradeAta = decimal.Parse(aliqIcmsCompraAta) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsCompAtaMassa", new { param = resultado, qtdSalvos = regSalvos });


        }




        //Edit Aliq icms st compra de atacado
        [HttpGet]
        public ActionResult EditAliqIcmsSTCompAtaMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsSTCompraDeAta != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsSTCompraDeAta.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsSTCompraDeAta == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsSTCompraDeAta.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTCompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTCompAtaMassaModalPost(string strDados, string aliqIcmsSTCompraAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTCompraAta = aliqIcmsSTCompraAta.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsSTCompraDeAta = (aliqIcmsSTCompraAta != "") ? trib.aliqIcmsSTCompraDeAta = decimal.Parse(aliqIcmsSTCompraAta) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsEntrada", "Tributacao");
        }

        //Edit aliq icms compra de simples nacional
        [HttpGet]
        public ActionResult EditAliqIcmsCompSNMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsCompradeSimpNacional != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsCompradeSimpNacional.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsCompradeSimpNacional == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsCompradeSimpNacional.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }
        [HttpGet]
        public ActionResult EditAliqIcmsCompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsCompSNMassaModalPost(string strDados, string aliqIcmsCompraSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsCompraSN = aliqIcmsCompraSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsCompradeSimpNacional = (aliqIcmsCompraSN != "") ? trib.aliqIcmsCompradeSimpNacional = decimal.Parse(aliqIcmsCompraSN) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsEntrada", "Tributacao");
        }

        //Edit aliq icms st compra de simples nacional
        [HttpGet]
        public ActionResult EditAliqIcmsSTCompSNMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsSTCompradeSimpNacional != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsSTCompradeSimpNacional.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsSTCompradeSimpNacional == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsSTCompradeSimpNacional.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]          
        public ActionResult EditAliqIcmsSTCompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTCompSNMassaModalPost(string strDados, string aliqIcmsSTCompraSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTCompraSN = aliqIcmsSTCompraSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsSTCompradeSimpNacional = (aliqIcmsSTCompraSN != "") ? trib.aliqIcmsSTCompradeSimpNacional = decimal.Parse(aliqIcmsSTCompraSN) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsEntrada", "Tributacao");
        }

        //Edit aliq Icms Nfe Ind
        [HttpGet]
        public ActionResult EditAliqIcmsNfeIndMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsNFE != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsNFE.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsNFE == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsNFE.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsNfeIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsNfeIndMassaModalPost(string strDados, string aliqIcmsNfeCompraInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsNfeCompraInd = aliqIcmsNfeCompraInd.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsNFE = (aliqIcmsNfeCompraInd != "") ? trib.aliqIcmsNFE = decimal.Parse(aliqIcmsNfeCompraInd) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsEntrada", "Tributacao");
        }

        //Edit aliq icms nfe Ata
        [HttpGet]
        public ActionResult EditAliqIcmsNfeAtaMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsNfeAta != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsNfeAta.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsNfeAta == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsNfeAta.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsNfeAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsNfeAtaMassaModalPost(string strDados, string aliqIcmsNfeCompraAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsNfeCompraAta = aliqIcmsNfeCompraAta.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsNfeAta = (aliqIcmsNfeCompraAta != "") ? trib.aliqIcmsNfeAta = decimal.Parse(aliqIcmsNfeCompraAta) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsEntrada", "Tributacao");
        }
        //Edit aliq icms nfe SN
        [HttpGet]
        public ActionResult EditAliqIcmsNfeSNMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;



            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsNfeSN != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsNfeSN.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsNfeSN == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsNfeSN.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsNfeSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsNfeSNMassaModalPost(string strDados, string aliqIcmsNfeCompraSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsNfeCompraSN = aliqIcmsNfeCompraSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsNfeSN = (aliqIcmsNfeCompraSN != "") ? trib.aliqIcmsNfeSN = decimal.Parse(aliqIcmsNfeCompraSN) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsEntrada", "Tributacao");
        }


        //Edit aliq Icms Venda Varejo consumidor final
        [HttpGet]
        public ActionResult EditAliqIcmsVenVarCFMassa(string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procurarPorAliq, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteAliq, string filtroCorrenteNCM,
            string filtroCorrenteCEST, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //variavel auxiliar
            string resultado = param;

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;



            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "2":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "2":
                            this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                    }
                    break;
            }


            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                this.tribMTX = this.tribMTX.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL.ToString() == procurarPorAliq).ToList();

            }


            switch (ordenacao)
            {
                case "Produto_desc":
                    this.tribMTX = this.tribMTX.OrderByDescending(s => s.DESCRICAO_PRODUTO).ToList();
                    break;

                default:
                    this.tribMTX = this.tribMTX.OrderBy(s => s.ID).ToList();
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";


            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsVenVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsVenVarCFMassaModalPost(string strDados, string aliqIcmsVenVarCF)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsVenVarCF = aliqIcmsVenVarCF.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsVendaVarejoConsFinal = (aliqIcmsVenVarCF != "") ? trib.aliqIcmsVendaVarejoConsFinal = decimal.Parse(aliqIcmsVenVarCF) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsSaida", "Tributacao");
        }

        //Edit Aliq ICMs ST Venda Varejo Consumidor finnal
        [HttpGet]
        public ActionResult EditAliqIcmsSTVenVarCFMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;


            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsSTVendaVarejoConsFinal != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsSTVendaVarejoConsFinal.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsSTVendaVarejoConsFinal == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsSTVendaVarejoConsFinal.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTVenVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTVenVarCFMassaModalPost(string strDados, string aliqIcmsSTVenVarCF)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTVenVarCF = aliqIcmsSTVenVarCF.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsSTVendaVarejoConsFinal = (aliqIcmsSTVenVarCF != "") ? trib.aliqIcmsVendaVarejoConsFinal = decimal.Parse(aliqIcmsSTVenVarCF) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsSaida", "Tributacao");
        }



        //daqui alteracao de muitas actions

        //Edit Aliq ICMs Venda Varejo para contribuinte
        [HttpGet]
        public ActionResult EditAliqIcmsVenVarContMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;


            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsVendaVarejoCont != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsVendaVarejoCont.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsVendaVarejoCont == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsVendaVarejoCont.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsVenVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsVenVarContMassaModalPost(string strDados, string aliqIcmsVenVarCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsVenVarCont = aliqIcmsVenVarCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsVendaVarejoCont = (aliqIcmsVenVarCont != "") ? trib.aliqIcmsVendaVarejoCont = decimal.Parse(aliqIcmsVenVarCont) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsSaida", "Tributacao");
        }

       
        //Edit Aliq ICMs STVenda Varejo para contribuinte
        [HttpGet]
        public ActionResult EditAliqIcmsSTVenVarContMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;


            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsSTVendaVarejo_Cont != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsSTVendaVarejo_Cont.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsSTVendaVarejo_Cont == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsSTVendaVarejo_Cont.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTVenVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTVenVarContMassaModalPost(string strDados, string aliqIcmsSTVenVarCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTVenVarCont = aliqIcmsSTVenVarCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsSTVendaVarejo_Cont = (aliqIcmsSTVenVarCont != "") ? trib.aliqIcmsSTVendaVarejo_Cont = decimal.Parse(aliqIcmsSTVenVarCont) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsSaida", "Tributacao");
        }
        


        //Edit Aliq ICMs venda atacado para contribuinte
        [HttpGet]
        public ActionResult EditAliqIcmsVenAtaContMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;


            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsVendaAtaCont != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsVendaAtaCont.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsVendaAtaCont == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsVendaAtaCont.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }


        [HttpGet]
        public ActionResult EditAliqIcmsVenAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }
        
        [HttpGet]
        public ActionResult EditAliqIcmsVenAtaContMassaModalPost(string strDados, string aliqIcmsVendAtaCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsVendAtaCont = aliqIcmsVendAtaCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsVendaAtaCont = (aliqIcmsVendAtaCont != "") ? trib.aliqIcmsVendaAtaCont = decimal.Parse(aliqIcmsVendAtaCont) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsSaida", "Tributacao");
        }
       

        //Edit Aliq ICMs ST venda atacado para contribuinte
        [HttpGet]
        public ActionResult EditAliqIcmsSTVenAtaContMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;


            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsSTVendaAtaCont != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsSTVendaAtaCont.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsSTVendaAtaCont == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsSTVendaAtaCont.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTVenAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }


        
        [HttpGet]
        public ActionResult EditAliqIcmsSTVenAtaContMassaModalPost(string strDados, string aliqIcmsSTVendAtaCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTVendAtaCont = aliqIcmsSTVendAtaCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsSTVendaAtaCont = (aliqIcmsSTVendAtaCont != "") ? trib.aliqIcmsSTVendaAtaCont = decimal.Parse(aliqIcmsSTVendAtaCont) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsSaida", "Tributacao");
        }
       
       

        //Edit Aliq ICMs  venda atacado para SN
        [HttpGet]
        public ActionResult EditAliqIcmsVenAtaSNMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;


            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsVendaAtaSimpNacional != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsVendaAtaSimpNacional.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsVendaAtaSimpNacional == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsVendaAtaSimpNacional.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }


        [HttpGet]
        public ActionResult EditAliqIcmsVenAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        
        [HttpGet]
        public ActionResult EditAliqIcmsVenAtaSNMassaModalPost(string strDados, string aliqIcmsVendAtaSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsVendAtaSN = aliqIcmsVendAtaSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsVendaAtaSimpNacional = (aliqIcmsVendAtaSN != "") ? trib.aliqIcmsVendaAtaSimpNacional = decimal.Parse(aliqIcmsVendAtaSN) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsSaida", "Tributacao");
        }
        

        //Edit Aliq ICMs ST venda atacado para SN
        [HttpGet]
        public ActionResult EditAliqIcmsSTVenAtaSNMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;


            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorAliq;

            /*PAra tipar */
            var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            if (opcao == "Com aliquota")
            {

                trib1 = trib1.Where(s => s.aliqIcmsSTVendaAtaSimpNacional != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsSTVendaAtaSimpNacional.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsSTVendaAtaSimpNacional == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsSTVendaAtaSimpNacional.ToString().Contains(procurarPorAliq));

                }

            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    trib1 = trib1.OrderByDescending(s => s.produtos.descricao);
                    break;

                default:
                    trib1 = trib1.OrderBy(s => s.id);
                    break;


            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(trib1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTVenAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }


        [HttpGet]
        public ActionResult EditAliqIcmsSTVenAtaSNMassaModalPost(string strDados, string aliqIcmsSTVendAtaSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTVendAtaSN = aliqIcmsSTVendAtaSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqIcmsSTVendaAtaSimpNacional = (aliqIcmsSTVendAtaSN != "") ? trib.aliqIcmsSTVendaAtaSimpNacional = decimal.Parse(aliqIcmsSTVendAtaSN) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoIcmsSaida", "Tributacao");
        }



        //actions auxiliares // ponto de ajuste: busca por aliquota
        private List<TributacaoGeralView> ProcurarPor(long? codBarrasL, string procurarPor,  string procuraCEST, string procuraNCM, List<TributacaoGeralView> tribMTX)
        {


            if (!String.IsNullOrEmpty(procurarPor))
            {
                tribMTX = (codBarrasL != 0) ? (tribMTX.Where(s => s.COD_BARRAS_PRODUTO.ToString().Contains(codBarrasL.ToString()))).ToList() : tribMTX = (tribMTX.Where(s => s.DESCRICAO_PRODUTO.ToString().ToUpper().Contains(procurarPor.ToUpper()))).ToList();
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                tribMTX = tribMTX.Where(s => s.CEST_PRODUTO == procuraCEST).ToList();
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                tribMTX = tribMTX.Where(s => s.NCM_PRODUTO == procuraNCM).ToList();

            }
           
            return tribMTX;
        }

        //retorno vazio para verificar a tempdata

        public EmptyResult VerificaTempData()
        {
            /*PAra tipar */
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista seja carregada novaente*/
            if (TempData["tributacaoMTX"] == null)
            {
                //this.tribMTX = (from a in db.Tributacao_GeralView where a.ID.ToString() != null select a).ToList();
                this.tribMTX = db.Tributacao_GeralView.ToList();
                TempData["tributacaoMTX"] = this.tribMTX; //cria a temp data e popula
                TempData.Keep("tributacaoMTX"); //persiste
            }
            else
            {
                this.tribMTX = (List<TributacaoGeralView>)TempData["tributacaoMTX"];//atribui a lista os valores de tempdata
                TempData.Keep("tributacaoMTX"); //persiste
            }

            return new EmptyResult();
        }

    }//fim controller

}