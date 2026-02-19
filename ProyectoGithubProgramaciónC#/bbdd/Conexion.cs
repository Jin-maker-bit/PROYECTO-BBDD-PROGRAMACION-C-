using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoGithubProgramaciónC_.bbdd
{
    internal class Conexion
    {
        private static MySqlConnection conn;

        private static readonly string url =
            "Server=127.0.0.1;" +
            "Database=ventaslibreria;" +
            "User=root;" +
            "Port=3307;" +
            "Password=;";

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

        // ======================================================
        // PRINCIPAL - TOTALES (UNA SOLA CONSULTA)
        // ======================================================

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

        // ======================================================
        // PRINCIPAL - TOP 3 TIENDA
        // ======================================================

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

        // ======================================================
        // PRINCIPAL - TOP 3 ONLINE
        // ======================================================

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

        // ======================================================
        // INFORME 1 - TOP 10 EDITORIALES
        // ======================================================

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

        // ======================================================
        // INFORME 2 - FACTURACION VENDEDORES ACTIVOS
        // ======================================================

        public static void CargarGridInforme2_Vendedores(DataGridView dgv)
        {
            string consulta =
                "SELECT v.nombre AS VENDEDOR, ROUND(SUM(vt.precio), 2) AS FACTURACION " +
                "FROM vendedores v " +
                "INNER JOIN ventas_tienda vt ON vt.codVendedor = v.codVendedor " +
                "GROUP BY v.codVendedor, v.nombre " +
                "ORDER BY FACTURACION DESC;";

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
                MessageBox.Show("Error al cargar Informe 2 (Vendedores).\n" + e.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }

        // ======================================================
        // INFORME 2 - PLATAFORMAS
        // ======================================================

        public static void CargarGridInforme2_Plataformas(DataGridView dgv)
        {
            string consulta =
                "SELECT p.nombre AS PLATAFORMA, ROUND(SUM(vo.precio), 2) AS FACTURACION " +
                "FROM plataformas p " +
                "INNER JOIN ventas_online vo ON vo.idPlataforma = p.idPlataforma " +
                "GROUP BY p.idPlataforma, p.nombre " +
                "ORDER BY FACTURACION DESC;";

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
                MessageBox.Show("Error al cargar Informe 2 (Plataformas).\n" + e.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }

        // ======================================================
        // INFORME 3 - VOLUMENES POR UBICACION SEGUN SECCION (1..9)
        // ======================================================

        public static void CargarGridInforme3(DataGridView dgv, int seccion)
        {
            string consulta =
                "SELECT u.descripcion AS UBICACION, COALESCE(SUM(l.stock),0) AS VOLUMENES " +
                "FROM libros l " +
                "INNER JOIN ubicacion u ON u.ubicacion = l.codUbicacion " +
                "WHERE l.idClasificacion = " + seccion + " " +
                "GROUP BY u.ubicacion, u.descripcion " +
                "ORDER BY VOLUMENES DESC;";

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
                MessageBox.Show("Error al cargar Informe 3.\n" + e.Message);
            }
            finally
            {
                cerrarConexion();
            }
        }

        // ======================================================
        // INFORME 4 - CCAA Y LIBROS EDITADOS
        // ======================================================

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

        // ======================================================
        // INFORME 5 - TOP 5 CIUDADES
        // ======================================================

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
