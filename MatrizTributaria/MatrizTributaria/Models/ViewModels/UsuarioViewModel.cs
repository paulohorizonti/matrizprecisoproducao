﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models.ViewModels
{
    public class UsuarioViewModel
    {


        public int id { get; set; }

        [Required(ErrorMessage = "O nome do Usuário é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Nome")]
        public string nome { get; set; }

        [Index(IsUnique = true)]
        [Required(ErrorMessage = "O e-mail é obrigatório", AllowEmptyStrings = false)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Insira uma e-mail válido")]
        public string email { get; set; }

        [Required(ErrorMessage = "O campo Sexo é obrigatório", AllowEmptyStrings = false)]
        public string sexo { get; set; }

        [Required(ErrorMessage = "O campo logradouro é obrigatório", AllowEmptyStrings = false)]
        public string logradouro { get; set; }

        [Required(ErrorMessage = "O campo número é obrigatório, caso não exista digiar: 0", AllowEmptyStrings = false)]
        public string numero { get; set; }

        [Required(ErrorMessage = "O campo CEP é obrigatório", AllowEmptyStrings = false)]
        public string cep { get; set; }

        [Required(ErrorMessage = "Campo senha é obrigatório")]
        public string senha { get; set; }


        public sbyte ativo { get; set; }


        public DateTime dataCad { get; set; }


        public DateTime dataAlt { get; set; }

        [Required(ErrorMessage = "Selecione um nível", AllowEmptyStrings = false)]
        public int idNivel { get; set; }


        public string telefone { get; set; }

        [Required(ErrorMessage = "Campo Cidade é obrigatório")]
        public string cidade { get; set; }

        [Required(ErrorMessage = "Campo Estado é obrigatório")]
        public EstadosEnun estado { get; set; }

        [Required(ErrorMessage = "Campo Empresa é obrigatório")]
        public int idEmpresa { get; set; }

       
        public sbyte primeiro_acesso { get; set; }

    }

    public enum EstadosEnun
    {
        AC,
        AL,
        AP,
        AM,
        BA,
        CE,
        DF,
        ES,
        GO,
        MA,
        MT,
        MS,
        MG,
        PA,
        PB,
        PR,
        PE,
        PI,
        RJ,
        RN,
        RS,
        RO,
        RR,
        SC,
        SP,
        SE,
        TO

    }
}