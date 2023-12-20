using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace BECASLC
{
    public partial class MEOBContext : DbContext
    {
        public MEOBContext()
        {
        }

        public MEOBContext(DbContextOptions<MEOBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Carga> Cargas { get; set; } = null!;
        public virtual DbSet<CargaEducacion> CargaEducacions { get; set; } = null!;
        public virtual DbSet<CargaEstipendio> CargaEstipendios { get; set; } = null!;
        public virtual DbSet<CargaEvaluacionPsicosocial> CargaEvaluacionPsicosocials { get; set; } = null!;
        public virtual DbSet<CargaSeguimientoAutoempleo> CargaSeguimientoAutoempleos { get; set; } = null!;
        public virtual DbSet<CargaSeguimientoPasantia> CargaSeguimientoPasantias { get; set; } = null!;
        public virtual DbSet<CargaSeguimientoPracticasPr> CargaSeguimientoPracticasPrs { get; set; } = null!;
        public virtual DbSet<CargaSeguimientoPsicosocial> CargaSeguimientoPsicosocials { get; set; } = null!;
        public virtual DbSet<Carrera> Carreras { get; set; } = null!;
        public virtual DbSet<CatAño> CatAños { get; set; } = null!;
        public virtual DbSet<CatCarrera> CatCarreras { get; set; } = null!;
        public virtual DbSet<CatMe> CatMes { get; set; } = null!;
        public virtual DbSet<CatSede> CatSedes { get; set; } = null!;
        public virtual DbSet<Cohorte> Cohortes { get; set; } = null!;
        public virtual DbSet<CursosCorto> CursosCortos { get; set; } = null!;
        public virtual DbSet<Departamento> Departamentos { get; set; } = null!;
        public virtual DbSet<EstadoPersona> EstadoPersonas { get; set; } = null!;
        public virtual DbSet<Grado> Grados { get; set; } = null!;
        public virtual DbSet<GradoSede> GradoSedes { get; set; } = null!;
        public virtual DbSet<Materium> Materia { get; set; } = null!;
        public virtual DbSet<Municipio> Municipios { get; set; } = null!;
        public virtual DbSet<Persona> Personas { get; set; } = null!;
        public virtual DbSet<Programa> Programas { get; set; } = null!;
        public virtual DbSet<Refiere> Refieres { get; set; } = null!;
        public virtual DbSet<Rol> Rols { get; set; } = null!;
        public virtual DbSet<Sede> Sedes { get; set; } = null!;
        public virtual DbSet<Sexo> Sexos { get; set; } = null!;
        public virtual DbSet<SocioImplementador> SocioImplementadors { get; set; } = null!;
        public virtual DbSet<TipoMatricula> TipoMatriculas { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public virtual DbSet<Zona> Zonas { get; set; } = null!;
        public virtual DbSet<Sector> Sectors { get; set; } = null!;
        public virtual DbSet<Proyectos> Proyectos { get; set;} = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                string con = ClassConnection.Connectionstring();
                optionsBuilder.UseSqlServer(con);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Carga>(entity =>
            {
                entity.HasKey(e => e.IdCarga);

                entity.ToTable("Carga");

                entity.Property(e => e.FechaCarga).HasColumnType("datetime");

                entity.Property(e => e.PIdOim)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("p_id_oim");

                entity.Property(e => e.RAño).HasColumnName("r_año");

                entity.Property(e => e.RFechafin)
                    .HasColumnType("datetime")
                    .HasColumnName("r_fechafin");

                entity.Property(e => e.RFechaini)
                    .HasColumnType("datetime")
                    .HasColumnName("r_fechaini");

                entity.Property(e => e.RMes)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("r_mes");
            });

            modelBuilder.Entity<CargaEducacion>(entity =>
            {
                entity.HasKey(e => e.IdCargaEducacion);

                //entity.HasOne(e => e.Persona) // Relación uno a uno o uno a muchos, según tus necesidades
                //    .WithMany(p => p.CargaEducacions)
                //    .HasForeignKey(e => e.PIdOim);

                entity.ToTable("CargaEducacion");

                entity.Property(e => e.DEstado)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("d_estado");

                entity.Property(e => e.DFechaReasg)
                    .HasColumnType("datetime")
                    .HasColumnName("d_fecha_reasg");

                entity.Property(e => e.DFechades)
                    .HasColumnType("datetime")
                    .HasColumnName("d_fechades");

                entity.Property(e => e.DMotivodesercion)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("d_motivodesercion");

                entity.Property(e => e.ICausaReprobacion)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("i_causa_reprobacion");

                entity.Property(e => e.IDiasAsistenciaEfectivos).HasColumnName("i_dias_asistencia_efectivos");

                entity.Property(e => e.IDiasAsistenciaEstablecidos).HasColumnName("i_dias_asistencia_establecidos");

                entity.Property(e => e.IModulosAprobados).HasColumnName("i_modulos_aprobados");

                entity.Property(e => e.IModulosInscritos).HasColumnName("i_modulos_inscritos");

                entity.Property(e => e.IModulosReprobados).HasColumnName("i_modulos_reprobados");

                entity.Property(e => e.IMotivoInasistencia)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("i_motivo_inasistencia");

                entity.Property(e => e.PIdOim)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("p_id_oim");

                entity.Property(e => e.RAño).HasColumnName("r_año");

                entity.Property(e => e.RMes)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("r_mes");
            });

            modelBuilder.Entity<CargaEstipendio>(entity =>
            {
                entity.HasKey(e => e.IdCargaEstipendios);

                entity.Property(e => e.AlimDiasPresencialesEfectivo)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_dias_presenciales_efectivo");

                entity.Property(e => e.AlimDiasPresencialesEspecie)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_dias_presenciales_especie");

                entity.Property(e => e.AlimDiasPresencialesTransferencia)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_dias_presenciales_transferencia");

                entity.Property(e => e.AlimEfectivo)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_efectivo");

                entity.Property(e => e.AlimEspecie)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_especie");

                entity.Property(e => e.AlimMontoEfectivo)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_monto_efectivo");

                entity.Property(e => e.AlimMontoEspecie)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_monto_especie");

                entity.Property(e => e.AlimMontoTotal)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_monto_total");

                entity.Property(e => e.AlimMontoTransferencia)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_monto_transferencia");

                entity.Property(e => e.AlimSubtotalEfectivo)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_subtotal_efectivo");

                entity.Property(e => e.AlimSubtotalEspecie)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_subtotal_especie");

                entity.Property(e => e.AlimSubtotalTransferencia)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_subtotal_transferencia");

                entity.Property(e => e.AlimTransferencia)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("alim_transferencia");

                entity.Property(e => e.Año).HasColumnName("año");

                entity.Property(e => e.ConecDiasPresencialesEfectivo)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("conec_dias_presenciales_efectivo");

                entity.Property(e => e.ConecDiasPresencialesTransferencia)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("conec_dias_presenciales_transferencia");

                entity.Property(e => e.ConecEfectivo)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("conec_efectivo");

                entity.Property(e => e.ConecMontoEfectivo)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("conec_monto_efectivo");

                entity.Property(e => e.ConecMontoTotal)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("conec_monto_total");

                entity.Property(e => e.ConecMontoTransferencia)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("conec_monto_transferencia");

                entity.Property(e => e.ConecSubtotalEfectivo)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("conec_subtotal_efectivo");

                entity.Property(e => e.ConecSubtotalTransferencia)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("conec_subtotal_transferencia");

                entity.Property(e => e.ConecTransferencia)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("conec_transferencia");

                entity.Property(e => e.EstipendioTotal)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("estipendio_total");

                entity.Property(e => e.Mes)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mes");

                entity.Property(e => e.PId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("p_id");

                entity.Property(e => e.TranspDiasPresencialesEfectivo)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transp_dias_presenciales_efectivo");

                entity.Property(e => e.TranspDiasPresencialesTransferencia)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transp_dias_presenciales_transferencia");

                entity.Property(e => e.TranspEfectivo)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transp_efectivo");

                entity.Property(e => e.TranspMontoEfectivo)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transp_monto_efectivo");

                entity.Property(e => e.TranspMontoTotal)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transp_monto_total");

                entity.Property(e => e.TranspMontoTransferencia)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transp_monto_transferencia");

                entity.Property(e => e.TranspSubtotalEfectivo)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transp_subtotal_efectivo");

                entity.Property(e => e.TranspSubtotalTransferencia)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transp_subtotal_transferencia");

                entity.Property(e => e.TranspTarifaDiferenciada)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transp_tarifa_diferenciada");

                entity.Property(e => e.TranspTransferencia)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("transp_transferencia");
            });

            modelBuilder.Entity<CargaEvaluacionPsicosocial>(entity =>
            {
                entity.HasKey(e => e.IdCargaEvaluacionPsicosocial);

                entity.ToTable("CargaEvaluacionPsicosocial");

                entity.Property(e => e.Año).HasColumnName("año");

                entity.Property(e => e.EpAlertaDesercion)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ep_alerta_desercion");

                entity.Property(e => e.EpInstrumentoRiesgo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ep_instrumento_riesgo");

                entity.Property(e => e.EpVulnerabilidades)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ep_vulnerabilidades");

                entity.Property(e => e.Mes)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mes");

                entity.Property(e => e.OvParticipacion).HasColumnName("ov_participacion");

                entity.Property(e => e.OvPuntajePos)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ov_puntaje_pos");

                entity.Property(e => e.OvPuntajePret)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ov_puntaje_pret");

                entity.Property(e => e.PId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("p_id");
            });

            modelBuilder.Entity<CargaSeguimientoAutoempleo>(entity =>
            {
                entity.HasKey(e => e.IdSeguimientoAutoempleo);

                entity.ToTable("CargaSeguimientoAutoempleo");

                entity.Property(e => e.AutoempEmpresa)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("autoemp_empresa");

                entity.Property(e => e.AutoempEstado)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("autoemp_estado");

                entity.Property(e => e.AutoempFechaInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("autoemp_fecha_inicio");

                entity.Property(e => e.AutoempPlanNegocios)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("autoemp_plan_negocios");

                entity.Property(e => e.AutoempRegistro)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("autoemp_registro");

                entity.Property(e => e.AutoempTipoCapital)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("autoemp_tipo_capital");

                entity.Property(e => e.AutoempTipoEmpresa)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("autoemp_tipo_empresa");

                entity.Property(e => e.AutoempTipoEmpresaOtro)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("autoemp_tipo_empresa_otro");

                entity.Property(e => e.AutoempTipoFinanciamiento)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("autoemp_tipo_financiamiento");

                entity.Property(e => e.Año).HasColumnName("año");

                entity.Property(e => e.Mes)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mes");

                entity.Property(e => e.PId)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("p_id");
            });

            modelBuilder.Entity<CargaSeguimientoPasantia>(entity =>
            {
                entity.HasKey(e => e.IdSeguimientoPasantias);

                entity.Property(e => e.Año).HasColumnName("año");

                entity.Property(e => e.Mes)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mes");

                entity.Property(e => e.PId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("p_id");

                entity.Property(e => e.PasCargo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pas_cargo");

                entity.Property(e => e.PasContratacion)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pas_contratacion");

                entity.Property(e => e.PasEmpresa)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pas_empresa");

                entity.Property(e => e.PasEntrevista)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pas_entrevista");

                entity.Property(e => e.PasFechaContratacion)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pas_fecha_contratacion");

                entity.Property(e => e.PasMontoRemuneracion)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pas_monto_remuneracion");

                entity.Property(e => e.PasPruebas)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pas_pruebas");
            });

            modelBuilder.Entity<CargaSeguimientoPracticasPr>(entity =>
            {
                entity.HasKey(e => e.IdSeguimientoPracticasPr);

                entity.ToTable("CargaSeguimientoPracticasPr");

                entity.Property(e => e.Año).HasColumnName("año");

                entity.Property(e => e.Mes)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mes");

                entity.Property(e => e.PId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("p_id");

                entity.Property(e => e.PpCargo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pp_cargo");

                entity.Property(e => e.PpDocenteAsign)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pp_docente_asign");

                entity.Property(e => e.PpEmpresa)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pp_empresa");

                entity.Property(e => e.PpGestion)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pp_gestion");

                entity.Property(e => e.PpMontoRemuneracion)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pp_monto_remuneracion");

                entity.Property(e => e.PpPosibilidadContratacion)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pp_posibilidad_contratacion");
            });

            modelBuilder.Entity<CargaSeguimientoPsicosocial>(entity =>
            {
                entity.HasKey(e => e.IdSeguimientoPsicosocial);

                entity.ToTable("CargaSeguimientoPsicosocial");

                entity.Property(e => e.Año).HasColumnName("año");

                entity.Property(e => e.Mes)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mes");

                entity.Property(e => e.PId)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("p_id");

                entity.Property(e => e.SegAlertaDesercion)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("seg_alerta_desercion");

                entity.Property(e => e.SegEstado)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("seg_estado");

                entity.Property(e => e.SegMedida)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("seg_medida");

                entity.Property(e => e.SegMotivo)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("seg_motivo");
            });

            modelBuilder.Entity<Carrera>(entity =>
            {
                entity.HasKey(e => e.IdCarrera);

                entity.ToTable("Carrera");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(300)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CatAño>(entity =>
            {
                entity.HasKey(e => e.IdAño);

                entity.ToTable("CatAño");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CatCarrera>(entity =>
            {
                entity.HasKey(e => e.IdCatCarrera);

                entity.ToTable("CatCarrera");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CatMe>(entity =>
            {
                entity.HasKey(e => e.IdMes);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CatSede>(entity =>
            {
                entity.HasKey(e => e.IdCatSede);

                entity.ToTable("CatSede");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Cohorte>(entity =>
            {
                entity.HasKey(e => e.IdCohorte);

                entity.ToTable("Cohorte");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CursosCorto>(entity =>
            {
                entity.HasKey(e => e.IdCursoCorto);

                entity.Property(e => e.Nombre).HasMaxLength(50);
            });

            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.HasKey(e => e.IdDepartamento);

                entity.ToTable("Departamento");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre2)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EstadoPersona>(entity =>
            {
                entity.HasKey(e => e.IdEstadoPersona);

                entity.ToTable("EstadoPersona");

                entity.Property(e => e.Nombre).HasMaxLength(50);
            });

            modelBuilder.Entity<Grado>(entity =>
            {
                entity.HasKey(e => e.IdGrado);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<GradoSede>(entity =>
            {
                entity.HasKey(e => e.IdGradoSede);

                entity.ToTable("GradoSede");
            });

            modelBuilder.Entity<Materium>(entity =>
            {
                entity.HasKey(e => e.IdMateria);

                entity.Property(e => e.Nombre).HasMaxLength(50);
            });

            modelBuilder.Entity<Municipio>(entity =>
            {
                entity.HasKey(e => e.IdMunicipio);

                entity.ToTable("Municipio");

                entity.Property(e => e.Nombre).HasMaxLength(50);
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasKey(e => e.IdPersona);

                entity.ToTable("Persona");

                entity.Property(e => e.Apellido).HasMaxLength(100);

                //entity.Property(e => e.Cohorte)
                //    .HasMaxLength(50)
                //    .IsUnicode(false);
                //entity.HasMany(persona => persona.CargaEducacions) // Una persona tiene muchas CargaEducacions
                //    .WithOne(carga => carga.Persona) // Una CargaEducacion pertenece a una persona
                //    .HasForeignKey(carga => carga.PIdOim); // Clave foránea

                entity.Property(e => e.Correo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                //entity.Property(e => e.CartaCompromiso).HasMaxLength(100);

                entity.Property(e => e.Discapacidad)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dui)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Empleo)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.EstadoInscripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                //entity.Property(e => e.EstadoMf)
                //    .HasMaxLength(100)
                //    .IsUnicode(false);

                //entity.Property(e => e.EstadoPersona)
                //    .HasMaxLength(50)
                //    .IsUnicode(false);

                entity.Property(e => e.FamiliaresMigrantes)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FamiliaresRetornados)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaEntrevista).HasColumnType("datetime");

                entity.Property(e => e.FechaNacimiento).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MigranteRetornado)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nie)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre).HasMaxLength(100);

                entity.Property(e => e.NombreCompleto)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PIdOim)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                //.HasColumnName("p_id_oim");

                entity.Property(e => e.PiensaMigrar)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                //entity.Property(e => e.SocioIm).HasColumnName("socio_im");

                entity.Property(e => e.Telefono1)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono2)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UltimoGradoAprobado)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VictimaViolencia)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Programa>(entity =>
            {
                entity.HasKey(e => e.IdPrograma);

                entity.ToTable("Programa");

                entity.Property(e => e.Nombre).HasMaxLength(50);
            });

            modelBuilder.Entity<Refiere>(entity =>
            {
                entity.HasKey(e => e.IdRefiere);

                entity.ToTable("Refiere");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.IdRol);

                entity.ToTable("Rol");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sede>(entity =>
            {
                entity.HasKey(e => e.IdSede);

                entity.ToTable("Sede");
            });

            modelBuilder.Entity<Sexo>(entity =>
            {
                entity.HasKey(e => e.IdSexo)
                    .HasName("PK_Genero");

                entity.ToTable("Sexo");

                entity.Property(e => e.Nombre).HasMaxLength(50);
            });

            modelBuilder.Entity<SocioImplementador>(entity =>
            {
                entity.HasKey(e => e.IdImplementador);

                entity.ToTable("SocioImplementador");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoMatricula>(entity =>
            {
                entity.HasKey(e => e.IdTipoMatricula)
                    .HasName("PK_TipoDocumento");

                entity.ToTable("TipoMatricula");

                entity.Property(e => e.Nombre).HasMaxLength(50);
            });

            modelBuilder.Entity<Proyectos>(entity =>
            {
                entity.HasKey(e=> e.IdProyecto);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);

                entity.ToTable("Usuario");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Zona>(entity =>
            {
                entity.HasKey(e => e.IdZona);

                entity.ToTable("Zona");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sector>(entity =>
            {
                entity.HasKey(e => e.IdSector);

                entity.ToTable("Sector");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
