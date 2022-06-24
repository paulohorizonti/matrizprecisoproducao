using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MatrizTributaria.Controllers
{
    public class EmpresaController : Controller
    {

        //Objego context
        readonly MatrizDbContext db;
        Empresa emp;

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



        //cadastro superlógica
        //Vai chegar ate aqui passando alguns parametros
        public dynamic CadSuperLogica(int? idplano, int? idcliente, int? identificador)
        {
            //os dados vão chegar pela url
            int? idPlanoa = idplano;
            int? idCliente = idcliente;
            int? iDentificador = identificador;

            if(TempData["msg"] != null)
            {
                ViewBag.Msg = TempData["msg"];
                ViewBag.CodMsg = TempData["codmsg"];
            }
             


            //verificar se os dados foram passados
            if (idPlanoa == null || idCliente == null || iDentificador == null)
            {
                ViewBag.Mensagem = "Dados incompletos, por favor verifique sua requisição e tente novamente!";
                ViewBag.CodMsg = 1;
                return View();
            }
            else
            {
                //validar o cliente na superlogica
                var requisicaoWeb = WebRequest.CreateHttp("https://api.superlogica.net/v2/financeiro/clientes/" + idCliente);

                //passar os tokens
                requisicaoWeb.Method = "GET";
                requisicaoWeb.Headers["app_token"] = "6c7a8c42-3291-39d5-bc1c-1e0ce8e1beef";//Adicionando o AuthToken  no Header da requisição
                requisicaoWeb.Headers["access_token"] = "05deedee-25c2-46ab-8fa3-10cd78f3f297";//Adicionando o AuthToken  no Header da requisição


                //verificando se o cadastro do cliente existe e esta ok
                using (HttpWebResponse resposta = (HttpWebResponse)requisicaoWeb.GetResponse())
                {
                    if (resposta.StatusCode == HttpStatusCode.OK)
                    {
                        string mensagem = ("\r\nResponse Status Code is OK and StatusDescription is: {0}", resposta.StatusDescription).ToString();
                        ViewBag.Mensagem = mensagem;

                        //pegando os dados do cliente da resposta da api
                        var streamDados = resposta.GetResponseStream();
                        StreamReader reader = new StreamReader(streamDados);


                        string objResponse = reader.ReadToEnd();


                        //se retornar vazio
                        if(objResponse.Equals("[]"))
                        {
                             mensagem = ("Cliente não encontrado ou dados incorretos. Por favor, verifique sua requisição e tente novamente");
                            ViewBag.Mensagem = mensagem;
                            ViewBag.CodMsg = 1;
                            return View();
                        }
                        else
                        {
                            var post = JsonConvert.DeserializeObject<Post>(objResponse.Substring(1, objResponse.Length - 2));
                            ViewBag.IdCliente = post.id_sacado_sac;
                            ViewBag.RazSocial = post.st_nome_sac;
                            ViewBag.TamanhoReg = post.st_cgc_sac.Length; //verifica o tamanho do registro para identificar se cgc ou cnpj
                            ViewBag.Endereco = post.st_endereco_sac;
                            ViewBag.Cep = post.st_cep_sac;
                            ViewBag.Bairro = post.st_bairro_sac;
                            ViewBag.Complemento = post.st_complemento_sac;
                            ViewBag.Cidade = post.st_cidade_sac;
                            ViewBag.Estado = post.st_estado_sac;
                            ViewBag.Telefone = post.st_telefone_sac;
                            ViewBag.Email = post.st_email_sac;
                            ViewBag.DataCadastro = post.dt_cadastro_sac;
                            ViewBag.Senha = post.st_senha_sac;

                            //parametros necessários
                            ViewBag.Plano = idPlanoa;
                            ViewBag.Identificador = iDentificador;






                            if (post.st_cgc_sac.Length > 11)
                            {
                                ViewBag.CNPJ = post.st_cgc_sac;
                            }
                            else
                            {
                                ViewBag.CGC = post.st_cgc_sac;
                            }
                           

                            streamDados.Close();
                            resposta.Close();
                        }
                       

                    }
                    else
                    {
                        //TO-DO para o retorno diferente de 200 (ok)
                        string mensagem = ("Cliente não encontrado ou dados não conclusivos: ", resposta.StatusDescription).ToString();
                        ViewBag.Mensagem = mensagem;
                    }

                    
                }


            }//fim do else





            var model = new EmpresaViewModel();
            return View(model);
        }


        [HttpPost]
        public ActionResult CadSuperLogicaPost(int? idCli, string inputCnpj, string inputCGC, string inputCEP, string inputLogradouro,
            string inputBairro, string inputComplemento, string inputCidade, string inputEstado, string inputTelefone, string inputEmail,
            string senhaSuperlogica, string fantasia, string userMtx, string inputUser, string inputSimpNacional, string planoSuperlogica, string identificadorSuperlogica)
        {


            //zerando as tempdata das mensagens
            TempData["msg"] = null;
            TempData["codmsg"] = null;
            TempData.Keep("codmsg");
            TempData.Keep("msg");


            //pegar as varaiveis e passar para caixa alta
            int? idSuperlogica = idCli;
            string cnpj = inputCnpj;
            string cgd = inputCGC;
            string cep = inputCEP;
            string logradouro = inputLogradouro.ToUpper();
            string bairro = inputBairro.ToUpper();
            string complemento = inputComplemento.ToUpper();
            string cidade = inputCidade.ToUpper();
            string estado = inputEstado.ToUpper();
            string telefone = inputTelefone.ToUpper();
            string email = inputEmail.ToLower(); //email sempre minusculo
            string senha = senhaSuperlogica; //caso a senha venha
            string mtx_fantasia = fantasia.ToUpper();
            string mtx_login = userMtx.ToLower(); //login do mtx - email do usuario
            string mtx_user = inputUser.ToUpper();
            string mtx_simpnacional = inputSimpNacional.ToUpper();
            string plano = planoSuperlogica;
            string identif = identificadorSuperlogica;

            //verificar os campos nulos
            if (mtx_user.Equals("") || mtx_login.Equals(""))
            {
                
                TempData["msg"] = "Nome do Usuário ou login não podem ser VAZIO ou NULO";
                TempData["codmsg"] = 2;
                TempData.Keep("codmsg");
                TempData.Keep("msg");
                ViewBag.CodMsg = 2;
                return RedirectToAction("CadSuperLogica", new { idplano = plano, idcliente = idSuperlogica, identificador = identif });
            }



            return null;
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


        public ActionResult MudarEmpresa()
        {
            int empresaUsuario = (int)Session["idEmpresa"];

            ViewBag.Empresas = db.Empresas.ToList();
            ViewBag.EmpUsuario = empresaUsuario;


            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MudarEmpresa(int? empresaUsuario)
        {
            int? usuarioemrpesa = empresaUsuario;
            if(usuarioemrpesa == null)
            {
                ViewBag.Empresas = db.Empresas.ToList();
                int empUser = (int)Session["idEmpresa"];
                ViewBag.EmpUsuario = empUser;
                return View();
            }
            this.emp = db.Empresas.Where(x => x.id == usuarioemrpesa).FirstOrDefault();
            //se nao, o sistema busca a empresa selecionado e aplica nas sessoes
            Session["idEmpresa"] = this.emp.id; //se nao esclhou nenhum  a session é com a propria empresa
            Session["cnpjEmp"] = this.emp.cnpj;
            Session["empresa"] = this.emp.fantasia;

            
            Session["empresas"] = this.emp;
            TempData["analise"] = null;
            TempData["analise2"] = null;
            return RedirectToAction("Index", "Home");


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