using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using System;
using System.Collections.Generic;
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

        public UsuarioController()
        {
            db = new MatrizDbContext();
        }
        // GET: Usuario
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            var usuario = db.Usuarios.ToList();
            return View(usuario);
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
            var hash = new Hash(SHA512.Create());
            //iformando a data do dia da criação do registro
            model.dataCad = DateTime.Now;
            model.dataAlt = DateTime.Now;
            model.ativo = 1; //ativando o registro no cadastro
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
                    dataCad = model.dataCad,
                    dataAlt = model.dataAlt,
                    idNivel = model.idNivel,
                    telefone = model.telefone,
                    cidade = model.cidade,
                    estado = model.estado.ToString(),
                    idEmpresa = model.idEmpresa

            };
               

                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Niveis = db.Niveis;
            ViewBag.Empresas = db.Empresas;
            return View(model);
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

        

    }
}