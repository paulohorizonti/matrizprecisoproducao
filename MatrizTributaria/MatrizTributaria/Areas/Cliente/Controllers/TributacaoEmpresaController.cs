﻿using MatrizTributaria.Models;
using MatrizTributaria.Areas.Cliente.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace MatrizTributaria.Areas.Cliente.Controllers
{
    public class TributacaoEmpresaController : Controller
    {
        readonly private MatrizDbContext db = new MatrizDbContext();

        //listas para analise
        List<AnaliseTributaria> analise = new List<AnaliseTributaria>();
        List<AnaliseTributaria> trib = new List<AnaliseTributaria>();
        List<AnaliseTributaria2> trib2 = new List<AnaliseTributaria2>();
        
        
        Usuario usuario;
        Empresa empresa;

        private void Write(string texto)
        {
            Debug.Write(texto + "\n");
        }

        [HttpGet]
        public object AnaliseTributaria()
        {
            string usuarioSessao = ""; //variavel auxiliar
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            else
            {
                usuarioSessao = Session["usuario"].ToString(); //pega o usuário da sessão
            }

            //Verifica se a variavel de sessão USUARIOS está nula, se estiver carrega as informações trazidas da sessão
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == usuarioSessao select a).FirstOrDefault(); //pega o usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                //será usada para carregar a lista pelo cnpj
                this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            }

            /*Verifica a variavel do tipo temp data ANALISE, caso esteja nula carregar a lista novamente*/
            if (TempData["analise"] == null)
            {
                //carrega a lista analise usando o cnpj da empresa do usuario
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise; //cria
                TempData.Keep("analise"); //salva
            }
            else //não estando nula apenas atribui à lista o valor carregado em tempdata
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Analises tributárias para ICMS de Saída*/

            /*OBS: 22072021: ACERTADO COMPARAÇÃO DE IGUALDADE: RETIRAR OS NULOS*/
            /*Aliq ICMS Venda Varejo Consumidor Final - ok*/
            ViewBag.AlqICMSVarejoCFMaior = this.analise.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO);
            ViewBag.AlqICMSVarejoCFMenor = this.analise.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO);
            ViewBag.AlqICMSVarejoCFIgual = this.analise.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null);
            ViewBag.AlqICMSVarejoCFNullaInterno = this.analise.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO == null); //onde nao for nulo no cliente mas no mtx sim
            ViewBag.AlqICMSVarejoCFNullaExterno = this.analise.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null);

            /*Aliq ICMS ST Venda Varejo Consumidor Final - ok*/
            ViewBag.AlqICMSSTVarejoCFMaior = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO);
            ViewBag.AlqICMSSTVarejoCFMenor = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO);
            ViewBag.AlqICMSSTVarejoCFIgual = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null);
            ViewBag.AlqICMSSTVarejoCFNulaInterno = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null);
            ViewBag.AlqICMSSTVarejoCFNulaExterno = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null);

            /*Aliq ICMS Venda Varejo Contribuinte - ok*/
            ViewBag.AlqICMSVendaVContMaior = this.analise.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.Aliq_Icms_Venda_Varejo_Cont_INTERNO);
            ViewBag.AlqICMSVendaVContMenor = this.analise.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.Aliq_Icms_Venda_Varejo_Cont_INTERNO);
            ViewBag.AlqICMSVendaVContIguais = this.analise.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.Aliq_Icms_Venda_Varejo_Cont_INTERNO && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null);
            ViewBag.AlqICMSVendaVContNulasInternos = this.analise.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.Aliq_Icms_Venda_Varejo_Cont_INTERNO == null);
            ViewBag.AlqICMSVendaVContNulasExternos = this.analise.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null);

            /*Aliq ICMS ST Venda Varejo Contribuinte - ok*/
            ViewBag.AlqICMSSTVendaVContMaior = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO);
            ViewBag.AlqICMSSTVendaVContMenor = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO);
            ViewBag.AlqICMSSTVendaVContIguais = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null);
            ViewBag.AlqICMSSTVendaVContNulasInternos = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO == null);
            ViewBag.AlqICMSSTVendaVContNulasExternos = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null);


            /*Aliq ICMS venda ATA - ok*/
            ViewBag.AlqICMSVataMaior = this.analise.Count(a => a.ALIQ_ICMS_VENDA_ATA > a.Aliq_Icms_Venda_Ata_Cont_INTERNO);
            ViewBag.AlqICMSVataMenor = this.analise.Count(a => a.ALIQ_ICMS_VENDA_ATA < a.Aliq_Icms_Venda_Ata_Cont_INTERNO);
            ViewBag.AlqICMSVataIgual = this.analise.Count(a => a.ALIQ_ICMS_VENDA_ATA == a.Aliq_Icms_Venda_Ata_Cont_INTERNO && a.ALIQ_ICMS_VENDA_ATA != null);
            ViewBag.AlqICMSVataNulaInterno = this.analise.Count(a => a.ALIQ_ICMS_VENDA_ATA != null && a.Aliq_Icms_Venda_Ata_Cont_INTERNO == null);
            ViewBag.AlqICMSVataNulaExterno = this.analise.Count(a => a.ALIQ_ICMS_VENDA_ATA == null);

            /*Aliq ICMS ST venda ATA - ok*/
            ViewBag.AlqICMSSTVataMaior = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO);
            ViewBag.AlqICMSSTVataMenor = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO);
            ViewBag.AlqICMSSTVataIgual = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO && a.ALIQ_ICMS_ST_VENDA_ATA != null);
            ViewBag.AlqICMSSTVataNulaInterno = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO == null);
            ViewBag.AlqICMSSTVataNulaExterno = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA == null);


            /*Aliq ICMS Vendo no atacado para Simples Nacional - ok*/
            ViewBag.AliqICMSVendaAtaSimpNacionalMaior = this.analise.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO);
            ViewBag.AliqICMSVendaAtaSimpNacionalMenor = this.analise.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO);
            ViewBag.AliqICMSVendaAtaSimpNacionalIgual = this.analise.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null);
            ViewBag.AliqICMSVendaAtaSimpNacionalNulaInterno = this.analise.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO == null);
            ViewBag.AliqICMSVendaAtaSimpNacionalNulaExterno = this.analise.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null);


            /*Aliq ICMS ST Venda no atacado para Simples Nacional - ok*/
            ViewBag.AliqICMSSTVendaAtaSimpNacionalMaior = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO);
            ViewBag.AliqICMSSTVendaAtaSimpNacionalMenor = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO);
            ViewBag.AliqICMSSTVendaAtaSimpNacionalIgual = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null);
            ViewBag.AliqICMSSTVendaAtaSimpNacionalNulaInterno = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null);
            ViewBag.AliqICMSVendaAtaSimpNacionalNulaExterno = this.analise.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null);


            return View();
        }

        [HttpGet]
        public ActionResult AnaliseRedBaseCalSai()
        {
            string usuarioSessao = ""; //variavel auxiliar
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            else
            {
                usuarioSessao = Session["usuario"].ToString(); //pega o usuário da sessão

            }

            //Verifica se a variavel de sessão USUARIOS está nula, se estiver carrega as informações trazidas da sessão
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == usuarioSessao select a).FirstOrDefault(); //pega o usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                //será usada para carregar a lista pelo cnpj
                this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            }

            /*Verifica a variavel do tipo temp data ANALISE, caso esteja nula carregar a lista novamente*/
            if (TempData["analise"] == null)
            {
                //carrega a lista analise usando o cnpj da empresa do usuario
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise; //cria
                TempData.Keep("analise"); //salva
            }
            else //não estando nula apenas atribui à lista o valor carregado em tempdata
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }
            //string user = Session["usuario"].ToString();
            //Usuario usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
            //Empresa empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa

            //List<AnaliseTributaria> analise2 = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();

            /*Aliq Redução da Base Calc ICMS venda CF*/
            ViewBag.AlqRBCIcmsCFMaior = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO);
            ViewBag.AlqRBCIcmsCFMenor = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO);
            ViewBag.AlqRBCIcmsCFIgual = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null);
            ViewBag.AlqRBCIcmsCFNullaInterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO == null);
            ViewBag.AlqRBCIcmsCFNullaExterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null);

            /*Aliq Redução Base Calc ICMS ST venda CF*/
            ViewBag.AlqRBCIcmsSTCFMaior = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO);
            ViewBag.AlqRBCIcmsSTCFMenor = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO);
            ViewBag.AlqRBCIcmsSTCFIguais = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null);
            ViewBag.AlqRBCIcmsSTCFNullaInternos = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null);
            ViewBag.AlqRBCIcmsSTCFNullaExternos = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null);

            /*Reedução Base de Calculo venda varejo contribuinte*/
            ViewBag.AlqRDBCICMSVendaVarContMarior = this.analise.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO);
            ViewBag.AlqRDBCICMSVendaVarContMenor = this.analise.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO);
            ViewBag.AlqRDBCICMSVendaVarContIguais = this.analise.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO && a.RED_BASE_CALC_VENDA_VAREJO_CONT != null);
            ViewBag.AlqRDBCICMSVendaVarContNulaInterno = this.analise.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO == null);
            ViewBag.AlqRDBCICMSVendaVarContNulaExterno = this.analise.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null);

            /*Reedução Base de Calculo ST venda varejo contribuinte*/
            ViewBag.AlqRDBCICMSSTVendaVarContMarior = this.analise.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO);
            ViewBag.AlqRDBCICMSSTVendaVarContMenor = this.analise.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO);
            ViewBag.AlqRDBCICMSSTVendaVarContIgual = this.analise.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null);
            ViewBag.AlqRDBCICMSSTVendaVarContNulaInterna = this.analise.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO == null);
            ViewBag.AlqRDBCICMSSTVendaVarContNulaExterna = this.analise.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null);


            /*Red Base Calc  ICMS  venda ATA PARA CONTRIBUINTE*/
            ViewBag.RedBaseCalcICMSVataMaior = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO);
            ViewBag.RedBaseCalcICMSVataMenor = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO);
            ViewBag.RedBaseCalcICMSVataIgual = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_ATA != null);
            ViewBag.RedBaseCalcICMSVataNulaInterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO == null);
            ViewBag.RedBaseCalcICMSVataNulaExterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null);

            /*Red Base Calc  ICMS ST  venda ATA PARA CONTRIBUINTE*/
            ViewBag.RedBaseCalcICMSSTVataMaior = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO);
            ViewBag.RedBaseCalcICMSSTVataMenor = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO);
            ViewBag.RedBaseCalcICMSSTVataIgual = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null);
            ViewBag.RedBaseCalcICMSSTVataNulaInterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO == null);
            ViewBag.RedBaseCalcICMSSTVataNulaExterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null);

            /*Redução base de calc ICMS venda no atacado para Simples Nacional*/
            ViewBag.RedBaseCalcICMSVATASimpNacionalMaior = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO);
            ViewBag.RedBaseCalcICMSVATASimpNacionalMenor = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO);
            ViewBag.RedBaseCalcICMSVATASimpNacionalIgual = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null);
            ViewBag.RedBaseCalcICMSVATASimpNacionalNulaInterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO == null);
            ViewBag.RedBaseCalcICMSVATASimpNacionalNulaExterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null);

            /*Redução base de calc ICMS ST venda no atacado para Simples Nacional*/
            ViewBag.RedBaseCalcICMSSTVATASimpNacionalMaior = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO);
            ViewBag.RedBaseCalcICMSSTVATASimpNacionalMenor = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO);
            ViewBag.RedBaseCalcICMSSTVATASimpNacionalIgual = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null);
            ViewBag.RedBaseCalcICMSSTVATASimpNacionalNulaInterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null);
            ViewBag.RedBaseCalcICMSSTVATASimpNacionalNulaExterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null);


            return View();
        }

        [HttpGet]
        public ActionResult AnaliseIcmsEntrada()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string user = Session["usuario"].ToString(); //pega o usuario da sessão

            //verifica se a variavel usuarios está nula, caso esteja ele carrega as informações de usuario e empresa
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa
            }

            //verifica a variavel analise, caso esteja nula carregar a lista novamente
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else //não estando nula apenas atribui o valor da variavel do tipo tempdata à lista
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }


            /*Aliquota ICMS Compra de Industria*/
            ViewBag.AliqICMSCompINDMaior = this.analise.Count(a => a.ALIQ_ICMS_COMP_DE_IND > a.Aliq_Icms_Comp_de_Ind_INTERNO);
            ViewBag.AliqICMSCompINDMenor = this.analise.Count(a => a.ALIQ_ICMS_COMP_DE_IND < a.Aliq_Icms_Comp_de_Ind_INTERNO);
            ViewBag.AliqICMSCompINDIgual = this.analise.Count(a => a.ALIQ_ICMS_COMP_DE_IND == a.Aliq_Icms_Comp_de_Ind_INTERNO && a.ALIQ_ICMS_COMP_DE_IND != null);
            ViewBag.AliqICMSCompINDNulaExterno = this.analise.Count(a => a.ALIQ_ICMS_COMP_DE_IND == null);
            ViewBag.AliqICMSCompIndNulaInterno = this.analise.Count(a => a.ALIQ_ICMS_COMP_DE_IND != null && a.Aliq_Icms_Comp_de_Ind_INTERNO == null); //onde não for nulo no cliente e nulo no mtx



            /*Aliquota ICMS ST Compra de Industria*/
            ViewBag.AliqICMSSTCompINDMaior = this.analise.Count(a => a.ALIQ_ICMS_ST_COMP_DE_IND > a.Aliq_Icms_ST_Comp_de_Ind_INTERNO);
            ViewBag.AliqICMSSTCompINDMenor = this.analise.Count(a => a.ALIQ_ICMS_ST_COMP_DE_IND < a.Aliq_Icms_ST_Comp_de_Ind_INTERNO);
            ViewBag.AliqICMSSTCompINDIgual = this.analise.Count(a => a.ALIQ_ICMS_ST_COMP_DE_IND == a.Aliq_Icms_ST_Comp_de_Ind_INTERNO && a.ALIQ_ICMS_ST_COMP_DE_IND != null);
            ViewBag.AliqICMSSTCompINDNulaExterno = this.analise.Count(a => a.ALIQ_ICMS_ST_COMP_DE_IND == null);
            ViewBag.AliqICMSSTCompIndNulaInterno = this.analise.Count(a => a.ALIQ_ICMS_ST_COMP_DE_IND != null && a.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null);



            /*Aliquota ICMS Compra de Atacado*/
            ViewBag.AliqICMSCompATAMaior = this.analise.Count(a => a.ALIQ_ICMS_COMPRA_DE_ATA > a.Aliq_Icms_Compra_de_Ata_INTERNO);
            ViewBag.AliqICMSCompATAMenor = this.analise.Count(a => a.ALIQ_ICMS_COMPRA_DE_ATA < a.Aliq_Icms_Compra_de_Ata_INTERNO);
            ViewBag.AliqICMSCompATAIgual = this.analise.Count(a => a.ALIQ_ICMS_COMPRA_DE_ATA == a.Aliq_Icms_Compra_de_Ata_INTERNO && a.ALIQ_ICMS_COMPRA_DE_ATA != null);
            ViewBag.AliqICMSCompATANulaInterno = this.analise.Count(a => a.ALIQ_ICMS_COMPRA_DE_ATA != null && a.Aliq_Icms_Compra_de_Ata_INTERNO == null);
            ViewBag.AliqICMSCompATANulaExterno = this.analise.Count(a => a.ALIQ_ICMS_COMPRA_DE_ATA == null);


            /*Aliquota ICMS ST Compra de Atacado*/
            ViewBag.AliqICMSSTCompATAMaior = this.analise.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA > a.Aliq_Icms_ST_Compra_de_Ata_INTERNO);
            ViewBag.AliqICMSSTCompATAMenor = this.analise.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA < a.Aliq_Icms_ST_Compra_de_Ata_INTERNO);
            ViewBag.AliqICMSSTCompATAIgual = this.analise.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == a.Aliq_Icms_ST_Compra_de_Ata_INTERNO && a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null);
            ViewBag.AliqICMSSTCompATANulaInterno = this.analise.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null && a.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null);
            ViewBag.AliqICMSSTCompATANulaExterno = this.analise.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == null);

            /*Aliquota ICMS Compra de Simples nacional*/
            ViewBag.AliqICMSCompSNMaior = this.analise.Count(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO);
            ViewBag.AliqICMSCompSNMenor = this.analise.Count(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL < a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO);
            ViewBag.AliqICMSCompSNIgual = this.analise.Count(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO && a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null);
            ViewBag.AliqICMSCompSNNulaInterno = this.analise.Count(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO == null);
            ViewBag.AliqICMSCompSNNulaExterno = this.analise.Count(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null);


            /*Aliquota ICMS ST Compra de Simples nacional*/
            ViewBag.AliqICMSSTCompSNMaior = this.analise.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO);
            ViewBag.AliqICMSSTCompSNMenor = this.analise.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL < a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO);
            ViewBag.AliqICMSSTCompSNIgual = this.analise.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO && a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null);
            ViewBag.AliqICMSSTCompSNNulaInterno = this.analise.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null);
            ViewBag.AliqICMSSTCompSNNulaExterno = this.analise.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null);


            /*Aliquota ICMS NFE INDUSTRIA*/
            ViewBag.AliqICMSNFEIndMaior = this.analise.Count(a => a.ALIQ_ICMS_NFE > a.Aliq_Icms_NFE_INTERNO);
            ViewBag.AliqICMSNFEIndMenor = this.analise.Count(a => a.ALIQ_ICMS_NFE < a.Aliq_Icms_NFE_INTERNO);
            ViewBag.AliqICMSNFEIndIguais = this.analise.Count(a => a.ALIQ_ICMS_NFE == a.Aliq_Icms_NFE_INTERNO && a.ALIQ_ICMS_NFE != null);
            ViewBag.AliqICMSNFEIndNulaInterno = this.analise.Count(a => a.ALIQ_ICMS_NFE != null && a.Aliq_Icms_NFE_INTERNO == null);
            ViewBag.AliqICMSNFEIndNulaExterno = this.analise.Count(a => a.ALIQ_ICMS_NFE == null);

            /*Aliquota ICMS NFE SIMPLES NACIONAL*/
            ViewBag.AliqICMSNFESNMaior = this.analise.Count(a => a.ALIQ_ICMS_NFE_FOR_SN > a.Aliq_Icms_NFE_For_SN_INTERNO);
            ViewBag.AliqICMSNFESNMenor = this.analise.Count(a => a.ALIQ_ICMS_NFE_FOR_SN > a.Aliq_Icms_NFE_For_SN_INTERNO);
            ViewBag.AliqICMSNFESNIguais = this.analise.Count(a => a.ALIQ_ICMS_NFE_FOR_SN == a.Aliq_Icms_NFE_For_SN_INTERNO && a.ALIQ_ICMS_NFE_FOR_SN != null);
            ViewBag.AliqICMSNFESNNullaInterno = this.analise.Count(a => a.ALIQ_ICMS_NFE_FOR_SN != null && a.Aliq_Icms_NFE_For_SN_INTERNO == null);
            ViewBag.AliqICMSNFESNNullaExterno = this.analise.Count(a => a.ALIQ_ICMS_NFE_FOR_SN == null);

            /*Aliquota ICMS NFE ATACADO*/
            ViewBag.AliqICMSNFEAtaMaior = this.analise.Count(a => a.ALIQ_ICMS_NFE_FOR_ATA > a.Aliq_Icms_NFE_For_Ata_INTERNO);
            ViewBag.AliqICMSNFEAtaMenor = this.analise.Count(a => a.ALIQ_ICMS_NFE_FOR_ATA < a.Aliq_Icms_NFE_For_Ata_INTERNO);
            ViewBag.AliqICMSNFEAtaIgual = this.analise.Count(a => a.ALIQ_ICMS_NFE_FOR_ATA == a.Aliq_Icms_NFE_For_Ata_INTERNO && a.ALIQ_ICMS_NFE_FOR_ATA != null);
            ViewBag.AliqICMSNFEAtaNulaInterno = this.analise.Count(a => a.ALIQ_ICMS_NFE_FOR_ATA != null && a.Aliq_Icms_NFE_For_Ata_INTERNO == null);
            ViewBag.AliqICMSNFEAtaNulaExterno = this.analise.Count(a => a.ALIQ_ICMS_NFE_FOR_ATA == null);


            return View();

        }

        [HttpGet]
        public ActionResult AnaliseRedBaseCalEnt()
        {

            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string user = Session["usuario"].ToString(); //pega o usuario da sessão

            //verifica se a variavel usuarios está nula, caso esteja ele carrega as informações de usuario e empresa
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa
            }

            //verifica a variavel analise, caso esteja nula carregar a lista novamente
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else //não estando nula apenas atribui o valor da variavel do tipo tempdata à lista
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

           
            /*Red bae calc ICMS Compra de Industria*/
            ViewBag.RedBaseCalcICMSCompINDMaior = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND > a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO);
            ViewBag.RedBaseCalcICMSCompINDMenor = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND < a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO);
            ViewBag.RedBaseCalcICMSCompINDIgual = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null);
            ViewBag.RedBaseCalcICMSCompINDNulaInterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null && a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO == null);
            ViewBag.RedBaseCalcICMSCompINDNulaExterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == null);


            /*Red bae calc ICMS ST Compra de Industria*/
            ViewBag.RedBaseCalcICMSSTCompINDMaior = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND > a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO);
            ViewBag.RedBaseCalcICMSSTCompINDMenor = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND < a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO);
            ViewBag.RedBaseCalcICMSSTCompINDIgual = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null);
            ViewBag.RedBaseCalcICMSSTCompINDNulaInterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null && a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO == null);
            ViewBag.RedBaseCalcICMSSTCompINDNulaExterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == null);

            /*Red Base Calc ICMS Compra de Atacado*/
            ViewBag.RedBaseCalcICMSCompATAMaior = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO);
            ViewBag.RedBaseCalcICMSCompATAMenor = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA < a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO);
            ViewBag.RedBaseCalcICMSCompATAIgual = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null);
            ViewBag.RedBaseCalcICMSCompATANulaInterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null && a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO == null);
            ViewBag.RedBaseCalcICMSCompATANulaExterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == null);


            /*Red Base Calc ICMS ST Compra de Atacado*/
            ViewBag.RedBaseCalcICMSSTCompATAMaior = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO);
            ViewBag.RedBaseCalcICMSSTCompATAMenor = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA < a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO);
            ViewBag.RedBaseCalcICMSSTCompATAIgual = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null);
            ViewBag.RedBaseCalcICMSSTCompATANulaInterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null && a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO == null);
            ViewBag.RedBaseCalcICMSSTCompATANulaExterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == null);

            /*Red Base Calc ICMS Compra de Simples Nacional*/
            ViewBag.RedBaseCalcICMSCompSNMaior = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO);
            ViewBag.RedBaseCalcICMSCompSNMenor = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO);
            ViewBag.RedBaseCalcICMSCompSNIgual = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL != null);
            ViewBag.RedBaseCalcICMSCompSNNulaInterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO == null);
            ViewBag.RedBaseCalcICMSCompSNNulaExterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == null);

            /*Red Base Calc ICMS ST Compra de Simples Nacional*/
            ViewBag.RedBaseCalcICMSSTCompSNMaior = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO);
            ViewBag.RedBaseCalcICMSSTCompSNMenor = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL < a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO);
            ViewBag.RedBaseCalcICMSSTCompSNIgual = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null);
            ViewBag.RedBaseCalcICMSSTCompSNNulaInterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null);
            ViewBag.RedBaseCalcICMSSTCompSNNulaExterno = this.analise.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null);

            return View();

        }


        [HttpGet]
        public ActionResult AnalisePisCofins()
        {

            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string user = Session["usuario"].ToString(); //pega o usuario da sessão

            //verifica se a variavel usuarios está nula, caso esteja ele carrega as informações de usuario e empresa
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa
            }

            //verifica a variavel analise, caso esteja nula carregar a lista novamente
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else //não estando nula apenas atribui o valor da variavel do tipo tempdata à lista
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            
            /*Pis*/
            /*Aliquota EntradaPIS*/
            ViewBag.AlqEPMaior = this.analise.Count(a => a.ALIQ_ENTRADA_PIS > a.Aliq_Ent_Pis_INTERNO);
            ViewBag.AlqEPMenor = this.analise.Count(a => a.ALIQ_ENTRADA_PIS < a.Aliq_Ent_Pis_INTERNO);
            ViewBag.AlqEPIgual = this.analise.Count(a => a.ALIQ_ENTRADA_PIS == a.Aliq_Ent_Pis_INTERNO && a.ALIQ_ENTRADA_PIS != null);
            ViewBag.AlqEPNulaInterno = this.analise.Count(a => a.ALIQ_ENTRADA_PIS != null && a.Aliq_Ent_Pis_INTERNO == null);
            ViewBag.AlqEPNulaCliente = this.analise.Count(a => a.ALIQ_ENTRADA_PIS == null);

            /*Aiquota Saida PIS*/
            ViewBag.AlqSPMaior = this.analise.Count(a => a.ALIQ_SAIDA_PIS > a.Aliq_Saida_Pis_INTERNO);
            ViewBag.AlqSPMenor = this.analise.Count(a => a.ALIQ_SAIDA_PIS < a.Aliq_Saida_Pis_INTERNO);
            ViewBag.AlqSPIguais = this.analise.Count(a => a.ALIQ_SAIDA_PIS == a.Aliq_Saida_Pis_INTERNO && a.ALIQ_SAIDA_PIS != null);
            ViewBag.AlqSPNulaInterno = this.analise.Count(a => a.ALIQ_SAIDA_PIS != null && a.Aliq_Saida_Pis_INTERNO == null);
            ViewBag.AlqSPNulaCliente = this.analise.Count(a => a.ALIQ_SAIDA_PIS == null);

            /*Cofins*/
            /*AlqEntradaCofins*/
            ViewBag.AlqEntradaCofinsMaior = this.analise.Count(a => a.ALIQ_ENTRADA_COFINS > a.Aliq_Ent_Cofins_INTERNO);
            ViewBag.AlqEntradaCofinsMenor = this.analise.Count(a => a.ALIQ_ENTRADA_COFINS < a.Aliq_Ent_Cofins_INTERNO);
            ViewBag.AlqEntradaCofinsIguais = this.analise.Count(a => a.ALIQ_ENTRADA_COFINS == a.Aliq_Ent_Cofins_INTERNO && a.ALIQ_ENTRADA_COFINS != null);
            ViewBag.AliqEntradaCofinsNullasInternas = this.analise.Count(a => a.ALIQ_ENTRADA_COFINS != null && a.Aliq_Ent_Cofins_INTERNO == null);
            ViewBag.AliqEntradaCofinsNullasExternas = this.analise.Count(a => a.ALIQ_ENTRADA_COFINS == null);

            /*Aliquota saida cofins*/
            ViewBag.AlqSaidaCofinsMaior = this.analise.Count(a => a.ALIQ_SAIDA_COFINS > a.Aliq_Saida_Cofins_INTERNO);
            ViewBag.AlqSaidaCofinsMenor = this.analise.Count(a => a.ALIQ_SAIDA_COFINS < a.Aliq_Saida_Cofins_INTERNO);
            ViewBag.AlqSaidaCofinsIguais = this.analise.Count(a => a.ALIQ_SAIDA_COFINS == a.Aliq_Saida_Cofins_INTERNO && a.ALIQ_SAIDA_COFINS != null);
            ViewBag.AlqSCNullaInterna = this.analise.Count(a => a.ALIQ_SAIDA_COFINS != null && a.Aliq_Saida_Cofins_INTERNO == null);
            ViewBag.AlqSCNullaCliente = this.analise.Count(a => a.ALIQ_SAIDA_COFINS == null);

            return View();
        }


        /*CST: Código de Situação Tributária*/
        [HttpGet]
        public ActionResult AnaliseCST()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string user = Session["usuario"].ToString(); //pega o usuario da sessão

            //verifica se a variavel usuarios está nula, caso esteja ele carrega as informações de usuario e empresa
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa
            }

            //verifica a variavel analise, caso esteja nula carregar a lista novamente
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else //não estando nula apenas atribui o valor da variavel do tipo tempdata à lista
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Entrada PIS Cofins (ACERTADO ALTERAÇÃO: TIRAR OS NULOS DA CONTA)
             Dessa forma ele compara os dois registros, se em um deles o valor
            for nulo ele retira da contagem, assim somente os registros realmente
            diferentes são analisados e a contagem de nulos no cliente e nulos na
            matriz ficará correta
            A comparação de igualdade acontece o mesmo, ele deve tirar os registros
            que forem nulos tanto no cliente quanto no mtx
             */
            ViewBag.CstEntradaPisCofinsNulaCliente = this.analise.Count(a => a.CST_ENTRADA_PIS_COFINS == null);
            ViewBag.CstEntradaPisCofinsNulaMtx     = this.analise.Count(a => a.Cst_Entrada_PisCofins_INTERNO == null);
            ViewBag.CstEntradaPisCofinsIgual       = this.analise.Count(a => a.CST_ENTRADA_PIS_COFINS == a.Cst_Entrada_PisCofins_INTERNO && a.CST_ENTRADA_PIS_COFINS != null && a.Cst_Entrada_PisCofins_INTERNO != null);
            ViewBag.CstEntradaPisCofinsDife        = this.analise.Count(a => a.CST_ENTRADA_PIS_COFINS != a.Cst_Entrada_PisCofins_INTERNO && a.CST_ENTRADA_PIS_COFINS != null && a.Cst_Entrada_PisCofins_INTERNO != null);

            /*Saída PIS Cofins*/
            ViewBag.CstSaidaPisCofinsNulaCliente = this.analise.Count(a =>  a.CST_SAIDA_PIS_COFINS == null);
            ViewBag.CstSaidaPisCofinsNulaMtx     = this.analise.Count(a =>  a.Cst_Saida_PisCofins_INTERNO == null);
            ViewBag.CstSaidaPisCofinsIgual       = this.analise.Count(a =>  a.CST_SAIDA_PIS_COFINS == a.Cst_Saida_PisCofins_INTERNO && a.CST_SAIDA_PIS_COFINS != null && a.Cst_Saida_PisCofins_INTERNO != null);
            ViewBag.CstSaidaPisCofinsDife        = this.analise.Count(a =>  a.CST_SAIDA_PIS_COFINS != a.Cst_Saida_PisCofins_INTERNO && a.CST_SAIDA_PIS_COFINS != null && a.Cst_Saida_PisCofins_INTERNO != null);

            /*CST Venda Varejo Consumidor Final*/
            ViewBag.CstVendaVarejoCFNulaCliente = this.analise.Count(a =>  a.CST_VENDA_VAREJO_CONS_FINAL == null);
            ViewBag.CstVendaVarejoCFNulaMtx     = this.analise.Count(a =>  a.Cst_Venda_Varejo_Cons_Final_INTERNO == null);
            ViewBag.CstVendaVarejoCFIgual       = this.analise.Count(a =>  a.CST_VENDA_VAREJO_CONS_FINAL == a.Cst_Venda_Varejo_Cons_Final_INTERNO && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.Cst_Venda_Varejo_Cons_Final_INTERNO != null);
            ViewBag.CstVendaVarejoCFDif         = this.analise.Count(a =>  a.CST_VENDA_VAREJO_CONS_FINAL != a.Cst_Venda_Varejo_Cons_Final_INTERNO && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.Cst_Venda_Varejo_Cons_Final_INTERNO != null);

            /*CST Venda Varejo Contribuinte*/
            ViewBag.CstVendaVarejoContNulaCliente   = this.analise.Count(a =>  a.CST_VENDA_VAREJO_CONT == null);
            ViewBag.CstVendaVarejoContNulaMtx       = this.analise.Count(a =>  a.Cst_Venda_Varejo_Cont_INTERNO == null);
            ViewBag.CstVendaVarejoContIgual         = this.analise.Count(a =>  a.CST_VENDA_VAREJO_CONT == a.Cst_Venda_Varejo_Cont_INTERNO && a.CST_VENDA_VAREJO_CONT != null && a.Cst_Venda_Varejo_Cont_INTERNO != null);
            ViewBag.CstVendaVarejoContDif           = this.analise.Count(a =>  a.CST_VENDA_VAREJO_CONT != a.Cst_Venda_Varejo_Cont_INTERNO && a.CST_VENDA_VAREJO_CONT != null && a.Cst_Venda_Varejo_Cont_INTERNO != null);


            /*CST Venda Atacado Contribuinte*/
            ViewBag.CstVendaAtaContNulaCliente  = this.analise.Count(a =>  a.CST_VENDA_ATA == null);
            ViewBag.CstVendaAtaContNulaMtx      = this.analise.Count(a =>  a.Cst_Venda_Ata_Cont_INTERNO == null);
            ViewBag.CstVendaAtaContIgual        = this.analise.Count(a =>  a.CST_VENDA_ATA == a.Cst_Venda_Ata_Cont_INTERNO && a.CST_VENDA_ATA != null && a.Cst_Venda_Ata_Cont_INTERNO != null);
            ViewBag.CstVendaAtaContDif          = this.analise.Count(a =>  a.CST_VENDA_ATA != a.Cst_Venda_Ata_Cont_INTERNO && a.CST_VENDA_ATA != null && a.Cst_Venda_Ata_Cont_INTERNO != null);


            /*CST Venda Atacado Simples Nacional*/
            ViewBag.CstVendaAtaSNNulaCliente = this.analise.Count(a =>  a.CST_VENDA_ATA_SIMP_NACIONAL == null);
            ViewBag.CstVendaAtaSNNulaMtx     = this.analise.Count(a =>  a.Cst_Venda_Ata_Simp_Nacional_INTERNO == null);
            ViewBag.CstVendaAtaSNIgual       = this.analise.Count(a =>  a.CST_VENDA_ATA_SIMP_NACIONAL == a.Cst_Venda_Ata_Simp_Nacional_INTERNO && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.Cst_Venda_Ata_Simp_Nacional_INTERNO != null);
            ViewBag.CstVendaAtaSNDif         = this.analise.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL != a.Cst_Venda_Ata_Simp_Nacional_INTERNO && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.Cst_Venda_Ata_Simp_Nacional_INTERNO != null);


            return View();
        }


        [HttpGet]
        public ActionResult AnaliseCSTEnt()
        {
            string user = "";
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            else
            {
                 user = Session["usuario"].ToString(); //pega o usuario da sessão
            }
           

            //verifica se a variavel usuarios está nula, caso esteja ele carrega as informações de usuario e empresa
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa
            }

            //verifica a variavel analise, caso esteja nula carregar a lista novamente
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else //não estando nula apenas atribui o valor da variavel do tipo tempdata à lista
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Entrada PIS Cofins (ACERTADO ALTERAÇÃO: TIRAR OS NULOS DA CONTA)
             Dessa forma ele compara os dois registros, se em um deles o valor
            for nulo ele retira da contagem, assim somente os registros realmente
            diferentes são analisados e a contagem de nulos no cliente e nulos na
            matriz ficará correta
            A comparação de igualdade acontece o mesmo, ele deve tirar os registros
            que forem nulos tanto no cliente quanto no mtx
             */

            /*CST Compra de industria*/
            ViewBag.CstCompraIndustriaNulaCliente = this.analise.Count(a => a.CST_COMPRA_DE_IND == null); //nula do cliente
            ViewBag.CstCompraIndustriaNulaMtx     = this.analise.Count(a => a.Cst_Compra_de_Ind_INTERNO == null); //nula no mtx
            ViewBag.CstCompraIndustriaIgual       = this.analise.Count(a => a.CST_COMPRA_DE_IND == a.Cst_Compra_de_Ind_INTERNO && a.CST_COMPRA_DE_IND != null && a.Cst_Compra_de_Ind_INTERNO != null); //compara tirando os nulos
            ViewBag.CstCompraIndustriaDif         = this.analise.Count(a => a.CST_COMPRA_DE_IND != a.Cst_Compra_de_Ind_INTERNO && a.CST_COMPRA_DE_IND != null && a.Cst_Compra_de_Ind_INTERNO != null);

            /*CST Compra de Atacado*/
            ViewBag.CstCompraAtacadoNulaCliente = this.analise.Count(a => a.CST_COMPRA_DE_ATA == null); //nula do cliente
            ViewBag.CstCompraAtacadoNulaMtx     = this.analise.Count(a => a.Cst_Compra_de_Ata_INTERNO == null); //nula no mtx
            ViewBag.CstCompraAtacadoIgual       = this.analise.Count(a => a.CST_COMPRA_DE_ATA == a.Cst_Compra_de_Ata_INTERNO && a.CST_COMPRA_DE_ATA != null && a.Cst_Compra_de_Ata_INTERNO != null); //compara tirando os nulos
            ViewBag.CstCompraAtacadoDif         = this.analise.Count(a => a.CST_COMPRA_DE_ATA != a.Cst_Compra_de_Ata_INTERNO && a.CST_COMPRA_DE_ATA != null && a.Cst_Compra_de_Ata_INTERNO != null);

            /*CST Compra de Simples nacional*/
            ViewBag.CstCompraSNNulaCliente = this.analise.Count(a => a.CST_COMPRA_DE_SIMP_NACIONAL == null); //nula do cliente
            ViewBag.CstCompraSNNulaMtx     = this.analise.Count(a => a.Cst_Compra_de_Simp_Nacional_INTERNO == null); //nula no mtx
            ViewBag.CstCompraSNIgual       = this.analise.Count(a => a.CST_COMPRA_DE_SIMP_NACIONAL == a.Cst_Compra_de_Simp_Nacional_INTERNO && a.CST_COMPRA_DE_SIMP_NACIONAL != null && a.Cst_Compra_de_Simp_Nacional_INTERNO != null); //compara tirando os nulos
            ViewBag.CstCompraSNDif         = this.analise.Count(a => a.CST_COMPRA_DE_SIMP_NACIONAL != a.Cst_Compra_de_Simp_Nacional_INTERNO && a.CST_COMPRA_DE_SIMP_NACIONAL != null && a.Cst_Compra_de_Simp_Nacional_INTERNO != null);


            /*Tres juntos: CST_NFE_IND, CST_NFE_ATA, CST_NFE_SN*/
            /*CST NFE Industria*/
            ViewBag.CstNFEIndNulaCliente = this.analise.Count(a => a.CST_DA_NFE_DA_IND_FORN == null); //nula do cliente
            ViewBag.CstNFEIndNulaMtx     = this.analise.Count(a => a.Cst_da_Nfe_da_Ind_FORN_INTERNO == null); //nula no mtx
            ViewBag.CstNFEIndIgual       = this.analise.Count(a => a.CST_DA_NFE_DA_IND_FORN == a.Cst_da_Nfe_da_Ind_FORN_INTERNO && a.CST_DA_NFE_DA_IND_FORN != null && a.Cst_da_Nfe_da_Ind_FORN_INTERNO != null); //compara tirando os nulos
            ViewBag.CstNFEIndDif         = this.analise.Count(a => a.CST_DA_NFE_DA_IND_FORN != a.Cst_da_Nfe_da_Ind_FORN_INTERNO && a.CST_DA_NFE_DA_IND_FORN != null && a.Cst_da_Nfe_da_Ind_FORN_INTERNO != null);

            /*CST NFE Atacado*/
            ViewBag.CstNFEAtaNulaCliente = this.analise.Count(a => a.CST_DA_NFE_DE_ATA_FORN == null); //nula do cliente
            ViewBag.CstNFEAtaNulaMtx     = this.analise.Count(a => a.Cst_da_Nfe_de_Ata_FORn_INTERNO == null); //nula no mtx
            ViewBag.CstNFEAtaIgual       = this.analise.Count(a => a.CST_DA_NFE_DE_ATA_FORN == a.Cst_da_Nfe_de_Ata_FORn_INTERNO && a.CST_DA_NFE_DE_ATA_FORN != null && a.Cst_da_Nfe_de_Ata_FORn_INTERNO != null); //compara tirando os nulos
            ViewBag.CstNFEAtaDif         = this.analise.Count(a => a.CST_DA_NFE_DE_ATA_FORN != a.Cst_da_Nfe_de_Ata_FORn_INTERNO && a.CST_DA_NFE_DE_ATA_FORN != null && a.Cst_da_Nfe_de_Ata_FORn_INTERNO != null);

            /*CSosnt NFE For Simples Nacional*/
            ViewBag.CstNFESNNulaCliente = this.analise.Count(a => a.CSOSNT_DANFE_DOS_NFOR == null); //nula do cliente
            ViewBag.CstNFESNNulaMtx     = this.analise.Count(a => a.CsosntdaNfedoSnFOR_INTERNO == null); //nula no mtx
            ViewBag.CstNFESNIgual       = this.analise.Count(a => a.CSOSNT_DANFE_DOS_NFOR == a.CsosntdaNfedoSnFOR_INTERNO && a.CSOSNT_DANFE_DOS_NFOR != null && a.CsosntdaNfedoSnFOR_INTERNO != null); //compara tirando os nulos
            ViewBag.CstNFESNDif         = this.analise.Count(a => a.CSOSNT_DANFE_DOS_NFOR != a.CsosntdaNfedoSnFOR_INTERNO && a.CSOSNT_DANFE_DOS_NFOR != null && a.CsosntdaNfedoSnFOR_INTERNO != null);

            return View();

        }

        /*Produtos*/
        [HttpGet]
        public ActionResult AnaliseProd()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            string user = Session["usuario"].ToString();

            //verifica se a variavel usuarios está nula, caso esteja ele carrega as informações de usuario e empresa
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa
            }

            
            //verifica a variavel analise, caso esteja nula carregar a lista novamente
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else //não estando nula apenas atribui o valor da variavel do tipo tempdata à lista
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }


            ///*Descrição: IGUAIS, DIFERENTES E NULOS*/
            //int iguais = analise.Count(a => a.PRODUTO_DESCRICAO == a.Descricao_INTERNO);
            //int nulas = analise.Count(a => a.PRODUTO_DESCRICAO == "" || a.PRODUTO_DESCRICAO == null);
            //iguais = iguais - nulas; //dessa forma ecxlui-se onde a descrição está nula no cliente e no mtx ao mesmo tempo

            //ViewBag.ProdDescIguais = iguais;
            //ViewBag.ProdDescNull = nulas;
            //ViewBag.ProdDescDif = analise.Count(a => a.PRODUTO_DESCRICAO != a.Descricao_INTERNO);

            /*Descrição: IGUAIS, DIFERENTES E NULOS*/
            ViewBag.ProdDescIguais = analise.Count(a=> a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_DESCRICAO == a.Descricao_INTERNO);
            ViewBag.ProdDescNull = analise.Count(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_DESCRICAO == "" || a.PRODUTO_DESCRICAO == null);
            ViewBag.ProdDescDif = analise.Count(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_DESCRICAO != a.Descricao_INTERNO);
           




            /*Cest: IGUAIS, DIFERENTES, NULOS*/
            ViewBag.ProdCESTNulo = analise.Count(a => a.PRODUTO_CEST == null); // não possuem cest
            ViewBag.ProdCESTDif = analise.Count(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_CEST != a.Cest_INTERNO && a.PRODUTO_CEST != null);
            ViewBag.ProdCESTIgual = analise.Count(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_CEST == a.Cest_INTERNO && a.PRODUTO_CEST != null);


            /*Ncm*/
            ViewBag.ProdNCMNulo= analise.Count(a => a.PRODUTO_NCM == null);
            ViewBag.ProdNCMDiferente = analise.Count(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_NCM != a.NCM_INTERNO && a.PRODUTO_NCM != null);
            ViewBag.ProdNCMIgual = analise.Count(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_NCM == a.NCM_INTERNO && a.PRODUTO_NCM != null);


            return View();
        }


        
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

        [HttpGet]
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

        //[HttpGet]
        //public ActionResult TabelaProduto(string sortOrder, string procuraNome, string procuraBarras, string procuraNCM, string procuraCEST, string currentFilter, int? page, string LinhasNum)
        //{
        //    /*Verificando a sessão*/
        //    if (Session["usuario"] == null)
        //    {
        //        return RedirectToAction("Login", "../Home");
        //    }

        //    /*Pegando o usuário e a empresa do usuário*/
        //    string user = Session["usuario"].ToString();
        //    Usuario usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
        //    Empresa empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa

        //    //Paginação 
        //    ViewBag.OrdemAtual = sortOrder;
        //    ViewBag.PorProdutoDesc = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";
        //    ViewBag.PorCatProd = sortOrder == "CatProd" ? "CatProd_desc" : "CatProd";


        //    if (procuraNome != null)
        //    {
        //        page = 1;
        //        procuraNome = procuraNome.Trim();
        //    }
        //    else
        //    {
        //        procuraNome = currentFilter;
        //    }

        //    if (procuraBarras != null)
        //    {
        //        page = 1;
        //        procuraBarras = procuraBarras.Trim();
        //    }
        //    else
        //    {
        //        procuraBarras = currentFilter;
        //    }

        //    if (procuraCEST != null)
        //    {
        //        page = 1;
        //        procuraCEST = procuraCEST.Trim();
        //    }
        //    else
        //    {
        //        procuraCEST = currentFilter;
        //    }
        //    if (procuraNCM != null)
        //    {
        //        page = 1;
        //        procuraNCM = procuraNCM.Trim();
        //    }
        //    else
        //    {
        //        procuraNCM = currentFilter;
        //    }


        //    var trib = from s in db.Analise_Tributaria select s;

        //    //busca
        //    if (!String.IsNullOrEmpty(procuraNome))
        //    {

        //        trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_DESCRICAO.Contains(procuraNome));
        //        trib = trib.OrderBy(s => s.Id_Produto_INTERNO);
        //        //trib = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == usuario.empresa.cnpj && a.PRODUTO_DESCRICAO.Contains(searchString.ToUpper()) || a.PRODUTO_COD_BARRAS.Contains(searchString) orderby a.PRODUTO_DESCRICAO select a).ToList();

        //        ViewBag.Quantidade = trib.Count();
        //    }
        //    else
        //    {
        //        if (!String.IsNullOrEmpty(procuraBarras))
        //        {

        //            trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_COD_BARRAS.Contains(procuraBarras));
        //            trib = trib.OrderBy(s => s.Id_Produto_INTERNO);


        //        }
        //        else
        //        {
        //            if (!String.IsNullOrEmpty(procuraNCM))
        //            {
        //                trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_NCM.Contains(procuraNCM));
        //                trib = trib.OrderBy(s => s.Id_Produto_INTERNO);
        //            }
        //            else
        //            {
        //                if (!String.IsNullOrEmpty(procuraCEST))
        //                {
        //                    trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj && s.PRODUTO_CEST.Contains(procuraCEST));
        //                    trib = trib.OrderBy(s => s.Id_Produto_INTERNO);
        //                }
        //                else
        //                {
        //                    trib = trib.Where(s => s.CNPJ_EMPRESA == usuario.empresa.cnpj);
        //                    trib = trib.OrderBy(s => s.Id_Produto_INTERNO);
        //                }
        //            }


        //        }


        //    }


        //    int pageSize = 0;

        //    if (String.IsNullOrEmpty(LinhasNum))
        //    {
        //        pageSize = 5;
        //    }
        //    else
        //    {

        //        ViewBag.Texto = LinhasNum;
        //        pageSize = Int32.Parse(LinhasNum);
        //    }

        //    int pageNumber = (page ?? 1);

        //    return View(trib.ToPagedList(pageNumber, pageSize)); //retorna a view com o numero de paginas e tamanho

        //}


        [HttpGet]
        public ActionResult TabelaProduto(string ordenacao,  int? filtroSelect, int? page, int? numeroLinhas, int? parFiltro=3, string filtroDados = "")
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            /*mensgem topo card*/
            ViewBag.Mensagem = "Produto Geral Cliente x MTX";

                     
            /*Pegando o usuario e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            //Criar uma sssion para evitar buscar novamente o usuario e a empresa
            if(Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault();//pegou o usuario no banco
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault();//empresa do usuario
                Session["usuarios"] = usuario; //cria sessao unica para o usuario
                Session["empresas"] = empresa; //cria sessao unica para empresa
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"]; //se nao for nula apenas atribui a empresa a sessao salva na condição acima
            }


            //numero de linhas? se o parametro numerolinhas vier preenchido ele atualiza a variavel, caso contrario continua em 10 (padrao)
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 5;
            //ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //filtro para mostrar todos os dados
            ViewBag.ParFiltro = (filtroDados != "") ? parFiltro : 3; //filtro para mostrar todos os dados
            ViewBag.DadoFiltrar = (filtroDados != null) ? filtroDados : null;
            ViewBag.FiltroSelect = (filtroSelect != null)? filtroSelect: 1;

            //ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decrescente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            page = (filtroDados == null) ? 1 : page; //atribui 1 a pagina caso os parametros nao sejam nulos

           
            if (ViewBag.FiltroSelect == 1)
            {
                if (TempData["trib"] == null)
                {
                    this.trib = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                    TempData["trib"] = this.trib;
                    TempData.Keep("trib");
                }
                else
                {
                    this.trib = (List<AnaliseTributaria>)TempData["trib"];
                    TempData.Keep("trib");
                }
                //procura diferenciado para tabela de  produto
                trib = ProcuraPorTabelaProduto(filtroDados, parFiltro, trib);


                int tamaanhoPagina = 0;

                //ternario para tamanho da pagina
                tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 5) ? ViewBag.numeroLinhas : (int)numeroLinhas);

                //ViewBag.MenssagemGravar = (resultado != null) ? resultado : "";

                int numeroPagina = (page ?? 1);


                return View(trib.ToPagedList(numeroPagina, tamaanhoPagina)); //retorna a view com o numero de paginas e tamanho

            }
            else
            {

                ////<!--Escrever codigo para produtos não encontrados na tabela -->
               
                return RedirectToAction("TabelaProduto2", new { filtroSelect = 2 });
            }

           

        }

        /*Action responsavel por mostrar os produtos importados mas que não há correspondecia com produtos
         dentro da matriz MTX*/
        [HttpGet]
        public ActionResult TabelaProduto2(string ordenacao, int? filtroSelect, int? page, int? numeroLinhas, int? parFiltro = 3, string filtroDados = "")
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            /*mensgem topo card*/
            ViewBag.Mensagem = "Produto Geral Cliente x MTX";

            ///*Variavel auxiliar para alguma alteração*/
            //string resultado = param;

            /*Pegando o usuario e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            //Criar uma sssion para evitar buscar novamente o usuario e a empresa
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault();//pegou o usuario no banco
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault();//empresa do usuario
                Session["usuarios"] = usuario; //cria sessao unica para o usuario
                Session["empresas"] = empresa; //cria sessao unica para empresa
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"]; //se nao for nula apenas atribui a empresa a sessao salva na condição acima
            }


            //numero de linhas? se o parametro numerolinhas vier preenchido ele atualiza a variavel, caso contrario continua em 10 (padrao)
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 5;
            //ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //filtro para mostrar todos os dados
            ViewBag.ParFiltro = (filtroDados != "") ? parFiltro : 3; //filtro para mostrar todos os dados
            ViewBag.DadoFiltrar = (filtroDados != null) ? filtroDados : null;
            ViewBag.FiltroSelect = (filtroSelect != null) ? filtroSelect : 1;

            //ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decrescente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            page = (filtroDados == null) ? 1 : page; //atribui 1 a pagina caso os parametros nao sejam nulos


            if (ViewBag.FiltroSelect == 2)
            {
                this.trib2 = (from a in db.Analise_Tributaria_2 where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();

                //procura diferenciado para tabela de  produto
                trib2 = ProcuraPorTabelaProduto2(filtroDados, parFiltro, trib2);


                int tamaanhoPagina = 0;

                //ternario para tamanho da pagina
                tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 5) ? ViewBag.numeroLinhas : (int)numeroLinhas);

                //ViewBag.MenssagemGravar = (resultado != null) ? resultado : "";

                int numeroPagina = (page ?? 1);


                return View(trib2.ToPagedList(numeroPagina, tamaanhoPagina)); //retorna a view com o numero de paginas e tamanho

            }
            else
            {

               
                return RedirectToAction("TabelaProduto");
            }

        }


        //Alteração em massa produto: - Descrição
        [HttpGet]
        public ActionResult EditClienteProdMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            /*Mensagem do head do card*/
            ViewBag.Mensagem = "Descrição do produto no Cliente X Descrição do Produto no MTX";

            /*Variavel auxiliar para retornar o resulado da alteração*/
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            //vai criar uma session para evitar buscar novamente o usuario e a empresa
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurar por
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //filtro parametro 3 para mostrar ambos
            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;//atribui 1 a pagina caso os parametreos nao sejam nulos

            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;

            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //viewbag
            ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;

            /*Para tipar*/
            //var analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a);
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            //ViewBag para guardar a opção
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Descrição igual":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1=IGUAIS
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_DESCRICAO == a.Descricao_INTERNO).ToList();
                            break;
                        case "2":
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_DESCRICAO != a.Descricao_INTERNO).ToList();
                            break;
                        case "3":
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_DESCRICAO == "" || a.PRODUTO_DESCRICAO == null).ToList();
                            break;
                    }//fim swithce filtro
                    break;
                case "Descrição diferente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2=DIFERENTES
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_DESCRICAO == a.Descricao_INTERNO).ToList();
                            break;
                        case "2":
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_DESCRICAO != a.Descricao_INTERNO).ToList();
                            break;
                        case "3":
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_DESCRICAO == "" || a.PRODUTO_DESCRICAO == null).ToList();
                            break;
                    }//fim swithce filtro
                    break;
                case "Descrição nula":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //3=NULOS
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_DESCRICAO == a.Descricao_INTERNO).ToList();
                            break;
                        case "2":
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_DESCRICAO != a.Descricao_INTERNO).ToList();
                            break;
                        case "3":
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_DESCRICAO == "" || a.PRODUTO_DESCRICAO == null).ToList();
                            break;
                    }//fim swithce filtro
                    break;


            }//fim switch
            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);


            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }


            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);


            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada

        }

        [HttpGet]
        public ActionResult EditClienteProdMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;

            string descCliente = "";
            string descMtx = "";

            try
            {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                 
                    descCliente=(analise.PRODUTO_DESCRICAO==null)? descCliente: analise.PRODUTO_DESCRICAO.ToString();
                    descMtx=(analise.Descricao_INTERNO == null)? descMtx : analise.Descricao_INTERNO.ToString();
                    
                    if(descCliente == descMtx)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //atribui o valor procurado na analise ao objeto instanciado
                        trib.PRODUTO_DESCRICAO = analise.Descricao_INTERNO;
                        db.SaveChanges();

                        regSalv++;
                    }
                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";
            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro : " + e.ToString();
            }

            
            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteProdMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteProdMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();
            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                //faz a busca no objeto criado instanciando um so objeto
                //trib = db.TributacaoEmpresas.Find(idTrb);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditClienteProdMassaManualModalPost(string strDados, string descProdClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.PRODUTO_DESCRICAO = (descProdClienteManual != "") ? trib.PRODUTO_DESCRICAO = descProdClienteManual : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteProdMassa", new { param = resultado, qtdSalvos = regSalvos });
           
        }

        //Alteração em massa produto: NCM
        [HttpGet]
        public ActionResult EditClienteNcmMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            /*Mensagem do head do card*/
            ViewBag.Mensagem = "NCM no Cliente X NCM no MTX"; 

            /*Variavel auxiliar para retornar o resulado da alteração*/
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            //vai criar uma session para evitar buscar novamente o usuario e a empresa
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurar por
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //filtro parametro 3 para mostrar ambos
            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;//atribui 1 a pagina caso os parametreos nao sejam nulos

            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;

            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //viewbag
            ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;

            /*Para tipar*/
            //var analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a);
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            //ViewBag para guardar a opção
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "NCM Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1=IGUAIS
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = this.analise.Where(a => a.PRODUTO_NCM == a.NCM_INTERNO & a.PRODUTO_NCM != null).ToList();
                            break;
                        case "2":
                            analise = this.analise.Where(a => a.PRODUTO_NCM != a.NCM_INTERNO & a.PRODUTO_NCM != null).ToList();
                            break;
                        case "3":
                            analise = this.analise.Where(a => a.PRODUTO_NCM == null).ToList();
                            break;
                    }//fim swithce filtro
                    break;
                case "NCM Diferentes":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2=DIFERENTES
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = this.analise.Where(a => a.PRODUTO_NCM == a.NCM_INTERNO & a.PRODUTO_NCM != null).ToList();
                            break;
                        case "2":
                            analise = this.analise.Where(a => a.PRODUTO_NCM != a.NCM_INTERNO & a.PRODUTO_NCM != null).ToList();
                            break;
                        case "3":
                            analise = this.analise.Where(a => a.PRODUTO_NCM == null).ToList();
                            break;
                    }//fim swithce filtro
                    break;
                case "NCM Nulo":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //2=NULOS
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = this.analise.Where(a => a.PRODUTO_NCM == a.NCM_INTERNO & a.PRODUTO_NCM != null).ToList();
                            break;
                        case "2":
                            analise = this.analise.Where(a => a.PRODUTO_NCM != a.NCM_INTERNO & a.PRODUTO_NCM != null).ToList();
                            break;
                        case "3":
                            analise = this.analise.Where(a => a.PRODUTO_NCM == null).ToList();
                            break;
                    }//fim swithce filtro
                    break;


            }//fim switch


            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);


            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }


            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);


            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada



        }

        [HttpGet]
        public ActionResult EditClienteNcmMassaModal(string strDados)
        {

            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string ncmCliente = "";
            string ncmMtx = "";


            try
            {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    ncmCliente = (analise.PRODUTO_NCM == null)?ncmCliente:analise.PRODUTO_NCM.ToString();
                    ncmMtx = (analise.NCM_INTERNO == null)?ncmMtx: analise.NCM_INTERNO.ToString();
               

                    if (ncmCliente == ncmMtx)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //atribui o valor procurado na analise ao objeto instanciado
                        trib.PRODUTO_NCM = analise.NCM_INTERNO;
                        db.SaveChanges();
                        regSalv++;
                    }
                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";
            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro : " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteNcmMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteNcmMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();
            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                //faz a busca no objeto criado instanciando um so objeto
                //trib = db.TributacaoEmpresas.Find(idTrb);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditClienteNcmMassaManualModalPost(string strDados, string ncmClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            //cestClienteManual = cestClienteManual.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.PRODUTO_NCM = (ncmClienteManual != "") ? trib.PRODUTO_NCM = ncmClienteManual : null;
                    try
                    {
                        db.SaveChanges();
                        regSalvos++;
                       
                    }
                    catch (Exception e)
                    {
                        resultado = "Problemas ao salvar o registro: " + e.ToString();
                    }

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteNcmMassa", new { param = resultado, qtdSalvos = regSalvos });
        }




        //Alterações em massa produto: CEST
        [HttpGet]
        public ActionResult EditClienteCestMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            ViewBag.Mensagem = "CEST no Cliente X CEST no MTX";

            string resultado = param;
            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"];
            }


            //se o filtro corrente estiver nulo ele busca pelo parametro procurar por
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;
            
            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //filtro parametro 3 para mostrar ambos
            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;//atribui 1 a pagina caso os parametreos nao sejam nulos
            
            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;

            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //viewbag
            ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;

            /*Para tipar*/
            //var analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a);
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }


            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "CEST Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";
                    switch (ViewBag.Filtro)
                    {
                        case "1": //iguais fora os nulos
                            //analise = this.analise.Where(a => a.PRODUTO_CEST == a.Cest_INTERNO & a.PRODUTO_CEST != null).ToList();
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_CEST == a.Cest_INTERNO & a.PRODUTO_CEST != null).ToList();

                            break;
                        case "2": //diferentes
                            //analise = this.analise.Where(a => a.PRODUTO_CEST != a.Cest_INTERNO & a.PRODUTO_CEST != null).ToList();
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_CEST != a.Cest_INTERNO && a.PRODUTO_CEST !=null).ToList();

                            break;
                        case "3": //nulos
                           
                            //analise = this.analise.Where(a => a.PRODUTO_CEST != null).ToList();
                            analise = this.analise.Where(a => a.PRODUTO_CEST == null).ToList();
                            break;
                    }//fim swithce filtro
                    break;
                case "CEST Diferentes":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";
                    //analise = this.analise.Where(s => s.PRODUTO_CEST == null || s.PRODUTO_CEST == "0").ToList();
                    switch (ViewBag.Filtro)
                    {
                        case "1": //iguais fora os nulos
                            //analise = this.analise.Where(a => a.PRODUTO_CEST == a.Cest_INTERNO & a.PRODUTO_CEST != null).ToList();
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_CEST == a.Cest_INTERNO & a.PRODUTO_CEST != null).ToList();

                            break;
                        case "2": //diferentes
                            //analise = this.analise.Where(a => a.PRODUTO_CEST != a.Cest_INTERNO & a.PRODUTO_CEST != null).ToList();
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_CEST != a.Cest_INTERNO && a.PRODUTO_CEST != null).ToList();

                            break;
                        case "3": //nulos

                            //analise = this.analise.Where(a => a.PRODUTO_CEST != null).ToList();
                            analise = this.analise.Where(a => a.PRODUTO_CEST == null).ToList();
                            break;
                    }//fim swithce filtro
                     break;
                case "CEST Nulo":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";
                    switch (ViewBag.Filtro)
                    {
                        case "1": //iguais fora os nulos
                            //analise = this.analise.Where(a => a.PRODUTO_CEST == a.Cest_INTERNO & a.PRODUTO_CEST != null).ToList();
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_CEST == a.Cest_INTERNO & a.PRODUTO_CEST != null).ToList();

                            break;
                        case "2": //diferentes
                            //analise = this.analise.Where(a => a.PRODUTO_CEST != a.Cest_INTERNO & a.PRODUTO_CEST != null).ToList();
                            analise = this.analise.Where(a => a.PRODUTO_COD_BARRAS == a.Cod_Barras_INTERNO && a.PRODUTO_CEST != a.Cest_INTERNO && a.PRODUTO_CEST != null).ToList();

                            break;
                        case "3": //nulos

                            //analise = this.analise.Where(a => a.PRODUTO_CEST != null).ToList();
                            analise = this.analise.Where(a => a.PRODUTO_CEST == null).ToList();
                            break;
                    }//fim swithce filtro
                    break;

            }//fim switch


            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);


            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }
            

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagem de retorno quando há alterações
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //número de páginas
            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
            
        }

        [HttpGet]
        public ActionResult EditClienteCestMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string cestCliente = "";
            string cestMTX = "";

            try
            {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //se for nullo ele nao aceita atribuir e da erro
                    //string ncmCliente = analise.PRODUTO_CEST.ToString();
                    cestCliente = (analise.PRODUTO_CEST == null)?cestCliente: analise.PRODUTO_CEST.ToString();
                    cestMTX = (analise.Cest_INTERNO == null)?cestMTX: analise.Cest_INTERNO.ToString();
                    //string ncmMtx = analise.Cest_INTERNO.ToString();
                    if (cestCliente == cestMTX)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //atribui o valor procurado na analise ao objeto instanciado
                        trib.PRODUTO_CEST = analise.Cest_INTERNO;
                        db.SaveChanges();
                        regSalv++;
                    }
                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";
            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro : " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteCestMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
           

        }
        [HttpGet]
        public ActionResult EditClienteCestMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();
            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                //faz a busca no objeto criado instanciando um so objeto
                //trib = db.TributacaoEmpresas.Find(idTrb);
               
                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }
           
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditClienteCestMassaManualModalPost(string strDados, string cestClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            //cestClienteManual = cestClienteManual.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;

            try
            {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.PRODUTO_CEST = (cestClienteManual != "") ? trib.PRODUTO_CEST = cestClienteManual : null;
                    try
                    {
                        db.SaveChanges();
                        regSalvos++;

                    }
                    catch (Exception e)
                    {
                        resultado = "Problemas ao salvar o registro: " + e.ToString();
                    }
                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch(Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;
            }


           

            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteCestMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        /// <summary>
        /// Edição Venda Varejo consumidor Final
        /// </summary>
        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaVarfCFMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS Venda no Varejo para consumidor final no Cliente X  no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";


                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }


            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada

        }


        [HttpGet]
        public ActionResult EditClienteAliqIcmsVenVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                   
                    //pegar valores
                    //analiseRetorno = (decimal)analise.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO;
                    /*Motivo da mudança:
                        caso o valor esteja nulo e o usuário quer que permaneça assim, quando
                        vier nulo do mtx obviamente vai atribuir nulo ao cliente e o usuário
                        tem a consciencia disso e quer que seja nulo mesmo, o sistema avalia
                        a variavel, caso ela esteja nula ele atribui o valor presente na 
                        variavel analisereorno que neste momento é 0.0, dai o fluxo segue*/
                    analiseRetorno = (analise.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO == null)?analiseRetorno : (decimal)analise.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO;

                    /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                    analiseTrib = (analise.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL);
                    
                    
                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if(analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL = analise.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO.ToString().Replace(",", ".");
                            
                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos

                    }

                    
                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsVendaVarfCFMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos= regNsalv });


        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVenVarCFMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);
                              
                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVenVarCFMassaManualModalPost(string strDados, string aliqIcmsVenVarCFClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsVenVarCFClienteManual = aliqIcmsVenVarCFClienteManual.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL = (aliqIcmsVenVarCFClienteManual != "") ? trib.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL = aliqIcmsVenVarCFClienteManual : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsVendaVarfCFMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

        /// <summary>
        /// Edição Venda Varejo ST Consumidor Final
        /// </summary>
       
        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaVarSTCFMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS ST Venda no Varejo para consumidor final no Cliente X  no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVenVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores que esta na analise
                    analiseRetorno = (analise.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO;

                    //pega valor que esta na tributação do produto na empresa
                    analiseTrib = (analise.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL);
                   


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {

                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL = analise.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos

                       
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsVendaVarSTCFMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVenVarCFMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);

        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVenVarCFMassaManualModalPost(string strDados, string aliqIcmsSTVenVarCFClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTVenVarCFClienteManual = aliqIcmsSTVenVarCFClienteManual.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL = (aliqIcmsSTVenVarCFClienteManual != "") ? trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL = aliqIcmsSTVenVarCFClienteManual : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsVendaVarSTCFMassa", new { param = resultado, qtdSalvos = regSalvos });
        }


        /// <summary>
        /// Edição ICMS Venda Varejo para Contribuinte
        /// </summary>
        /// <returns></returns>
        
        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaVarContMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS  Venda no Varejo para CONTRIBUINTE no Cliente X  no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                      
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.Aliq_Icms_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.Aliq_Icms_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.Aliq_Icms_Venda_Varejo_Cont_INTERNO && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.Aliq_Icms_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.Aliq_Icms_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.Aliq_Icms_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.Aliq_Icms_Venda_Varejo_Cont_INTERNO && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.Aliq_Icms_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.Aliq_Icms_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.Aliq_Icms_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.Aliq_Icms_Venda_Varejo_Cont_INTERNO && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.Aliq_Icms_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.Aliq_Icms_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.Aliq_Icms_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.Aliq_Icms_Venda_Varejo_Cont_INTERNO && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.Aliq_Icms_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.Aliq_Icms_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.Aliq_Icms_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.Aliq_Icms_Venda_Varejo_Cont_INTERNO && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.Aliq_Icms_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVenVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;

            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                    
                    analiseRetorno = (analise.Aliq_Icms_Venda_Varejo_Cont_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_Venda_Varejo_Cont_INTERNO;

                    analiseTrib = (analise.ALIQ_ICMS_VENDA_VAREJO_CONT == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_VENDA_VAREJO_CONT);
                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_VENDA_VAREJO_CONT = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_VENDA_VAREJO_CONT = analise.Aliq_Icms_Venda_Varejo_Cont_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsVendaVarContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVenVarContMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVenVarContMassaManualModalPost(string strDados, string aliqIcmsVenVarContClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsVenVarContClienteManual = aliqIcmsVenVarContClienteManual.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_VENDA_VAREJO_CONT = (aliqIcmsVenVarContClienteManual != "") ? trib.ALIQ_ICMS_VENDA_VAREJO_CONT = aliqIcmsVenVarContClienteManual : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsVendaVarContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

        /*Edição ICMS ST Venda varejo para contribuinte*/
     
        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVendaVarContMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS ST Venda no Varejo para CONTRIBUINTE no Cliente X  no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVenVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                  
                    analiseRetorno = (analise.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO;

                    analiseTrib = (analise.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONT);


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONT = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONT = analise.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsSTVendaVarContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVenVarContMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVenVarContMassaManualModalPost(string strDados, string aliqIcmsSTVenVarContClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTVenVarContClienteManual = aliqIcmsSTVenVarContClienteManual.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONT = (aliqIcmsSTVenVarContClienteManual != "") ? trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONT = aliqIcmsSTVenVarContClienteManual : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsSTVendaVarContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }


        /*Edição ICMS Venda Atacado para Contribuinte*/
        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaAtaContMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS ST  Venda no Atacado para CONTRIBUINTE no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.Aliq_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.Aliq_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.Aliq_Icms_Venda_Ata_Cont_INTERNO && a.ALIQ_ICMS_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.Aliq_Icms_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.Aliq_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.Aliq_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.Aliq_Icms_Venda_Ata_Cont_INTERNO && a.ALIQ_ICMS_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.Aliq_Icms_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.Aliq_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.Aliq_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.Aliq_Icms_Venda_Ata_Cont_INTERNO && a.ALIQ_ICMS_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.Aliq_Icms_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.Aliq_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.Aliq_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.Aliq_Icms_Venda_Ata_Cont_INTERNO && a.ALIQ_ICMS_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.Aliq_Icms_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.Aliq_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.Aliq_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.Aliq_Icms_Venda_Ata_Cont_INTERNO && a.ALIQ_ICMS_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.Aliq_Icms_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVenAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                   
                    analiseRetorno = (analise.Aliq_Icms_Venda_Ata_Cont_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_Venda_Ata_Cont_INTERNO;

                    analiseTrib = (analise.ALIQ_ICMS_VENDA_ATA == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_VENDA_ATA);
                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_VENDA_ATA = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_VENDA_ATA = analise.Aliq_Icms_Venda_Ata_Cont_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsVendaAtaContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVenAtaContMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);

        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVenAtaContMassaManualModalPost(string strDados, string aliqIcmsVenAtaContClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsVenAtaContClienteManual = aliqIcmsVenAtaContClienteManual.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_VENDA_ATA = (aliqIcmsVenAtaContClienteManual != "") ? trib.ALIQ_ICMS_VENDA_ATA = aliqIcmsVenAtaContClienteManual : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsVendaAtaContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }
       
        

        /*Edição ICMS ST Venda Atacado para Contribuinte*/
        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVendaAtaContMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS ST  Venda no Atacado para CONTRIBUINTE no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO && a.ALIQ_ICMS_ST_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO && a.ALIQ_ICMS_ST_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO && a.ALIQ_ICMS_ST_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO && a.ALIQ_ICMS_ST_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO && a.ALIQ_ICMS_ST_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVenAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                   
                    analiseRetorno = (analise.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO;

                    analiseTrib = (analise.ALIQ_ICMS_ST_VENDA_ATA == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_ST_VENDA_ATA);

                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_ST_VENDA_ATA = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_ST_VENDA_ATA = analise.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsSTVendaAtaContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }
        
        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVenAtaContMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVenAtaContMassaManualModalPost(string strDados, string aliqIcmsSTVenAtaContClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTVenAtaContClienteManual = aliqIcmsSTVenAtaContClienteManual.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_ST_VENDA_ATA = (aliqIcmsSTVenAtaContClienteManual != "") ? trib.ALIQ_ICMS_ST_VENDA_ATA = aliqIcmsSTVenAtaContClienteManual : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsSTVendaAtaContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }


        /*Edição ICMS Venda Atacado para Simples Nacional*/
        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaAtaSNMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS  Venda no Atacado para SIMPLES NACIONAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVenAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                 
                    analiseRetorno = (analise.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO;

                    analiseTrib = (analise.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL);

                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL = analise.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVenAtaSNMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);

        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVenAtaSNMassaManualModalPost(string strDados, string aliqIcmsVenAtaSNClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsVenAtaSNClienteManual = aliqIcmsVenAtaSNClienteManual.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL = (aliqIcmsVenAtaSNClienteManual != "") ? trib.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL = aliqIcmsVenAtaSNClienteManual : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

        /*Edição ICMS ST Venda Atacado para Simples Nacional*/
        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVendaAtaSNMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS ST Venda no Atacado para SIMPLES NACIONAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVenAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                  
                    analiseRetorno = (analise.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO;

                    analiseTrib = (analise.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL);
                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = analise.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsSTVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVenAtaSNMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVenAtaSNMassaManualModalPost(string strDados, string aliqIcmsSTVenAtaSNClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTVenAtaSNClienteManual = aliqIcmsSTVenAtaSNClienteManual.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = (aliqIcmsSTVenAtaSNClienteManual != "") ? trib.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = aliqIcmsSTVenAtaSNClienteManual : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsSTVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalvos });
        }


        /*Edição Red Base de Calc ICMS Venda Consumidor Final*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenVarCFMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS Venda para CONSUMIDOR FINAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                   
                    //pegar valores
                    analiseRetorno = (analise.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL);
                    

                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL = null;
                        }
                        else
                        {
                            trib.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL = analise.Red_Base_Calc_Icms_Venda_Varejo_Cons_Final_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenVarCFMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenVarCFMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet] 
        public ActionResult EdtCliAliqRedBasCalcIcmsVenVarCFMassaManualModalPost(string strDados, string redBasCalcAliqIcmsVenVarCF)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsVenVarCF = redBasCalcAliqIcmsVenVarCF.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL = (redBasCalcAliqIcmsVenVarCF != "") ? trib.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL = redBasCalcAliqIcmsVenVarCF : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenVarCFMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

        /*Edição Red Base de Calc ICMS ST Venda Consumidor Final*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarCFMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS ST Venda para CONSUMIDOR FINAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null).ToList();
                            break;
                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                  
                    analiseRetorno = (analise.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL);



                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL = null;
                        }
                        else
                        {
                            trib.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL = analise.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenVarCFMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarCFMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarCFMassaManualModalPost(string strDados, string redBasCalcAliqIcmsSTVenVarCF)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsSTVenVarCF = redBasCalcAliqIcmsSTVenVarCF.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL = (redBasCalcAliqIcmsSTVenVarCF != "") ? trib.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL = redBasCalcAliqIcmsSTVenVarCF : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenVarCFMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

        /*Edição Red Base de Calc ICMS Venda varejo para contrbuinte*/
       [HttpGet]
       public ActionResult EdtCliAliqRedBasCalcIcmsVenVarContMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS Venda para CONTRIBUINTE no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO && a.RED_BASE_CALC_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO && a.RED_BASE_CALC_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO && a.RED_BASE_CALC_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO && a.RED_BASE_CALC_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO && a.RED_BASE_CALC_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

       [HttpGet]
       public ActionResult EdtCliAliqRedBasCalcIcmsVenVarContMassaModal(string strDados)
       {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    
                    analiseRetorno = (analise.Red_Base_Calc_Venda_Varejo_Cont_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Venda_Varejo_Cont_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_VENDA_VAREJO_CONT == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_VENDA_VAREJO_CONT);
                    

                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.RED_BASE_CALC_VENDA_VAREJO_CONT = null;
                        }
                        else
                        {
                            trib.RED_BASE_CALC_VENDA_VAREJO_CONT = analise.Red_Base_Calc_Venda_Varejo_Cont_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenVarContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenVarContMassaManualModalPost(string strDados, string redBasCalcAliqIcmsVenVarCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsVenVarCont = redBasCalcAliqIcmsVenVarCont.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.RED_BASE_CALC_VENDA_VAREJO_CONT = (redBasCalcAliqIcmsVenVarCont != "") ? trib.RED_BASE_CALC_VENDA_VAREJO_CONT = redBasCalcAliqIcmsVenVarCont : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenVarContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenVarContMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        /*Edição Red Base de Calc ICMS ST Venda varejo para contrbuinte*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarContMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS ST Venda Varejo para CONTRIBUINTE no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                      
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    analiseRetorno = (analise.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ST_VENDA_VAREJO_CONT);

                 
                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.RED_BASE_CALC_ST_VENDA_VAREJO_CONT = null;
                        }
                        else
                        {
                            trib.RED_BASE_CALC_ST_VENDA_VAREJO_CONT = analise.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenVarContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarContMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarContMassaManualModalPost(string strDados, string redBasCalcAliqIcmsSTVenVarCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsSTVenVarCont = redBasCalcAliqIcmsSTVenVarCont.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.RED_BASE_CALC_ST_VENDA_VAREJO_CONT = (redBasCalcAliqIcmsSTVenVarCont != "") ? trib.RED_BASE_CALC_ST_VENDA_VAREJO_CONT = redBasCalcAliqIcmsSTVenVarCont : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenVarContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

        /*Edição Red Base de Calc ICMS  Venda Atacado para contrbuinte*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaContMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS  Venda Atacado para CONTRIBUINTE no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                       
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                   
                    analiseRetorno = (analise.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_ICMS_VENDA_ATA == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ICMS_VENDA_ATA);
                   

                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.RED_BASE_CALC_ICMS_VENDA_ATA = null;
                        }
                        else
                        {
                            trib.RED_BASE_CALC_ICMS_VENDA_ATA = analise.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenAtaContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaContMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaContMassaManualModalPost(string strDados, string redBasCalcAliqIcmsVenAtaCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsVenAtaCont = redBasCalcAliqIcmsVenAtaCont.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);
                    trib.RED_BASE_CALC_ICMS_VENDA_ATA = (redBasCalcAliqIcmsVenAtaCont != "") ? trib.RED_BASE_CALC_ICMS_VENDA_ATA = redBasCalcAliqIcmsVenAtaCont : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenAtaContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

        /*Edição Red Base de Calc ICMS ST  Venda Atacado para contrbuinte*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaContMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS ST Venda Atacado para CONTRIBUINTE no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO == null).ToList();
                            break;
                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                   
                    analiseRetorno = (analise.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA);
                    

                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA = null;
                        }
                        else
                        {
                            trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA = analise.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenAtaContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaContMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaContMassaManualModalPost(string strDados, string redBasCalcAliqIcmsSTVenAtaCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsSTVenAtaCont = redBasCalcAliqIcmsSTVenAtaCont.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);
                    trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA = (redBasCalcAliqIcmsSTVenAtaCont != "") ? trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA = redBasCalcAliqIcmsSTVenAtaCont : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenAtaContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        /*Edição Red Base de Calc ICMS  Venda Atacado para Simples Nacional*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaSNMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS  Venda Atacado para SIMPLES NACIONAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }
        
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    analiseRetorno = (analise.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL);
                   
                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL = null;
                        }
                        else
                        {
                            trib.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL = analise.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaSNMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaSNMassaManualModalPost(string strDados, string redBasCalcAliqIcmsVenAtaSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsVenAtaSN = redBasCalcAliqIcmsVenAtaSN.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);
                    trib.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL = (redBasCalcAliqIcmsVenAtaSN != "") ? trib.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL = redBasCalcAliqIcmsVenAtaSN : null;
                    trib.DT_ALTERACAO = DateTime.Now;
                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenAtaSNMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

        /*Edição Red Base de Calc ICMS ST Venda Atacado para Simples Nacional*/
        
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS ST Venda Atacado para SIMPLES NACIONAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    analiseRetorno = (analise.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL);
                   
                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = null;
                        }
                        else
                        {
                            trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = analise.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassaManualModalPost(string strDados, string redBasCalcAliqIcmsSTVenAtaSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsSTVenAtaSN = redBasCalcAliqIcmsSTVenAtaSN.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);
                    trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = (redBasCalcAliqIcmsSTVenAtaSN != "") ? trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = redBasCalcAliqIcmsSTVenAtaSN : null;
                    trib.DT_ALTERACAO = DateTime.Now;
                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

        //Edição Aliquota ICMS  compra de industria
        [HttpGet]
        public ActionResult EditClienteAliqIcmsCompIndMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            
            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS Compra de Industria no Cliente X  no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();
           
            if(Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"];
            }


            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
           
            
            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            //List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
            if(TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }
           
               

            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND > a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND < a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND == a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND != null && a.Aliq_Icms_Comp_de_Ind_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND > a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND < a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND == a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND != null && a.Aliq_Icms_Comp_de_Ind_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND > a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND < a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND == a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND != null &&  a.Aliq_Icms_Comp_de_Ind_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";


                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND > a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND < a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND == a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND != null && a.Aliq_Icms_Comp_de_Ind_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND > a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND < a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND == a.Aliq_Icms_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMP_DE_IND != null && a.Aliq_Icms_Comp_de_Ind_INTERNO == null).ToList();
                            break;
                          
                    }
                    break;
                    
                    


            }
            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            
            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }



            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNSalvos = (qtdNSalvos != null) ? qtdNSalvos : "";
            
            int numeroPagina = (page ?? 1);
            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada

           
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsCompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

          
            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                   
                    analiseRetorno = (analise.Aliq_Icms_Comp_de_Ind_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_Comp_de_Ind_INTERNO;

                    analiseTrib = (analise.ALIQ_ICMS_COMP_DE_IND == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_COMP_DE_IND);

                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_COMP_DE_IND = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_COMP_DE_IND = analise.Aliq_Icms_Comp_de_Ind_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }

                   
                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";
               
            }
            catch(Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }

            
            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsCompIndMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsCompIndMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();
            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                //faz a busca no objeto criado instanciando um so objeto
                //trib = db.TributacaoEmpresas.Find(idTrb);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsCompIndMassaManualModalPost(string strDados, string aliqIcmsCompIndClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsCompIndClienteManual = aliqIcmsCompIndClienteManual.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_COMP_DE_IND = (aliqIcmsCompIndClienteManual != "") ? trib.ALIQ_ICMS_COMP_DE_IND = aliqIcmsCompIndClienteManual : null;
                    
                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch(Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsCompIndMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        /*Analise ICMS ST Compra de Industria*/
        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTCompIndMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem para o cabeçalho do card da view
            ViewBag.Mensagem = "Alíquota ICMS ST Compra de Industria no Cliente X  no MTX";

            //Resultado da edição
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"];
            }

           

            //se o filtro corrente estiver nulo ele busca pelo parametro procurar por
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 à pagina caso venham nulo
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;//atribui 1 a pagina caso os parametreos nao sejam nulos

            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;

           
            //viewbag
            ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            //var analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a);

            //List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            ViewBag.Opcao = opcao;

            //Aliquotas ICMS compra industria

            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND > a.Aliq_Icms_ST_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND < a.Aliq_Icms_ST_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND == a.Aliq_Icms_ST_Comp_de_Ind_INTERNO && a.ALIQ_ICMS_ST_COMP_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND != null && a.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND > a.Aliq_Icms_ST_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND < a.Aliq_Icms_ST_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND == a.Aliq_Icms_ST_Comp_de_Ind_INTERNO && a.ALIQ_ICMS_ST_COMP_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND != null && a.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND > a.Aliq_Icms_ST_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND < a.Aliq_Icms_ST_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND == a.Aliq_Icms_ST_Comp_de_Ind_INTERNO && a.ALIQ_ICMS_ST_COMP_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND != null && a.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND > a.Aliq_Icms_ST_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND < a.Aliq_Icms_ST_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND == a.Aliq_Icms_ST_Comp_de_Ind_INTERNO && a.ALIQ_ICMS_ST_COMP_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND != null && a.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND > a.Aliq_Icms_ST_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND < a.Aliq_Icms_ST_Comp_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND == a.Aliq_Icms_ST_Comp_de_Ind_INTERNO && a.ALIQ_ICMS_ST_COMP_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMP_DE_IND != null && a.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
            }//fim switche opcao

            //Action de procura
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);


            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNSalvos = (qtdNSalvos != null) ? qtdNSalvos : "";

            int numeroPagina = (page ?? 1);
            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada


        }


        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTCompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                    analiseRetorno = (decimal)analise.Aliq_Icms_ST_Comp_de_Ind_INTERNO;
                  
                    analiseTrib = (analise.ALIQ_ICMS_ST_COMP_DE_IND == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_ST_COMP_DE_IND);

                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //atribui o valor procurado na analise ao objeto instanciado
                        trib.ALIQ_ICMS_ST_COMP_DE_IND = analise.Aliq_Icms_ST_Comp_de_Ind_INTERNO.ToString().Replace(",", ".");
                        db.SaveChanges();
                        TempData["analise"] = null;
                        regSalv++; //contagem de registros salvos
                    }


                }
               
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsSTCompIndMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv});
        }


        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTCompIndMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();
            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                //faz a busca no objeto criado instanciando um so objeto e adicionando à lista
                 trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);

        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTCompIndMassaManualModalPost(string strDados, string aliqIcmsSTCompIndClienteManual)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTCompIndClienteManual = aliqIcmsSTCompIndClienteManual.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_ST_COMP_DE_IND = (aliqIcmsSTCompIndClienteManual != "") ? trib.ALIQ_ICMS_ST_COMP_DE_IND = aliqIcmsSTCompIndClienteManual : null;

                    db.SaveChanges();
                    regSalvos++;

                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsSTCompIndMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

        /*Analise ICMS COMPRA DE ATACADO*/
        [HttpGet]
        public ActionResult EditClienteAliqIcmsCompAtaMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem para o cabeçalho do card da view
            ViewBag.Mensagem = "Alíquota ICMS Compra de Atacado no Cliente X  no MTX";

            //Resultado da edição
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"];
            }



            //se o filtro corrente estiver nulo ele busca pelo parametro procurar por
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 à pagina caso venham nulo
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;//atribui 1 a pagina caso os parametreos nao sejam nulos

            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            //viewbag
            ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            //var analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a);

            //List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            ViewBag.Opcao = opcao;

            //Aliquotas ICMS compra industria

            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA > a.Aliq_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA < a.Aliq_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA == a.Aliq_Icms_Compra_de_Ata_INTERNO && a.ALIQ_ICMS_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA != null && a.Aliq_Icms_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA > a.Aliq_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA < a.Aliq_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA == a.Aliq_Icms_Compra_de_Ata_INTERNO && a.ALIQ_ICMS_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA != null && a.Aliq_Icms_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA > a.Aliq_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA < a.Aliq_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA == a.Aliq_Icms_Compra_de_Ata_INTERNO && a.ALIQ_ICMS_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA != null && a.Aliq_Icms_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA > a.Aliq_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA < a.Aliq_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA == a.Aliq_Icms_Compra_de_Ata_INTERNO && a.ALIQ_ICMS_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA != null && a.Aliq_Icms_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA > a.Aliq_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA < a.Aliq_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA == a.Aliq_Icms_Compra_de_Ata_INTERNO && a.ALIQ_ICMS_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_ATA != null && a.Aliq_Icms_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
            }//fim switche opcao

            //Action de procura
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);


            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNSalvos = (qtdNSalvos != null) ? qtdNSalvos : "";

            int numeroPagina = (page ?? 1);
            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada


        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsCompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                   
                    analiseRetorno = (analise.Aliq_Icms_Compra_de_Ata_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_Compra_de_Ata_INTERNO;

                    analiseTrib = (analise.ALIQ_ICMS_COMPRA_DE_ATA == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_COMPRA_DE_ATA);

                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_COMPRA_DE_ATA = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_COMPRA_DE_ATA = analise.Aliq_Icms_Compra_de_Ata_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsCompAtaMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsCompAtaMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();
            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                //faz a busca no objeto criado instanciando um so objeto e adicionando à lista
                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);

        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsCompAtaMassaManualModalPost(string strDados, string aliqIcmsCompAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsCompAta = aliqIcmsCompAta.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            

            int regSalv = 0;
            int regNsalv = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_COMPRA_DE_ATA = (aliqIcmsCompAta != "") ? trib.ALIQ_ICMS_COMPRA_DE_ATA = aliqIcmsCompAta : null;
                    trib.DT_ALTERACAO = DateTime.Now;
                    try
                    {
                        db.SaveChanges();
                        regSalv++;
                    }
                    catch (Exception e)
                    {
                        resultado = "Problemas ao salvar o registro: " + e.ToString();
                        regNsalv++;
                    }
                   

                }
                
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsCompAtaMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        /*Analise ICMS ST COMPRA DE ATACADO*/
        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTCompAtaMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem para o cabeçalho do card da view
            ViewBag.Mensagem = "Alíquota ICMS ST Compra de Atacado no Cliente X  no MTX";

            //Resultado da edição
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"];
            }



            //se o filtro corrente estiver nulo ele busca pelo parametro procurar por
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 à pagina caso venham nulo
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;//atribui 1 a pagina caso os parametreos nao sejam nulos

            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            //viewbag
            ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            //var analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a);

            //List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            ViewBag.Opcao = opcao;

            //Aliquotas ICMS compra industria

            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA > a.Aliq_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA < a.Aliq_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == a.Aliq_Icms_ST_Compra_de_Ata_INTERNO && a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null && a.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA > a.Aliq_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA < a.Aliq_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == a.Aliq_Icms_ST_Compra_de_Ata_INTERNO && a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null && a.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA > a.Aliq_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA < a.Aliq_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == a.Aliq_Icms_ST_Compra_de_Ata_INTERNO && a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null && a.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA > a.Aliq_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA < a.Aliq_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == a.Aliq_Icms_ST_Compra_de_Ata_INTERNO && a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null && a.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA > a.Aliq_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA < a.Aliq_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == a.Aliq_Icms_ST_Compra_de_Ata_INTERNO && a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null && a.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
            }//fim switche opcao

            //Action de procura
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);


            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNSalvos = (qtdNSalvos != null) ? qtdNSalvos : "";

            int numeroPagina = (page ?? 1);
            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada


        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTCompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                    
                    analiseRetorno = (analise.Aliq_Icms_ST_Compra_de_Ata_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_ST_Compra_de_Ata_INTERNO;

                    analiseTrib = (analise.ALIQ_ICMS_ST_COMPRA_DE_ATA == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_ST_COMPRA_DE_ATA);

                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_ST_COMPRA_DE_ATA = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_ST_COMPRA_DE_ATA = analise.Aliq_Icms_ST_Compra_de_Ata_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsSTCompAtaMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTCompAtaMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();
            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                //faz a busca no objeto criado instanciando um so objeto e adicionando à lista
                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);

        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTCompAtaMassaManualModalPost(string strDados, string aliqIcmsSTCompAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTCompAta = aliqIcmsSTCompAta.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();


            int regSalv = 0;
            int regNsalv = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_ST_COMPRA_DE_ATA = (aliqIcmsSTCompAta != "") ? trib.ALIQ_ICMS_ST_COMPRA_DE_ATA = aliqIcmsSTCompAta : null;
                    trib.DT_ALTERACAO = DateTime.Now;
                    try
                    {
                        db.SaveChanges();
                        regSalv++;
                    }
                    catch (Exception e)
                    {
                        resultado = "Problemas ao salvar o registro: " + e.ToString();
                        regNsalv++;
                    }


                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsSTCompAtaMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }


        /*Analise ICMS  COMPRA DE SIMPLES NACIONAL*/
        [HttpGet]
        public ActionResult EditClienteAliqIcmsCompSNMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem para o cabeçalho do card da view
            ViewBag.Mensagem = "Alíquota ICMS  Compra de Simples Nacional no Cliente X  no MTX";

            //Resultado da edição
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"];
            }



            //se o filtro corrente estiver nulo ele busca pelo parametro procurar por
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 à pagina caso venham nulo
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;//atribui 1 a pagina caso os parametreos nao sejam nulos

            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            //viewbag
            ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            //var analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a);

            //List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            ViewBag.Opcao = opcao;

            //Aliquotas ICMS compra industria

            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL < a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO && a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL < a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO && a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL < a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO && a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL < a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO && a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL < a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO && a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
            }//fim switche opcao

            //Action de procura
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);


            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNSalvos = (qtdNSalvos != null) ? qtdNSalvos : "";

            int numeroPagina = (page ?? 1);
            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada


        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsCompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                    analiseRetorno = (analise.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO;

                    analiseTrib = (analise.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL);


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL = analise.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsCompSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsCompSNMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();
            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                //faz a busca no objeto criado instanciando um so objeto e adicionando à lista
                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsCompSNMassaManualModalPost(string strDados, string aliqIcmsCompSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsCompSN = aliqIcmsCompSN.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();


            int regSalv = 0;
            int regNsalv = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL = (aliqIcmsCompSN != "") ? trib.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL = aliqIcmsCompSN : null;
                    trib.DT_ALTERACAO = DateTime.Now;
                    try
                    {
                        db.SaveChanges();
                        regSalv++;
                    }
                    catch (Exception e)
                    {
                        resultado = "Problemas ao salvar o registro: " + e.ToString();
                        regNsalv++;
                    }


                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsCompSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }


        /*Analise ICMS ST COMPRA DE SIMPLES NACIONAL*/
        
        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTCompSNMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem para o cabeçalho do card da view
            ViewBag.Mensagem = "Alíquota ICMS ST Compra de Simples Nacional no Cliente X  no MTX";

            //Resultado da edição
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"];
            }



            //se o filtro corrente estiver nulo ele busca pelo parametro procurar por
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 à pagina caso venham nulo
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;//atribui 1 a pagina caso os parametreos nao sejam nulos

            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            //viewbag
            ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            //var analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a);

            //List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            ViewBag.Opcao = opcao;

            //Aliquotas ICMS compra industria

            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL < a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO && a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL < a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO && a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL < a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO && a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL < a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO && a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL < a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO && a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;
                    }
                    break;
            }//fim switche opcao

            //Action de procura
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);


            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNSalvos = (qtdNSalvos != null) ? qtdNSalvos : "";

            int numeroPagina = (page ?? 1);
            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada


        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTCompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                  
                    analiseRetorno = (analise.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO;


                    analiseTrib = (analise.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL);

                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = analise.Aliq_Icms_ST_Compra_de_Simp_Nacional_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsSTCompSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTCompSNMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();
            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                //faz a busca no objeto criado instanciando um so objeto e adicionando à lista
                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }


        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTCompSNMassaManualModalPost(string strDados, string aliqIcmsSTCompSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTCompSN = aliqIcmsSTCompSN.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();


            int regSalv = 0;
            int regNsalv = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = (aliqIcmsSTCompSN != "") ? trib.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = aliqIcmsSTCompSN : null;
                    trib.DT_ALTERACAO = DateTime.Now;
                    try
                    {
                        db.SaveChanges();
                        regSalv++;
                    }
                    catch (Exception e)
                    {
                        resultado = "Problemas ao salvar o registro: " + e.ToString();
                        regNsalv++;
                    }


                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsSTCompSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        /*Analise ICMS NFE COMPRA DE INDUSTRIA*/
        
        [HttpGet]
        public ActionResult EditClienteAliqIcmsNFECompIndMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem para o cabeçalho do card da view
            ViewBag.Mensagem = "Alíquota ICMS NFE compra de Indústria no Cliente X  no MTX";

            //Resultado da edição
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"];
            }



            //se o filtro corrente estiver nulo ele busca pelo parametro procurar por
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 à pagina caso venham nulo
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;//atribui 1 a pagina caso os parametreos nao sejam nulos

            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            //viewbag
            ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            //var analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a);

            //List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            ViewBag.Opcao = opcao;

            //Aliquotas ICMS compra industria

            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE > a.Aliq_Icms_NFE_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE < a.Aliq_Icms_NFE_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE == a.Aliq_Icms_NFE_INTERNO && a.ALIQ_ICMS_NFE != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE != null && a.Aliq_Icms_NFE_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE > a.Aliq_Icms_NFE_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE < a.Aliq_Icms_NFE_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE == a.Aliq_Icms_NFE_INTERNO && a.ALIQ_ICMS_NFE != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE != null && a.Aliq_Icms_NFE_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE > a.Aliq_Icms_NFE_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE < a.Aliq_Icms_NFE_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE == a.Aliq_Icms_NFE_INTERNO && a.ALIQ_ICMS_NFE != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE != null && a.Aliq_Icms_NFE_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE > a.Aliq_Icms_NFE_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE < a.Aliq_Icms_NFE_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE == a.Aliq_Icms_NFE_INTERNO && a.ALIQ_ICMS_NFE != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE != null && a.Aliq_Icms_NFE_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE > a.Aliq_Icms_NFE_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE < a.Aliq_Icms_NFE_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE == a.Aliq_Icms_NFE_INTERNO && a.ALIQ_ICMS_NFE != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE != null && a.Aliq_Icms_NFE_INTERNO == null).ToList();
                            break;
                    }
                    break;
            }//fim switche opcao

            //Action de procura
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);


            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNSalvos = (qtdNSalvos != null) ? qtdNSalvos : "";

            int numeroPagina = (page ?? 1);
            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada


        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsNFECompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                   
                    analiseRetorno = (analise.Aliq_Icms_NFE_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_NFE_INTERNO;

                    analiseTrib = (analise.ALIQ_ICMS_NFE == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_NFE);


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_NFE = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_NFE = analise.Aliq_Icms_NFE_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsNFECompIndMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }
        
        [HttpGet]
        public ActionResult EditClienteAliqIcmsNFECompIndMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();
            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                //faz a busca no objeto criado instanciando um so objeto e adicionando à lista
                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }
        
        [HttpGet]
        public ActionResult EditClienteAliqIcmsNFECompIndMassaManualModalPost(string strDados, string aliqIcmsNFECompInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsNFECompInd = aliqIcmsNFECompInd.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();


            int regSalv = 0;
            int regNsalv = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_NFE = (aliqIcmsNFECompInd != "") ? trib.ALIQ_ICMS_NFE = aliqIcmsNFECompInd : null;
                    trib.DT_ALTERACAO = DateTime.Now;
                    try
                    {
                        db.SaveChanges();
                        regSalv++;
                    }
                    catch (Exception e)
                    {
                        resultado = "Problemas ao salvar o registro: " + e.ToString();
                        regNsalv++;
                    }


                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsNFECompIndMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        /*Analise ICMS NFE COMPRA DE SN*/
        
        [HttpGet]
        public ActionResult EditClienteAliqIcmsNFECompSNMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem para o cabeçalho do card da view
            ViewBag.Mensagem = "Alíquota ICMS NFE compra de SIMPLES NACIONAL no Cliente X  no MTX";

            //Resultado da edição
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"];
            }



            //se o filtro corrente estiver nulo ele busca pelo parametro procurar por
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 à pagina caso venham nulo
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;//atribui 1 a pagina caso os parametreos nao sejam nulos

            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            //viewbag
            ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            //var analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a);

            //List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            ViewBag.Opcao = opcao;

            //Aliquotas ICMS compra industria

            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN > a.Aliq_Icms_NFE_For_SN_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN < a.Aliq_Icms_NFE_For_SN_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN == a.Aliq_Icms_NFE_For_SN_INTERNO && a.ALIQ_ICMS_NFE_FOR_SN != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN != null && a.Aliq_Icms_NFE_For_SN_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN > a.Aliq_Icms_NFE_For_SN_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN < a.Aliq_Icms_NFE_For_SN_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN == a.Aliq_Icms_NFE_For_SN_INTERNO && a.ALIQ_ICMS_NFE_FOR_SN != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN != null && a.Aliq_Icms_NFE_For_SN_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN > a.Aliq_Icms_NFE_For_SN_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN < a.Aliq_Icms_NFE_For_SN_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN == a.Aliq_Icms_NFE_For_SN_INTERNO && a.ALIQ_ICMS_NFE_FOR_SN != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN != null && a.Aliq_Icms_NFE_For_SN_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN > a.Aliq_Icms_NFE_For_SN_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN < a.Aliq_Icms_NFE_For_SN_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN == a.Aliq_Icms_NFE_For_SN_INTERNO && a.ALIQ_ICMS_NFE_FOR_SN != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN != null && a.Aliq_Icms_NFE_For_SN_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN > a.Aliq_Icms_NFE_For_SN_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN < a.Aliq_Icms_NFE_For_SN_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN == a.Aliq_Icms_NFE_For_SN_INTERNO && a.ALIQ_ICMS_NFE_FOR_SN != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_SN != null && a.Aliq_Icms_NFE_For_SN_INTERNO == null).ToList();
                            break;
                    }
                    break;
            }//fim switche opcao

            //Action de procura
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);


            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNSalvos = (qtdNSalvos != null) ? qtdNSalvos : "";

            int numeroPagina = (page ?? 1);
            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada


        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsNFECompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    //pegar valores
                
                    analiseRetorno = (analise.Aliq_Icms_NFE_For_SN_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_NFE_For_SN_INTERNO;


                    analiseTrib = (analise.ALIQ_ICMS_NFE_FOR_SN == null)?analiseTrib : decimal.Parse(trib.ALIQ_ICMS_NFE_FOR_SN);
                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_NFE_FOR_SN = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_NFE_FOR_SN = analise.Aliq_Icms_NFE_For_SN_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsNFECompSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsNFECompSNMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();
            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                //faz a busca no objeto criado instanciando um so objeto e adicionando à lista
                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsNFECompSNMassaManualModalPost(string strDados, string aliqIcmsNFECompSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsNFECompSN = aliqIcmsNFECompSN.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();


            int regSalv = 0;
            int regNsalv = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_NFE_FOR_SN = (aliqIcmsNFECompSN != "") ? trib.ALIQ_ICMS_NFE_FOR_SN = aliqIcmsNFECompSN : null;
                    trib.DT_ALTERACAO = DateTime.Now;
                    try
                    {
                        db.SaveChanges();
                        regSalv++;
                    }
                    catch (Exception e)
                    {
                        resultado = "Problemas ao salvar o registro: " + e.ToString();
                        regNsalv++;
                    }


                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsNFECompSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }




        /*Analise ICMS NFE COMPRA DE ATACADO*/

        [HttpGet]
        public ActionResult EditClienteAliqIcmsNFECompAtaMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem para o cabeçalho do card da view
            ViewBag.Mensagem = "Alíquota ICMS NFE compra de ATACADO no Cliente X  no MTX";

            //Resultado da edição
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else
            {
                this.empresa = (Empresa)Session["empresas"];
            }



            //se o filtro corrente estiver nulo ele busca pelo parametro procurar por
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 à pagina caso venham nulo
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;//atribui 1 a pagina caso os parametreos nao sejam nulos

            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            //viewbag
            ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            //var analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a);

            //List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            ViewBag.Opcao = opcao;

            //Aliquotas ICMS compra industria

            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA > a.Aliq_Icms_NFE_For_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA < a.Aliq_Icms_NFE_For_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA == a.Aliq_Icms_NFE_For_Ata_INTERNO && a.ALIQ_ICMS_NFE_FOR_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA != null && a.Aliq_Icms_NFE_For_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA > a.Aliq_Icms_NFE_For_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA < a.Aliq_Icms_NFE_For_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA == a.Aliq_Icms_NFE_For_Ata_INTERNO && a.ALIQ_ICMS_NFE_FOR_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA != null && a.Aliq_Icms_NFE_For_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA > a.Aliq_Icms_NFE_For_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA < a.Aliq_Icms_NFE_For_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA == a.Aliq_Icms_NFE_For_Ata_INTERNO && a.ALIQ_ICMS_NFE_FOR_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA != null && a.Aliq_Icms_NFE_For_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA > a.Aliq_Icms_NFE_For_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA < a.Aliq_Icms_NFE_For_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA == a.Aliq_Icms_NFE_For_Ata_INTERNO && a.ALIQ_ICMS_NFE_FOR_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA != null && a.Aliq_Icms_NFE_For_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA > a.Aliq_Icms_NFE_For_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA < a.Aliq_Icms_NFE_For_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA == a.Aliq_Icms_NFE_For_Ata_INTERNO && a.ALIQ_ICMS_NFE_FOR_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA == null).ToList(); ;
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ICMS_NFE_FOR_ATA != null && a.Aliq_Icms_NFE_For_Ata_INTERNO == null).ToList();
                            break;
                    }
                    break;
            }//fim switche opcao

            //Action de procura
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);


            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNSalvos = (qtdNSalvos != null) ? qtdNSalvos : "";

            int numeroPagina = (page ?? 1);
            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada


        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsNFECompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();
                    
                    //pegar valores
                  
                    analiseRetorno = (analise.Aliq_Icms_NFE_For_Ata_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Icms_NFE_For_Ata_INTERNO;


                    analiseTrib = (analise.ALIQ_ICMS_NFE_FOR_ATA == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ICMS_NFE_FOR_ATA);
                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        //verificar se a variavel veio 0.0
                        if (analiseRetorno == 0.0M)
                        {
                            //se veio 0.0 o valor deve ser atribuido nulo
                            trib.ALIQ_ICMS_NFE_FOR_ATA = null;
                        }
                        else
                        {
                            trib.ALIQ_ICMS_NFE_FOR_ATA = analise.Aliq_Icms_NFE_For_Ata_INTERNO.ToString().Replace(",", ".");

                        }
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsNFECompAtaMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsNFECompAtaMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();
            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                //faz a busca no objeto criado instanciando um so objeto e adicionando à lista
                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsNFECompAtaMassaManualModalPost(string strDados, string aliqIcmsNFECompAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsNFECompAta = aliqIcmsNFECompAta.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();


            int regSalv = 0;
            int regNsalv = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ICMS_NFE_FOR_ATA = (aliqIcmsNFECompAta != "") ? trib.ALIQ_ICMS_NFE_FOR_ATA = aliqIcmsNFECompAta : null;
                    trib.DT_ALTERACAO = DateTime.Now;
                    try
                    {
                        db.SaveChanges();
                        regSalv++;
                    }
                    catch (Exception e)
                    {
                        resultado = "Problemas ao salvar o registro: " + e.ToString();
                        regNsalv++;
                    }


                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditClienteAliqIcmsNFECompAtaMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }



        /*Edição Red Base de Calc ICMS Compra de Industria*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsCompIndMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS de Compra de INDÚSTRIA no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND > a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND < a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null && a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND > a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND < a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null && a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND > a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND < a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null && a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND > a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND < a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null && a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO == null).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND > a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND < a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null && a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO == null).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsCompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    analiseRetorno = (analise.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_ICMS_COMPRA_DE_IND == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ICMS_COMPRA_DE_IND);


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {

                        if (analiseRetorno == 0.0M)
                        {
                            trib.RED_BASE_CALC_ICMS_COMPRA_DE_IND = null;
                        }
                        else
                        {
                            //atribui o valor procurado na analise ao objeto instanciado
                            trib.RED_BASE_CALC_ICMS_COMPRA_DE_IND = analise.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO.ToString().Replace(",", ".");

                        }

                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsCompIndMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsCompIndMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsCompIndMassaManualModalPost(string strDados, string redBasCalcAliqIcmsCompInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsCompInd = redBasCalcAliqIcmsCompInd.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.RED_BASE_CALC_ICMS_COMPRA_DE_IND = (redBasCalcAliqIcmsCompInd != "") ? trib.RED_BASE_CALC_ICMS_COMPRA_DE_IND = redBasCalcAliqIcmsCompInd : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsCompIndMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

        /*Edição Red Base de Calc ICMS ST Compra de Industria*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTCompIndMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS ST de Compra de INDÚSTRIA no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND > a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND < a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null && a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND > a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND < a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null && a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO == null).ToList();
                            break;


                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND > a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND < a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null && a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND > a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND < a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null && a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND > a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND < a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null && a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO == null).ToList();
                            break;


                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTCompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    //analiseRetorno = (decimal)analise.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO; //da tabela de analise
                    analiseRetorno = (analise.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO;
                 
                    analiseTrib = (analise.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND);

                   
                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        if(analiseRetorno == 0.0M)
                        {
                            trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND = null;
                        }
                        else
                        {
                            //atribui o valor procurado na analise ao objeto instanciado
                            trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND = analise.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO.ToString().Replace(",", ".");
                        }
                      
                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTCompIndMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTCompIndMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTCompIndMassaManualModalPost(string strDados, string redBasCalcAliqIcmsSTCompInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsSTCompInd = redBasCalcAliqIcmsSTCompInd.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND = (redBasCalcAliqIcmsSTCompInd != "") ? trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND = redBasCalcAliqIcmsSTCompInd : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTCompIndMassa", new { param = resultado, qtdSalvos = regSalvos });
        }
        /*Actions auxiliares*/


        
        /*Edição Red Base de Calc ICMS  Compra de Atacado*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsCompAtaMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS  Compra de ATACADO no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA < a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null && a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA < a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null && a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO == null).ToList();
                            break;


                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA < a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null && a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA < a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null && a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA < a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null && a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO == null).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsCompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            decimal analiseRetorno = 0.0M;
            decimal analiseTrib = 0.0M;
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    //analiseRetorno = (decimal)analise.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO; //da tabela de analise
                    analiseRetorno = (analise.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ICMS_COMPRA_DE_ATA);


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++;
                    }
                    else
                    {
                        if (analiseRetorno == 0.0M)
                        {
                            trib.RED_BASE_CALC_ICMS_COMPRA_DE_ATA = null;
                        }
                        else
                        {
                            //atribui o valor procurado na analise ao objeto instanciado
                            trib.RED_BASE_CALC_ICMS_COMPRA_DE_ATA = analise.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO.ToString().Replace(",", ".");
                        }

                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTCompIndMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsCompAtaMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsCompAtaMassaManualModalPost(string strDados, string redBasCalcAliqIcmsCompAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsCompAta = redBasCalcAliqIcmsCompAta.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.RED_BASE_CALC_ICMS_COMPRA_DE_ATA = (redBasCalcAliqIcmsCompAta != "") ? trib.RED_BASE_CALC_ICMS_COMPRA_DE_ATA = redBasCalcAliqIcmsCompAta : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsCompAtaMassa", new { param = resultado, qtdSalvos = regSalvos });
        }


        /*Edição Red Base de Cals ICMS ST compra de Atacado EdtCliAliqRedBasCalcIcmsSTCompAtaMassa*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTCompAtaMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS ST  Compra de ATACADO no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA < a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null && a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA < a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null && a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;



                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA < a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null && a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;


                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA < a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null && a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;


                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA < a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null && a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO == null).ToList();
                            break;


                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTCompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            decimal analiseRetorno = 0.0M; //atribui zero ao valor
            decimal analiseTrib = 0.0M; //atribui zero ao valor
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    /*Caso esteja nulo o retorno do valor a variavel continuar com 0 evitando erro de valores nulos*/
                    analiseRetorno = (analise.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA);


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++; //se são iguais não salva
                    }
                    else
                    { //se são diferentes
                        if (analiseRetorno == 0.0M)
                        {  //se o valor continnuar 0 atribui-se ao valor na base de dados nulo
                            trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA = null;
                        }
                        else
                        {
                            //caso contrario atribui o valor procurado na analise ao objeto instanciado
                            trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA = analise.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO.ToString().Replace(",", ".");
                        }

                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTCompAtaMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTCompAtaMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTCompAtaMassaManualModalPost(string strDados, string redBasCalcAliqIcmsSTCompAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsSTCompAta = redBasCalcAliqIcmsSTCompAta.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA = (redBasCalcAliqIcmsSTCompAta != "") ? trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA = redBasCalcAliqIcmsSTCompAta : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTCompAtaMassa", new { param = resultado, qtdSalvos = regSalvos });
        }


        
        /*Edição Red Base de Cals ICMS  compra de Simples Nacional*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsCompSNMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS Compra de SIMPLES NACIONAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;


                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;


                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL < a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;


                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsCompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            decimal analiseRetorno = 0.0M; //atribui zero ao valor
            decimal analiseTrib = 0.0M; //atribui zero ao valor
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    /*Caso esteja nulo o retorno do valor a variavel continuar com 0 evitando erro de valores nulos*/
                    analiseRetorno = (analise.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL);


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++; //se são iguais não salva
                    }
                    else
                    { //se são diferentes
                        if (analiseRetorno == 0.0M)
                        {  //se o valor continnuar 0 atribui-se ao valor na base de dados nulo
                            trib.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL = null;
                        }
                        else
                        {
                            //caso contrario atribui o valor procurado na analise ao objeto instanciado
                            trib.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL = analise.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO.ToString().Replace(",", ".");
                        }

                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsCompSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsCompSNMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsCompSNMassaManualModalPost(string strDados, string redBasCalcAliqIcmsCompSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsCompSN = redBasCalcAliqIcmsCompSN.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL = (redBasCalcAliqIcmsCompSN != "") ? trib.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL = redBasCalcAliqIcmsCompSN : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsCompSNMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

       
        /*Edição Red Base de Cals ICMS ST  compra de Simples Nacional*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTCompSNMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS ST Compra de SIMPLES NACIONAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL < a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL < a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;



                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL < a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;


                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL < a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;



                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL < a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO && a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null).ToList();
                            break;



                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTCompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            decimal analiseRetorno = 0.0M; //atribui zero ao valor
            decimal analiseTrib = 0.0M; //atribui zero ao valor
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    /*Caso esteja nulo o retorno do valor a variavel continuar com 0 evitando erro de valores nulos*/
                    analiseRetorno = (analise.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO == null) ? analiseRetorno : (decimal)analise.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO;

                    analiseTrib = (analise.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null) ? analiseTrib : decimal.Parse(trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL);


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++; //se são iguais não salva
                    }
                    else
                    { //se são diferentes
                        if (analiseRetorno == 0.0M)
                        {  //se o valor continnuar 0 atribui-se ao valor na base de dados nulo
                            trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = null;
                        }
                        else
                        {
                            //caso contrario atribui o valor procurado na analise ao objeto instanciado
                            trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = analise.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO.ToString().Replace(",", ".");
                        }

                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTCompSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTCompSNMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTCompSNMassaManualModalPost(string strDados, string redBasCalcAliqIcmsSTCompSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            redBasCalcAliqIcmsSTCompSN = redBasCalcAliqIcmsSTCompSN.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = (redBasCalcAliqIcmsSTCompSN != "") ? trib.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = redBasCalcAliqIcmsSTCompSN : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTCompSNMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        /*Edição Aliquota de Pis de entrada*/
        [HttpGet]
        public ActionResult EdtCliAliqEntradaPisMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota de Entrada para PIS no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS > a.Aliq_Ent_Pis_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS < a.Aliq_Ent_Pis_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS == a.Aliq_Ent_Pis_INTERNO && a.ALIQ_ENTRADA_PIS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS != null && a.Aliq_Ent_Pis_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS > a.Aliq_Ent_Pis_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS < a.Aliq_Ent_Pis_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS == a.Aliq_Ent_Pis_INTERNO && a.ALIQ_ENTRADA_PIS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS != null && a.Aliq_Ent_Pis_INTERNO == null).ToList();
                            break;



                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS > a.Aliq_Ent_Pis_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS < a.Aliq_Ent_Pis_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS == a.Aliq_Ent_Pis_INTERNO && a.ALIQ_ENTRADA_PIS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS != null && a.Aliq_Ent_Pis_INTERNO == null).ToList();
                            break;



                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS > a.Aliq_Ent_Pis_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS < a.Aliq_Ent_Pis_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS == a.Aliq_Ent_Pis_INTERNO && a.ALIQ_ENTRADA_PIS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS != null && a.Aliq_Ent_Pis_INTERNO == null).ToList();
                            break;



                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS > a.Aliq_Ent_Pis_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS < a.Aliq_Ent_Pis_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS == a.Aliq_Ent_Pis_INTERNO && a.ALIQ_ENTRADA_PIS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_PIS != null && a.Aliq_Ent_Pis_INTERNO == null).ToList();
                            break;




                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqEntradaPisMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            decimal analiseRetorno = 0.0M; //atribui zero ao valor
            decimal analiseTrib = 0.0M; //atribui zero ao valor
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    /*Caso esteja nulo o retorno do valor a variavel continuar com 0 evitando erro de valores nulos*/
                    analiseRetorno = (analise.Aliq_Ent_Pis_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Ent_Pis_INTERNO;

                    analiseTrib = (analise.ALIQ_ENTRADA_PIS == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ENTRADA_PIS);


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++; //se são iguais não salva
                    }
                    else
                    { //se são diferentes
                        if (analiseRetorno == 0.0M)
                        {  //se o valor continnuar 0 atribui-se ao valor na base de dados nulo
                            trib.ALIQ_ENTRADA_PIS = null;
                        }
                        else
                        {
                            //caso contrario atribui o valor procurado na analise ao objeto instanciado
                            trib.ALIQ_ENTRADA_PIS = analise.Aliq_Ent_Pis_INTERNO.ToString().Replace(",", ".");
                        }

                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqEntradaPisMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqEntradaPisMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqEntradaPisMassaManualModalPost(string strDados, string aliqEntradaPis)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqEntradaPis = aliqEntradaPis.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ENTRADA_PIS = (aliqEntradaPis != "") ? trib.ALIQ_ENTRADA_PIS = aliqEntradaPis : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqEntradaPisMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        /*Edição Aliquota de Pis de saída*/
        [HttpGet]
        public ActionResult EdtCliAliqSaidaPisMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota de Saída para PIS no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS > a.Aliq_Saida_Pis_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS < a.Aliq_Saida_Pis_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS == a.Aliq_Saida_Pis_INTERNO && a.ALIQ_SAIDA_PIS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS != null && a.Aliq_Saida_Pis_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS > a.Aliq_Saida_Pis_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS < a.Aliq_Saida_Pis_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS == a.Aliq_Saida_Pis_INTERNO && a.ALIQ_SAIDA_PIS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS != null && a.Aliq_Saida_Pis_INTERNO == null).ToList();
                            break;


                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS > a.Aliq_Saida_Pis_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS < a.Aliq_Saida_Pis_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS == a.Aliq_Saida_Pis_INTERNO && a.ALIQ_SAIDA_PIS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS != null && a.Aliq_Saida_Pis_INTERNO == null).ToList();
                            break;



                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS > a.Aliq_Saida_Pis_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS < a.Aliq_Saida_Pis_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS == a.Aliq_Saida_Pis_INTERNO && a.ALIQ_SAIDA_PIS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS != null && a.Aliq_Saida_Pis_INTERNO == null).ToList();
                            break;



                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS > a.Aliq_Saida_Pis_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS < a.Aliq_Saida_Pis_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS == a.Aliq_Saida_Pis_INTERNO && a.ALIQ_SAIDA_PIS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_SAIDA_PIS != null && a.Aliq_Saida_Pis_INTERNO == null).ToList();
                            break;




                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqSaidaPisMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            decimal analiseRetorno = 0.0M; //atribui zero ao valor
            decimal analiseTrib = 0.0M; //atribui zero ao valor
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    /*Caso esteja nulo o retorno do valor a variavel continuar com 0 evitando erro de valores nulos*/
                    analiseRetorno = (analise.Aliq_Saida_Pis_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Saida_Pis_INTERNO;

                    analiseTrib = (analise.ALIQ_SAIDA_PIS == null) ? analiseTrib : decimal.Parse(trib.ALIQ_SAIDA_PIS);


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++; //se são iguais não salva
                    }
                    else
                    { //se são diferentes
                        if (analiseRetorno == 0.0M)
                        {  //se o valor continnuar 0 atribui-se ao valor na base de dados nulo
                            trib.ALIQ_SAIDA_PIS = null;
                        }
                        else
                        {
                            //caso contrario atribui o valor procurado na analise ao objeto instanciado
                            trib.ALIQ_SAIDA_PIS = analise.Aliq_Saida_Pis_INTERNO.ToString().Replace(",", ".");
                        }

                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqSaidaPisMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqSaidaPisMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqSaidaPisMassaManualModalPost(string strDados, string aliqSaidaPis)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqSaidaPis = aliqSaidaPis.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_SAIDA_PIS = (aliqSaidaPis != "") ? trib.ALIQ_SAIDA_PIS = aliqSaidaPis : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqSaidaPisMassa", new { param = resultado, qtdSalvos = regSalvos });
        }


        /*Edição Aliquota Cofins de Entrada*/
        [HttpGet]
        public ActionResult EdtCliAliqEntCofinsMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota de Saída para PIS no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS > a.Aliq_Ent_Cofins_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS < a.Aliq_Ent_Cofins_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS == a.Aliq_Ent_Cofins_INTERNO && a.ALIQ_ENTRADA_COFINS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS != null && a.Aliq_Ent_Cofins_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS > a.Aliq_Ent_Cofins_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS < a.Aliq_Ent_Cofins_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS == a.Aliq_Ent_Cofins_INTERNO && a.ALIQ_ENTRADA_COFINS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS != null && a.Aliq_Ent_Cofins_INTERNO == null).ToList();
                            break;


                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS > a.Aliq_Ent_Cofins_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS < a.Aliq_Ent_Cofins_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS == a.Aliq_Ent_Cofins_INTERNO && a.ALIQ_ENTRADA_COFINS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS != null && a.Aliq_Ent_Cofins_INTERNO == null).ToList();
                            break;



                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS > a.Aliq_Ent_Cofins_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS < a.Aliq_Ent_Cofins_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS == a.Aliq_Ent_Cofins_INTERNO && a.ALIQ_ENTRADA_COFINS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS != null && a.Aliq_Ent_Cofins_INTERNO == null).ToList();
                            break;


                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS > a.Aliq_Ent_Cofins_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS < a.Aliq_Ent_Cofins_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS == a.Aliq_Ent_Cofins_INTERNO && a.ALIQ_ENTRADA_COFINS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_ENTRADA_COFINS != null && a.Aliq_Ent_Cofins_INTERNO == null).ToList();
                            break;



                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqEntCofinsMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            decimal analiseRetorno = 0.0M; //atribui zero ao valor
            decimal analiseTrib = 0.0M; //atribui zero ao valor
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    /*Caso esteja nulo o retorno do valor a variavel continuar com 0 evitando erro de valores nulos*/
                    analiseRetorno = (analise.Aliq_Ent_Cofins_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Ent_Cofins_INTERNO;

                    analiseTrib = (analise.ALIQ_ENTRADA_COFINS == null) ? analiseTrib : decimal.Parse(trib.ALIQ_ENTRADA_COFINS);


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++; //se são iguais não salva
                    }
                    else
                    { //se são diferentes
                        if (analiseRetorno == 0.0M)
                        {  //se o valor continnuar 0 atribui-se ao valor na base de dados nulo
                            trib.ALIQ_ENTRADA_COFINS = null;
                        }
                        else
                        {
                            //caso contrario atribui o valor procurado na analise ao objeto instanciado
                            trib.ALIQ_ENTRADA_COFINS = analise.Aliq_Ent_Cofins_INTERNO.ToString().Replace(",", ".");
                        }

                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqEntCofinsMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqEntCofinsMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqEntCofinsMassaManualModalPost(string strDados, string aliqEntCofins)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqEntCofins = aliqEntCofins.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_ENTRADA_COFINS = (aliqEntCofins != "") ? trib.ALIQ_ENTRADA_COFINS = aliqEntCofins : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqEntCofinsMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



     
        /*Edição Aliquota cofins de saída*/
        [HttpGet]
        public ActionResult EdtCliAliqSaiCofinsMassa(string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota de Saída para COFINS no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            /*Inicializando a variavel de sessão usarios caso nao exista*/
            if (Session["usuarios"] == null)
            {
                this.usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault(); //usuario
                this.empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault(); //empresa
                Session["usuarios"] = usuario;
                Session["empresas"] = empresa;
            }
            else //se existir apenas atribui à empresa
            {
                this.empresa = (Empresa)Session["empresas"];
            }

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            /*Para tipar*/ /*Ponto de ajuste: verificar se houve alteração na lista se houver instancia novamente caso contrario passa*/
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista carregue novamente
             */
            if (TempData["analise"] == null)
            {
                this.analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == this.empresa.cnpj select a).ToList();
                TempData["analise"] = this.analise;
                TempData.Keep("analise");
            }
            else
            {
                this.analise = (List<AnaliseTributaria>)TempData["analise"];
                TempData.Keep("analise");
            }

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS > a.Aliq_Saida_Cofins_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS < a.Aliq_Saida_Cofins_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS == a.Aliq_Saida_Cofins_INTERNO && a.ALIQ_SAIDA_COFINS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS != null && a.Aliq_Saida_Cofins_INTERNO == null).ToList();
                            break;

                    }
                    break;
                case "Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS > a.Aliq_Saida_Cofins_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS < a.Aliq_Saida_Cofins_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS == a.Aliq_Saida_Cofins_INTERNO && a.ALIQ_SAIDA_COFINS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS != null && a.Aliq_Saida_Cofins_INTERNO == null).ToList();
                            break;



                    }
                    break;
                case "Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS > a.Aliq_Saida_Cofins_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS < a.Aliq_Saida_Cofins_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS == a.Aliq_Saida_Cofins_INTERNO && a.ALIQ_SAIDA_COFINS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS != null && a.Aliq_Saida_Cofins_INTERNO == null).ToList();
                            break;




                    }
                    break;
                case "Nulas Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS > a.Aliq_Saida_Cofins_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS < a.Aliq_Saida_Cofins_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS == a.Aliq_Saida_Cofins_INTERNO && a.ALIQ_SAIDA_COFINS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS != null && a.Aliq_Saida_Cofins_INTERNO == null).ToList();
                            break;


                    }
                    break;
                case "Nulas MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS > a.Aliq_Saida_Cofins_INTERNO).ToList();
                            break;
                        case "2":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS < a.Aliq_Saida_Cofins_INTERNO).ToList();
                            break;
                        case "3":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS == a.Aliq_Saida_Cofins_INTERNO && a.ALIQ_SAIDA_COFINS != null).ToList();
                            break;
                        case "4":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS == null).ToList();
                            break;
                        case "5":
                            analise = analise.Where(a => a.ALIQ_SAIDA_COFINS != null && a.Aliq_Saida_Cofins_INTERNO == null).ToList();
                            break;




                    }
                    break;


            }//fim do switche

            //Action para procurar
            analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);

            switch (ordenacao)
            {
                case "Produto_desc":
                    analise = analise.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    analise = analise.OrderBy(s => s.Id_Produto_INTERNO).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            int numeroPagina = (page ?? 1);

            return View(analise.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EdtCliAliqSaiCofinsMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            decimal analiseRetorno = 0.0M; //atribui zero ao valor
            decimal analiseTrib = 0.0M; //atribui zero ao valor
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //faz a busca pelo CODIGO DE BARRAS DO PRODUTO e instancia um outro objeto da analise tributária
                    AnaliseTributaria analise = (from a in db.Analise_Tributaria where a.PRODUTO_COD_BARRAS == trib.PRODUTO_COD_BARRAS select a).FirstOrDefault();

                    //pegar valores
                    /*Caso esteja nulo o retorno do valor a variavel continuar com 0 evitando erro de valores nulos*/
                    analiseRetorno = (analise.Aliq_Saida_Cofins_INTERNO == null) ? analiseRetorno : (decimal)analise.Aliq_Saida_Cofins_INTERNO;

                    analiseTrib = (analise.ALIQ_SAIDA_COFINS == null) ? analiseTrib : decimal.Parse(trib.ALIQ_SAIDA_COFINS);


                    //analisar se já são iguais
                    if (analiseTrib == analiseRetorno)
                    {
                        regNsalv++; //se são iguais não salva
                    }
                    else
                    { //se são diferentes
                        if (analiseRetorno == 0.0M)
                        {  //se o valor continnuar 0 atribui-se ao valor na base de dados nulo
                            trib.ALIQ_SAIDA_COFINS = null;
                        }
                        else
                        {
                            //caso contrario atribui o valor procurado na analise ao objeto instanciado
                            trib.ALIQ_SAIDA_COFINS = analise.Aliq_Saida_Cofins_INTERNO.ToString().Replace(",", ".");
                        }

                        db.SaveChanges();
                        regSalv++; //contagem de registros salvos
                    }


                }
                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqSaiCofinsMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }

        [HttpGet]
        public ActionResult EdtCliAliqSaiCofinsMassaManualModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Objeto do tipo tributação empresa
            //TributacaoEmpresa trib = new TributacaoEmpresa();
            List<TributacaoEmpresa> trib;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();
            trib = new List<TributacaoEmpresa>();

            for (int i = 0; i < idTrib.Length; i++)
            {
                //converter em inteiro
                int idTrb = int.Parse(idTrib[i]);

                trib.Add(db.TributacaoEmpresas.Find(idTrb));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EdtCliAliqSaiCofinsMassaManualModalPost(string strDados, string aliqSaiCofins)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqSaiCofins = aliqSaiCofins.Replace(",", ".");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');
            string resultado = "";
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            TributacaoEmpresa trib = new TributacaoEmpresa();
            int regSalvos = 0;
            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    trib.ALIQ_SAIDA_COFINS = (aliqSaiCofins != "") ? trib.ALIQ_SAIDA_COFINS = aliqSaiCofins : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["analise"] = null;
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqSaiCofinsMassa", new { param = resultado, qtdSalvos = regSalvos });
        }
        //Recebe os parametros e faz a busca na tabela por codigo de barras e cest
        [HttpGet]
        public List<AnaliseTributaria> ProcuraPor(long? codBarrasL, string procuraPor, string procuraCEST, string procuraNCM, List<AnaliseTributaria> analise)
        {
            if (!String.IsNullOrEmpty(procuraPor))
            {
                analise = (codBarrasL != 0) ? (analise.Where(s => s.PRODUTO_COD_BARRAS.ToString().Contains(codBarrasL.ToString()))).ToList() : analise = (analise.Where(s => s.PRODUTO_DESCRICAO.ToString().ToUpper().Contains(procuraPor.ToUpper()))).ToList();
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                analise = analise.Where(s => s.PRODUTO_CEST == procuraCEST).ToList();
                //analise = analise.Where(s => s.PRODUTO_CEST.ToString().Contains(procuraCEST.ToString())).ToList();
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                analise = analise.Where(s => s.PRODUTO_NCM == procuraNCM).ToList();
                //analise = analise.Where(s => s.PRODUTO_CEST.ToString().Contains(procuraCEST.ToString())).ToList();
            }

            return analise;
           
        }


        //procurapor diferenciado para tabela de produto
        [HttpGet]
        public List<AnaliseTributaria> ProcuraPorTabelaProduto(string filtroDados, int? parFiltro, List<AnaliseTributaria> analise)
        {
            string procuraPor= null;
            string procuraCEST=null;
            string procuraNCM=null;
            long codBarrasla = 0;

            switch (parFiltro)
            {
                case 1:
                    procuraCEST = filtroDados;
                    break;
                case 2:
                    procuraNCM = filtroDados;
                    break;
                case 3:
                    procuraPor = filtroDados;
                   
                    bool canConvert = long.TryParse(procuraPor, out codBarrasla);
                    break;

            }
            if (!String.IsNullOrEmpty(procuraPor))
            {
                analise = (codBarrasla != 0) ? (analise.Where(s => s.PRODUTO_COD_BARRAS.ToString().Contains(codBarrasla.ToString()))).ToList() : analise = (analise.Where(s => s.PRODUTO_DESCRICAO.ToString().ToUpper().Contains(procuraPor.ToUpper()))).ToList();
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                analise = analise.Where(s => s.PRODUTO_CEST == procuraCEST).ToList();
                //analise = analise.Where(s => s.PRODUTO_CEST.ToString().Contains(procuraCEST.ToString())).ToList();
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                analise = analise.Where(s => s.PRODUTO_NCM == procuraNCM).ToList();
                //analise = analise.Where(s => s.PRODUTO_CEST.ToString().Contains(procuraCEST.ToString())).ToList();
            }

            return analise;
        }

        //procura por diferenciado para tabela de produto
        [HttpGet]
        public List<AnaliseTributaria2> ProcuraPorTabelaProduto2(string filtroDados, int? parFiltro, List<AnaliseTributaria2> analise)
        {
            string procuraPor = null;
            string procuraCEST = null;
            string procuraNCM = null;
            long codBarrasla = 0;

            switch (parFiltro)
            {
                case 1:
                    procuraCEST = filtroDados;
                    break;
                case 2:
                    procuraNCM = filtroDados;
                    break;
                case 3:
                    procuraPor = filtroDados;

                    bool canConvert = long.TryParse(procuraPor, out codBarrasla);
                    break;

            }
            if (!String.IsNullOrEmpty(procuraPor))
            {
                analise = (codBarrasla != 0) ? (analise.Where(s => s.PRODUTO_COD_BARRAS.ToString().Contains(codBarrasla.ToString()))).ToList() : analise = (analise.Where(s => s.PRODUTO_DESCRICAO.ToString().ToUpper().Contains(procuraPor.ToUpper()))).ToList();
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                analise = analise.Where(s => s.PRODUTO_CEST == procuraCEST).ToList();
                //analise = analise.Where(s => s.PRODUTO_CEST.ToString().Contains(procuraCEST.ToString())).ToList();
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                analise = analise.Where(s => s.PRODUTO_NCM == procuraNCM).ToList();
                //analise = analise.Where(s => s.PRODUTO_CEST.ToString().Contains(procuraCEST.ToString())).ToList();
            }

            return analise;
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
