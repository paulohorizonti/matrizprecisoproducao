using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using System;
using System.Collections.Generic;
using PagedList;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web.Mvc;


namespace MatrizTributaria.Controllers
{
    public class UsuarioController : Controller
    {

        //Objego context
       readonly MatrizDbContext db;
        List<Usuario> listUser = new List<Usuario>();

        public UsuarioController()
        {
            db = new MatrizDbContext();
        }
        // GET: Usuario
        //public ActionResult Index()
        //{
        //    if (Session["usuario"] == null)
        //    {
        //        return RedirectToAction("../Home/Login");
        //    }
        //    var usuario = db.Usuarios.ToList();
        //    return View(usuario);
        //}


        public ActionResult Index(string param, string ordenacao, string qtdSalvos, string procurarPor, string procuraEmpresa,
            string filtroCorrente, string filtroEmpresa, int? page, int? numeroLinhas)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            
            //variavel auxiliar
            string resultado = param;

            procurarPor = (filtroCorrente != null) ? filtroCorrente : procurarPor; //procura por nome
            procuraEmpresa = (procuraEmpresa != null) ? procuraEmpresa : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroNome = String.IsNullOrEmpty(ordenacao) ? "Nome_desc" : ""; //Se nao vier nula a ordenacao aplicar por nome decrescente

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procuraEmpresa != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo


            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraEmpresa = (procuraEmpresa == null) ? filtroEmpresa : procuraEmpresa;

            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteEmpresa = procuraEmpresa;

            this.listUser = db.Usuarios.ToList();

            //procura
            if (!String.IsNullOrEmpty(procurarPor))
            {
                this.listUser = listUser.Where(s => s.nome.Contains(procurarPor)).ToList();
            }
            if (!String.IsNullOrEmpty(procuraEmpresa))
            {
                listUser = listUser.Where(s => s.empresa.id.ToString() == procuraEmpresa).ToList();
            }
            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);
            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";

            ViewBag.Empresas = db.Empresas.ToList();
            return View(listUser.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
           
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

            ViewBag.Niveis = db.Niveis;
            ViewBag.Empresas = db.Empresas;
            var model = new UsuarioViewModel();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsuarioViewModel model)
        {
            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;
            var hash = new Hash(SHA512.Create());
            //iformando a data do dia da criação do registro
            model.dataCad = DateTime.Now;
            model.dataAlt = DateTime.Now;
            model.ativo = 1; //ativando o registro no cadastro
            model.primeiro_acesso = 0; //validar esse requisito
            //ativando o registro no cadastro
            if (ModelState.IsValid)
            {
               
                //verificar se existe o usuario no banco
                var users = from s in db.Usuarios select s; //select na tabela
                                                                             //where: Pegar somente o registro do cnpj da empresa do usuario (da sessão)
                users = users.Where(s => s.email.Contains(model.email));

                if (users.Count() > 0)
                {
                    int par = 5;
                    
                    return RedirectToAction("../Erro/Erro", new { param = par });
                }
                var usuario = new Usuario()
                {

                    nome = model.nome,
                    email = model.email,
                    sexo = model.sexo,
                    logradouro = model.logradouro,
                    numero = model.numero,
                    cep = model.cep,
                    //criptografar senha
                    senha = hash.CriptografarSenha(model.senha),

                    ativo = model.ativo,
                    primeiro_acesso = model.primeiro_acesso,
                    dataCad = model.dataCad,
                    dataAlt = model.dataAlt,
                    idNivel = model.idNivel,
                    telefone = model.telefone,
                    cidade = model.cidade,
                    estado = model.estado.ToString(),
                    idEmpresa = model.idEmpresa

            };

                try{
                    db.Usuarios.Add(usuario);
                    db.SaveChanges();
                    regSalvos++;
                    resultado = "Registro Salvo com Sucesso!!";
                    


                }
                catch (Exception e)
                {
                    string ex = e.ToString();
                    regSalvos = 0;
                    resultado = "Não foi possivel salvar o registro!!";
                  
                }

            }
            else
            {
                ViewBag.Niveis = db.Niveis;
                ViewBag.Empresas = db.Empresas;
                return View();
            }
            ViewBag.Niveis = db.Niveis;
            ViewBag.Empresas = db.Empresas;

            return RedirectToAction("Index", new { param = resultado, qtdSalvos = regSalvos });
        }

        //editar Nível
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
            Usuario usuario = db.Usuarios.Find(id);

            /*Verifica se o objeto foi instanciado*/
            if (usuario == null)
            {
                return HttpNotFound();
            }

            /*ViewBags com os dados necessários para preencher as dropbox na view*/
            ViewBag.Niveis = db.Niveis;
            ViewBag.Empresas = db.Empresas;

            /*Retorna a view passando o objeto como parametro*/
            return View(usuario);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nome,email,sexo,logradouro,numero,cep,senha,ativo,dataAlt,idNivel,telefone,cidade,estado,idEmpresa")] Usuario model)
        {
            var hash = new Hash(SHA512.Create());
            var usuario = db.Usuarios.Find(model.id);
            model.dataAlt = DateTime.Now;
            if (ModelState.IsValid)
            {
               
                if (usuario == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                usuario.nome = model.nome;
                usuario.email = model.email;
                usuario.sexo = model.sexo;
                usuario.logradouro = model.logradouro;
                usuario.numero = model.numero;
                usuario.cep = model.cep;

                //criptografar senha
                usuario.senha = hash.CriptografarSenha(model.senha);

                //usuario.senha = model.senha;
                usuario.ativo = model.ativo;
                usuario.nome = model.nome;
                usuario.dataAlt = model.dataAlt;
                usuario.idNivel = model.idNivel;
                usuario.telefone = model.telefone;
                usuario.cidade = model.cidade;
                usuario.estado = model.estado;
                usuario.idEmpresa = model.idEmpresa;


                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Niveis = db.Niveis;
            ViewBag.Empresas = db.Empresas;
            return View(model);
        }


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
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            ViewBag.DataCad = db.Usuarios.Find( id).dataCad;
            ViewBag.DataAlt = db.Usuarios.Find( id).dataAlt;

            return View(usuario);
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
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataCad = db.Usuarios.Find( id).dataCad;
            ViewBag.DataAlt = db.Usuarios.Find( id).dataAlt;
            return View(usuario);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
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