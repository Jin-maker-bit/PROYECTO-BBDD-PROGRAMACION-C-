

using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace ProyectoGithubProgramaciónC_.bbdd
{
    internal class Conexion
    {
        private static MySqlConnection conn;

        private static readonly string url =
            "Server=195.35.53.72;" +
            "Database=u812167471_grupo5;" +
            "User=u812167471_grupo5;" +
            "Port=3306;" +
            "Password=2026-Grupo5;";

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


        public static void obtenerTotales(out int totalLibros, out int totalVolumenes, out int totalVentas)
        {
            totalLibros = 0;
            totalVolumenes = 0;
            totalVentas = 0;

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

        // INFORME 2 - FACTURACION VENDEDORES ACTIVOS (VENDEDOR, FACTURACION €, ESTADO)
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

        // INFORME 2 - LIBROS Y PLATAFORMAS 
        public static void CargarGridInforme2_LibrosPlataformas(DataGridView dgv)
        {
            string consulta =
                "SELECT l.titulo AS LIBRO, p.nombre AS PLATAFORMAS " +
                "FROM ventas_online vo " +
                "INNER JOIN libros l ON vo.idLibro = l.idLibro " +
                "INNER JOIN plataformas p ON vo.idPlataforma = p.idPlataforma " +
                "ORDER BY p.nombre, l.titulo;";

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
                MessageBox.Show("Error al cargar Informe 2 (Libros/Plataformas).\n" + e.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }
        // INFORME 3 - VOLUMENES POR UBICACION SEGUN SECCION (1..9)


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
