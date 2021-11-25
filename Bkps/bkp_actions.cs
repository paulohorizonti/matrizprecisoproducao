 /*Backup action Tabela icms entrada*/
        [HttpGet]
        public ActionResult TabelaIcmsEntrada(string sortOrder, string searchString, string searchString2, string currentFilter, int? page, string LinhasNum)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();
            Usuario usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
            Empresa empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa

            //Paginação 
            ViewBag.OrdemAtual = sortOrder;
            ViewBag.PorProdutoDesc = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";
            ViewBag.PorCatProd = sortOrder == "CatProd" ? "CatProd_desc" : "CatProd";

            if (searchString != null)
            {
                page = 1;
                searchString = searchString.Trim();


            }
            else
            {
                searchString = currentFilter;
            }

            if (searchString2 != null)
            {
                page = 1;
                searchString2 = searchString2.Trim();


            }
            else
            {
                searchString2 = currentFilter;
            }


            
            var trib = from s in db.Analise_Tributaria select s;

            //busca
            if (!String.IsNullOrEmpty(searchString))
            {

                trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_DESCRICAO.Contains(searchString));
                trib = trib.OrderBy(s => s.Id_Produto_INTERNO);

            }
            else
            {
                if (!String.IsNullOrEmpty(searchString2))
                {

                    trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_COD_BARRAS.Contains(searchString2));
                    trib = trib.OrderBy(s => s.Id_Produto_INTERNO);


                }
                else
                {
                    //trib = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == usuario.empresa.cnpj orderby a.PRODUTO_DESCRICAO select a).ToList();
                    trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj);
                    trib = trib.OrderBy(s => s.Id_Produto_INTERNO);

                }


            }


            int pageSize = 0;

            if (String.IsNullOrEmpty(LinhasNum))
            {
                pageSize = 5;
            }
            else
            {

                ViewBag.Texto = LinhasNum;
                pageSize = Int32.Parse(LinhasNum);
            }

            int pageNumber = (page ?? 1);

            return View(trib.ToPagedList(pageNumber, pageSize)); //retorna a view com o numero de paginas e tamanho
        }
		
		//icms saida
		[HttpGet]
		 public ActionResult TabelaIcmsSaida(string sortOrder, string searchString, string searchString2, string currentFilter, int? page, string LinhasNum)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
          
            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();  
            Usuario usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
            Empresa empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa

            //Paginação 
            ViewBag.OrdemAtual = sortOrder;
            ViewBag.PorProdutoDesc = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";
            ViewBag.PorCatProd = sortOrder == "CatProd" ? "CatProd_desc" : "CatProd";


            if (searchString != null)
            {
                page = 1;
                searchString = searchString.Trim();

            }
            else
            {
                searchString = currentFilter;
            }

            if (searchString2 != null)
            {
                page = 1;
                searchString2 = searchString2.Trim();

            }
            else
            {
                searchString2 = currentFilter;
            }

            var trib = from s in db.Analise_Tributaria select s;
            //busca
            if (!String.IsNullOrEmpty(searchString))
            {

                trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_DESCRICAO.Contains(searchString));
                trib = trib.OrderBy(s => s.TE_ID);

            }
            else
            {
                if (!String.IsNullOrEmpty(searchString2))
                {

                    trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_COD_BARRAS.Contains(searchString2));
                    trib = trib.OrderBy(s => s.TE_ID);

                }
                else
                {
                    //trib = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == usuario.empresa.cnpj orderby a.PRODUTO_DESCRICAO select a).ToList();
                    trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj);
                    trib = trib.OrderBy(s => s.TE_ID);

                }


            }




            int pageSize = 0;


            if (String.IsNullOrEmpty(LinhasNum))
            {
                pageSize = 5;
            }
            else
            {

                ViewBag.Texto = LinhasNum;
                pageSize = Int32.Parse(LinhasNum);
            }

            int pageNumber = (page ?? 1);

            return View(trib.ToPagedList(pageNumber, pageSize)); //retorna a view com o numero de paginas e tamanho
        }


==================

        [HttpGet]
        public ActionResult TabelaRedBasCalSaida(string sortOrder, string searchString, string searchString2, string currentFilter, int? page, string LinhasNum)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();
            Usuario usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
            Empresa empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa

            //Paginação 
            ViewBag.OrdemAtual = sortOrder;
            ViewBag.PorProdutoDesc = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";
            ViewBag.PorCatProd = sortOrder == "CatProd" ? "CatProd_desc" : "CatProd";

            if (searchString != null)
            {
                page = 1;
                searchString = searchString.Trim();


            }
            else
            {
                searchString = currentFilter;
            }

            if (searchString2 != null)
            {
                page = 1;
                searchString2 = searchString2.Trim();


            }
            else
            {
                searchString2 = currentFilter;
            }

           
            var trib = from s in db.Analise_Tributaria select s;

            //busca
            if (!String.IsNullOrEmpty(searchString))
            {

                trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_DESCRICAO.Contains(searchString));
                trib = trib.OrderByDescending(s => s.PRODUTO_DESCRICAO);
            }
            else
            {
                if (!String.IsNullOrEmpty(searchString2))
                {

                    trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_COD_BARRAS.Contains(searchString2));
                    trib = trib.OrderByDescending(s => s.PRODUTO_DESCRICAO);

                }
                else
                {
                    //trib = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == usuario.empresa.cnpj orderby a.PRODUTO_DESCRICAO select a).ToList();
                    trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj);
                    trib = trib.OrderBy(s => s.PRODUTO_DESCRICAO);

                }


            }



            int pageSize = 0;

            if (String.IsNullOrEmpty(LinhasNum))
            {
                pageSize = 6;
            }
            else
            {

                ViewBag.Texto = LinhasNum;
                pageSize = Int32.Parse(LinhasNum);
            }

            int pageNumber = (page ?? 1);

            return View(trib.ToPagedList(pageNumber, pageSize)); //retorna a view com o numero de paginas e tamanho
        }
		
		
		========================
		 public ActionResult TabelaRedBasCalEntrada(string sortOrder, string searchString, string searchString2, string currentFilter, int? page, string LinhasNum)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();
            Usuario usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
            Empresa empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa

            //Paginação 
            ViewBag.OrdemAtual = sortOrder;
            ViewBag.PorProdutoDesc = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";
            ViewBag.PorCatProd = sortOrder == "CatProd" ? "CatProd_desc" : "CatProd";


            if (searchString != null)
            {
                page = 1;
                searchString = searchString.Trim();


            }
            else
            {
                searchString = currentFilter;
            }

            if (searchString2 != null)
            {
                page = 1;
                searchString2 = searchString2.Trim();


            }
            else
            {
                searchString2 = currentFilter;
            }


            var trib = from s in db.Analise_Tributaria select s;

            //busca
            if (!String.IsNullOrEmpty(searchString))
            {

                trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_DESCRICAO.Contains(searchString));
                trib = trib.OrderByDescending(s => s.PRODUTO_DESCRICAO);
            }
            else
            {
                if (!String.IsNullOrEmpty(searchString2))
                {

                    trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_COD_BARRAS.Contains(searchString2));
                    trib = trib.OrderByDescending(s => s.PRODUTO_DESCRICAO);

                }
                else
                {
                    //trib = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == usuario.empresa.cnpj orderby a.PRODUTO_DESCRICAO select a).ToList();
                    trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj);
                    trib = trib.OrderBy(s => s.PRODUTO_DESCRICAO);

                }


            }



            int pageSize = 0;

            if (String.IsNullOrEmpty(LinhasNum))
            {
                pageSize = 6;
            }
            else
            {

                ViewBag.Texto = LinhasNum;
                pageSize = Int32.Parse(LinhasNum);
            }

            int pageNumber = (page ?? 1);

            return View(trib.ToPagedList(pageNumber, pageSize)); //retorna a view com o numero de paginas e tamanho
        }
======================
 [HttpGet]
        public ActionResult TabelaPisCofins(string sortOrder, string searchString, string searchString2, string currentFilter, int? page, string LinhasNum)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();
            Usuario usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
            Empresa empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa

            //Paginação 
            ViewBag.OrdemAtual = sortOrder;
            ViewBag.PorProdutoDesc = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";
            ViewBag.PorCatProd = sortOrder == "CatProd" ? "CatProd_desc" : "CatProd";


            if (searchString != null)
            {
                page = 1;
                searchString = searchString.Trim();
                

            }
            else
            {
                searchString = currentFilter;
            }

            if (searchString2 != null)
            {
                page = 1;
                searchString2 = searchString2.Trim();


            }
            else
            {
                searchString2 = currentFilter;
            }


          

            //List<AnaliseTributaria> trib = new List<AnaliseTributaria>();
            var trib = from s in db.Analise_Tributaria select s;
            //busca
            if (!String.IsNullOrEmpty(searchString))
            {

                trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj &&  s.PRODUTO_DESCRICAO.Contains(searchString));
                trib = trib.OrderByDescending(s => s.PRODUTO_DESCRICAO);
                //trib = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == usuario.empresa.cnpj && a.PRODUTO_DESCRICAO.Contains(searchString.ToUpper()) || a.PRODUTO_COD_BARRAS.Contains(searchString) orderby a.PRODUTO_DESCRICAO select a).ToList();

                ViewBag.Quantidade = trib.Count();
            }
            else
            {
                if (!String.IsNullOrEmpty(searchString2))
                {

                    trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_COD_BARRAS.Contains(searchString2));
                    trib = trib.OrderByDescending(s => s.PRODUTO_DESCRICAO);
                    //trib = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == usuario.empresa.cnpj && a.PRODUTO_DESCRICAO.Contains(searchString.ToUpper()) || a.PRODUTO_COD_BARRAS.Contains(searchString) orderby a.PRODUTO_DESCRICAO select a).ToList();

                    ViewBag.Quantidade = trib.Count();

                }
                else {
                    //trib = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == usuario.empresa.cnpj orderby a.PRODUTO_DESCRICAO select a).ToList();
                    trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj);
                    trib = trib.OrderBy(s => s.PRODUTO_DESCRICAO);
                    ViewBag.Quantidade = trib.Count();

                }
                

            }



            int pageSize = 0;

            if (String.IsNullOrEmpty(LinhasNum))
            {
                pageSize = 6;
            }
            else
            {

                ViewBag.Texto = LinhasNum;
                pageSize = Int32.Parse(LinhasNum);
            }

            int pageNumber = (page ?? 1);

            return View(trib.ToPagedList(pageNumber, pageSize)); //retorna a view com o numero de paginas e tamanho

        }
===============================
  [HttpGet]
        public ActionResult EditAliqIcmsCompIndMassa(string opcao, string ordenacao, string procurarPor, string procurarPorAliq, string filtroCorrente, string filtroCorrenteAliq, int? page, int? numeroLinhas)
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

                trib1 = trib1.Where(s => s.aliqIcmsCompDeInd != null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsCompDeInd.ToString().Contains(procurarPorAliq));

                }


            }
            else
            {
                trib1 = trib1.Where(s => s.aliqIcmsCompDeInd == null);

                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {

                    trib1 = (codBarrasL != 0) ? trib1.Where(s => s.produtos.codBarras.ToString().Contains(codBarrasL.ToString())) : trib1 = trib1.Where(s => s.produtos.descricao.ToString().ToUpper().Contains(procurarPor.ToUpper()));


                }
                if (!String.IsNullOrEmpty(procurarPorAliq))
                {
                    trib1 = trib1.Where(s => s.aliqIcmsCompDeInd.ToString().Contains(procurarPorAliq));

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
