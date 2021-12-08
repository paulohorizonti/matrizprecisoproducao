using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class EmpresaController : Controller
    {

        //Objego context
        readonly MatrizDbContext db;

        //Construtor da classe
        public EmpresaController()
        {
            db = new MatrizDbContext();
        }
        // GET: Empresa
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            var empresa = db.Empresas.ToList();
            return View(empresa);
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

            var model = new EmpresaViewModel();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmpresaViewModel model)
        {
            //iformando a data do dia da criação do registro
            model.datacad = DateTime.Now;
            model.dataalt = DateTime.Now;
            model.ativo = 1; //ativando o registro no cadastro
            if (ModelState.IsValid)
            {
                //verificar se existe empresa com o cnpj informado cadastrado no banco
                var empr = from s in db.Empresas select s; //select na tabela
                empr = empr.Where(s => s.cnpj.Contains(model.cnpj));

                if (empr.Count() > 0)
                {
                    int par = 6;

                    return RedirectToAction("../Erro/Erro", new { param = par });
                }
                var empresa = new Empresa() 
                {
                 razacaosocial = model.razacaosocial,
                 fantasia = model.fantasia,
                 cnpj = model.cnpj,
                 logradouro = model.logradouro,
                 numero = model.numero,
                 cep = model.cep,
                 complemento = model.complemento,
                 cidade = model.cidade,
                 estado = model.estado,
                 telefone = model.telefone,
                 email = model.email,
                 ativo = model.ativo,
                 datacad = model.datacad,
                 dataalt = model.dataalt


            };
                

                db.Empresas.Add(empresa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        //detalhes
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
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }

            ViewBag.DataCad = db.Empresas.Find( id).datacad;
            ViewBag.DataAlt = db.Empresas.Find( id).dataalt;

            return View(empresa);

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
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }

            return View(empresa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,razacaosocial, fantasia, cnpj, logradouro, numero, cep, complemento, cidade, estado, telefone, ativo, email")] Empresa model)
        {
            if (ModelState.IsValid)
            {
                var empresa = db.Empresas.Find(model.id);
                if (empresa == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                 empresa.razacaosocial = model.razacaosocial;
                 empresa.fantasia = model.fantasia;
                 empresa.cnpj= model.cnpj;
                 empresa.logradouro = model.logradouro;
                 empresa.numero = model.numero;
                 empresa.cep = model.cep;
                 empresa.complemento = model.complemento;
                 empresa.cidade = model.cidade;
                 empresa.estado = model.estado;
                 empresa.telefone = model.telefone;
                 empresa.ativo = model.ativo;
                 empresa.email = model.email;
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
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
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataCad = db.Empresas.Find( id).datacad;
            ViewBag.DataAlt = db.Empresas.Find( id).dataalt;
            return View(empresa);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empresa empresa = db.Empresas.Find(id);
            db.Empresas.Remove(empresa);
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