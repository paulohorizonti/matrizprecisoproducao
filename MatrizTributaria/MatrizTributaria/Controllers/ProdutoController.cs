using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class ProdutoController : Controller
    {
        //Objeto context
       readonly MatrizDbContext db;
        List<Produto> prod;
        List<Produto> prodMTX = new List<Produto>();
        List<TributacaoGeralView> tribMTX = new List<TributacaoGeralView>(); //TESTE COM A VIEW DA TRIBUTAÇÃO
        public ProdutoController()
        {
            db = new MatrizDbContext();
        }
        public ActionResult Index(string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procuraNCM, string procuraCEST,  string filtroCorrente,  string procuraCate, string filtroCate, string filtroCorrenteNCM,
            string filtroCorrenteCEST, string filtroNulo, string filtraPor, string filtroFiltraPor, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
           
            procurarPor = (procurarPor != null) ? procurarPor.Trim() : procurarPor;
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
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) ||  (procuraCEST != null) || (procuraNCM != null)  || (procuraCate != null)  ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            
            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
           
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
           
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

          
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //converter o valor da procura por setor ou categoria em inteiro
           
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }

            var produtos = from s in db.Produtos select s;


            if (!String.IsNullOrEmpty(procurarPor))
            {
                produtos = (codBarrasL != 0) ? (produtos.Where(s => s.codBarras.ToString().Contains(codBarrasL.ToString()))) : produtos = (produtos.Where(s => s.descricao.ToUpper().Contains(procurarPor.ToUpper())));

            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                produtos = produtos.Where(s => s.cest == procuraCEST);
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                produtos = produtos.Where(s => s.ncm == procuraNCM);

            }
                       
            //Busca por categoria
            if (!String.IsNullOrEmpty(procuraCate))
            {
                produtos = produtos.Where(s => s.idCategoria.ToString() == procuraCate);


            }
            switch (ordenacao)
            {
                case "Produto_desc":
                    produtos = produtos.OrderByDescending(s => s.descricao);
                    break;
                case "Produto_asc":
                    produtos = produtos.OrderBy(s => s.descricao);
                    break;
                case "Id_desc":
                    produtos = produtos.OrderBy(s => s.Id);
                    break;
                default:
                    produtos = produtos.OrderBy(s => s.descricao);
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
          
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();
           

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(produtos.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist




        }
        // GET: Produto
        //public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page, string linhasNum)
        //{
        //    if (Session["usuario"] == null)
        //    {
        //        return RedirectToAction("../Home/Login");
        //    }

        //    // ViewBag.NumLinhas = linhasNum;
        //    ViewBag.CurrentSort = sortOrder;
        //    ViewBag.ProdutoParam = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";
        //    ViewBag.CatProduto = sortOrder == "CatProd" ? "CatProd_desc" : "CatProd";

        //    if (searchString != null)
        //    {
        //        page = 1;
        //    }
        //    else
        //    {
        //        searchString = currentFilter;
        //    }
        //    ViewBag.CurrentFilter = searchString;

        //    var produtos = from s in db.Produtos select s;
        //    if (!String.IsNullOrEmpty(searchString))
        //    {

        //        produtos = produtos.Where(s => s.descricao.ToString().ToUpper().Contains(searchString.ToUpper()) || s.categoriaProduto.descricao.ToString().ToUpper().Contains(searchString.ToUpper()));


        //    }
        //    switch (sortOrder)
        //    {
        //        case "Produto_desc":
        //            produtos = produtos.OrderByDescending(s => s.Id);
        //            break;
        //        case "CatProd":
        //            produtos = produtos.OrderBy(s => s.idCategoria);
        //            break;
        //        case "CatProd_desc":
        //            produtos = produtos.OrderByDescending(s => s.idCategoria);
        //            break;
        //        default:
        //            produtos = produtos.OrderBy(s => s.Id);
        //            break;
        //    }
        //    int pageSize = 0;

        //    if (String.IsNullOrEmpty(linhasNum))
        //    {
        //        pageSize = 10;
        //    }
        //    else
        //    {

        //        ViewBag.Texto = linhasNum;
        //        pageSize = Int32.Parse(linhasNum);
        //    }


        //    int pageNumber = (page ?? 1);

        //    //var produtos = db.Produtos.ToList();
        //    return View(produtos.ToPagedList(pageNumber, pageSize)); //retorna a view com o numero de paginas e tamanho
        //}
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
            Produto produto = db.Produtos.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            // ViewBag.Categoria = db.Produtos.Find(produto.idCategoria).descricao; //categoria
            ViewBag.DataCad = produto.dataCad;
            ViewBag.DataAlt = produto.dataAlt;
            return View(produto);
        }

        // GET: Produtos/Delete/5
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Produto produto = db.Produtos.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataCad = produto.dataCad;
            ViewBag.DataAlt = produto.dataAlt;
            return View(produto);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Produto produto = db.Produtos.Find(id);
            db.Produtos.Remove(produto);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //editar Nível
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            if (Session["nivel"].Equals("USUARIO"))
            {
                int par = 2;
                return RedirectToAction("../Erro/Erro", new { param = par });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = db.Produtos.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }

            ViewBag.DataAlt = DateTime.Now;
            ViewBag.Categorias = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();
            //ViewBag.Categorias = db.CategoriaProdutos;
            return View(produto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id, codinterno, codbarras, descricao, cest, ncm, datacad, dataalt, idcategoria, status")] Produto model)
        {
            //Aa aliquotas podem estar com a virgula, na hora de salvar o sistema passa para decimal
            string buscaNCM = model.ncm != null ? model.ncm.Trim() : null; //ternario para remover eventuais espaços
            if(model.ncm != null)
            {
                model.ncm = buscaNCM.Replace(".", ""); //tirar os pontos da string
            }
            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            if (ModelState.IsValid)
            {
                var produto = db.Produtos.Find(model.Id);
                if (produto == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                

                model.dataAlt = DateTime.Now;

                produto.Id = model.Id;
                produto.cest = model.cest;
                produto.codBarras = model.codBarras;
                produto.codInterno = model.codInterno;
                produto.descricao = model.descricao.ToUpper();
                produto.ncm = model.ncm;
                produto.idCategoria = model.idCategoria;
                produto.status = model.status;
                produto.dataAlt = model.dataAlt;

                try{
                    db.SaveChanges();
                    TempData["tributacaoMTX"] = null;
                    regSalvos++;
                    resultado = "Registro Salvo com Sucesso!!";

                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                }
                
                //return RedirectToAction("Index");
            }
            //ViewBag.Categorias = db.CategoriaProdutos;
            ViewBag.Categorias = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();
            //Redirecionar para a tela de graficos
            return RedirectToAction("Index", new { param = resultado, qtdSalvos = regSalvos });
        }



        //Chamando a view para criar o usuario
        public ActionResult Create()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            if (Session["nivel"].Equals("USUARIO"))
            {
                int par = 1;
                return RedirectToAction("../Erro/Erro", new { param = par });
            }
            ViewBag.Categoria = db.CategoriaProdutos;
            ViewBag.DataAlt = DateTime.Now;
            ViewBag.DataCad = DateTime.Now;
            var model = new ProdutoViewModel();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProdutoViewModel model)
        {
            //iformando a data do dia da criação do registro
            model.dataCad = DateTime.Now;
            model.dataAlt = DateTime.Now;
            model.status = 1; //ativando o registro no cadastro
            if (ModelState.IsValid)
            {

                var produto = new Produto()
                {

                    codInterno = model.codInterno,
                    codBarras = model.codBarras,
                    descricao = model.descricao,
                    cest = model.cest,
                    ncm = model.ncm,
                    dataCad = model.dataCad,
                    dataAlt = model.dataAlt,
                    idCategoria = model.idCategoria,
                    status = model.status


            };
              


                db.Produtos.Add(produto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categoria = db.CategoriaProdutos;

            return View(model);
        }

        [HttpGet]
        public ActionResult GraficoAnaliseProdutos()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //chmar action auxiliar para verificar e carregar a tempdata com a lista
            //VerificaTempDataProd();
            VerificaTempData();

         
            ///*Código de Barras*/
            //ViewBag.CodBarras = this.prodMTX.Count(a=>a.codBarras !=0);
            //ViewBag.CodBarrasNull = this.prodMTX.Count(a => a.codBarras == 0);

            ViewBag.CodBarras = this.tribMTX.Count(a => a.COD_BARRAS_PRODUTO != "0");
            ViewBag.CodBarrasNull = this.tribMTX.Count(a => a.COD_BARRAS_PRODUTO == "0");



            /*Cest*/
            //ViewBag.Cest = this.prodMTX.Count(a => a.cest != null);
            //ViewBag.CestNull = this.prodMTX.Count(a => a.cest == null);

            ViewBag.Cest = this.tribMTX.Count(a => a.CEST_PRODUTO != null);
            ViewBag.CestNull = this.tribMTX.Count(a => a.CEST_PRODUTO == null);



            /*Ncm*/
            //ViewBag.Ncm = this.prodMTX.Count(a => a.ncm != null);
            //ViewBag.NcmNull = this.prodMTX.Count(a => a.ncm == null);

            ViewBag.Ncm = this.tribMTX.Count(a => a.NCM_PRODUTO != null);
            ViewBag.NcmNull = this.tribMTX.Count(a => a.NCM_PRODUTO == null);


            return View();

        }


        //EditNCM
        [HttpGet]
        public ActionResult EditMassa(string opcao, string param, string ordenacao, string qtdSalvos, string qtdNSalvos, string procuraNCM, string procuraCEST,
            string procurarPor, string filtroCorrente, string procuraSetor, string filtroSetor, string procuraCate, string filtroCate, string filtroCorrenteNCM,
            string filtroCorrenteCEST, int? page, int? numeroLinhas, string filtroNulo, string auditadosNCM, string filtraPor, string filtroFiltraPor,
            string filtroCorrenteAudNCM) 
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
            auditadosNCM = (auditadosNCM != null) ? auditadosNCM : "2";

            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if(filtraPor != "Setor")
            {
               
                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

           
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


          

            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) ||(procuraCate != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;

            auditadosNCM = (auditadosNCM == null) ? filtroCorrenteAudNCM : auditadosNCM; //todos os que não foram auditados

            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;

            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;
            ViewBag.FiltroCorrenteAuditado = auditadosNCM; 
            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            switch (auditadosNCM)
            {
                case "0": //SOMENTE OS NÃO AUDITADOS
                    this.tribMTX = this.tribMTX.Where(s => s.AUDITADO_POR_NCM == 0).ToList();
                    break;
                case "1": //SOMENTE OS AUDITADOS
                    this.tribMTX = this.tribMTX.Where(s => s.AUDITADO_POR_NCM == 1).ToList();
                    break;
                case "2": //TODOS
                    this.tribMTX = this.tribMTX.Where(s => s.ID !=null).ToList();
                    break;
            }


            switch (opcao)
            {
                case "Com NCM":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //COM NCM
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.tribMTX = this.tribMTX.Where(s => s.NCM_PRODUTO != null).ToList();
                            break;
                        case "2":
                            this.tribMTX = this.tribMTX.Where(s => s.NCM_PRODUTO == null).ToList();
                            break;
                    }
                    break;
                case "Sem NCM":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //SEM NCM
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.tribMTX = this.tribMTX.Where(s => s.NCM_PRODUTO != null).ToList();
                            break;
                        case "2":
                            this.tribMTX = this.tribMTX.Where(s => s.NCM_PRODUTO == null).ToList();
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);

            //verificar isso - setor categoria
            //Busca por setor
            if (!String.IsNullOrEmpty(procuraSetor))
            {
                this.tribMTX = this.tribMTX.Where(s => s.ID_SETOR.ToString() == procuraSetor).ToList();
              

            }
            //Busca por categoria
            if (!String.IsNullOrEmpty(procuraCate))
            {
                this.tribMTX = this.tribMTX.Where(s => s.ID_CATEGORIA.ToString() == procuraCate).ToList();


            }
            switch (ordenacao)
            {
                case "Produto_desc":
                    this.tribMTX = this.tribMTX.OrderByDescending(s => s.DESCRICAO_PRODUTO).ToList();
                    break;
                case "Produto_asc":
                    this.tribMTX = this.tribMTX.OrderBy(s => s.DESCRICAO_PRODUTO).ToList();
                    break;
                case "Id_desc":
                    this.tribMTX = this.tribMTX.OrderBy(s => s.ID).ToList();
                    break;
                default:
                    this.tribMTX = this.tribMTX.OrderBy(s => s.DESCRICAO_PRODUTO).ToList();
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
            ViewBag.RegNSalvos = (qtdNSalvos != null) ? qtdNSalvos : "";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().ToList();
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().ToList();





            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist


        }
        [HttpGet]
        public ActionResult EditMassaModal(string array)
        {

            string[] dadosDoCadastro = array.Split(',');

            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento

            prod = new List<Produto>();

            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                prod.Add(db.Produtos.Find(aux));


            }
            ViewBag.Produtos = prod;

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


            return View();
        }

        [HttpGet]
        public ActionResult EditMassaNCMModal(string id, string ncm)
        {
            //receber os dados do ncm para alterar
            string ncmAlterar = ncm;
            ViewBag.NCM = ncmAlterar;

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


            return View();
        }

        [HttpGet]
        public ActionResult EditMassaNCMModalPost(string ncm, string fecp, string CodReceita, string CstSaidaPisCofins, string aliqSaidaPis, string aliqSaidaCofins,
            string IdFundamentoLegal, string CstVendaVarejoConsFinal, string alVeVarCF, string alVeVarCFSt, string rBcVeVarCF, string rBcSTVeVarCF, 
            string CstVendaVarejoCont, string alVeVarCont, string alVeVarContSt, string rBcVeVarCont, string rBcSTVeVarCont, string CstVendaAtaCont,
            string aliqIcmsVendaAtaCont, string aliqIcmsSTVendaAtaCont, string redBaseCalcIcmsVendaAtaCont, string redBaseCalcIcmsSTVendaAtaCont, 
            string CstVendaAtaSimpNacional, string alVSN, string alVSNSt, string rBcVSN, string rBcSTVSN, string IdFundLegalSaidaICMS)
        {
            int regSalvos = 0;
            int regNSalvos = 0;
            int regParaSalvar = 0;

            string retorno = "";
            //buscar os cst pela descrição
            int? cstSaidaPisCofins       = (CstSaidaPisCofins       == "") ? null : (int?)(long)(from a in db.CstPisCofinsSaidas where a.descricao == CstSaidaPisCofins select a.codigo).FirstOrDefault();
            int? cstVendaVarejoConsFinal = (CstVendaVarejoConsFinal == "") ? null : (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CstVendaVarejoConsFinal select a.codigo).FirstOrDefault();
            int? cstVendaVarejoCont      = (CstVendaVarejoCont      == "") ? null : (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CstVendaVarejoCont select a.codigo).FirstOrDefault();
            int? cstVendaAtaCont         = (CstVendaAtaCont         == "") ? null : (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CstVendaAtaCont select a.codigo).FirstOrDefault();
            int? cstVendaAtaSimpNacional = (CstVendaAtaSimpNacional == "") ? null : (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CstVendaAtaSimpNacional select a.codigo).FirstOrDefault();
            //natureza da reeita
            int? codNatRec = 0;

            if (CodReceita == "")
            {
                codNatRec = null;
            }
            else
            {
                codNatRec = int.Parse(CodReceita);
            }

            //Fundamento Legal cofins
            int? fundLegalCofins = 0;
            if(IdFundamentoLegal == "")
            {
                fundLegalCofins = null;
            }
            else
            {
                fundLegalCofins = int.Parse(IdFundamentoLegal);
            }

            //fundamento legal icms saida
            int? fundLegalIcmsSaida = 0;
            if(IdFundLegalSaidaICMS == "")
            {
                fundLegalIcmsSaida = null;
            }
            else
            {
                fundLegalIcmsSaida = int.Parse(IdFundLegalSaidaICMS);
            }

            
            
            //Aa aliquotas podem estar com a virgula, na hora de salvar o sistema passa para decimal
            string buscaNCM = ncm != "" ? ncm.Trim() : null; //ternario para remover eventuais espaços

            buscaNCM = buscaNCM.Replace(".", ""); //tirar os pontos da string

            VerificaTempData();

            if (!String.IsNullOrEmpty(buscaNCM))
            {
                tribMTX = tribMTX.Where(s => s.NCM_PRODUTO == buscaNCM).ToList();

            }

            //foreach para atualizar os produtos
            //retira o elemento vazio do array
           //objeto produto
            Tributacao tributaCao = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < tribMTX.Count; i++)
            {
                int? idTrib = (tribMTX[i].ID); //pega o id do registro da tributação a ser alterada
                tributaCao = db.Tributacoes.Find(idTrib); //busca a tributação pelo seu id

                //verifica se os parametros vieram preenchidos, caso true ele atribui ao objeto e conta um registro para salvar
                if(cstSaidaPisCofins != null)
                {
                    tributaCao.cstSaidaPisCofins = cstSaidaPisCofins;
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }
                if(cstVendaVarejoConsFinal != null)
                {
                    tributaCao.cstVendaVarejoConsFinal = cstVendaVarejoConsFinal;
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if (cstVendaVarejoCont !=null) 
                {

                    tributaCao.cstVendaVarejoCont = cstVendaVarejoCont;
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if(cstVendaAtaCont != null)
                {
                    tributaCao.cstVendaAtaCont = cstVendaAtaCont;
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if(cstVendaAtaSimpNacional != null)
                {
                    tributaCao.cstVendaAtaSimpNacional = cstVendaAtaSimpNacional;
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if (fecp != "")
                {
                    tributaCao.fecp = decimal.Parse(fecp);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }
                
                if(codNatRec != null)
                {
                    tributaCao.codNatReceita = codNatRec;
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }
                if(aliqSaidaPis != "")
                {
                    tributaCao.aliqSaidaPis = decimal.Parse(aliqSaidaPis);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if (aliqSaidaCofins != "")
                {
                    tributaCao.aliqSaidaCofins = decimal.Parse(aliqSaidaCofins);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if (fundLegalCofins != null)
                {
                    tributaCao.idFundamentoLegal = (fundLegalCofins);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }
                
                if(alVeVarCF != "")
                {
                    tributaCao.aliqIcmsVendaVarejoConsFinal = decimal.Parse(alVeVarCF);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if(alVeVarCFSt != "")
                {
                    tributaCao.aliqIcmsSTVendaVarejoConsFinal = decimal.Parse(alVeVarCFSt);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }
                if(rBcVeVarCF != "")
                {
                    tributaCao.redBaseCalcIcmsVendaVarejoConsFinal = decimal.Parse(rBcVeVarCF);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }
                if(rBcSTVeVarCF != "")
                {
                    tributaCao.redBaseCalcIcmsSTVendaVarejoConsFinal = decimal.Parse(rBcSTVeVarCF);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if(alVeVarCont != "")
                {
                    tributaCao.aliqIcmsVendaVarejoCont = decimal.Parse(alVeVarCont);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if(alVeVarContSt != "")
                {
                    tributaCao.aliqIcmsSTVendaVarejo_Cont = decimal.Parse(alVeVarContSt);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if(rBcVeVarCont != "")
                {
                    tributaCao.redBaseCalcVendaVarejoCont = decimal.Parse(rBcVeVarCont);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if(rBcSTVeVarCont != "")
                {
                    tributaCao.RedBaseCalcSTVendaVarejo_Cont = decimal.Parse(rBcSTVeVarCont);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if(aliqIcmsVendaAtaCont != "")
                {
                    tributaCao.aliqIcmsVendaAtaCont = decimal.Parse(aliqIcmsVendaAtaCont);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if(aliqIcmsSTVendaAtaCont != "")
                {
                    tributaCao.aliqIcmsSTVendaAtaCont = decimal.Parse(aliqIcmsSTVendaAtaCont);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if (redBaseCalcIcmsVendaAtaCont != "")
                {
                    tributaCao.redBaseCalcIcmsVendaAtaCont = decimal.Parse(redBaseCalcIcmsVendaAtaCont);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }
                if(redBaseCalcIcmsSTVendaAtaCont != "")
                {
                    tributaCao.redBaseCalcIcmsSTVendaAtaCont = decimal.Parse(redBaseCalcIcmsSTVendaAtaCont);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if (alVSN != "")
                {
                    tributaCao.aliqIcmsVendaAtaSimpNacional = decimal.Parse(alVSN);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }
                if(alVSNSt != "")
                {
                    tributaCao.aliqIcmsSTVendaAtaSimpNacional = decimal.Parse(alVSNSt);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }
                if (rBcVSN != "")
                {
                    tributaCao.redBaseCalcIcmsVendaAtaSimpNacional = decimal.Parse(rBcVSN);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }
                if(rBcSTVSN != "")
                {
                    tributaCao.redBaseCalcIcmsSTVendaAtaSimpNacional = decimal.Parse(rBcSTVSN);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }
                if (fundLegalIcmsSaida != null)
                {
                    tributaCao.idFundLegalSaidaICMS = (fundLegalIcmsSaida);
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }

                if(regParaSalvar != 0)
                {
                    tributaCao.auditadoPorNCM = 1; //marca como auditado
                    tributaCao.dataAlt = DateTime.Now; //data da alteração
                    try
                    {
                        db.SaveChanges();
                        regSalvos++;
                        retorno = "Registro Salvo com Sucesso!!";
                    }
                    catch (Exception e)
                    {
                        string exC = e.ToString();
                        regNSalvos++;
                    }
                }
                else
                {
                    retorno = "Nenhum item do  registro alterado";
                    //Redirecionar para registros
                    return RedirectToAction("EditMassa", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                }

                
               
            }
            //zera a tempdata caso tenha salvo algum registros
            if(regSalvos > 0)
            {
                TempData["tributacaoMTX"] = null;//cria a temp data e popula
            }
            
            //Redirecionar para registros
            return RedirectToAction("EditMassa", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
        }
        [HttpGet]
        public ActionResult EditMassaModalPost(string strDados, string ncm)
        {
            //variaveis de auxilio
            int regSalvos = 0;
            int regNSalvos = 0;
            

            string retorno = "";


            //varivael para recebe o novo ncm
            string ncmMudar = "";

            //separar a String em um array
            string[] idProdutos = strDados.Split(',');

            //retira o elemento vazio do array
            idProdutos = idProdutos.Where(item => item != "").ToArray();

            ncmMudar = ncm != "" ? ncm.Trim() : null; //ternario para remover eventuais espaços

            ncmMudar = ncmMudar.Replace(".", ""); //tirar os pontos da string



            //objeto produto
            Produto prod = new Produto();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idProdutos.Length; i++)
            {
                int idProd = Int32.Parse(idProdutos[i]);
                prod = db.Produtos.Find(idProd);
                if (prod !=null)
                {
                    

                    prod.ncm = ncmMudar;
                    prod.dataAlt = DateTime.Now; //data da alteração
                    try
                    {
                        db.SaveChanges();
                        regSalvos++;
                    }
                    catch(Exception e)
                    {
                        string ex = e.ToString();
                        regNSalvos++;
                    }
                   
                }
                
            }
            if(regSalvos > 0)
            {
                retorno = "Registro Salvo com Sucesso!!";
                TempData["tributacaoMTX"] = null;//cria a temp data e popula
            }
            else
            {
                retorno = "Nenhum item do  registro alterado";
                //Redirecionar para registros
                return RedirectToAction("EditMassa", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
            }

            //Redirecionar para registros
            return RedirectToAction("EditMassa", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
        }



        //Editar CEST
        [HttpGet]
        public ActionResult EditCestMassa(string opcao, string ordenacao, string procurarPor, string procurarPorCest, string filtroCorrente, string filtroCorrenteCest, int? page, int? numeroLinhas)
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



            //ViewBag para número de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //Ordenação
            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            if (opcao != null)
            {
                TempData["opcao"] = opcao;
            }
            else
            {
                opcao = TempData["opcao"].ToString();
            }

            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");


            if (procurarPor != null || procurarPorCest != null)
            {
                page = 1;
            }
            else
            {
                procurarPor = filtroCorrente;
                procurarPorCest = filtroCorrenteCest;
            }

            //Atribui o filtro corrente
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorCest;

            /*Para tipar */
            var prod1 = from s in db.Produtos select s; //variavel carregado de produtos

            if (opcao == "Com Cest")
            {
                prod1 = prod1.Where(s => s.cest != null);

                if (!String.IsNullOrEmpty(procurarPor))
                {
                    prod1 = (codBarrasL != 0) ? prod1.Where(s => s.codBarras.ToString().Contains(codBarrasL.ToString())) : prod1 = prod1.Where(s => s.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));

                }
                if (!String.IsNullOrEmpty(procurarPorCest))
                {
                    prod1 = prod1.Where(s => s.cest.Contains(procurarPorCest));

                }

            }
            else
            {
                prod1 = prod1.Where(s => s.cest == null);


                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {
                    prod1 = (codBarrasL != 0) ? prod1.Where(s => s.codBarras.ToString().Contains(codBarrasL.ToString())) : prod1 = prod1.Where(s => s.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));

                }
                if (!String.IsNullOrEmpty(procurarPorCest))
                {
                    prod1 = prod1.Where(s => s.cest.Contains(procurarPorCest));

                }

            }

            //Aplicar ordenação
            switch (ordenacao)
            {
                case "Produto_desc":
                    prod1 = prod1.OrderByDescending(s => s.descricao);
                    break;
                default:
                    prod1 = prod1.OrderBy(s => s.Id);
                    break;


            }
            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);


            int numeroPagina = (page ?? 1);

            return View(prod1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist


        }



        [HttpGet]
        public ActionResult EditCestMassaModal(string array) 
        {
            string[] dadosDoCadastro = array.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            prod = new List<Produto>();

            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                prod.Add(db.Produtos.Find(aux));


            }
            ViewBag.Produtos = prod;

            return View();
        }
               

        [HttpGet]
        public ActionResult EditCestMassaModalPost(string strDados, string cest)
        {

            
            //varivael para recebe o novo cest
            string cestMudar = "";

            //separar a String em um array
            string[] idProdutos = strDados.Split(',');

            //retira o elemento vazio do array
            idProdutos = idProdutos.Where(item => item != "").ToArray();

            cestMudar = cest != "" ? cest.Trim() : null; //ternario para remover eventuais espaços

            //objeto produto
            Produto prod = new Produto();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idProdutos.Length; i++)
            {
                int idProd = Int32.Parse(idProdutos[i]);
                prod = db.Produtos.Find(idProd);
                prod.dataAlt = DateTime.Now; //data da alteração
                prod.cest = cestMudar; //novo ceste
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoAnaliseProdutos", "Produto");


           
        }

        //Alterar Codigo de Barras
        [HttpGet]
        public ActionResult EditCodBarrasMassa(string opcao, string ordenacao, string procurarPor, string filtroCorrente, int? page, int? numeroLinhas)
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

            //ViewBag para persistir o numero de linhas 
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //Ordenação
            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            if (opcao != null)
            {
                TempData["opcao"] = opcao;
            }
            else
            {
                opcao = TempData["opcao"].ToString();
            }

            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");


            if (procurarPor != null)
            {
                page = 1;
            }
            else
            {
                procurarPor = filtroCorrente;
            }

            //Atribui ao filtro corrente
            ViewBag.FiltroCorrente = procurarPor;


            /*Para tipar */
            var prod1 = from s in db.Produtos select s; //variavel carregado de produtos

            //Verifica qual a opção da tabela
            if (opcao == "Com Cod. Barras")
            {
                prod1 = prod1.Where(s => s.codBarras != 0);

                //Verifica se tem busca
                if (!String.IsNullOrEmpty(procurarPor))
                {
                    prod1 = (codBarrasL != 0) ? prod1.Where(s => s.codBarras.ToString().Contains(codBarrasL.ToString())) : prod1.Where(s => s.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));

                }

                //Verifica a Ordenação
                switch (ordenacao)
                {
                    case "Produto_desc":
                        prod1 = prod1.OrderByDescending(s => s.descricao);
                        break;
                    default:
                        prod1 = prod1.OrderBy(s => s.Id);
                        break;


                }

            }
            else
            {
                prod1 = prod1.Where(s => s.codBarras == 0);


                //Verifica se tem busca
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    prod1 = (codBarrasL != 0) ? prod1.Where(s => s.codBarras.ToString().Contains(codBarrasL.ToString())) : prod1 = prod1.Where(s => s.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));

                }
                switch (ordenacao)
                {
                    case "Produto_desc":
                        prod1 = prod1.OrderByDescending(s => s.descricao);
                        break;

                    default:
                        prod1 = prod1.OrderBy(s => s.Id);
                        break;


                }
            }//fim else

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);



            int numeroPagina = (page ?? 1);

            return View(prod1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist

        }


        [HttpGet]
        public ActionResult EditCodBarrasMassaModal(string strDados)
        {
            string[] dadosDoCadastro = strDados.Split(',');

            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento

            prod = new List<Produto>();

            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                prod.Add(db.Produtos.Find(aux));


            }
            ViewBag.Produtos = prod;

            return View();
        }

        [HttpGet]
        public ActionResult EditCodBarrasMassaModalPost(string strDados, string codBarras)
        {
            //varivael para recebe o novo ncm
            string codBarrasMudar = "";
            //separar a String em um array
            string[] idProdutos = strDados.Split(',');
            //retira o elemento vazio do array
            idProdutos = idProdutos.Where(item => item != "").ToArray();
            codBarrasMudar = codBarras != "" ? codBarras.Trim() : "0"; //ternario para remover eventuais espaços

            //objeto produto
            Produto prod = new Produto();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idProdutos.Length; i++)
            {
                int idProd = Int32.Parse(idProdutos[i]);
                prod = db.Produtos.Find(idProd);
                prod.dataAlt = DateTime.Now; //data da alteração
                prod.codBarras = Int64.Parse(codBarrasMudar);
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoAnaliseProdutos", "Produto");

        }


       
        //Actions auxiliar
        public EmptyResult VerificaTempDataProd()
        {
            /*PAra tipar */
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista seja carregada novaente*/
            if (TempData["tributacaoProdMTX"] == null)
            {
                //this.prodMTX = (from a in db.Produtos where a.Id.ToString() != null select a).ToList();
                this.prodMTX = db.Produtos.ToList();
               
                TempData["tributacaoProdMTX"] = this.prodMTX; //cria a temp data e popula
                TempData.Keep("tributacaoProdMTX"); //persiste
            }
            else
            {
                this.prodMTX = (List<Produto>)TempData["tributacaoProdMTX"];//atribui a lista os valores de tempdata
                TempData.Keep("tributacaoProdMTX"); //persiste
            }

            return new EmptyResult();
        }
        public EmptyResult VerificaTempData()
        {
            /*PAra tipar */
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista seja carregada novaente*/
            if (TempData["tributacaoMTX"] == null)
            {
                //this.tribMTX = (from a in db.Tributacao_GeralView where a.ID.ToString() != null select a).ToList();
                this.tribMTX = db.Tributacao_GeralView.AsNoTracking().ToList();
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
        //actions auxiliares // ponto de ajuste: busca por aliquota
        private List<TributacaoGeralView> ProcurarPor(long? codBarrasL, string procurarPor, string procuraCEST, string procuraNCM, List<TributacaoGeralView> tribMTX)
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}