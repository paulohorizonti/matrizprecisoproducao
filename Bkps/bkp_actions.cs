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
