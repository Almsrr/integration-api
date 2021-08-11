using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegrationAPI.Models {
  public class Transaction {
    [Key]
    public int Id { get; set; }
    public DateTime FechaCreacion { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MontoTransaccion { get; set; }

    [Required]
    public int IDCuentaEmisor { get; set; }

    [Required]
    public int IDCuentaReceptor { get; set; }

    [Required]
    [StringLength(50)]
    public string TipoTransaccion { get; set; }
    [Required]
    [StringLength(50)]
    public string Descripción { get; set; }
  }
}