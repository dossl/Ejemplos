using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeasWebEntities.Entities.Activos
{
    [Serializable]
    public class BEactactivos
    {

        public string cnumactivo { get; set; }
        public string ctipoactiv { get; set; }
        public string cdescactiv { get; set; }
        public string cnomactiv { get; set; }
        public string cmarcaacti { get; set; }
        public string cserieacti { get; set; }
        public string cmodeloact { get; set; }
        public DateTime? dfechaacti { get; set; }
        public decimal nvalorcom { get; set; }
        public decimal nporcdepre { get; set; }
        public decimal ndepreacum { get; set; }
        public string ccodproved { get; set; }
        public string cnumckcomp { get; set; }
        public string ccoddepart { get; set; }
        public string cactdeprec { get; set; }
        public decimal nmontomejo { get; set; }
        public decimal nmontoreva { get; set; }
        public decimal nmontdpmej { get; set; }
        public decimal nmontdprev { get; set; }
        public decimal nmontliqui { get; set; }
        public decimal nvalorresi { get; set; }
        public decimal nvalorrepo { get; set; }
        public string cestactivo { get; set; }
        public string ccodrefere { get; set; }
        public string ctipodepre { get; set; }
        public decimal nperiovida { get; set; }
        public decimal nmontdesva { get; set; }
        public string cfotoactiv { get; set; }
        public string cidubicaci { get; set; }
        public string cnumgarant { get; set; }
        public string cdescrigar { get; set; }
        public decimal nvalorcom1 { get; set; }
        public decimal nvalorcomi { get; set; }
        public string CCODCENTRO { get; set; }
        public string cnumeorden { get; set; }
        public string ctranreval { get; set; }
        public string ctranmejor { get; set; }
        public string ctipasiact { get; set; }
        public string ctipasirev { get; set; }
        public string ctipasimej { get; set; }

        //ATRIBUTOS AGREGADOS PARA LA OPCION Reporte de Mejoras y revaluaciones
        
        public decimal nvallibros { get; set; }
        public decimal nvalorreva { get; set; }
        
        public string cnumtransa { get; set; }
        public string ctipotrans { get; set; }
        public DateTime? dfechamovi { get; set; }

        //Agregado en asignacion de activos por pólizas
        public string nseleccion { get; set; }

        ///Agregado para liquidacion de activos
        public string cconsecliv { get; set; }
        public string cconseclig { get; set; }
        public string cconseclib { get; set; }
      
        
    }
}
