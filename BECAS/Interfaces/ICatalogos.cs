using BECASLC;
using Microsoft.EntityFrameworkCore;

namespace BECAS.Interfaces
{
    public interface ICatalogos
    {
        public TipoMatricula GetTipoMatricula(string nombre);
        public Programa GetPrograma(string nombre);
        public CatSede GetSede(string nombre);
        public CatCarrera GetCarrera(string nombre);
        public SocioImplementador GetSocioImplementador(string nombre);
        public Sexo GetSexo(string nombre);
        public Departamento GetDepartamento(string nombre);
        public Municipio GetMunicipio(string nombre);
        public Refiere GetReferencias(string nombre);
        public Zona GetZona(string nombre);
        public CatAño GetYear(string nombre);
        public CatMe GetMes(string nombre);
        public Cohorte GetCohorte(string nombre);
    }

    public class CatalogosRepository : ICatalogos
    {
        private readonly MEOBContext _context;
        public CatalogosRepository(MEOBContext context)
        {
            _context = context;
        }
        public CatCarrera GetCarrera(string nombre)
        {
            try
            {

                var catCarrera = _context.Grados.Where(x => x.Nombre.Equals(nombre)).Select(x => new CatCarrera
                {
                    Nombre = x.Nombre,
                    IdCatCarrera = x.IdGrado
                }).FirstOrDefault();

                CatCarrera carrera = _context.CatCarreras.FirstOrDefault(x => x.Nombre.Equals(nombre));
                
                if (carrera != null)
                {
                    return carrera;
                }


                if (catCarrera != null)
                {
                    return catCarrera;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }


        }

        public Programa GetPrograma(string nombre)
        {
            Programa programa = _context.Programas.FirstOrDefault(x => x.Nombre.Equals(nombre));
            return programa;
        }

        public CatSede GetSede(string nombre)
        {
            CatSede sedes = _context.CatSedes.FirstOrDefault(x => x.Nombre.Equals(nombre));
            if (sedes == null)
            {
                var error = "";
            }
            return sedes;
        }

        public SocioImplementador GetSocioImplementador(string nombre)
        {
            SocioImplementador socio = _context.SocioImplementadors.FirstOrDefault(x => x.Nombre.Equals(nombre));
            return socio;
        }

        public TipoMatricula GetTipoMatricula(string nombre)
        {
            TipoMatricula matricula = _context.TipoMatriculas.FirstOrDefault(x => x.Nombre.Equals(nombre));
            return matricula;
        }

        public Sexo GetSexo(string nombre)
        {
            Sexo sexo = _context.Sexos.FirstOrDefault(x => x.Nombre.Equals(nombre));
            return sexo;
        }

        public Departamento GetDepartamento(string nombre)
        {
            Departamento departamento = _context.Departamentos.FirstOrDefault(x => x.Nombre2.Equals(nombre));
            return departamento;
        }

        public Refiere GetReferencias(string nombre)
        {
            Refiere refiere = _context.Refieres.FirstOrDefault(x => x.Nombre.Equals(nombre));
            return refiere;
        }

        public Zona GetZona(string nombre)
        {
            Zona zona = _context.Zonas.FirstOrDefault(x => x.Nombre.Equals(nombre));
            return zona;
        }

        public CatAño GetYear(string nombre)
        {
            CatAño year = _context.CatAños.FirstOrDefault(x => x.Nombre.Equals(nombre));
            return year;
        }
        public CatMe GetMes(string nombre)
        {
            CatMe mes = _context.CatMes.FirstOrDefault(x => x.Nombre.Equals(nombre));
            return mes;
        }

        public Cohorte GetCohorte(string nombre)
        {
            Cohorte cohorte = _context.Cohortes.FirstOrDefault(x => x.Nombre.Equals(nombre));
            return cohorte;
        }

        public Municipio GetMunicipio(string nombre)
        {
            Municipio municipio = _context.Municipios.FirstOrDefault(x => x.Nombre.Equals(nombre));
            return municipio;
        }
    }
}
