﻿/*=============== ATUALIZAÇÃO ALIQUOTA SAÍDA COFINS NO CLIENTE  =====================*/
/*Alteração ALIQUOTA SAÍDA COFINS NO CLIENTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-edtAlqSaiCofins"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelAliqSaiCofinsCli').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelAliqSaiCofinsCli"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqSaiCofinsMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    //fiz alterações aqui de novo

    $('#edtSelAliqSaiCofinsCliManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelAliqSaiCofinsCliManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqSaiCofinsMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQUOTA SAÍDA COFINS NO CLIENTE MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveAliqCofinsSaiCliManual = document.getElementById("saveAliqCofinsSaiCliManual"); //variavel para receber o botao de salvar
    if (saveAliqCofinsSaiCliManual) {
        saveAliqCofinsSaiCliManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqSaiCofins = document.getElementById("aliCofinsSaiCliManual").value; //pegar o valor do imput
            if (aliqSaiCofins) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqSaiCofins: aliqSaiCofins },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqSaiCofinsMassaManualModalPost?strDados=' + strDados + '&aliqSaiCofins=' + aliqSaiCofins;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqSaiCofins: aliqSaiCofins },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqSaiCofinsMassaManualModalPost?strDados=' + strDados + '&aliqSaiCofins=' + aliqSaiCofins;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliCofinsSaiCliManual").focus();
                }

            }

        });
    }

});




/*=============== ATUALIZAÇÃO ALIQUOTA ENTRADA COFINS NO CLIENTE  =====================*/
/*Alteração ALIQUOTA ENTRADA COFINS NO CLIENTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-edtAlqEntCofins"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelAliqEntCofinsCli').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelAliqEntCofinsCli"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqEntCofinsMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    //fiz alterações aqui de novo

    $('#edtSelAliqEntCofinsCliManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelAliqEntCofinsCliManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqEntCofinsMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQUOTA ENTRADA COFINS NO CLIENTE MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveAliqCofinsEntCliManual = document.getElementById("saveAliqCofinsEntCliManual"); //variavel para receber o botao de salvar
    if (saveAliqCofinsEntCliManual) {
        saveAliqCofinsEntCliManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqEntCofins = document.getElementById("aliqCofinsEntCliManual").value; //pegar o valor do imput
            if (aliqEntCofins) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqEntCofins: aliqEntCofins },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqEntCofinsMassaManualModalPost?strDados=' + strDados + '&aliqEntCofins=' + aliqEntCofins;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqEntCofins: aliqEntCofins },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqEntCofinsMassaManualModalPost?strDados=' + strDados + '&aliqEntCofins=' + aliqEntCofins;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqCofinsEntCliManual").focus();
                }

            }

        });
    }

});




/*=============== ATUALIZAÇÃO ALIQUOTA SAÍDA PIS NO CLIENTE  =====================*/
/*Alteração ALIQUOTA SAÍDA PIS NO CLIENTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-edtAlqSaiPis"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelAliqSaiPisCli').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelAliqSaiPisCli"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqSaidaPisMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    //fiz alterações aqui de novo

    $('#edtSelAliqSaiPisCliManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelAliqSaiPisCliManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqSaidaPisMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQUOTA SAÍDA PIS NO CLIENTE MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveAliqPisSaiCliManual = document.getElementById("saveAliqPisSaiCliManual"); //variavel para receber o botao de salvar
    if (saveAliqPisSaiCliManual) {
        saveAliqPisSaiCliManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqSaidaPis = document.getElementById("aliqPisSaiCliManual").value; //pegar o valor do imput
            if (aliqSaidaPis) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqSaidaPis: aliqSaidaPis },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqSaidaPisMassaManualModalPost?strDados=' + strDados + '&aliqSaidaPis=' + aliqSaidaPis;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqSaidaPis: aliqSaidaPis },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqSaidaPisMassaManualModalPost?strDados=' + strDados + '&aliqSaidaPis=' + aliqSaidaPis;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqPisSaiCliManual").focus();
                }

            }

        });
    }

});







/*=============== ATUALIZAÇÃO ALIQUOTA ENTRADA PIS NO CLIENTE  =====================*/
/*Alteração ALIQUOTA ENTRADA PIS NO CLIENTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-edtAlqEntPis"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelAliqEntPisCli').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelAliqEntPisCli"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqEntradaPisMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    //fiz alterações aqui de novo

    $('#edtSelAliqEntPisCliManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelAliqEntPisCliManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqEntradaPisMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar LIQUOTA ENTRADA PIS NO CLIENTE MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveAliqPisEntCliManual = document.getElementById("saveAliqPisEntCliManual"); //variavel para receber o botao de salvar
    if (saveAliqPisEntCliManual) {
        saveAliqPisEntCliManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqEntradaPis = document.getElementById("aliqPisEntCliManual").value; //pegar o valor do imput
            if (aliqEntradaPis) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqEntradaPis: aliqEntradaPis },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqEntradaPisMassaManualModalPost?strDados=' + strDados + '&aliqEntradaPis=' + aliqEntradaPis;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqEntradaPis: aliqEntradaPis },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqEntradaPisMassaManualModalPost?strDados=' + strDados + '&aliqEntradaPis=' + aliqEntradaPis;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqPisEntCliManual").focus();
                }

            }

        });
    }

});








/*=============== ATUALIZAÇÃO RED BAS CALC ALIQ ICMS ST COMPRA DE SIMPLES NACIONAL =====================*/
/*Alteração RED BAS CALC ALIQ ICMS ST  COMPRA DE SIMPLES NACIONAL*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-redBasCalcIcmsSTCompSN"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelRedBasCalcIcmsSTCompSNCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelRedBasCalcIcmsSTCompSNCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTCompSNMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    //fiz alterações aqui de novo

    $('#edtSelRedBasCalcIcmsSTCompSNClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelRedBasCalcIcmsSTCompSNClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTCompSNMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED BAS CALC ALIQ ICMS ST  COMPRA DE SIMPLES NACIONAL MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveRedBaseCalcAliqIcmsSTCompSNClienteManual = document.getElementById("saveRedBaseCalcAliqIcmsSTCompSNClienteManual"); //variavel para receber o botao de salvar
    if (saveRedBaseCalcAliqIcmsSTCompSNClienteManual) {
        saveRedBaseCalcAliqIcmsSTCompSNClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsSTCompSN = document.getElementById("redBaseCalcAliqIcmsSTCompSNClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsSTCompSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsSTCompSN: redBasCalcAliqIcmsSTCompSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTCompSNMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTCompSN=' + redBasCalcAliqIcmsSTCompSN;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsSTCompSN: redBasCalcAliqIcmsSTCompSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTCompSNMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTCompSN=' + redBasCalcAliqIcmsSTCompSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsSTCompSNClienteManual").focus();
                }

            }

        });
    }

});




/*=============== ATUALIZAÇÃO RED BAS CALC ALIQ ICMS  COMPRA DE SIMPLES NACIONAL =====================*/
/*Alteração RED BAS CALC ALIQ ICMS  COMPRA DE SIMPLES NACIONAL*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-redBasCalcIcmsCompSN"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelRedBasCalcIcmsCompSNCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelRedBasCalcIcmsCompSNCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsCompSNMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    //fiz alterações aqui de novo

    $('#edtSelRedBasCalcIcmsCompSNClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelRedBasCalcIcmsCompSNClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsCompSNMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED BAS CALC ALIQ ICMS  COMPRA DE SIMPLES NACIONAL MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveRedBaseCalcAliqIcmsCompSNClienteManual = document.getElementById("saveRedBaseCalcAliqIcmsCompSNClienteManual"); //variavel para receber o botao de salvar
    if (saveRedBaseCalcAliqIcmsCompSNClienteManual) {
        saveRedBaseCalcAliqIcmsCompSNClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsCompSN = document.getElementById("redBaseCalcAliqIcmsCompSNClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsCompSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsCompSN: redBasCalcAliqIcmsCompSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsCompSNMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsCompSN=' + redBasCalcAliqIcmsCompSN;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsCompSN: redBasCalcAliqIcmsCompSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsCompSNMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsCompSN=' + redBasCalcAliqIcmsCompSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsCompSNClienteManual").focus();
                }

            }

        });
    }

});





/*=============== ATUALIZAÇÃO RED BAS CALC ALIQ ICMS st COMPRA DE ATACADO =====================*/
/*Alteração RED BAS CALC ALIQ ICMS st COMPRA DE ATACADO*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-redBasCalcIcmsSTCompAta"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelRedBasCalcIcmsSTCompAtaCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelRedBasCalcIcmsSTCompAtaCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTCompAtaMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    //fiz alterações aqui de novo

    $('#edtSelRedBasCalcIcmsSTCompAtaClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelRedBasCalcIcmsSTCompAtaClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTCompAtaMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED BAS CALC ALIQ ICMS st COMPRA DE ATACADO MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveRedBaseCalcAliqIcmsSTCompAtaClienteManual = document.getElementById("saveRedBaseCalcAliqIcmsSTCompAtaClienteManual"); //variavel para receber o botao de salvar
    if (saveRedBaseCalcAliqIcmsSTCompAtaClienteManual) {
        saveRedBaseCalcAliqIcmsSTCompAtaClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsSTCompAta = document.getElementById("redBaseCalcAliqIcmsSTCompAtaClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsSTCompAta) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsSTCompAta: redBasCalcAliqIcmsSTCompAta },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTCompAtaMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTCompAta=' + redBasCalcAliqIcmsSTCompAta;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsSTCompAta: redBasCalcAliqIcmsSTCompAta },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTCompAtaMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTCompAta=' + redBasCalcAliqIcmsSTCompAta;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsSTCompAtaClienteManual").focus();
                }

            }

        });
    }

});



/*=============== ATUALIZAÇÃO RED BAS CALC ALIQ ICMS COMPRA DE ATACADO =====================*/
/*Alteração RED BAS CALC ALIQ ICMS COMPRA DE ATACADO*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-redBasCalcIcmsCompAta"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelecionadosRedBasCalcIcmsCompAtaCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsCompAtaCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsCompAtaMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelecionadosRedBasCalcIcmsCompAtaClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsCompAtaClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsCompAtaMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED BAS CALC ALIQ ICMS  COMPRA DE ATACADO MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarRedBaseCalcAliqIcmsCompAtaClienteManual = document.getElementById("salvarRedBaseCalcAliqIcmsCompAtaClienteManual"); //variavel para receber o botao de salvar
    if (salvarRedBaseCalcAliqIcmsCompAtaClienteManual) {
        salvarRedBaseCalcAliqIcmsCompAtaClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsCompAta = document.getElementById("redBaseCalcAliqIcmsCompAtaClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsCompAta) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsCompAta: redBasCalcAliqIcmsCompAta },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsCompAtaMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsCompAta=' + redBasCalcAliqIcmsCompAta;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsCompAta: redBasCalcAliqIcmsCompAta },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsCompAtaMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsCompAta=' + redBasCalcAliqIcmsCompAta;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsCompAtaClienteManual").focus();
                }

            }

        });
    }

});




/*=============== ATUALIZAÇÃO RED BAS CALC ALIQ ICMS ST COMPRA DE INDUSTRIA =====================*/
/*Alteração RED BAS CALC ALIQ ICMS ST COMPRA DE INDUSRIA*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-redBasCalcIcmsSTCompInd"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelecionadosRedBasCalcIcmsSTCompIndCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsSTCompIndCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTCompIndMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelecionadosRedBasCalcIcmsSTCompIndClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsSTCompIndClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTCompIndMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED BAS CALC ALIQ ICMS ST COMPRA DE INDUSTRIA MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarRedBaseCalcAliqIcmsSTCompIndClienteManual = document.getElementById("salvarRedBaseCalcAliqIcmsSTCompIndClienteManual"); //variavel para receber o botao de salvar
    if (salvarRedBaseCalcAliqIcmsSTCompIndClienteManual) {
        salvarRedBaseCalcAliqIcmsSTCompIndClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsSTCompInd = document.getElementById("redBaseCalcAliqIcmsSTCompIndClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsSTCompInd) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsSTCompInd: redBasCalcAliqIcmsSTCompInd },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTCompIndMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTCompInd=' + redBasCalcAliqIcmsSTCompInd;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsSTCompInd: redBasCalcAliqIcmsSTCompInd },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTCompIndMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTCompInd=' + redBasCalcAliqIcmsSTCompInd;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsSTCompIndClienteManual").focus();
                }

            }

        });
    }

});





/*=============== ATUALIZAÇÃO RED BAS CALC ALIQ ICMS COMPRA DE INDUSTRIA =====================*/
/*Alteração RED BAS CALC ALIQ ICMS COMPRA DE INDUSTRIA*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-redBasCalcIcmsCompInd"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelecionadosRedBasCalcIcmsCompIndCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsCompIndCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsCompIndMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelecionadosRedBasCalcIcmsCompIndClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsCompIndClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsCompIndMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED BAS CALC ALIQ ICMS COMPRA DE INDUSTRIA MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarRedBaseCalcAliqIcmsCompIndClienteManual = document.getElementById("salvarRedBaseCalcAliqIcmsCompIndClienteManual"); //variavel para receber o botao de salvar
    if (salvarRedBaseCalcAliqIcmsCompIndClienteManual) {
        salvarRedBaseCalcAliqIcmsCompIndClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsCompInd = document.getElementById("redBaseCalcAliqIcmsCompIndClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsCompInd) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsCompInd: redBasCalcAliqIcmsCompInd },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsCompIndMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsCompInd=' + redBasCalcAliqIcmsCompInd;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsCompInd: redBasCalcAliqIcmsCompInd },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsCompIndMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsCompInd=' + redBasCalcAliqIcmsCompInd;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsCompIndClienteManual").focus();
                }

            }

        });
    }

});








/*=============== ATUALIZAÇÃO ALIQ ICMS NFE COMPRA DE ATACADO =====================*/
/*Alteração ALIQ ICMS NFE COMPRA DE ATACADO*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-edtIcmsNfeAta"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelIcmsNFECompAtaCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelIcmsNFECompAtaCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsNFECompAtaMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelIcmsNFECompAtaClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelIcmsNFECompAtaClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsNFECompAtaMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS NFE COMPRA DE ATACADO*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveAliqIcmsNFECompAtaCliManual = document.getElementById("saveAliqIcmsNFECompAtaCliManual"); //variavel para receber o botao de salvar
    if (saveAliqIcmsNFECompAtaCliManual) {
        saveAliqIcmsNFECompAtaCliManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsNFECompAta = document.getElementById("aliqIcmsNFECompAtaCliManual").value; //pegar o valor do imput
            if (aliqIcmsNFECompAta) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsNFECompAta: aliqIcmsNFECompAta },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsNFECompAtaMassaManualModalPost?strDados=' + strDados + '&aliqIcmsNFECompAta=' + aliqIcmsNFECompAta;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsNFECompAta: aliqIcmsNFECompAta },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsNFECompAtaMassaManualModalPost?strDados=' + strDados + '&aliqIcmsNFECompAta=' + aliqIcmsNFECompAta;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsNFECompAtaCliManual").focus();
                }

            }

        });
    }

});





/*=============== ATUALIZAÇÃO ALIQ ICMS NFE COMPRA DE INDUSTRIA =====================*/
/*Alteração ALIQ ICMS NFE COMPRA DE INDUSTRIA*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-edtIcmsNfeSN"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelIcmsNFECompSNCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelIcmsNFECompSNCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsNFECompSNMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelIcmsNFECompSNClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelIcmsNFECompSNClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsNFECompSNMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS NFE COMPRA DE INDUSTRIA*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveAliqIcmsNFECompSNCliManual = document.getElementById("saveAliqIcmsNFECompSNCliManual"); //variavel para receber o botao de salvar
    if (saveAliqIcmsNFECompSNCliManual) {
        saveAliqIcmsNFECompSNCliManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsNFECompSN = document.getElementById("aliqIcmsNFECompSNCliManual").value; //pegar o valor do imput
            if (aliqIcmsNFECompSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsNFECompSN: aliqIcmsNFECompSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsNFECompSNMassaManualModalPost?strDados=' + strDados + '&aliqIcmsNFECompSN=' + aliqIcmsNFECompSN;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsNFECompSN: aliqIcmsNFECompSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsNFECompSNMassaManualModalPost?strDados=' + strDados + '&aliqIcmsNFECompSN=' + aliqIcmsNFECompSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsNFECompSNCliManual").focus();
                }

            }

        });
    }

});







/*=============== ATUALIZAÇÃO ALIQ ICMS NFE COMPRA DE INDUSTRIA =====================*/
/*Alteração ALIQ ICMS NFE COMPRA DE INDUSTRIA*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-edtIcmsNfeInd"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelIcmsNFECompIndCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelIcmsNFECompIndCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsNFECompIndMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelIcmsNFECompIndClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelIcmsNFECompIndClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsNFECompIndMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS NFE COMPRA DE INDUSTRIA*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveAliqIcmsNFECompIndCliManual = document.getElementById("saveAliqIcmsNFECompIndCliManual"); //variavel para receber o botao de salvar
    if (saveAliqIcmsNFECompIndCliManual) {
        saveAliqIcmsNFECompIndCliManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsNFECompInd = document.getElementById("aliqIcmsNFECompIndCliManual").value; //pegar o valor do imput
            if (aliqIcmsNFECompInd) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsNFECompInd: aliqIcmsNFECompInd },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsNFECompIndMassaManualModalPost?strDados=' + strDados + '&aliqIcmsNFECompInd=' + aliqIcmsNFECompInd;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsNFECompInd: aliqIcmsNFECompInd },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsNFECompIndMassaManualModalPost?strDados=' + strDados + '&aliqIcmsNFECompInd=' + aliqIcmsNFECompInd;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsNFECompIndCliManual").focus();
                }

            }

        });
    }

});






/*=============== ATUALIZAÇÃO ALIQ ICMS ST COMPRA DE SIMPLES NACIONAL =====================*/
/*Alteração ALIQ ICMS ST COMPRA DE SIMPLES NACIONAL*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";

    var tabela = document.getElementById("tbl-edtAlqIcmsSTCompSN"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsSTCompSNCli').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTCompSNCli"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTCompSNMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsSTCompSNCliManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTCompSNCliManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTCompSNMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS ST COMPRA DE SIMPLES NACIONAL MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveAliqIcmsSTCompSNCliManual = document.getElementById("saveAliqIcmsSTCompSNCliManual"); //variavel para receber o botao de salvar
    if (saveAliqIcmsSTCompSNCliManual) {
        saveAliqIcmsSTCompSNCliManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTCompSN = document.getElementById("aliqIcmsSTCompSNCliManual").value; //pegar o valor do imput
            if (aliqIcmsSTCompSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTCompSN: aliqIcmsSTCompSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTCompSNMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTCompSN=' + aliqIcmsSTCompSN;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTCompSN: aliqIcmsSTCompSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTCompSNMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTCompSN=' + aliqIcmsSTCompSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsSTCompSNCliManual").focus();
                }

            }

        });
    }

});





/*=============== ATUALIZAÇÃO ALIQ ICMS  COMPRA DE SIMPLES NACIONAL =====================*/
/*Alteração ALIQ ICMS COMPRA DE SIMPLES NACIONAL*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
   
    var tabela = document.getElementById("tbl-edtAlqIcmsCompSN"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsCompSNCli').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsCompSNCli"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsCompSNMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsCompSNCliManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsCompSNCliManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsCompSNMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS  COMPRA DE SIMPLES NACIONAL MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveAliqIcmsCompSNCliManual = document.getElementById("saveAliqIcmsCompSNCliManual"); //variavel para receber o botao de salvar
    if (saveAliqIcmsCompSNCliManual) {
        saveAliqIcmsCompSNCliManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsCompSN = document.getElementById("aliqIcmsCompSNCliManual").value; //pegar o valor do imput
            if (aliqIcmsCompSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsCompSN: aliqIcmsCompSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsCompSNMassaManualModalPost?strDados=' + strDados + '&aliqIcmsCompSN=' + aliqIcmsCompSN;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsCompSN: aliqIcmsCompSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsCompSNMassaManualModalPost?strDados=' + strDados + '&aliqIcmsCompSN=' + aliqIcmsCompSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsCompSNCliManual").focus();
                }

            }

        });
    }

});





/*=============== ATUALIZAÇÃO ALIQ ICMS ST COMPRA DE ATACADO =====================*/
/*Alteração ALIQ ICMS COMPRA DE ATACADO*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("tbl-edtAlqIcmsSTCompAta"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsSTCompAtaCli').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTCompAtaCli"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTCompAtaMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsSTCompAtaCliManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTCompAtaCliManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTCompAtaMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS ST COMPRA DE ATACADO MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveAliqIcmsSTCompAtaCliManual = document.getElementById("saveAliqIcmsSTCompAtaCliManual"); //variavel para receber o botao de salvar
    if (saveAliqIcmsSTCompAtaCliManual) {
        saveAliqIcmsSTCompAtaCliManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTCompAta = document.getElementById("aliqIcmsSTCompAtaCliManual").value; //pegar o valor do imput
            if (aliqIcmsSTCompAta) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTCompAta: aliqIcmsSTCompAta },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTCompAtaMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTCompAta=' + aliqIcmsSTCompAta;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTCompAta: aliqIcmsSTCompAta },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTCompAtaMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTCompAta=' + aliqIcmsSTCompAta;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsSTCompAtaCliManual").focus();
                }

            }

        });
    }

});






/*=============== ATUALIZAÇÃO ALIQ ICMS COMPRA DE ATACADO =====================*/
/*Alteração ALIQ ICMS COMPRA DE ATACADO*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("tbl-edtAlqIcmsCompAta"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsCompAtaCli').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsCompAtaCli"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsCompAtaMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsCompAtaCliManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsCompAtaCliManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsCompAtaMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS COMPRA DE ATACADO MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveAliqIcmsCompAtaCliManual = document.getElementById("saveAliqIcmsCompAtaCliManual"); //variavel para receber o botao de salvar
    if (saveAliqIcmsCompAtaCliManual) {
        saveAliqIcmsCompAtaCliManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsCompAta = document.getElementById("aliqIcmsCompAtaCliManual").value; //pegar o valor do imput
            if (aliqIcmsCompAta) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsCompAta: aliqIcmsCompAta },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsCompAtaMassaManualModalPost?strDados=' + strDados + '&aliqIcmsCompAta=' + aliqIcmsCompAta;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsCompAta: aliqIcmsCompAta },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsCompAtaMassaManualModalPost?strDados=' + strDados + '&aliqIcmsCompAta=' + aliqIcmsCompAta;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsCompAtaCliManual").focus();
                }

            }

        });
    }

});




/*=============== ATUALIZAÇÃO RED ALIQ ICMS ST  VENDA ATACADO PARA SIMPLES NACIONAL =====================*/
/*Alteração RED ALIQ ICMS ST VENDA ATACADO PARA SIMPLES NACIONAL*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-redBasCalcIcmsSTVenAtaSN"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelecionadosRedBasCalcIcmsSTVenAtaSNCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsSTVenAtaSNCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelecionadosRedBasCalcIcmsSTVenAtaSNClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsSTVenAtaSNClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED ALIQ ICMS ST  VENDA ATACADO PARA SIMPLES NACIONAL MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarRedBaseCalcAliqIcmsSTVenAtaSNClienteManual = document.getElementById("salvarRedBaseCalcAliqIcmsSTVenAtaSNClienteManual"); //variavel para receber o botao de salvar
    if (salvarRedBaseCalcAliqIcmsSTVenAtaSNClienteManual) {
        salvarRedBaseCalcAliqIcmsSTVenAtaSNClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsSTVenAtaSN = document.getElementById("redBaseCalcAliqIcmsSTVenAtaSNClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsSTVenAtaSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsSTVenAtaSN: redBasCalcAliqIcmsSTVenAtaSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTVenAtaSN=' + redBasCalcAliqIcmsSTVenAtaSN;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsSTVenAtaSN: redBasCalcAliqIcmsSTVenAtaSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTVenAtaSN=' + redBasCalcAliqIcmsSTVenAtaSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsSTVenAtaSNClienteManual").focus();
                }

            }

        });
    }

});



/*=============== ATUALIZAÇÃO RED ALIQ ICMS  VENDA ATACADO PARA SIMPLES NACIONAL =====================*/
/*Alteração RED ALIQ ICMS ST VENDA ATACADO PARA SIMPLES NACIONAL*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-redBasCalcIcmsVenAtaSN"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelecionadosRedBasCalcIcmsVenAtaSNCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsVenAtaSNCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsVenAtaSNMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelecionadosRedBasCalcIcmsVenAtaSNClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsVenAtaSNClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsVenAtaSNMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED ALIQ ICMS  VENDA ATACADO PARA SIMPLES NACIONAL MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarRedBaseCalcAliqIcmsVenAtaSNClienteManual = document.getElementById("salvarRedBaseCalcAliqIcmsVenAtaSNClienteManual"); //variavel para receber o botao de salvar
    if (salvarRedBaseCalcAliqIcmsVenAtaSNClienteManual) {
        salvarRedBaseCalcAliqIcmsVenAtaSNClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsVenAtaSN = document.getElementById("redBaseCalcAliqIcmsVenAtaSNClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsVenAtaSN) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsVenAtaSN: redBasCalcAliqIcmsVenAtaSN },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsVenAtaSNMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsVenAtaSN=' + redBasCalcAliqIcmsVenAtaSN;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsVenAtaSN: redBasCalcAliqIcmsVenAtaSN },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsVenAtaSNMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsVenAtaSN=' + redBasCalcAliqIcmsVenAtaSN;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsVenAtaSNClienteManual").focus();
                }

            }

        });
    }

});








/*=============== ATUALIZAÇÃO RED ALIQ ICMS ST VENDA ATACADO PARA CONTRIBUINTE =====================*/
/*Alteração RED ALIQ ICMS ST VENDA ATACADO PARA CONTRIBUINTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-redBasCalcIcmsSTVenAtaCont"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelecionadosRedBasCalcIcmsSTVenAtaContCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsSTVenAtaContCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTVenAtaContMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelecionadosRedBasCalcIcmsSTVenAtaContClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsSTVenAtaContClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTVenAtaContMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED ALIQ ICMS ST VENDA ATACADO PARA CONTRIBUINTE MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarRedBaseCalcAliqIcmsSTVenAtaContClienteManual = document.getElementById("salvarRedBaseCalcAliqIcmsSTVenAtaContClienteManual"); //variavel para receber o botao de salvar
    if (salvarRedBaseCalcAliqIcmsSTVenAtaContClienteManual) {
        salvarRedBaseCalcAliqIcmsSTVenAtaContClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsSTVenAtaCont = document.getElementById("redBaseCalcAliqIcmsSTVenVarContClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsSTVenAtaCont) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsSTVenAtaCont: redBasCalcAliqIcmsSTVenAtaCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTVenAtaContMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTVenAtaCont=' + redBasCalcAliqIcmsSTVenAtaCont;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsSTVenAtaCont: redBasCalcAliqIcmsSTVenAtaCont },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTVenAtaContMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTVenAtaCont=' + redBasCalcAliqIcmsSTVenAtaCont;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsSTVenVarContClienteManual").focus();
                }

            }

        });
    }

});




/*=============== ATUALIZAÇÃO RED ALIQ ICMS  VENDA ATACADO PARA CONTRIBUINTE =====================*/
/*Alteração RED ALIQ ICMS  VENDA ATACADO PARA CONTRIBUINTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-redBasCalcIcmsVenAtaCont"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelecionadosRedBasCalcIcmsVenAtaContCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsVenAtaContCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsVenAtaContMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelecionadosRedBasCalcIcmsVenAtaContClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsVenAtaContClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsVenAtaContMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED ALIQ ICMS  VENDA ATACADO PARA CONTRIBUINTE MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarRedBaseCalcAliqIcmsVenAtaContClienteManual = document.getElementById("salvarRedBaseCalcAliqIcmsVenAtaContClienteManual"); //variavel para receber o botao de salvar
    if (salvarRedBaseCalcAliqIcmsVenAtaContClienteManual) {
        salvarRedBaseCalcAliqIcmsVenAtaContClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsVenAtaCont = document.getElementById("redBaseCalcAliqIcmsVenAtaContClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsVenAtaCont) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsVenAtaCont: redBasCalcAliqIcmsVenAtaCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsVenAtaContMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsVenAtaCont=' + redBasCalcAliqIcmsVenAtaCont;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsVenAtaCont: redBasCalcAliqIcmsVenAtaCont },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsVenAtaContMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsVenAtaCont=' + redBasCalcAliqIcmsVenAtaCont;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsVenAtaContClienteManual").focus();
                }

            }

        });
    }

});







/*=============== ATUALIZAÇÃO RED ALIQ ICMS ST VENDA VAREJO PARA CONTRIBUINTE =====================*/
/*Alteração RED ALIQ ICMS  VENDA VAREJO PARA CONTRIBUINTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-redBasCalcIcmsSTVenVarCont"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelecionadosRedBasCalcIcmsSTVenVarContCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsSTVenVarContCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTVenVarContMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelecionadosRedBasCalcIcmsSTVenVarContClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsSTVenVarContClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTVenVarContMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED ALIQ ICMS  ST VENDA VAREJO PARA CONTRIBUINTE MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarRedBaseCalcAliqIcmsSTVenVarContClienteManual = document.getElementById("salvarRedBaseCalcAliqIcmsSTVenVarContClienteManual"); //variavel para receber o botao de salvar
    if (salvarRedBaseCalcAliqIcmsSTVenVarContClienteManual) {
        salvarRedBaseCalcAliqIcmsSTVenVarContClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsSTVenVarCont = document.getElementById("redBaseCalcAliqIcmsSTVenVarContClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsSTVenVarCont) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsSTVenVarCont: redBasCalcAliqIcmsSTVenVarCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTVenVarContMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTVenVarCont=' + redBasCalcAliqIcmsSTVenVarCont;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsSTVenVarCont: redBasCalcAliqIcmsSTVenVarCont },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTVenVarContMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTVenVarCont=' + redBasCalcAliqIcmsSTVenVarCont;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsSTVenVarContClienteManual").focus();
                }

            }

        });
    }

});







/*=============== ATUALIZAÇÃO RED ALIQ ICMS VENDA VAREJO PARA CONTRIBUINTE =====================*/
/*Alteração RED ALIQ ICMS  VENDA VAREJO PARA CONTRIBUINTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-redBasCalcIcmsVenVarCont"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelecionadosRedBasCalcIcmsVenVarContCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsVenVarContCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsVenVarContMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelecionadosRedBasCalcIcmsVenVarContClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsVenVarContClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsVenVarContMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED ALIQ ICMS  VENDA VAREJO PARA CONTRIBUINTE FINAL MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarRedBaseCalcAliqIcmsVenVarContClienteManual = document.getElementById("salvarRedBaseCalcAliqIcmsVenVarContClienteManual"); //variavel para receber o botao de salvar
    if (salvarRedBaseCalcAliqIcmsVenVarContClienteManual) {
        salvarRedBaseCalcAliqIcmsVenVarContClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsVenVarCont = document.getElementById("redBaseCalcAliqIcmsVenVarContClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsVenVarCont) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsVenVarCont: redBasCalcAliqIcmsVenVarCont },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsVenVarContMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsVenVarCont=' + redBasCalcAliqIcmsVenVarCont;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsVenVarCont: redBasCalcAliqIcmsVenVarCont },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsVenVarContMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsVenVarCont=' + redBasCalcAliqIcmsVenVarCont;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsVenVarContClienteManual").focus();
                }

            }

        });
    }

});







/*=============== ATUALIZAÇÃO RED ALIQ ICMS ST  VENDA VAREJO PARA CONSUMIDOR FINAL =====================*/
/*Alteração RED ALIQ ICMS ST VENDA VAREJO PARA CONSUMIDOR FINAL*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-redBasCalcIcmsSTVenVarCF"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelecionadosRedBasCalcIcmsSTVenVarCFCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsSTVenVarCFCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTVenVarCFMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelecionadosRedBasCalcIcmsSTVenVarCFClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsSTVenVarCFClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsSTVenVarCFMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED ALIQ ICMS ST  VENDA VAREJO PARA CONSUMIDOR FINAL MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var saveredBaseCalcAliqIcmsVenVarCFClienteManual = document.getElementById("salvarredBaseCalcAliqIcmsSTVenVarCFClienteManual"); //variavel para receber o botao de salvar
    if (saveredBaseCalcAliqIcmsVenVarCFClienteManual) {
        saveredBaseCalcAliqIcmsVenVarCFClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsSTVenVarCF = document.getElementById("redBaseCalcAliqIcmsSTVenVarCFClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsSTVenVarCF) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsSTVenVarCF: redBasCalcAliqIcmsSTVenVarCF },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTVenVarCFMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTVenVarCF=' + redBasCalcAliqIcmsSTVenVarCF;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsSTVenVarCF: redBasCalcAliqIcmsSTVenVarCF },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsSTVenVarCFMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsSTVenVarCF=' + redBasCalcAliqIcmsSTVenVarCF;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsSTVenVarCFClienteManual").focus();
                }

            }

        });
    }

});
/*=============== ATUALIZAÇÃO RED ALIQ ICMS  VENDA VAREJO PARA CONSUMIDOR FINAL =====================*/
/*Alteração RED ALIQ ICMS  VENDA VAREJO PARA CONSUMIDOR FINAL*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-redBasCalcIcmsVenVarCF"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#edtSelecionadosRedBasCalcIcmsVenVarCFCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsVenVarCFCliente"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsVenVarCFMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#edtSelecionadosRedBasCalcIcmsVenVarCFClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("edtSelecionadosRedBasCalcIcmsVenVarCFClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EdtCliAliqRedBasCalcIcmsVenVarCFMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar RED ALIQ ICMS  VENDA VAREJO PARA CONSUMIDOR FINAL MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarredBaseCalcAliqIcmsVenVarCFClienteManual = document.getElementById("salvarredBaseCalcAliqIcmsVenVarCFClienteManual"); //variavel para receber o botao de salvar
    if (salvarredBaseCalcAliqIcmsVenVarCFClienteManual) {
        salvarredBaseCalcAliqIcmsVenVarCFClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var redBasCalcAliqIcmsVenVarCF = document.getElementById("redBaseCalcAliqIcmsVenVarCFClienteManual").value; //pegar o valor do imput
            if (redBasCalcAliqIcmsVenVarCF) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, redBasCalcAliqIcmsVenVarCF: redBasCalcAliqIcmsVenVarCF },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsVenVarCFMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsVenVarCF=' + redBasCalcAliqIcmsVenVarCF;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, redBasCalcAliqIcmsVenVarCF: redBasCalcAliqIcmsVenVarCF },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EdtCliAliqRedBasCalcIcmsVenVarCFMassaManualModalPost?strDados=' + strDados + '&redBasCalcAliqIcmsVenVarCF=' + redBasCalcAliqIcmsVenVarCF;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("redBaseCalcAliqIcmsVenVarCFClienteManual").focus();
                }

            }

        });
    }

});

/*=============== ATUALIZAÇÃO ALIQ ICMS ST VENDA ATACADO PARA SIMPLES NACIONAL =====================*/
/*Alteração ALIQ ICMS ST VENDA ATACADO PARA SIMPLES NACIONAL*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-aliqIcmsSTVenAtaSN"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsSTVenAtaSNCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTVenAtaSNCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTVenAtaSNMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsSTVenAtaSNClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTVenAtaSNClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTVenAtaSNMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS ST  VENDA ATACADO PARA SIMPLES NACIONAL MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarIcmsSTVenAtaSNClienteManual = document.getElementById("salvaraliqIcmsSTVenAtaSNClienteManual"); //variavel para receber o botao de salvar
    if (salvarIcmsSTVenAtaSNClienteManual) {
        salvarIcmsSTVenAtaSNClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTVenAtaSNClienteManual = document.getElementById("aliqIcmsSTVenAtaSNClienteManual").value; //pegar o valor do imput
            if (aliqIcmsSTVenAtaSNClienteManual) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTVenAtaSNClienteManual: aliqIcmsSTVenAtaSNClienteManual },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTVenAtaSNMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTVenAtaSNClienteManual=' + aliqIcmsSTVenAtaSNClienteManual;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTVenAtaSNClienteManual: aliqIcmsSTVenAtaSNClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTVenAtaSNMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTVenAtaSNClienteManual=' + aliqIcmsSTVenAtaSNClienteManual;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsSTVenAtaSNClienteManual").focus();
                }

            }

        });
    }

});


/*=============== ATUALIZAÇÃO ALIQ ICMS  VENDA ATACADO PARA SIMPLES NACIONAL =====================*/
/*Alteração ALIQ ICMS  VENDA ATACADO PARA SIMPLES NACIONAL*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-aliqIcmsVenAtaSN"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsVenAtaSNCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsVenAtaSNCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsVenAtaSNMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsVenAtaSNClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsVenAtaSNClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsVenAtaSNMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS VENDA ATACADO PARA SIMPLES NACIONAL MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarIcmsVenAtaSNClienteManual = document.getElementById("salvaraliqIcmsVenAtaSNClienteManual"); //variavel para receber o botao de salvar
    if (salvarIcmsVenAtaSNClienteManual) {
        salvarIcmsVenAtaSNClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsVenAtaSNClienteManual = document.getElementById("aliqIcmsVenAtaSNClienteManual").value; //pegar o valor do imput
            if (aliqIcmsVenAtaSNClienteManual) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsVenAtaSNClienteManual: aliqIcmsVenAtaSNClienteManual },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsVenAtaSNMassaManualModalPost?strDados=' + strDados + '&aliqIcmsVenAtaSNClienteManual=' + aliqIcmsVenAtaSNClienteManual;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsVenAtaSNClienteManual: aliqIcmsVenAtaSNClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsVenAtaSNMassaManualModalPost?strDados=' + strDados + '&aliqIcmsVenAtaSNClienteManual=' + aliqIcmsVenAtaSNClienteManual;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsVenAtaSNClienteManual").focus();
                }

            }

        });
    }

});


/*=============== ATUALIZAÇÃO ALIQ ICMS ST VENDA ATACADO PARA CONTRIBUINTE =====================*/
/*Alteração ALIQ ICMS ST VENDA ATACADO PARA CONTRIBUINTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-aliqIcmsSTVenAtaCont"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsSTVenAtaContCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTVenAtaContCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTVenAtaContMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsSTVenAtaContClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTVenAtaContClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTVenAtaContMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS ST ATA CONTRIBUINTE MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarIcmsSTVenAtaContClienteManual = document.getElementById("salvaraliqIcmsSTVenAtaContClienteManual"); //variavel para receber o botao de salvar
    if (salvarIcmsSTVenAtaContClienteManual) {
        salvarIcmsSTVenAtaContClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTVenAtaContClienteManual = document.getElementById("aliqIcmsSTVenAtaContClienteManual").value; //pegar o valor do imput
            if (aliqIcmsSTVenAtaContClienteManual) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTVenAtaContClienteManual: aliqIcmsSTVenAtaContClienteManual },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTVenAtaContMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTVenAtaContClienteManual=' + aliqIcmsSTVenAtaContClienteManual;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTVenAtaContClienteManual: aliqIcmsSTVenAtaContClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTVenAtaContMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTVenAtaContClienteManual=' + aliqIcmsSTVenAtaContClienteManual;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsSTVenAtaContClienteManual").focus();
                }

            }

        });
    }

});



/*=============== ATUALIZAÇÃO ALIQ ICMS  VENDA ATACADO PARA CONTRIBUINTE =====================*/
/*Alteração ALIQ ICMS  VENDA ATACADO PARA CONTRIBUINTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-aliqIcmsVenAtaCont"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsVenAtaContCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsVenAtaContCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsVenAtaContMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsVenAtaContClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsVenAtaContClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsVenAtaContMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS  ATA CONTRIBUINTE MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarIcmsVenAtaContClienteManual = document.getElementById("salvaraliqIcmsVenAtaContClienteManual"); //variavel para receber o botao de salvar
    if (salvarIcmsVenAtaContClienteManual) {
        salvarIcmsVenAtaContClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsVenAtaContClienteManual = document.getElementById("aliqIcmsVenAtaContClienteManual").value; //pegar o valor do imput
            if (aliqIcmsVenAtaContClienteManual) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsVenAtaContClienteManual: aliqIcmsVenAtaContClienteManual },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsVenAtaContMassaManualModalPost?strDados=' + strDados + '&aliqIcmsVenAtaContClienteManual=' + aliqIcmsVenAtaContClienteManual;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsVenAtaContClienteManual: aliqIcmsVenAtaContClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsVenAtaContMassaManualModalPost?strDados=' + strDados + '&aliqIcmsVenAtaContClienteManual=' + aliqIcmsVenAtaContClienteManual;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsVenAtaContClienteManual").focus();
                }

            }

        });
    }

});


/*=============== ATUALIZAÇÃO ALIQ ICMS ST VENDA VAREJO PARA CONTRIBUINTE =====================*/
/*Alteração ALIQ ICMS ST  VENDA VAR PARA CONTRIBUINTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-aliqIcmsSTVenVarCont"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsSTVenVarContCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTVenVarContCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTVenVarContMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsSTVenVarContClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTVenVarContClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTVenVarContMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS  VAR CONTRIBUINTE MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarIcmsSTVenVarContClienteManual = document.getElementById("salvaraliqIcmsSTVenVarContClienteManual"); //variavel para receber o botao de salvar
    if (salvarIcmsSTVenVarContClienteManual) {
        salvarIcmsSTVenVarContClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTVenVarContClienteManual = document.getElementById("aliqIcmsSTVenVarContClienteManual").value; //pegar o valor do imput
            if (aliqIcmsSTVenVarContClienteManual) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTVenVarContClienteManual: aliqIcmsSTVenVarContClienteManual },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTVenVarContMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTVenVarContClienteManual=' + aliqIcmsSTVenVarContClienteManual;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTVenVarContClienteManual: aliqIcmsSTVenVarContClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                         window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTVenVarContMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTVenVarContClienteManual=' + aliqIcmsSTVenVarContClienteManual;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsSTVenVarContClienteManual").focus();
                }

            }

        });
    }

});



/*=================== ATUALIZAÇÃO ALIQ ICMS VENDA VAREJO PARA CONTRIBUINTE ==========================*/
/*Alteração ALIQ ICMS  VENDA VAR PARA CONTRIBUINTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-aliqIcmsVenVarCont"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsVenVarContCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsVenVarContCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsVenVarContMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsVenVarContClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsVenVarContClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsVenVarContMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS  VAR CONTRIBUINTE MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarIcmsVenVarContClienteManual = document.getElementById("salvaraliqIcmsVenVarContClienteManual"); //variavel para receber o botao de salvar
    if (salvarIcmsVenVarContClienteManual) {
        salvarIcmsVenVarContClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsVenVarContClienteManual = document.getElementById("aliqIcmsVenVarContClienteManual").value; //pegar o valor do imput
            if (aliqIcmsVenVarContClienteManual) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsVenVarContClienteManual: aliqIcmsVenVarContClienteManual },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsVenVarContMassaManualModalPost?strDados=' + strDados + '&aliqIcmsVenVarContClienteManual=' + aliqIcmsVenVarContClienteManual;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsVenVarContClienteManual: aliqIcmsVenVarContClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsVenVarContMassaManualModalPost?strDados=' + strDados + '&aliqIcmsVenVarContClienteManual=' + aliqIcmsVenVarContClienteManual;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");

                    document.getElementById("aliqIcmsVenVarContClienteManual").focus();
                }

            }

        });
    }

});




/* ==================== ATUALIZAÇÃO ALIQ ICMS VENDA VAREJO CONSUMIDOR FINAL ======================= */

/*Alteração ALIQ ICMS ST VENDA VAR CONS FINAL*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-aliqIcmsSTVenVarCF"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsSTVenVarCFCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTVenVarCFCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTVenVarCFMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsSTVenVarCFClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTVenVarCFClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTVenVarCFMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Atualizar ALIQ ICMS ST VAR CONS. FINAL MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarIcmsSTVenVarCFClienteManual = document.getElementById("salvaraliqIcmsSTVenVarCFClienteManual"); //variavel para receber o botao de salvar
    if (salvarIcmsSTVenVarCFClienteManual) {
        salvarIcmsSTVenVarCFClienteManual.addEventListener("click", function () { 
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTVenVarCFClienteManual = document.getElementById("aliqIcmsSTVenVarCFClienteManual").value; //pegar o valor do imput
            if (aliqIcmsSTVenVarCFClienteManual) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTVenVarCFClienteManual: aliqIcmsSTVenVarCFClienteManual },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTVenVarCFMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTVenVarCFClienteManual=' + aliqIcmsSTVenVarCFClienteManual;

                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTVenVarCFClienteManual: aliqIcmsSTVenVarCFClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTVenVarCFMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTVenVarCFClienteManual=' + aliqIcmsSTVenVarCFClienteManual;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");
                   
                    document.getElementById("aliqIcmsSTVenVarCFClienteManual").focus();
                }

            }

        });
    }

});


/*Alterção ALIQ ICMS VENDA VAREJO CONSUMIDOR FINAL- Igualar ao MTX*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-aliqIcmsVenVarCF"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsVenVarCFCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsVenVarCFCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsVenVarCFMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsVenVarCFClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsVenVarCFClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsVenVarCFMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});


/*Atualizar ALIQ ICMS VENDA VAREJO CONSUMIDOR FINAL  - manualmente*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarIcmsVenVarCFClienteManual = document.getElementById("salvaraliqIcmsVenVarCFClienteManual"); //variavel para receber o botao de salvar
    if (salvarIcmsVenVarCFClienteManual) {
        salvarIcmsVenVarCFClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsVenVarCFClienteManual = document.getElementById("aliqIcmsCompIndClienteManual").value; //pegar o valor do imput
            if (aliqIcmsVenVarCFClienteManual) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsVenVarCFClienteManual: aliqIcmsVenVarCFClienteManual },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsVenVarCFMassaManualModalPost?strDados=' + strDados + '&aliqIcmsVenVarCFClienteManual=' + aliqIcmsVenVarCFClienteManual;
                        
                    }

                });


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsVenVarCFClienteManual: aliqIcmsVenVarCFClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsVenVarCFMassaManualModalPost?strDados=' + strDados + '&aliqIcmsVenVarCFClienteManual=' + aliqIcmsVenVarCFClienteManual;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsCompIndClienteManual").focus();
                }

            }

        });
    }

});



/* ===================  ATUALIZAÇÃO ALIQ ICMS ST COMPRA DE INDUSTRIA ==========================*/

/*Alterção ALIQ ICMS ST COMPRA DE INDUSTRIA - Igualar ao MTX*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-aliqIcmsStCompInd"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsSTCompIndCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTCompIndCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTCompIndMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsSTCompIndClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsSTCompIndClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsSTCompIndMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});



/*Atualizar ALIQ ICMS ST COMPRA DE INDUSTRIA - manualmente*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarIcmsSTCompIndClienteManual = document.getElementById("salvaraliqIcmsSTCompIndClienteManual"); //variavel para receber o botao de salvar
    if (salvarIcmsSTCompIndClienteManual) {
        salvarIcmsSTCompIndClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsSTCompIndClienteManual = document.getElementById("aliqIcmsSTCompIndClienteManual").value; //pegar o valor do imput
            if (aliqIcmsSTCompIndClienteManual) {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, aliqIcmsSTCompIndClienteManual: aliqIcmsSTCompIndClienteManual },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTCompIndMassaManualModalPost?strDados=' + strDados + '&aliqIcmsSTCompIndClienteManual=' + aliqIcmsSTCompIndClienteManual;

                    }

                });




                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsSTCompIndClienteManual: aliqIcmsSTCompIndClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsSTCompIndMassaManualModalPost?strDados=' + strDados + '&aliqIcmsCompIndClienteManual=' + aliqIcmsCompIndClienteManual;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsSTCompIndClienteManual").focus();
                }

            }

        });
    }

});



/* ===========================================================================================*/

/* ===================  ATUALIZAÇÃO ALIQ ICMS COMPRA DE INDUSTRIA ==========================*/

/*Alterção ALIQ ICMS COMPRA DE INDUSTRIA - Igualar ao MTX*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("tablepr-3"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosIcmsCompIndCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsCompIndCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsCompIndMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosIcmsCompIndClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosIcmsCompIndClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteAliqIcmsCompIndMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});

/*Alteração Icms Compra de Indrutria manualmente*/


/*Atualizar ALIQ ICMS COMPRA DE INDUSTRIA - manualmente*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarIcmsCompIndClienteManual = document.getElementById("salvaraliqIcmsCompIndClienteManual"); //variavel para receber o botao de salvar
    if (salvarIcmsCompIndClienteManual) {
        salvarIcmsCompIndClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var aliqIcmsCompIndClienteManual = document.getElementById("aliqIcmsCompIndClienteManual").value; //pegar o valor do imput
            if (aliqIcmsCompIndClienteManual) {
                        for (var i = 0; i < selecionados.length; i++) {
                            var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                            selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                            dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                            dados[i] = dados[i].trim();
                            strDados += dados[i] + ",";
                        }//fim do for
                        bloqueioTela();//bloqueia tela

                        //agora o ajax
                        $.ajax({

                            data: { strDados: strDados, aliqIcmsCompIndClienteManual: aliqIcmsCompIndClienteManual },
                            types: "GET",
                            processData: true,
                            success: function () {

                                window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsCompIndMassaManualModalPost?strDados=' + strDados + '&aliqIcmsCompIndClienteManual=' + aliqIcmsCompIndClienteManual;

                            }

                        });
               
                


                //fim if verificacao vazio
            } else {

                var resultado = confirm("A Alíquota informada foi NULA ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, aliqIcmsCompIndClienteManual: aliqIcmsCompIndClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteAliqIcmsCompIndMassaManualModalPost?strDados=' + strDados + '&aliqIcmsCompIndClienteManual=' + aliqIcmsCompIndClienteManual;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Aliquotas para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("aliqIcmsCompIndClienteManual").focus();
                }

            }

        });
    }

});

/* ===========================================================================================*/



/*=========================== ATUALIZAÇÃO DE DESCRIÇÃO ===========================================* /
/*Efetiva alteração em massa informando A DESCRIÇÃO DO PRODUTO manualmente*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarDescProdClienteManual = document.getElementById("salvarDescProdClienteManual"); //variavel para receber o botao de salvar
    if (salvarDescProdClienteManual) {
        salvarDescProdClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var descProdClienteManual = document.getElementById("descProdClienteManual").value; //pegar o valor do imput
           
            if (descProdClienteManual.length <= 0) {
                toastr.warning("Favor informar a descrição a ser atribuida");
                /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                document.getElementById("descProdClienteManual").focus();
            } else {
                for (var i = 0; i < selecionados.length; i++) {
                    var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                    selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                    dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                    dados[i] = dados[i].trim();
                    strDados += dados[i] + ",";
                }//fim do for
                bloqueioTela();//bloqueia tela

                //agora o ajax
                $.ajax({

                    data: { strDados: strDados, descProdClienteManual: descProdClienteManual },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/EditClienteProdMassaManualModalPost?strDados=' + strDados + '&descProdClienteManual=' + descProdClienteManual;

                    }

                });
            }

        });
    }

});


/*Efetiva alteração em massa informando A DESCRIÇÃO DO PRODUTO igualando ao MTX ou MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-editDescProdCliente"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosDescProdCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosDescProdCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteProdMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });
    
    $('#editarSelecionadosDescProdClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosDescProdClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteProdMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecinadosDescricao(btnAlterarSelecionados, controller);
    });

});







/*=========================== ATUALIZAÇÃO DE NCM ===========================================* /
/*Efetiva alteração em massa informando o ncm manualmente*/
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarNcmClienteManual = document.getElementById("salvarNcmClienteManual"); //variavel para receber o botao de salvar
    if (salvarNcmClienteManual) {
        salvarNcmClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var ncmClienteManual = document.getElementById("ncm").value; //pegar o valor do imput
            if (ncmClienteManual) {
                if (ncmClienteManual.length < 8) {
                    var resultado = confirm("O NCM informado tem tamanho inferior ao tamanho padrão, deseja continuar?");
                    if (resultado == true) {
                        for (var i = 0; i < selecionados.length; i++) {
                            var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                            selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                            dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                            dados[i] = dados[i].trim();
                            strDados += dados[i] + ",";
                        }//fim do for
                        bloqueioTela();//bloqueia tela

                        //agora o ajax
                        $.ajax({

                            data: { strDados: strDados, ncmClienteManual: ncmClienteManual },
                            types: "GET",
                            processData: true,
                            success: function () {

                                window.location.href = '/Cliente/TributacaoEmpresa/EditClienteNcmMassaManualModalPost?strDados=' + strDados + '&ncmClienteManual=' + ncmClienteManual;

                            }

                        });
                    } else {
                        toastr.warning("Atribuição de Ncm para o(s) produto(s) selecionado(s) abortada");
                        /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                        document.getElementById("ncm").focus();
                    }//fim do else
                } else {//se for do tamabhio padrão ele segue o fluxo normal
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }//fim do for
                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, ncmClienteManual: ncmClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteNcmMassaManualModalPost?strDados=' + strDados + '&ncmClienteManual=' + ncmClienteManual;

                        }

                    });
                }


                //fim if verificacao vazio
            } else {

                var resultado = confirm("O NCM informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, ncmClienteManual: ncmClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteNcmMassaManualModalPost?strDados=' + strDados + '&ncmClienteManual=' + ncmClienteManual;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Ncm para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("ncm").focus();
                }

            }

        });
    }

});

/*Alteração em massa (NCM) igualando ao MTX ou MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("table-editNcmCliente"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);


    $('#editarSelecionadosNcmCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosNcmCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteNcmMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosNcmClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosNcmClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteNcmMassaManualModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

});




/* ===================  ATUALIZAÇÃO DE CEST ==========================*/
//Salvar em massa MANUALMENTE CEST do produto
$(document).ready(function () {
    toastOpcoes(); //configurar o toast
    var salvarCestClienteManual = document.getElementById("salvarCestClienteManual"); //variavel para receber o botao de salvar
    if (salvarCestClienteManual) {
        salvarCestClienteManual.addEventListener("click", function () {
            var selecionados = document.getElementsByClassName("sel"); //pega os elementos da linha com a classe selecionado
            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = ""; //variavel auxiliar para receber o ID sem espaços
            var cestClienteManual = document.getElementById("cest").value; //pegar o valor do imput
            if (cestClienteManual) {
                if (cestClienteManual.length < 9) {
                    var resultado = confirm("O Cest informado tem tamanho inferior ao tamanho padrão, deseja continuar?");
                    if (resultado == true) {
                        for (var i = 0; i < selecionados.length; i++) {
                            var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                            selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                            dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                            dados[i] = dados[i].trim();
                            strDados += dados[i] + ",";
                        }//fim do for
                        bloqueioTela();//bloqueia tela

                        //agora o ajax
                        $.ajax({

                            data: { strDados: strDados, cestClienteManual: cestClienteManual },
                            types: "GET",
                            processData: true,
                            success: function () {

                                window.location.href = '/Cliente/TributacaoEmpresa/EditClienteCestMassaManualModalPost?strDados=' + strDados + '&cestClienteManual=' + cestClienteManual;

                            }

                        });
                    } else {
                        toastr.warning("Atribuição de Cest para o(s) produto(s) selecionado(s) abortada");
                        /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                        document.getElementById("cest").focus();
                    }//fim do else
                } else {//se for do tamabhio padrão ele segue o fluxo normal
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }//fim do for
                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, cestClienteManual: cestClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteCestMassaManualModalPost?strDados=' + strDados + '&cestClienteManual=' + cestClienteManual;

                        }

                    });
                }

                
                //fim if verificacao vazio
            } else {
                
                var resultado = confirm("O Cest informada foi NULO ou 0 (ZERO), deseja continuar ?");
                if (resultado == true) {
                    /*Laço para varrer os elementos com a tag TD*/
                    for (var i = 0; i < selecionados.length; i++) {
                        var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                        selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                        dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice 0 à variavel dados
                        dados[i] = dados[i].trim();
                        strDados += dados[i] + ",";
                    }

                    bloqueioTela();//bloqueia tela

                    //agora o ajax
                    $.ajax({

                        data: { strDados: strDados, cestClienteManual: cestClienteManual },
                        types: "GET",
                        processData: true,
                        success: function () {

                            window.location.href = '/Cliente/TributacaoEmpresa/EditClienteCestMassaManualModalPost?strDados=' + strDados + '&cestClienteManual=' + cestClienteManual;

                        }

                    });

                } else {
                    toastr.warning("Atribuição de Cest para o(s) produto(s) selecionado(s) abortada");
                    /* alert("Atribuição de NCM para os produtos selecionados abortada");*/
                    document.getElementById("cest").focus();
                }

            }

        });
    }

});


/*Alteração em massa igualando ao MTX ou MANUALMENTE*/
$(document).ready(function () {
    toastOpcoes(); //configarar toast
    var btnAlterarSelecionados = "";
    var controller = "";
    var tabela = document.getElementById("tablepr-2"); //variavel para a tabela
    //funcao para selecionar as linhas da tabela: o parametro é a tabela pelo seu ID
    selecionaLinhas(tabela);

   
    $('#editarSelecionadosCestCliente').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosCestCliente"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteCestMassaModal"; //envia o nome da Action para a função
        //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
        alterarSelecionados(btnAlterarSelecionados, controller);
    });

    $('#editarSelecionadosCestClienteManual').click(function () {
        btnAlterarSelecionados = document.getElementById("editarSelecionadosCestClienteManual"); //botao para confirmar a edição dos selecionados
        controller = "EditClienteCestMassaManualModal"; //envia o nome da Action para a função
      //funcao para enviar os selecionados para action: o parametro são os selecionados pelo ID e a action, definida pela var controller
       alterarSelecionados(btnAlterarSelecionados, controller);
    });
    
});

/* =================================================================*/





/*
 *FUNÇÕES AUXILIARES
 * 
 * */

//funcao para selecionar as linhas: o parametro eh a tabela
function selecionaLinhas(paramTabela) {
    //se a tabela existir ele continuar
    if (paramTabela) {
        var linhas = paramTabela.getElementsByTagName("tr");//variavel para as linhas da tabela
        //laço for para as linhas
        for (var i = 0; i < linhas.length; i++) {
            var linha = linhas[i];

            linha.addEventListener("click", function () {
                //Adicionar ao atual
                selLinha(this, true); //Selecione apenas um (parametro true seleciona mais de uma linha)


            });

        }//fim for

    }//fim if tabela
}


//alterar selecionados
function alterarSelecionados(par1, par2) {
    //verificar se o botão existe
    if (par1) {
        //alterar selecionados
       
            //pega os elementos da linha com a classe selecionado
            var selecionados = document.getElementsByClassName("selecionado");

            //Verificar se está selecionado
            if (selecionados.length < 1) {
                toastr.error("Selecione pelo menos uma linha para proceder a alteração do registro!");
                /*  toastPadrao();*/
               /* alert("Selecione pelo menos  uma linha para alteração de registro")*/
                return false;
            }

            var dados = {}; //variavel auxiliar para receber o ID
            var strDados = "";

            /*Laço para varrer os elementos com a tag TD*/
            for (var i = 0; i < selecionados.length; i++) {
                var selecionado = selecionados[i]; //variavel para conter os itens selecionados
                selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
                dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
                dados[i] = dados[i].trim();
                strDados += dados[i] + ",";

            }

            bloqueioTela();//bloqueia a tela ate a chamada do ajax

            //agora mandar esse vetor para o modal e passar o valor correto do ncm
            $.ajax(
                {
                    data: { array: strDados },
                    types: "GET",
                    processData: true,
                    success: function () {

                        window.location.href = '/Cliente/TributacaoEmpresa/' + par2 + '?strDados=' + strDados;

                    }

                });

      
    }//fim if botao atlerar selecionados
}




function alterarSelecinadosDescricao(par1, par2)
{
    //verificar se o botão existe
    if (par1) {
        //alterar selecionados

        //pega os elementos da linha com a classe selecionado
        var selecionados = document.getElementsByClassName("selecionado");

        //Verificar se está selecionado
        if (selecionados.length > 1) {
            toastr.error("Para alterar a descrição do produto manualmente, somente uma linha deve ser selecionada");
            /*  toastPadrao();*/
            /* alert("Selecione pelo menos  uma linha para alteração de registro")*/
            return false;
        }
        //Verificar se está selecionado
        if (selecionados.length == 0) {
            toastr.error("Para alterar a descrição do produto manualmente, é preciso selecionar uma linha na tabela");
            /*  toastPadrao();*/
            /* alert("Selecione pelo menos  uma linha para alteração de registro")*/
            return false;
        }

        var dados = {}; //variavel auxiliar para receber o ID
        var strDados = "";

        /*Laço para varrer os elementos com a tag TD*/
        for (var i = 0; i < selecionados.length; i++) {
            var selecionado = selecionados[i]; //variavel para conter os itens selecionados
            selecionado = selecionado.getElementsByTagName("td"); //atribui o item com a tag td
            dados[i] = selecionado[0].innerHTML;//atribui o valor presente no indice i ao vetor dados
            dados[i] = dados[i].trim();
            strDados += dados[i] + ",";

        }

        bloqueioTela();//bloqueia a tela ate a chamada do ajax

        //agora mandar esse vetor para o modal e passar o valor correto do ncm
        $.ajax(
            {
                data: { array: strDados },
                types: "GET",
                processData: true,
                success: function () {

                    window.location.href = '/Cliente/TributacaoEmpresa/' + par2 + '?strDados=' + strDados;

                }

            });


    }//fim if botao atlerar selecionados
}



//função selecionar a linha: vai adicionar a tag selecionado
function selLinha(linha, multiplos) {
    if (!multiplos) {

        var linhas = linha.parentElement.getElementsByTagName("tr");
        for (var i = 0; i < linhas.length; i++) {
            var linha_ = linhas[i];
            linha_.classList.remove("selecionado");
        }
    }
    linha.classList.toggle("selecionado");
}


//opcoes toast
function toastOpcoes() {
    toastr.options = {
        "closeButton": true,
        "debug": true,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-center",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "100",
        "hideDuration": "1000",
        "timeOut": "4000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
}

//pegar quantidade linhas da tabela
$(document).ready(function () {

    var tabela = document.getElementById("table-editmassa");

    if (tabela) {
        var linhas = tabela.getElementsByTagName('tr');
        var lab = document.getElementById('lab');
        lab.innerHTML = (linhas.length - 1);

    }


});

$(document).ready(function () {
    setTimeout(function () {
        $(".alert").fadeOut("slow", function () {

            $(this).alert('close');
        });
       
    }, 5000);
});


/*Converter inputs em maiusculo*/
function maiuscula(z) {
    v = z.value.toUpperCase();
    z.value = v;
}
function minuscula(z) {
    v = z.value.toLowerCase();
    z.value = v;
}

/*somente numeros*/
function onlynumber(evt) {
    var theEvent = evt || window.event;
    var key = theEvent.keyCode || theEvent.which;
    key = String.fromCharCode(key);
    //var regex = /^[0-9.,]+$/;
    var regex = /^[0-9.]+$/;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}

   


