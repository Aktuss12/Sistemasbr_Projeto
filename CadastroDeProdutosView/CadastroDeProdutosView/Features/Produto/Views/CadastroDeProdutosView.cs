﻿using CadastroDeProdutosView.Features.Commons;
using CadastroDeProdutosView.Features.Produto.Enums;
using DevExpress.XtraEditors;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace CadastroDeProdutosView.Features.Produto.Views
{
    public partial class CadastroDeProdutosView : Form
    {
        private bool isValidating;
        private const string connectionString = @"User ID=SYSDBA;Password=masterkey;Database=C:\Users\admin\Documents\BANCODEDADOSPRODUTOS.FDB;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;";


        public CadastroDeProdutosView(int idProduto)
        {
            InitializeComponent();
            InitializeLookUpEdit();
        }

        private void InitializeLookUpEdit()
        {
            unidadeDeMedidaLookUpEdit.PreencherLookUpEditComOValorDoEnum<UnidadeDeMedidaView.UnidadeDeMedida>();
            categoriaDeProdutosLookUpEdit.PreencherLookUpEditComOValorDoEnum<CategoriaDoProdutoView.CategoriaDeProdutos>();
            marcaLookUpEdit.PreencherLookUpEditComOValorDoEnum<MarcaDoProdutoView.MarcaDoProduto>();
            origemDaMercadoriaLookUpEdit.PreencherLookUpEditComOValorDoEnum<OrigemDaMercadoriaView.OrigemDaMercadoria>();
            situacaoTributariaLookUpEdit.PreencherLookUpEditComOValorDoEnum<SituacaoTributariaView.SituacaoTributaria>();
            naturezaDaOperacaoLookUpEdit.PreencherLookUpEditComOValorDoEnum<NaturezaDaOperacaoView.NaturezaDaOperacao>();
        }

        private void CadastroDeProdutosView_Load(object sender, EventArgs e) { }

        private void labelControl_Click(object sender, EventArgs e) { }


        // Validando se o codigo de barras é do tipo EAN-13 e possui 13 caracteres para passar pela verificação
        private void codigoDeBarrasTextEdit(object sender, EventArgs e)
        {
            if (isValidating) return;

            isValidating = true;
            codigodebarrasTextEdit.Properties.MaxLength = 13;
            var texto = codigodebarrasTextEdit.Text;

            if (texto.Length == 13)
            {
                var valido = ValidarCodigoDeBarrasEAN13(texto);
                AtualizarEstiloLabelCodigoBarras(valido);
                if (!valido)
                {
                    XtraMessageBox.Show("Código de Barras EAN-13 inválido. Verifique os dados inseridos e tente novamente.");
                    codigodebarrasTextEdit.Text = string.Empty;
                }
            }
            else
            {
                AtualizarEstiloLabelCodigoBarras(true);
            }

            isValidating = false;
        }

        // Calculando o preço do custo * + 1 markup / 100 para que preencha o Preço da venda
        private void CalcularPrecoVenda()
        {
            if (!decimal.TryParse(custoTextEdit.Text, out var custo) ||
                !decimal.TryParse(markupTextEdit.Text, out var markup))
                return;
            var precoVenda = custo * (1 + (markup / 100));
            precoVendaTextEdit.Text = precoVenda.ToString("F2");
        }

        // Calculo para definir um codigo de barras EAN-13
        private bool ValidarCodigoDeBarrasEAN13(string codigoDeBarras)
        {
            if (codigoDeBarras.Length != 13 || !long.TryParse(codigoDeBarras, out _))
                return false;

            var numeros = new int[12];
            for (var i = 0; i < 12; i++)
            {
                numeros[i] = int.Parse(codigoDeBarras[i].ToString());
            }

            var somaPar = 0;
            var somaImpar = 0;
            for (var i = 0; i < 12; i++)
            {
                if (i % 2 == 0)
                    somaImpar += numeros[i];
                else
                    somaPar += numeros[i];
            }

            var somaTotal = somaImpar + (somaPar * 3);
            var digitoVerificadorCalculado = (10 - (somaTotal % 10)) % 10;
            var digitoVerificadorInformado = int.Parse(codigoDeBarras[12].ToString());

            return digitoVerificadorCalculado == digitoVerificadorInformado;
        }

        // Clear LookUpEdits
        public void LimparLookUpEdits()
        {
            unidadeDeMedidaLookUpEdit.EditValue = null;
            categoriaDeProdutosLookUpEdit.EditValue = null;
            marcaLookUpEdit.EditValue = null;
            origemDaMercadoriaLookUpEdit.EditValue = null;
            situacaoTributariaLookUpEdit.EditValue = null;
            naturezaDaOperacaoLookUpEdit.EditValue = null;
        }

        // Clear TextEdit
        public void LimparTextEdits()
        {
            nomeTextEdit.Text = null;
            fornecedorTextEdit.Text = null;
            codigodebarrasTextEdit.Text = null;
            estoqueTextEdit.Text = null;
            precoVendaTextEdit.Text = null;
            custoTextEdit.Text = null;
            markupTextEdit.Text = null;
            ncmTextEdit.Text = null;
            aliquotaDeIcmsTextEdit.Text = null;
            reducaoDeCalculoIcmsTextEdit.Text = null;
        }

        private void fornecedorTextEdit_EditValueChanged(object sender, EventArgs e)
        {
            fornecedorTextEdit.Properties.MaxLength = 100;
        }

        private void estoqueTextEdit_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void precoVendaTextEdit_EditValueChanged(object sender, EventArgs e)
        {
            precoVendaTextEdit.Properties.MaxLength = 18;
        }


        private void custoTextEdit_EditValueChanged(object sender, EventArgs e)
        {
            custoTextEdit.Properties.MaxLength = 18;
            if (isValidating) return;

            isValidating = true;
            CalcularPrecoVenda();
            isValidating = false;
        }

        private void markupTextEdit_EditValueChanged(object sender, EventArgs e)
        {
            markupTextEdit.Properties.MaxLength = 18;
            if (isValidating) return;

            isValidating = true;
            CalcularPrecoVenda();
            isValidating = false;
        }

        private void ncmTextEdit_EditValueChanged(object sender, EventArgs e)
        {
            ncmTextEdit.Properties.MaxLength = 8;
        }

        private void aliquotaDeIcmsTextEdit_EditValueChanged(object sender, EventArgs e)
        {
            aliquotaDeIcmsTextEdit.Properties.MaxLength = 18;
        }

        private void reducaoDeCalculoIcmsTextEdit_EditValueChanged(object sender, EventArgs e)
        {
            reducaoDeCalculoIcmsTextEdit.Properties.MaxLength = 18;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { }

         // Validação para o botão de salvar criando um try (tentativa) de implementar o produto no banco de dados, caso de errado, retorna um catch com rollback
        private void salvarButtomItem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!ValidarCamposObrigatorios())
            {
                XtraMessageBox.Show("Todos os campos obrigatórios devem ser preenchidos!");
                return;
            }

            if (!string.IsNullOrWhiteSpace(codigodebarrasTextEdit.Text) &&
                !ValidarCodigoDeBarrasEAN13(codigodebarrasTextEdit.Text))
            {
                XtraMessageBox.Show("O campo código de barras não é um EAN-13 válido. Por favor, verifique e tente novamente.");
                AtualizarEstiloLabelCodigoBarras(false);
                return;
            }

            AtualizarEstiloLabelCodigoBarras(true);

            try
            {
                using (var connection = new FbConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            const string insertProdutoQuery = @"
                        INSERT INTO PRODUTO (Nome, Categoria, Fornecedor, CodigoDeBarras, UnidadeDeMedida, Estoque, Marca, Custo, Markup, PrecoDaVenda)
                        VALUES (@nome, @categoria, @fornecedor, @codigoDeBarras, @unidadeDeMedida, @estoque, @marca, @custo, @markup, @precoDaVenda)
                        RETURNING idProduto";

                            int idProduto;

                            using (var command = new FbCommand(insertProdutoQuery, connection, transaction))
                            {
                                command.Parameters.Add("@Nome", FbDbType.VarChar).Value = nomeTextEdit.Text ?? (object)DBNull.Value;
                                command.Parameters.Add("@Categoria", FbDbType.VarChar).Value = categoriaDeProdutosLookUpEdit.EditValue ?? DBNull.Value;
                                command.Parameters.Add("@Fornecedor", FbDbType.VarChar).Value = fornecedorTextEdit.Text ?? (object)DBNull.Value;
                                command.Parameters.Add("@CodigoDeBarras", FbDbType.VarChar).Value = codigodebarrasTextEdit.Text ?? (object)DBNull.Value;
                                command.Parameters.Add("@UnidadeDeMedida", FbDbType.VarChar).Value = unidadeDeMedidaLookUpEdit.EditValue ?? DBNull.Value;
                                command.Parameters.Add("@Estoque", FbDbType.Integer).Value = int.TryParse(estoqueTextEdit.EditValue.ToString(), out var estoque) ? estoque : (object)DBNull.Value;
                                command.Parameters.Add("@Marca", FbDbType.VarChar).Value = marcaLookUpEdit.EditValue ?? DBNull.Value;

                                if (decimal.TryParse(custoTextEdit.Text, out var custo))
                                    command.Parameters.Add("@Custo", FbDbType.Decimal).Value = custo;
                                else
                                    command.Parameters.Add("@Custo", FbDbType.Decimal).Value = DBNull.Value;

                                if (decimal.TryParse(markupTextEdit.Text, out var markup))
                                    command.Parameters.Add("@Markup", FbDbType.Decimal).Value = markup;
                                else
                                    command.Parameters.Add("@Markup", FbDbType.Decimal).Value = DBNull.Value;

                                if (decimal.TryParse(precoVendaTextEdit.Text, out var precoVenda))
                                    command.Parameters.Add("@PrecoDaVenda", FbDbType.Decimal).Value = precoVenda;
                                else
                                    command.Parameters.Add("@PrecoDaVenda", FbDbType.Decimal).Value = DBNull.Value;

                                idProduto = (int)command.ExecuteScalar();
                            }

                            const string insertInformacoesFiscaisQuery = @"
                        INSERT INTO INFORMACOESFISCAIS (idProduto, origemDaMercadoria, situacaoTributaria, naturezaDaOperacao, ncm, aliquotaDeIcms, reducaoDeCalculo)
                         VALUES (@idProduto, @origemDaMercadoria, @situacaoTributaria, @naturezaDaOperacao, @ncm, @aliquotaDeIcms, @reducaoDeCalculo)";

                            using (var informacoesCommand = new FbCommand(insertInformacoesFiscaisQuery, connection, transaction))
                            {
                                informacoesCommand.Parameters.Add("@idProduto", FbDbType.Integer).Value = idProduto;
                                informacoesCommand.Parameters.Add("@origemDaMercadoria", FbDbType.VarChar).Value = origemDaMercadoriaLookUpEdit.EditValue ?? DBNull.Value;
                                informacoesCommand.Parameters.Add("@situacaoTributaria", FbDbType.VarChar).Value = situacaoTributariaLookUpEdit.EditValue ?? DBNull.Value;
                                informacoesCommand.Parameters.Add("@naturezaDaOperacao", FbDbType.VarChar).Value = naturezaDaOperacaoLookUpEdit.EditValue ?? DBNull.Value;
                                informacoesCommand.Parameters.Add("@ncm", FbDbType.VarChar).Value = ncmTextEdit.Text ?? (object)DBNull.Value;

                                if (decimal.TryParse(aliquotaDeIcmsTextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var aliquotaIcms))
                                    informacoesCommand.Parameters.Add("@aliquotaDeIcms", FbDbType.Decimal).Value = aliquotaIcms;
                                else
                                    informacoesCommand.Parameters.Add("@aliquotaDeIcms", FbDbType.Decimal).Value = DBNull.Value;

                                if (decimal.TryParse(reducaoDeCalculoIcmsTextEdit.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var reducaoCalculo))
                                    informacoesCommand.Parameters.Add("@reducaoDeCalculo", FbDbType.Decimal).Value = reducaoCalculo;
                                else
                                    informacoesCommand.Parameters.Add("@reducaoDeCalculo", FbDbType.Decimal).Value = DBNull.Value;

                                informacoesCommand.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            XtraMessageBox.Show("Produto cadastrado com sucesso");
                            LimparTextEdits();
                            LimparLookUpEdits();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Erro ao inserir produto no banco de dados", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Erro ao salvar o produto: {ex.Message}");
            }
        }


        private void AtualizarEstiloLabelCodigoBarras(bool valido)
        {
            codigodebarrasLabelControl.Text = valido
                ? "Codigo de Barras: "
                : "Codigo de Barras: <color=red>*</color>";

            codigodebarrasLabelControl.AllowHtmlString = !valido;
        }

        // Validação de todos os campos obrigatorios adicionando um "*"
        private bool ValidarCamposObrigatorios()
        {
            var todosCamposPreenchidos = true;

            todosCamposPreenchidos &= !string.IsNullOrWhiteSpace(nomeTextEdit.Text);
            todosCamposPreenchidos &= !string.IsNullOrWhiteSpace(estoqueTextEdit.Text);
            todosCamposPreenchidos &= !string.IsNullOrWhiteSpace(precoVendaTextEdit.Text);
            todosCamposPreenchidos &= unidadeDeMedidaLookUpEdit.EditValue != null;
            todosCamposPreenchidos &= categoriaDeProdutosLookUpEdit.EditValue != null;

            nomeLabelControl.Text = string.IsNullOrWhiteSpace(nomeTextEdit.Text) ? "Nome: <color=red>*</color>" : "Nome: *";
            nomeLabelControl.AllowHtmlString = string.IsNullOrWhiteSpace(nomeTextEdit.Text);

            estoqueLabelControl.Text = string.IsNullOrWhiteSpace(estoqueTextEdit.Text) ? "Estoque: <color=red>*</color>" : "Estoque: *";
            estoqueLabelControl.AllowHtmlString = string.IsNullOrWhiteSpace(estoqueTextEdit.Text);

            precoDaVendaLabelControl.Text = string.IsNullOrWhiteSpace(precoVendaTextEdit.Text) ? "Preço da Venda: <color=red>*</color>" : "Preço da Venda: *";
            precoDaVendaLabelControl.AllowHtmlString = string.IsNullOrWhiteSpace(precoVendaTextEdit.Text);

            unidadeDeMedidaLabelControl.Text = unidadeDeMedidaLookUpEdit.EditValue == null ? "Und. de Medida: <color=red>*</color>" : "Und. de Medida: *";
            unidadeDeMedidaLabelControl.AllowHtmlString = unidadeDeMedidaLookUpEdit.EditValue == null;

            categoriaLabelControl.Text = categoriaDeProdutosLookUpEdit.EditValue == null ? "Categoria: <color=red>*</color>" : "Categoria: *";
            categoriaLabelControl.AllowHtmlString = categoriaDeProdutosLookUpEdit.EditValue == null;

            return todosCamposPreenchidos;
        }

        private void custoLabelControl_Click(object sender, EventArgs e)
        {

        }

        private void produtosTabNavigationPage_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pesquisarProdutoButtomItem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var pesquisaDeProdutos = new PesquisarProdutosView(string.Empty);
            pesquisaDeProdutos.ShowDialog();
        }

        private void nomeTextEdit_EditValueChanged(object sender, EventArgs e)
        {
            nomeTextEdit.Properties.MaxLength = 100;
        }

        private void categoriaDeProdutosLookUpEdit_EditValueChanged(object sender, EventArgs e)
        {
            categoriaDeProdutosLookUpEdit.Properties.MaxLength = 50;
        }

        private void unidadeDeMedidaLookUpEdit_EditValueChanged(object sender, EventArgs e)
        {
            unidadeDeMedidaLookUpEdit.Properties.MaxLength = 50;
        }

        private void marcaLookUpEdit_EditValueChanged(object sender, EventArgs e)
        {
            marcaLookUpEdit.Properties.MaxLength = 50;
        }

        private void origemDaMercadoriaLookUpEdit_EditValueChanged(object sender, EventArgs e)
        {
            origemDaMercadoriaLookUpEdit.Properties.MaxLength = 50;
        }

        private void situacaoTributariaLookUpEdit_EditValueChanged(object sender, EventArgs e)
        {
            situacaoTributariaLookUpEdit.Properties.MaxLength = 50;
        }

        private void naturezaDaOperacaoLookUpEdit_EditValueChanged(object sender, EventArgs e)
        {
            naturezaDaOperacaoLookUpEdit.Properties.MaxLength = 50;
        }
    }
}