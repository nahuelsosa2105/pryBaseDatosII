using System;
using System.Data.SqlClient;

namespace pryBaseDatosII
{
    public class conectorBaseDatos
    {
        private readonly string cadenaConexion;

        public conectorBaseDatos()
        {
            // Ajusta la cadena de conexión según tu configuración
            cadenaConexion = "Server=DESKTOP-ND5NU74;Database=AdventureWorks2022;User Id=admin;Password=admin**;";
        }

        public SqlConnection CrearConexion()
        {
            SqlConnection conexion = new SqlConnection(cadenaConexion);
            try
            {
                conexion.Open();
                Console.WriteLine("Conexión exitosa a la base de datos.");
                return conexion;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al conectar a la base de datos: " + ex.Message);
                return null;
            }
        }

        public void CerrarConexion(SqlConnection conexion)
        {
            if (conexion != null && conexion.State == System.Data.ConnectionState.Open)
            {
                conexion.Close();
                Console.WriteLine("Conexión cerrada correctamente.");
            }
        }
    }
}