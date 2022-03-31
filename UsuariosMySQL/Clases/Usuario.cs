using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace UsuariosMySQL.Clases
{
     public class Usuario
    {
        public int id_usuario { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string email { get; set; }

        //metodos de la clase

        public Usuario ()
        {
        }

            public Usuario (int pidUsuario, string pNombre,string pApellido, string pEmail)
            {
            this.id_usuario = pidUsuario;
            this.nombre = pNombre;
            this.apellido = pApellido;
            this.email = pEmail;
            }

        //    private static int AgregarUsuario()
        //{

        //}
        //    private static int ActualizarUsuario()
        //{

        //}

        //    private static int EliminarUsuario()
        //{

        //}
            public static IList<Usuario>Buscar(MySqlConnection conexion,string pNombre,string pApellido, string PEmail)
        {
            List<Usuario> lista = new List<Usuario>();

            MySqlCommand comando = new MySqlCommand(String.Format("SELECT id_usuario,nombre,apellido,email FROM usuario WHERE nombre LIKE ('%(0)%') AND apellido LIKE ('%(1)%') AND LIKE ('%(2)%')", pNombre,pApellido,PEmail),conexion);
            MySqlDataReader reader = comando.ExecuteReader();


            while (reader.Read())
            {
                Usuario pUsuario = new Usuario();
                pUsuario.id_usuario = reader.GetInt32(0);
                pUsuario.nombre = reader.GetString(1);
                pUsuario.apellido = reader.GetString(2);
                pUsuario.email = reader.GetString(3);

                lista.Add(pUsuario);
            }
            return lista;

        }
    }
}
