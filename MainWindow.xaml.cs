using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Teste.Capta.WPF.Models;

namespace Teste.Capta.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client = new HttpClient();
        public MainWindow()
        {
            client.BaseAddress = new Uri("http://localhost:39383");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
            );

            InitializeComponent();

            PopularGridClientes();
            PopularComboboxTiposClientes();
            PopularComboboxSituacoesClientes();

        }

        /* API Calls Listagem */
        private async void PopularGridClientes()
        {
            Loading(true);

            try
            {
                var response = await client.GetStringAsync("/api/clientes");
                var clientes = JsonConvert.DeserializeObject<RetornoDTO>(response);

                dgClientes.DataContext = (clientes != null ? clientes.data : null);
                Loading(false);
                return;
            }
            catch (HttpRequestException exHttp)
            {
                MessageBox.Show($"Erro ao listar clientes: {exHttp.Message}", "Atenção");
                Loading(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao listar clientes: {ex.Message}", "Atenção");
                Loading(false);
            }
        }
        private async void PopularComboboxTiposClientes() {

            try
            {
                var httpResponse = await client.GetStringAsync("/api/clientes/tipos");
                var jsonResponse = JsonConvert.DeserializeObject<RetornoDominioDTO>(httpResponse);

                cmbTipo.ItemsSource = (jsonResponse != null ? jsonResponse.data : null);
            }
            catch (HttpRequestException exHttp)
            {
                MessageBox.Show($"Erro ao listar tipos de clientes: {exHttp.Message}", "Atenção");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao listar tipos de clientes: {ex.Message}", "Atenção");
            }

        }
        private async void PopularComboboxSituacoesClientes() {

            try
            {
                var httpResponse = await client.GetStringAsync("/api/clientes/situacoes");
                var jsonResponse = JsonConvert.DeserializeObject<RetornoDominioDTO>(httpResponse);

                cmbSituacao.ItemsSource = (jsonResponse != null ? jsonResponse.data : null);
            }
            catch (HttpRequestException exHttp)
            {
                MessageBox.Show($"Erro ao listar situações de clientes: {exHttp.Message}", "Atenção");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao listar situações de clientes: {ex.Message}", "Atenção");
            }

        }


        /* API Calls Persistencia */
        private async void SalvarCliente(Cliente cliente)
        {
            btnSalvar.IsEnabled = false;

            try
            {
                HttpResponseMessage httpResponse;

                if (cliente.Id == 0) {
                    httpResponse = await client.PostAsJsonAsync("/api/clientes", cliente);
                }
                else {
                    httpResponse = await client.PutAsJsonAsync("/api/clientes", cliente);
                }

                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseBody = await httpResponse.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<RetornoDTO>(responseBody);

                    if (result != null && !result.success)
                    {
                        MessageBox.Show($"Erro ao salvar cliente: {result.message}", "Atenção");
                    }
                }

                btnSalvar.IsEnabled = true;
                LimparFormulario();
                PopularGridClientes();
            }
            catch (Exception ex) {

                MessageBox.Show($"Erro ao salvar cliente: {ex.Message}", "Atenção");
                btnSalvar.IsEnabled = true;
                return;
            }

        }
        private async void DeletarCliente(int Id)
        {
            try
            {
                HttpResponseMessage httpResponse = await client.DeleteAsync("/api/clientes/" + Id);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseBody = await httpResponse.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<RetornoDTO>(responseBody);

                    if (result != null && !result.success)
                    {
                        MessageBox.Show($"Erro ao deletar cliente: {result.message}", "Atenção");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao deletar cliente: {ex.Message}", "Atenção");
                return;
            }

            LimparFormulario();
            PopularGridClientes();
        }


        /* Eventos Click */
        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario()) {
                MessageBox.Show("Por favor, preencha todos os campos", "Atenção");
                return;
            }

            var cliente = RetornarClientePeloFormulario();
            SalvarCliente(cliente);
        }
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimparFormulario();
        }
        void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            ClienteDTO cliente = ((FrameworkElement)sender).DataContext as ClienteDTO;

            if (cliente != null) {

                txtId.Text = cliente.Id.ToString();
                txtNome.Text = cliente.Nome;
                txtCPF.Text = cliente.CPF;
                cmbSexo.Text = cliente.Sexo;
                cmbTipo.SelectedValue = cliente.IdTipoCliente;
                cmbSituacao.SelectedValue = cliente.IdSituacaoCliente;
            }

        }
        void btnDeletar_Click(object sender, RoutedEventArgs e)
        {
            ClienteDTO cliente = ((FrameworkElement)sender).DataContext as ClienteDTO;

            if (cliente == null) {
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Confirma deleção do cliente? \nId: {cliente.Id} Nome: {cliente.Nome}", "Atenção!", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                DeletarCliente(cliente.Id);
            }

        }
        void btnAtualizarGrid_Click(object sender, RoutedEventArgs e)
        {
            PopularGridClientes();
        }


        /* Funcoes de Apoio */
        private Cliente RetornarClientePeloFormulario()
        {
            var cliente = new Cliente();

            cliente.Id = Convert.ToInt32(txtId.Text);
            cliente.Nome = txtNome.Text;
            cliente.CPF = txtCPF.Text;
            cliente.Sexo = cmbSexo.Text;
            cliente.IdTipoCliente = (int)cmbTipo.SelectedValue;
            cliente.IdSituacaoCliente = (int)cmbSituacao.SelectedValue;

            return cliente;
        }
        private bool ValidarFormulario()
        {
            if (txtNome.Text == "" || txtCPF.Text == "" || cmbSexo.Text == ""
                || cmbTipo.Text == "" || cmbSituacao.Text == "")
            {
                return false;
            }

            return true;
        }
        private void LimparFormulario()
        {
            txtId.Text = "0";
            txtNome.Text = "";
            txtCPF.Text = "";
            cmbSexo.SelectedIndex = -1;
            cmbTipo.SelectedIndex = -1;
            cmbSituacao.SelectedIndex = -1;
        }
        private void Loading(bool show)
        {
            lblLoading.Visibility = (show) ? Visibility.Visible : Visibility.Hidden;
            dgClientes.IsEnabled = !show;
        }

    }
}
