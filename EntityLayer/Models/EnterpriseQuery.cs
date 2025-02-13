
namespace EntityLayer.Models
{
    public class EnterpriseQuery
    {
        public int Id_empresa { get; set; }
        public string Nombre { get; set; }
        public string Ruc { get; set; }
        public string Nombre_comercial { get; set; }
        public string Nombre_corto { get; set; }
        public string Numeracion_universal { get; set; }
        public string Erp_murano { get; set; }
        public string Rcsa { get; set; }
        public string Registro_civil { get; set; }
        public string Contrato_mercantil { get; set; }
        public string Decevale { get; set; }
        public int BusinessWare { get; set; }
        public string Factura_cero { get; set; }
        public string Ruta_logo_empresa { get; set; }
        public string Ruta_logo_reporte1 { get; set; }
        public string Ruta_logo_reporte2 { get; set; }
        public string Ruta_logo_reporte3 { get; set; }
        public string Ruta_para_formatos { get; set; }
        public string Ruta_pagina_web { get; set; }
        public string Estado { get; set; }
        public string Fec_notificacion { get; set; }
        public int? Frecuencia { get; set; }
        public string Notificacion_vencimiento { get; set; }
        public int? Id_cat_fecha_factura { get; set; }
        public int? Id_cat_tipo_firma { get; set; }
        public DateTime Fec_creacion { get; set; }
        public int Id_usuario_creacion { get; set; }
        public DateTime Fec_modificacion { get; set; }
        public int Id_usuario_modificacion { get; set; }
        public DateTime Fec_eliminacion { get; set; }
        public int Id_usuario_eliminacion { get; set; }
        //public List<BranchOfficeQuery> Sucursales { get; set; }
        //public List<ApprovemenParameterQuery> Aprobaciones { get; set; }
        public string Log { get; set; }

    }
}
