using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Agenda
{
    public class ConexionBD
    {
        private string cadenaConexion;

        public ConexionBD()
        {
            cadenaConexion = ConfigurationManager.ConnectionStrings["ConexionAgenda"].ConnectionString;
        }

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }

        // Método para ejecutar SELECT
        public DataTable EjecutarConsulta(string query, SqlParameter[] parametros = null)
        {
            DataTable tabla = new DataTable();

            try
            {
                using (SqlConnection conexion = ObtenerConexion())
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;

                    if (parametros != null)
                        comando.Parameters.AddRange(parametros);

                    using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                    {
                        adaptador.Fill(tabla);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar consulta: " + ex.Message);
            }

            return tabla;
        }

        // INSERT/UPDATE/DELETE -> filas afectadas
        public int EjecutarComando(string query, SqlParameter[] parametros = null)
        {
            try
            {
                using (SqlConnection conexion = ObtenerConexion())
                {
                    conexion.Open();

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.CommandType = CommandType.Text;

                        if (parametros != null)
                            comando.Parameters.AddRange(parametros);

                        return comando.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar comando: " + ex.Message);
            }
        }

        // SELECT que devuelve 1 valor (COUNT, SCOPE_IDENTITY, etc.)
        public object EjecutarEscalar(string query, SqlParameter[] parametros = null)
        {
            try
            {
                using (SqlConnection conexion = ObtenerConexion())
                {
                    conexion.Open();

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.CommandType = CommandType.Text;

                        if (parametros != null)
                            comando.Parameters.AddRange(parametros);

                        return comando.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar escalar: " + ex.Message);
            }
        }
    }
}