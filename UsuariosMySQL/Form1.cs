using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;


namespace UsuariosMySQL
{
    public partial class Form1 : Form
    {
        Clases.Conexion conexion = new Clases.Conexion();
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (conexion.AbrirConexion() == true)
                {
                    ListarUsuarios(conexion.conexion, txtnombre.Text, txtapellido.Text, txtemail.Text);
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ListarUsuarios(MySqlConnection conexion, String pNombre, String pApellido, String pEmail)
        {
            dgvUsuario.DataSource = Clases.Usuario.Buscar(conexion, pNombre, pApellido, pEmail);
        }
    }
}
