using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pryBaseDatosII
{
    public partial class Form1 : Form
    {
        private conectorBaseDatos conexionDB;

        public Form1()
        {
            InitializeComponent();
            conexionDB = new conectorBaseDatos();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Estado: Desconocido";
            toolStripStatusLabel1.ForeColor = System.Drawing.Color.Black;
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            SqlConnection conexion = conexionDB.CrearConexion();

            if (conexion != null)
            {
                toolStripStatusLabel1.Text = "Conectado ✔";
                toolStripStatusLabel1.ForeColor = System.Drawing.Color.Green;
                MessageBox.Show("Conexión exitosa a la base de datos.");

                // Aquí podrías realizar consultas SQL

                conexionDB.CerrarConexion(conexion);
            }
            else
            {
                toolStripStatusLabel1.Text = "Desconectado ❌";
                toolStripStatusLabel1.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show("Error al conectar con la base de datos.");
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            // Puedes agregar lógica aquí si deseas que haga algo cuando se haga clic en el label.
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Cargar Datos Masivos con sobrecarga en el servidor
            using (SqlConnection conexion = conexionDB.CrearConexion())
            {
                if (conexion != null)
                {
                    string query = @"
                SELECT s.SalesOrderID, s.OrderDate, p.Name, c.FirstName, c.LastName, 
                       d.LineTotal, SUM(d.LineTotal) OVER (PARTITION BY s.SalesOrderID) AS TotalPorOrden,
                       AVG(d.LineTotal) OVER () AS PromedioTotal
                FROM Sales.SalesOrderHeader s
                INNER JOIN Sales.SalesOrderDetail d ON s.SalesOrderID = d.SalesOrderID
                INNER JOIN Production.Product p ON d.ProductID = p.ProductID
                INNER JOIN Person.Person c ON s.CustomerID = c.BusinessEntityID
                ORDER BY s.OrderDate DESC";

                    SqlCommand comando = new SqlCommand(query, conexion);
                    SqlDataReader reader = comando.ExecuteReader();

                    int contador = 0;
                    while (reader.Read())
                    {
                        contador++;
                    }
                    reader.Close();

                    MessageBox.Show($"Consulta completada con carga elevada. Se procesaron {contador} registros.");
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Filtrar con Join Complejos y carga intensiva
            using (SqlConnection conexion = conexionDB.CrearConexion())
            {
                if (conexion != null)
                {
                    string query = @"
                SELECT s.SalesOrderID, s.OrderDate, p.Name, c.FirstName, c.LastName, 
       d.LineTotal, SUM(d.LineTotal) OVER (PARTITION BY s.SalesOrderID) AS TotalPorOrden,
       AVG(d.LineTotal) OVER () AS PromedioTotal,
       COUNT(*) OVER (PARTITION BY s.CustomerID) AS ComprasPorCliente,
       RANK() OVER (ORDER BY s.OrderDate DESC) AS OrdenRanking
FROM Sales.SalesOrderHeader s
INNER JOIN Sales.SalesOrderDetail d ON s.SalesOrderID = d.SalesOrderID
INNER JOIN Production.Product p ON d.ProductID = p.ProductID
INNER JOIN Person.Person c ON s.CustomerID = c.BusinessEntityID
ORDER BY s.OrderDate DESC;
";

                    SqlCommand comando = new SqlCommand(query, conexion);
                    comando.CommandTimeout = 180; // Extiende el tiempo de espera
                    SqlDataReader reader = comando.ExecuteReader();

                    int contador = 0;
                    while (reader.Read())
                    {
                        contador++;
                    }
                    reader.Close();

                    MessageBox.Show($"Consulta con joins complejos ejecutada. Se procesaron {contador} registros con carga avanzada.");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Actualizar Precios Masivos
            using (SqlConnection conexion = conexionDB.CrearConexion())
            {
                if (conexion != null)
                {
                    string query = "EXEC sp_ActualizarPreciosMasivos"; // Ejecutar el procedimiento almacenado
                    SqlCommand comando = new SqlCommand(query, conexion);
                    int filasAfectadas = comando.ExecuteNonQuery();

                    MessageBox.Show($"Precios actualizados en {filasAfectadas} productos.");
                }
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                using (SqlConnection conexion = conexionDB.CrearConexion())
                {
                    if (conexion != null)
                    {
                        SqlCommand comando = new SqlCommand("EXEC sp_ProcesamientoIntensivo2", conexion);
                        comando.CommandTimeout = 180; // Extiende el tiempo de espera

                        try
                        {
                            comando.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show($"Error en la ejecución: {ex.Message}");
                        }
                    }
                }
            });

            MessageBox.Show("Proceso intensivo ejecutado correctamente.");
        }
    }
}