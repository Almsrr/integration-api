using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IntegrationAPI.Data;
using IntegrationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntegrationAPI.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class TransactionsController : ControllerBase {

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private readonly IntegrationContext _context;
    private readonly string CoreAPIURL = "https://webcoreapiazure.azurewebsites.net/api/CoreTransaction";
    private static readonly HttpClient httpClient = new HttpClient();

    public TransactionsController(IntegrationContext context) {
      _context = context;
    }

    // GET: api/Transactions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions() {
      try {
        // Get Transactions from Core
        HttpResponseMessage response = await httpClient.GetAsync($"{CoreAPIURL}");
        List<Transaction> transactions = await response.Content.ReadAsAsync<List<Transaction>>();
        // Add new Transactions to our own Db
        log.Info($"Get {transactions.Count} Transactions from Core");
        return Ok(transactions);
      } catch (System.Exception) {
        log.Warn("Could not connect to core. Trying to get Transactions from Internal Database");
        try {
          List<Transaction> transactions = await _context.Transactions.ToListAsync();
          log.Info($"Get {transactions.Count} from Internal Database");
          return Ok(transactions);
        } catch (System.Exception e) {
          log.Error("Could not connect to core or internal db", e);
          return BadRequest("Could not return transactions");
        }
      }
    }

    // GET: api/Transactions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetTransaction(int id) {
      try {
        HttpResponseMessage response = await httpClient.GetAsync($"{CoreAPIURL}?idCuenta={id}");
        Transaction transaction = await response.Content.ReadAsAsync<Transaction>();
        return Ok(transaction);
      } catch (System.Exception) {
        try {
          Transaction transaction = await _context.Transactions.FindAsync(id);
          return Ok(transaction);
        } catch (System.Exception) {
          return NotFound();
        }
      }
    }

    // PUT: api/Transactions/5
    [HttpPut("{id}")]
    public IActionResult PutTransaction() =>
      BadRequest("PUT Operation not available for Transactions");

    // POST: api/Transactions
    // To protect from overposting attacks, enable the specific properties you want to bind to, for
    // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
    [HttpPost]
    public async Task<ActionResult<Transaction>> PostTransaction(int? cuentaEmisor, int? cuentaReceptor, decimal monto, string tipoTransaction, string? descripcion, int? cuentaRetiro, int? cedula) {
      Task<HttpResponseMessage> responseTask;
      Transaction transaction;

      if (cuentaRetiro != null) {
        responseTask = httpClient.PostAsync($"{CoreAPIURL}?cuentaRetiro={cuentaRetiro}&monto={monto}&tipoTransaction={tipoTransaction}", new StringContent(""));
        transaction = new Transaction {
          IDCuentaEmisor = (int)cuentaRetiro,
          TipoTransaccion = tipoTransaction,
          MontoTransaccion = monto
        };
      } else if (cedula != null) {
        responseTask = httpClient.PostAsync($"{CoreAPIURL}?cedula={cedula}&cuentaReceptor={cuentaReceptor}&monto={monto}&tipoTransaction={tipoTransaction}&descripcion={descripcion}", new StringContent(""));
        transaction = new Transaction {
          IDCuentaReceptor = (int)cuentaReceptor,
          MontoTransaccion = monto,
          TipoTransaccion = tipoTransaction,
          Descripción = descripcion
        };
      } else {
        responseTask = httpClient.PostAsync($"{CoreAPIURL}?cuentaEmisor={cuentaEmisor}&cuentaReceptor={cuentaReceptor}&monto={monto}&tipoTransaction={tipoTransaction}&descripcion={descripcion}", new StringContent(""));
        transaction = new Transaction {
          IDCuentaEmisor = (int)cuentaEmisor,
          IDCuentaReceptor = (int)cuentaReceptor,
          MontoTransaccion = monto,
          Descripción = descripcion
        };
      }

      try {
        HttpResponseMessage response = await responseTask;
        if (response.IsSuccessStatusCode) {
          try {
            // could connect to core
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
          } catch (System.Exception) {
            throw;
          }

          // core returned OK
          try {
            // save to own db & return OK
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return Ok();
          } catch (System.Exception) {
            // could not connect to core or save to own db
            return BadRequest("Could not create a transaction");
          }
        }
        // core didn't return OK
        return Ok();
      } catch (System.Exception) {
        // could not connect to core
        try {
          // save to own db & return OK
          _context.Transactions.Add(transaction);
          await _context.SaveChangesAsync();
          return Ok();
        } catch (System.Exception) {
          // could not connect to core or save to own db
          return BadRequest("Could not create a transaction");
        }
      }
    }

    // DELETE: api/Transactions
    [HttpDelete]
    public ActionResult DeleteTransaction() =>
      BadRequest("DELETE Operation not available for Transactions");

    private bool TransactionExists(int id) {
      return _context.Transactions.Any(e => e.Id == id);
    }
  }
}
