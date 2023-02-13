﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{
    [Table("tributacao_produtos_sn")]
    public class TributacaoSN
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int id { get; set; }


        //[Column("Estado")]
        //public string estado { get; set; }

        [Column("UF_Origem")]
        public string UF_Origem { get; set; }

        [Column("UF_Destino")]
        public string UF_Destino { get; set; }

        [ForeignKey("produtos")]
        [Column("Id_Produto")]
        public int idProduto { get; set; }


        [ForeignKey("setorProdutos")]
        [Column("Id_Setor")]
        public int idSetor { get; set; }

        [Column("Fecp")]
        public Nullable<decimal> fecp { get; set; }

        [ForeignKey("naturezaReceita")]
        [Column("Cod_Nat_Receita")]
        public Nullable<int> codNatReceita { get; set; }


        [ForeignKey("cstPisCofinsE")]
        [Column("Cst_Entrada_PisCofins")]
        public Nullable<int> cstEntradaPisCofins { get; set; }

        [ForeignKey("cstPisCofinsS")]
        [Column("Cst_Saida_PisCofins")]
        public int? cstSaidaPisCofins { get; set; }


        [Column("Aliq_Ent_Pis")]
        public Nullable<decimal> aliqEntPis { get; set; }

        [Column("Aliq_Saida_Pis")]
        public decimal? aliqSaidaPis { get; set; }

        [Column("Aliq_Ent_Cofins")]
        public Nullable<decimal> aliqEntCofins { get; set; }

        [Column("Aliq_Saida_Cofins")]
        public Nullable<decimal> aliqSaidaCofins { get; set; }


        [ForeignKey("fundamentoLegal")]
        [Column("Id_Fundamento_Legal")]
        public Nullable<int> idFundamentoLegal { get; set; }


        [Column("Cst_Venda_Ata_Cont")]
        public Nullable<int> cstVendaAtaCont { get; set; }

        [Column("Aliq_Icms_Venda_Ata_Cont")]
        public Nullable<decimal> aliqIcmsVendaAtaCont { get; set; }

        [Column("Aliq_Icms_ST_Venda_Ata_Cont")]
        public Nullable<decimal> aliqIcmsSTVendaAtaCont { get; set; }

        [Column("Red_Base_Calc_Icms_Venda_Ata_Cont")]
        public Nullable<decimal> redBaseCalcIcmsVendaAtaCont { get; set; }

        [Column("Red_Base_Calc_Icms_ST_Venda_Ata_Cont")]
        public Nullable<decimal> redBaseCalcIcmsSTVendaAtaCont { get; set; }


        [Column("Cst_Venda_Ata_Simp_Nacional")]
        public Nullable<int> cstVendaAtaSimpNacional { get; set; }

        [Column("Aliq_Icms_Venda_Ata_Simp_Nacional")]
        public Nullable<decimal> aliqIcmsVendaAtaSimpNacional { get; set; }

        [Column("Aliq_Icms_ST_Venda_Ata_Simp_Nacional")]
        public Nullable<decimal> aliqIcmsSTVendaAtaSimpNacional { get; set; }

        [Column("Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional")]
        public Nullable<decimal> redBaseCalcIcmsVendaAtaSimpNacional { get; set; }

        [Column("Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional")]
        public Nullable<decimal> redBaseCalcIcmsSTVendaAtaSimpNacional { get; set; }


        [Column("Cst_Venda_Varejo_Cont")]
        public Nullable<int> cstVendaVarejoCont { get; set; }

        [Column("Aliq_Icms_Venda_Varejo_Cont")]
        public Nullable<decimal> aliqIcmsVendaVarejoCont { get; set; }

        [Column("Aliq_Icms_ST_Venda_Varejo_Cont")]
        public Nullable<decimal> aliqIcmsSTVendaVarejo_Cont { get; set; }

        [Column("Red_Base_Calc_Venda_Varejo_Cont")]
        public Nullable<decimal> redBaseCalcVendaVarejoCont { get; set; }

        [Column("Red_Base_Calc_ST_Venda_Varejo_Cont")]
        public Nullable<decimal> RedBaseCalcSTVendaVarejo_Cont { get; set; }


        [Column("Cst_Venda_Varejo_Cons_Final")]
        public Nullable<int> cstVendaVarejoConsFinal { get; set; }

        [Column("Aliq_Icms_Venda_Varejo_Cons_Final")]
        public Nullable<decimal> aliqIcmsVendaVarejoConsFinal { get; set; }

        [Column("Aliq_Icms_ST_Venda_Varejo_Cons_Final")]
        public Nullable<decimal> aliqIcmsSTVendaVarejoConsFinal { get; set; }

        [Column("Red_Base_Calc_Icms_Venda_Varejo_Cons_Final")]
        public Nullable<decimal> redBaseCalcIcmsVendaVarejoConsFinal { get; set; }

        [Column("Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final")]
        public Nullable<decimal> redBaseCalcIcmsSTVendaVarejoConsFinal { get; set; }


        [Column("Id_Fund_Legal_Saida_ICMS")]
        public Nullable<int> idFundLegalSaidaICMS { get; set; }


        [Column("Id_Fund_Lelgal_Entrada_ICMS")]
        public Nullable<int> idFundLelgalEntradaICMS { get; set; }


        [Column("Cst_Compra_de_Ind")]
        public Nullable<int> cstCompraDeInd { get; set; }

        [Column("Aliq_Icms_Comp_de_Ind")]
        public Nullable<decimal> aliqIcmsCompDeInd { get; set; }

        [Column("Aliq_Icms_ST_Comp_de_Ind")]
        public Nullable<decimal> aliqIcmsSTCompDeInd { get; set; }

        [Column("Red_Base_Calc_Icms_Compra_de_Ind")]
        public Nullable<decimal> redBaseCalcIcmsCompraDeInd { get; set; }

        [Column("Red_Base_Calc_Icms_ST_Compra_de_Ind")]
        public Nullable<decimal> redBaseCalcIcmsSTCompraDeInd { get; set; }


        [Column("Cst_Compra_de_Ata")]
        public Nullable<int> cstCompradeAta { get; set; }

        [Column("Aliq_Icms_Compra_de_Ata")]
        public Nullable<decimal> aliqIcmsCompradeAta { get; set; }

        [Column("Aliq_Icms_ST_Compra_de_Ata")]
        public Nullable<decimal> aliqIcmsSTCompraDeAta { get; set; }

        [Column("Red_Base_Calc_Icms_Compra_de_Ata")]
        public Nullable<decimal> redBaseCalcIcmsCompraDeAta { get; set; }

        [Column("Red_Base_Calc_Icms_ST_Compra_de_Ata")]
        public Nullable<decimal> redBaseCalcIcmsSTCompraDeAta { get; set; }


        [Column("Cst_Compra_de_Simp_Nacional")]
        public Nullable<int> cstCompradeSimpNacional { get; set; }

        [Column("Aliq_Icms_Compra_de_Simp_Nacional")]
        public Nullable<decimal> aliqIcmsCompradeSimpNacional { get; set; }

        [Column("Aliq_Icms_ST_Compra_de_Simp_Nacional")]
        public Nullable<decimal> aliqIcmsSTCompradeSimpNacional { get; set; }

        [Column("Red_Base_Calc_Icms_Compra_de_Simp_Nacional")]
        public Nullable<decimal> redBaseCalcIcmsCompradeSimpNacional { get; set; }

        [Column("Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional")]
        public Nullable<decimal> redBaseCalcIcmsSTCompradeSimpNacional { get; set; }


        [Column("Cst_da_Nfe_da_Ind_FORN")]
        public Nullable<int> cstdaNfedaIndFORN { get; set; }


        [Column("Cst_da_Nfe_de_Ata_FORn")]
        public Nullable<int> cstdaNfedeAtaFORn { get; set; }


        [Column("CsosntdaNfedoSnFOR")]
        public Nullable<int> CsosntdaNfedoSnFOR { get; set; }

        [Column("Aliq_Icms_NFE")]
        public Nullable<decimal> aliqIcmsNFE { get; set; }

        [Column("Aliq_Icms_NFE_For_SN")]
        public Nullable<decimal> aliqIcmsNfeSN { get; set; }

        [Column("Aliq_Icms_NFE_For_Ata")]
        public Nullable<decimal> aliqIcmsNfeAta { get; set; }


        [Column("Tipo_MVA")]
        public string tipoMVA { get; set; }

        [Column("ValorMVAInd")]
        public Nullable<decimal> valorMVAInd { get; set; }

        [Column("Inicio_Vigencia_MVA")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        public Nullable<System.DateTime> inicioVigenciaMVA { get; set; }

        [Column("Fim_Vigencia_MVA")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        public Nullable<System.DateTime> fimVigenciaMVA { get; set; }

        [Column("Credito_Outorgado")]
        public Nullable<sbyte> creditoOutorgado { get; set; }

        [Column("Valor_MVA_Atacado")]
        public Nullable<decimal> valorMVAAtacado { get; set; }

        [Column("Regime_2560")]
        public Nullable<sbyte> regime2560 { get; set; }

        [Column("Auditado_porNCM")]
        public Nullable<sbyte> auditadoPorNCM { get; set; }

        [Column("DataCad")]
        public DateTime? dataCad { get; set; }

        [Column("DataAlt")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        public DateTime? dataAlt { get; set; }

        public string Data
        {
            get { return dataAlt?.ToShortDateString(); }
        }

        public virtual CstPisCofinsEntrada cstPisCofinsE { get; set; }

        public virtual CstPisCofinsSaida cstPisCofinsS { get; set; }



        public virtual Legislacao fundamentoLegal { get; set; }

        //public virtual CstIcmsGeral CstCompradeAta { get; set; }

        public virtual NaturezaReceita naturezaReceita { get; set; }

        public virtual Produto produtos { get; set; }
        public virtual SetorProdutos setorProdutos { get; set; }
    }
}