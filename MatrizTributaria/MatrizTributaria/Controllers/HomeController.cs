﻿using MatrizTributaria.Areas.Cliente.Models;
using MatrizTributaria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class HomeController : Controller
    {
        readonly MatrizDbContext db;
        /*so um teste*/
        List<AnaliseTributaria> analise = new List<AnaliseTributaria>();
        List<AnaliseTributaria> trib = new List<AnaliseTributaria>();
        List<AnaliseTributaria2> trib2 = new List<AnaliseTributaria2>();
        List<TributacaoGeralView> tribMTX = new List<TributacaoGeralView>();
        List<Produto> prodMTX = new List<Produto>();
        Usuario user;

        Usuario usuario;
        Empresa empresa;

        public HomeController()
        {
            db = new MatrizDbContext();
        }

        public ActionResult Index()
        {
            
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                //verificar
                if(!(Session["cnpjEmp"].ToString() == "30.272.433/0001-67"))
                {
                    return RedirectToRoute("cliente");
                }
            }
            //string cnpj = Session["cnpjEmp"].ToString();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Login()
        {
            try
            {
                //pegar a ultima versão
                var versao = db.Versoes.ToList();
                var item = versao[versao.Count - 1];
                ViewBag.Versao = item.versao;
                ViewBag.Nota = item.nota;
            }
            catch(Exception e)
            {
                var erro = e.ToString();
                int par = 7;
                return RedirectToAction("../Erro/ErroLogin", new { param = par });
            }
           
           
            return View();
        }

        [HttpPost]
        public ActionResult Login([Bind(Include = "email, senha")] Usuario usuario)
        {
            string hashTxtSenha = null;
            var hash = new Hash(SHA512.Create());
            
            try
            {
                this.user = db.Usuarios.Where(x => x.email == usuario.email).FirstOrDefault();
            }
            catch (Exception e)
            {
                var erro = e.ToString();
                int par = 7;
                return RedirectToAction("../Erro/ErroLogin", new { param = par });
            }

            //Usuario user = db.Usuarios.Where(x => x.email == usuario.email).FirstOrDefault();
            //Usuario user = (from u in db.Usuarios where u.email.Equals(usuario.email) select u).FirstOrDefault<Usuario>();
            if (user == null)
            {
                Session["usuario"] = null;
                ViewBag.Message = "Usuário não encontrato!";
                return View();
            }
            else
            {
                hashTxtSenha = hash.CriptografarSenha(usuario.senha);

                if (user.senha.Equals(hashTxtSenha) || user.senha.Equals(usuario.senha))
                {
                  
                    ViewBag.Message = "Bem vindo : " + user.nome;

                    Session["usuario"] = user.nome;
                    Session["cnpjEmp"] = user.empresa.cnpj;
                    Session["idEmpresa"] = user.empresa.id;
                    Session["empresa"] = user.empresa.fantasia;
                    Session["email"] = user.email;
                    Session["id"] = user.id;
                    Session["nivel"] = user.nivel.descricao;

                    string usuarioSessao = Session["usuario"].ToString(); //pega o usuário da sessão
                    string empresaUsuario = Session["cnpjEmp"].ToString();
                    //this.usuario = (from a in db.Usuarios where a.nome == usuarioSessao select a).FirstOrDefault(); //pega o usuario


                    try
                    {
                        this.usuario = db.Usuarios.Where(x => x.nome == usuarioSessao).FirstOrDefault();
                    }
                    catch (Exception e)
                    {
                        var erro = e.ToString();
                        int par = 7;
                        return RedirectToAction("../Erro/ErroLogin", new { param = par });
                    }




                    //this.empresa = (from a in db.Empresas where a.cnpj == empresaUsuario select a).FirstOrDefault(); //empresa

                    try
                    {
                        this.empresa = db.Empresas.Where(x => x.cnpj == empresaUsuario).FirstOrDefault();
                    }
                    catch (Exception e)
                    {
                        var erro = e.ToString();
                        int par = 7;
                        return RedirectToAction("../Erro/ErroLogin", new { param = par });
                    }


                   
                    Session["usuarios"] = usuario;
                    Session["empresas"] = empresa;

                    ///*Montar a temp-data*/
                    ///*Verifica a variavel do tipo temp data ANALISE, caso esteja nula carregar a lista novamente*/
                    //if (TempData["analise"] == null)
                    //{
                    //    //carrega a lista analise usando o cnpj da empresa do usuario
                    //    //this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();


                    //    try
                    //    {
                    //        this.analise = db.Analise_Tributaria.Where(x => x.CNPJ_EMPRESA == empresa.cnpj).ToList();
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        var erro = e.ToString();
                    //        int par = 7;
                    //        return RedirectToAction("../Erro/ErroLogin", new { param = par });
                    //    }


                    //    TempData["analise"] = this.analise; //cria
                    //    TempData.Keep("analise"); //salva
                    //}
                    //else //não estando nula apenas atribui à lista o valor carregado em tempdata
                    //{
                    //    this.analise = (List<AnaliseTributaria>)TempData["analise"];
                    //    TempData.Keep("analise");
                    //}

                    ///*Montar a temp-data*/
                    ///*Verifica a variavel do tipo temp data ANALISE, caso esteja nula carregar a lista novamente*/
                    //if (TempData["tributacaoMTX"] == null)
                    //{
                    //    //carrega a lista analise usando o cnpj da empresa do usuario
                    //    //this.tribMTX = (from a in db.Tributacao_GeralView where a.ID.ToString() != null select a).ToList();
                    //    try
                    //    {
                    //        this.tribMTX = db.Tributacao_GeralView.ToList();
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        var erro = e.ToString();
                    //        int par = 7;
                    //        return RedirectToAction("../Erro/ErroLogin", new { param = par });
                    //    }

                    //    TempData["tributacaoMTX"] = this.tribMTX; //cria
                    //    TempData.Keep("tributacaoMTX"); //salva
                    //}
                    //else //não estando nula apenas atribui à lista o valor carregado em tempdata
                    //{
                    //    this.tribMTX = (List<TributacaoGeralView>)TempData["tributacaoMTX"];//atribui a lista os valores de tempdata
                    //    TempData.Keep("tributacaoMTX");
                    //}

                    /*Temp data do produto*/

                    //if (TempData["tributacaoProdMTX"] == null)
                    //{
                    //    //this.prodMTX = (from a in db.Produtos where a.Id.ToString() != null select a).ToList();
                    //    try
                    //    {
                    //        this.prodMTX = db.Produtos.ToList();
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        var erro = e.ToString();
                    //        int par = 7;
                    //        return RedirectToAction("../Erro/ErroLogin", new { param = par });
                    //    }


                    //    TempData["tributacaoProdMTX"] = this.prodMTX; //cria a temp data e popula
                    //    TempData.Keep("tributacaoProdMTX"); //persiste
                    //}
                    //else
                    //{
                    //    this.prodMTX = (List<Produto>)TempData["tributacaoProdMTX"];//atribui a lista os valores de tempdata
                    //    TempData.Keep("tributacaoProdMTX"); //persiste
                    //}
                    

                }
                else
                {
                    Session["usuario"] = null;
                    ViewBag.Message = "Senha incorreta!";
                    return View();
                }

                
            }

            return RedirectToAction("Index","Home");
        }
        public ActionResult LogOut()
        {
            if (Session["usuario"] != null)
            {
                Session["usuario"] = null;
                Session["empresa"] = null;
                Session["email"] = null;
                TempData["analise"] = null;
                TempData["tributacaoMTX"] = null;
                TempData["tributacaoProdMTX"] = null;
                Session["usuarios"] = null;
                Session["empresas"] = null;
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }


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