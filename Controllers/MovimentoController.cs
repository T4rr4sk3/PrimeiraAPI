using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using MovimentoClass;


namespace Tentativa2APIviaJSON.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MovimentoController : ControllerBase
    {
        // GET: Movimento
        [HttpGet]
        public IActionResult Get()
        {
            List<Movimento> lista = new Movimento().ListarTodos();
            if (lista.Count < 1)
            {
                return null;
            }
            return Ok(Movimento.Serializar(lista));
        }

        // GET: Movimento/Resgate
        [HttpGet("{action}",Name = "Resgate")]
        public IActionResult Resgate()
        {
            List<Movimento> lista = new Movimento().ListarResgate();
            if (lista.Count < 1)
            {
                return null;
            }

            return Ok(Movimento.Serializar(lista));
        }

        // GET: Movimento/Aplicacao
        [HttpGet("{action}", Name = "Aplicacao")]
        public IActionResult Aplicacao()
        {
            List<Movimento> lista = new Movimento().ListarAplicacao();
            if (lista.Count < 1)
            {
                return null;
            }

            return Ok(Movimento.Serializar(lista));
        }

        // POST: Movimento
        [HttpPost]
        public IActionResult Post([FromBody] JsonElement json)
        {
            string str = json.ToString();
            
            if (str.StartsWith("["))
            {
                List<Movimento> lista = Movimento.Deserializar(str);

                for (int i = 0; i < lista.Count; i++)
                {
                    lista[i].Criar();
                }
            }
            else
            {
                Movimento movimento = Movimento.Deserializar(str, new Movimento().GetType());

                movimento.Criar();
            }

            return Ok();
        }
        
        // DELETE: Movimento
        [HttpDelete]
        public IActionResult Delete([FromBody] JsonElement json)
        {
            string str = json.ToString();
            if (str.StartsWith("["))
            {
                List<Movimento> lista = Movimento.Deserializar(str);

                for (int i = 0; i < lista.Count; i++)
                {
                    lista[i].Deletar();
                }
            }
            else
            {
                Movimento movimento = Movimento.Deserializar(str, new Movimento().GetType());

                movimento.Deletar();
            }

            return Ok();
        }


    }
}
