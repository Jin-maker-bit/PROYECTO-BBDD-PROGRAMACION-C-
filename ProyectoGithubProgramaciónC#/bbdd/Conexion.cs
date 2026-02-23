using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace ProyectoGithubProgramaciónC_.bbdd
{
    // Clase que gestiona la conexión y las consultas a la base de datos MySQL
    internal class Conexion
    {
        private static MySqlConnection conn;

        // Parámetros del servidor remoto
        private static readonly string url =
            "Server=195.35.53.72;" +
            "Database=u812167471_grupo5;" +
            "User=u812167471_grupo5;" +
            "Port=3306;" +
            "Password=2026-Grupo5;";

        // Abre la conexión con la base de datos
        public static void conectar()
        {
            try
            {
                conn = new MySqlConnection(url);
                conn.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error al conectar con la base de datos.\n" + e.Message);
            }
        }

        // Cierra la conexión con la base de datos 
        public static void cerrarConexion()
        {
            if (conn != null)
            {
                try
                {
                    conn.Close();
                }
                catch (MySqlException e)
                {
                    MessageBox.Show("Error al cerrar la conexión.\n" + e.Message);
                }
            }
        }

        // PRINCIPAL - TOTALES

        // Obtiene tres contadores globales: total de libros, total de volúmenes en stock
        // y total de ventas combinadas (tienda y online). Se devuelven como parámetros de salida.
        public static void obtenerTotales(out int totalLibros, out int totalVolumenes, out int totalVentas)
        {
            totalLibros = 0;
            totalVolumenes = 0;
            totalVentas = 0;

            // Consulta que agrupa las tres subconsultas
            string consulta =
                "SELECT " +
                "(SELECT COUNT(*) FROM libros) AS totalLibros, " +
                "(SELECT COALESCE(SUM(stock),0) FROM libros) AS totalVolumenes, " +
                "((SELECT COUNT(*) FROM ventas_tienda) + (SELECT COUNT(*) FROM ventas_online)) AS totalVentas;";

            conectar();

            try
            {
                MySqlCommand comando = new MySqlCommand(consulta, conn);
                MySqlDataReader reader = comando.ExecuteReader();

                
                if (reader.Read())
                {
                    totalLibros = reader.GetInt32(0);
                    totalVolumenes = reader.GetInt32(1);
                    totalVentas = reader.GetInt32(2);
                }

                reader.Close();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error al obtener los totales.\n" + e.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }

        
        // PRINCIPAL - TOP 3 TIENDA

        // Carga en el DataGridView los 3 libros más vendidos en tienda física,
        // ordenados de mayor a menor número de ventas
        public static void CargarGridTop3Tienda(DataGridView dgv)
        {
            string consulta =
                "SELECT l.titulo AS LIBRO, COUNT(vt.idVenta) AS VENTAS " +
                "FROM ventas_tienda vt " +
                "INNER JOIN libros l ON l.idLibro = vt.idLibro " +
                "GROUP BY l.idLibro, l.titulo " +
                "ORDER BY VENTAS DESC " +
                "LIMIT 3;";

            conectar();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(consulta, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error al cargar Top 3 Tienda.\n" + e.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }

        // PRINCIPAL - TOP 3 ONLINE

        // Carga en el DataGridView los 3 libros más vendidos por canal online,
        // ordenados de mayor a menor número de ventas
        public static void CargarGridTop3Online(DataGridView dgv)
        {
            string consulta =
                "SELECT l.titulo AS LIBRO, COUNT(vo.idVenta) AS VENTAS " +
                "FROM ventas_online vo " +
                "INNER JOIN libros l ON l.idLibro = vo.idLibro " +
                "GROUP BY l.idLibro, l.titulo " +
                "ORDER BY VENTAS DESC " +
                "LIMIT 3;";

            conectar();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(consulta, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error al cargar Top 3 Online.\n" + e.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }

        // INFORME 1 - TOP 10 EDITORIALES
        

        // Carga en el DataGridView las 10 editoriales con mayor número de libros
        // registrados en el catálogo, ordenadas de mayor a menor
        public static void CargarGridInforme1(DataGridView dgv)
        {
            string consulta =
                "SELECT e.nombre AS EDITORIAL, COUNT(l.idLibro) AS LIBROS " +
                "FROM editoriales e " +
                "INNER JOIN libros l ON l.idEditorial = e.idEditorial " +
                "GROUP BY e.idEditorial, e.nombre " +
                "ORDER BY LIBROS DESC " +
                "LIMIT 10;";

            conectar();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(consulta, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error al cargar Informe 1.\n" + e.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }

        // INFORME 2 - FACTURACIÓN VENDEDORES ACTIVOS

        // Carga en el DataGridView la facturación total de cada vendedor con estado activo,
        // calculada como la suma de los precios de sus ventas en tienda, ordenada de mayor a menor.
        // Aplica formato monetario con dos decimales a la columna de facturación.
        public static void CargarGridInforme2_Vendedores(DataGridView dgv)
        {
            string consulta =
                "SELECT v.nombre AS VENDEDOR, " +
                "       SUM(vt.precio) AS FACTURACION, " +
                "       e.estado AS ESTADO " +
                "FROM vendedores v " +
                "INNER JOIN estados e ON v.idEstado = e.idEstado " +
                "INNER JOIN ventas_tienda vt ON vt.codVendedor = v.codVendedor " +
                "WHERE e.estado = 'Activo' " +
                "GROUP BY v.codVendedor, v.nombre, e.estado " +
                "ORDER BY FACTURACION DESC;";

            conectar();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(consulta, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;

                // Se aplica formato visual con símbolo de euro y alineación a la derecha
                if (dgv.Columns["FACTURACION"] != null)
                {
                    dgv.Columns["FACTURACION"].DefaultCellStyle.Format = "0.00 €";
                    dgv.Columns["FACTURACION"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error al cargar Informe 2 (Vendedores).\n" + e.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }

        // INFORME 2 - FACTURACIÓN POR PLATAFORMA

        // Carga en el DataGridView la facturación total agrupada por plataforma,
        // ordenada de mayor a menor facturación.
        public static void CargarGridInforme2_LibrosPlataformas(DataGridView dgv)
        {
            string consulta =
                "SELECT p.nombre AS PLATAFORMA, " +
                "       SUM(vo.precio) AS CANTIDAD " +
                "FROM plataformas p " +
                "INNER JOIN ventas_online vo ON p.idPlataforma = vo.idPlataforma " +
                "GROUP BY p.idPlataforma, p.nombre ";

            conectar();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(consulta, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;

                // Renombrar columna y aplicar formato monetario
                if (dt.Columns["FACTURACION_TOTAL"] != null)
                    dt.Columns["FACTURACION_TOTAL"].ColumnName = "FACTURACION TOTAL";

                if (dgv.Columns["FACTURACION TOTAL"] != null)
                {
                    dgv.Columns["FACTURACION TOTAL"].DefaultCellStyle.Format = "0.00 €";
                    dgv.Columns["FACTURACION TOTAL"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error al cargar Informe 2 (Plataformas).\n" + e.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }


        // INFORME 3 - VOLÚMENES POR UBICACIÓN SEGÚN SECCIÓN (1..9)


        // Carga en el DataGridView los volúmenes de libros agrupados por ubicación
        // dentro de la sección indicada como parámetro.
        // Utiliza una expresión regular para extraer el número de sección del código de ubicación.
        // Devuelve el total acumulado de volúmenes de dicha sección.
        public static int CargarGridInforme3(DataGridView dgv, int piso)
        {
            int total = 0;


            string consulta =
                "SELECT u.ubicacion AS UBICACION, COALESCE(SUM(l.stock),0) AS VOLUMENES " +
                "FROM libros l " +
                "INNER JOIN ubicacion u ON l.codUbicacion = u.ubicacion " +
                "WHERE CAST(REGEXP_SUBSTR(u.ubicacion, '^[0-9]+') AS UNSIGNED) = " + piso + " " +
                "GROUP BY u.ubicacion, u.descripcion " +
                "ORDER BY REGEXP_SUBSTR(u.ubicacion, '[A-Za-z]+$');";

            conectar();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(consulta, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv.DataSource = dt;

                // Se recorren todas las filas para acumular el total de volúmenes de la sección
                foreach (DataRow row in dt.Rows)
                    total += Convert.ToInt32(row["VOLUMENES"]);
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error al cargar Informe 3.\n" + e.Message);
                total = 0;
            }
            finally
            {
                cerrarConexion();
            }

            return total;
        }

       
        // INFORME 4 - CCAA Y LIBROS EDITADOS
  

        // Carga en el DataGridView el número de libros publicados agrupados
        // por Comunidad Autónoma de edición, ordenados de mayor a menor
        public static void CargarGridInforme4(DataGridView dgv)
        {
            string consulta =
                "SELECT le.ccaa AS CCAA, COUNT(l.idLibro) AS LIBROS " +
                "FROM lugar_edicion le " +
                "INNER JOIN libros l ON l.idLugar = le.idLugar " +
                "GROUP BY le.ccaa " +
                "ORDER BY LIBROS DESC;";

            conectar();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(consulta, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error al cargar Informe 4.\n" + e.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }


        // INFORME 5 - TOP 5 CIUDADES


        // Carga en el DataGridView las 5 ciudades con mayor número de libros
        // editados en ellas, ordenadas de mayor a menor
        public static void CargarGridInforme5(DataGridView dgv)
        {
            string consulta =
                "SELECT le.lugar AS CIUDAD, COUNT(l.idLibro) AS LIBROS " +
                "FROM lugar_edicion le " +
                "INNER JOIN libros l ON l.idLugar = le.idLugar " +
                "GROUP BY le.lugar " +
                "ORDER BY LIBROS DESC " +
                "LIMIT 5;";

            conectar();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(consulta, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error al cargar Informe 5.\n" + e.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }
    }
}