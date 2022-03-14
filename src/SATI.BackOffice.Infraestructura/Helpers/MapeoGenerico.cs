
namespace SATI.BackOffice.Infraestructura.Helpers
{
    using SATI.BackOffice.Infraestructura.Intefaces;
    using Microsoft.Data.SqlClient;
    using System;
    using System.Data;

    public interface IMapeoDatos<T>
    {
        T Map(SqlDataReader dr);
    }

    #region DataMapper DR2Entidad
    /// <summary>
    /// Patrón Mapper orientado a mapear entidad desde el SqlDataReader y una entidad del contrato.
    /// </summary>
    /// <typeparam name="T">Entidad que devuelve</typeparam>
    /// 
    public class MapeoGenerico<T> : MapeoGenericoBase, IMapeoDatos<T> where T : IEntidades
   {
        public T Map(SqlDataReader dr)
        {
            var entidad = base.MapAutomatico<T>(dr);

            return entidad;
        }
    }

    public abstract class MapeoGenericoBase
    {
        public bool HasColumn(SqlDataReader Reader, string ColumnName)
        {
            foreach (DataRow row in Reader.GetSchemaTable().Rows)
            {
                if (row["ColumnName"].ToString() == ColumnName)
                    return true;
            }
            return false;
        }

        internal T MapAutomatico<T>(SqlDataReader dr) where T : IEntidades
        {
            var t = typeof(T);

            T result = Activator.CreateInstance<T>();

            var properties = t.GetProperties();

            foreach (var p in properties)
            {
                if (p.PropertyType == typeof(int))
                {
                    p.SetValue(result, this.MapInt(dr, p.Name));
                    continue;
                }

                if (p.PropertyType == typeof(int?))
                {
                    p.SetValue(result, this.MapIntNulleable(dr, p.Name));
                    continue;

                }

                if (p.PropertyType == typeof(short))
                {
                    p.SetValue(result, this.MapShort(dr, p.Name));
                    continue;

                }

                if (p.PropertyType == typeof(short?))
                {
                    p.SetValue(result, this.MapShortNulleable(dr, p.Name));
                    continue;

                }

                if (p.PropertyType == typeof(string))
                {
                    p.SetValue(result, this.MapString(dr, p.Name));
                    continue;
                }

                if (p.PropertyType == typeof(bool))
                {
                    p.SetValue(result, this.MapBool(dr, p.Name));
                    continue;
                }

                if (p.PropertyType == typeof(bool?))
                {
                    p.SetValue(result, this.MapBoolNulleable(dr, p.Name));
                    continue;
                }

                if (p.PropertyType == typeof(char))
                {
                    p.SetValue(result, this.MapChar(dr, p.Name));
                    continue;
                }

                if (p.PropertyType == typeof(char?))
                {
                    p.SetValue(result, this.MapCharNulleable(dr, p.Name));
                    continue;
                }

                if (p.PropertyType == typeof(DateTime))
                {
                    p.SetValue(result, this.MapDateTime(dr, p.Name));
                    continue;
                }

                if (p.PropertyType == typeof(DateTime?))
                {
                    p.SetValue(result, this.MapDateTimeNulleable(dr, p.Name));
                    continue;
                }

                if (p.PropertyType == typeof(long))
                {
                    p.SetValue(result, this.MapInt64Nulleable(dr, p.Name));
                    continue;
                }

                if (p.PropertyType == typeof(long?))
                {
                    p.SetValue(result, this.MapInt64Nulleable(dr, p.Name));
                    continue;
                }

                if (p.PropertyType == typeof(Guid))
                {
                    p.SetValue(result, this.MapGuid(dr, p.Name));
                    continue;
                }

                if (p.PropertyType == typeof(Guid?))
                {
                    p.SetValue(result, this.MapGuidNulleable(dr, p.Name));
                    continue;
                }
            }

            return result;
        }


        internal int MapInt(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                return Convert.ToInt32(dr[column]);
            }

            return 0;
        }
        internal int? MapIntNulleable(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                if (dr[column] != null && dr[column] != DBNull.Value)
                {
                    return Convert.ToInt32(dr[column]);
                }
            }

            return null;
        }

        internal short MapShort(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                return Convert.ToInt16(dr[column]);
            }

            return 0;
        }
        internal short? MapShortNulleable(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                if (dr[column] != null && dr[column] != DBNull.Value)
                {
                    return Convert.ToInt16(dr[column]);
                }
            }

            return null;
        }


        internal char MapChar(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                return Convert.ToChar(dr[column]);
            }

            return char.MinValue;
        }
        internal char? MapCharNulleable(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                if (dr[column] != null && dr[column] != DBNull.Value)
                {
                    return Convert.ToChar(dr[column]);
                }
            }

            return null;
        }

       internal long MapInt64(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                return Convert.ToInt64(dr[column]);
            }

            return 0;
        }

        internal long? MapInt64Nulleable(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                if (dr[column] != null && dr[column] != DBNull.Value)
                    return Convert.ToInt64(dr[column]);
            }

            return null;
        }

        private Guid MapGuid(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                return Guid.Parse(dr[column].ToString());
            }

            return default;
        }

        private Guid? MapGuidNulleable(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                Guid res;
                if (Guid.TryParse(dr[column].ToString(), out res))
                {
                    return res;
                }
            }

            return null;
        }

        internal DateTime MapDateTime(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                return Convert.ToDateTime(dr[column]);
            }

            return DateTime.MinValue;
        }

        internal DateTime? MapDateTimeNulleable(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                if (dr[column] != null && dr[column] != DBNull.Value)
                    return Convert.ToDateTime(dr[column]);
            }

            return null;
        }

        internal string MapString(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                return dr[column].ToString();
            }

            return null;
        }

        internal bool MapBool(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                return Convert.ToBoolean(dr[column]);
            }

            return false;
        }

        internal bool? MapBoolNulleable(SqlDataReader dr, string column)
        {
            if (HasColumn(dr, column))
            {
                if (dr[column] != null && dr[column] != DBNull.Value)
                    return Convert.ToBoolean(dr[column]);
            }

            return null;
        }

    }

    #endregion
}
